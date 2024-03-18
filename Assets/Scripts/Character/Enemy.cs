using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

// 적 상태값들. 상태에 따라서 행동 방식이 달라짐.
public enum EnemyStates
{
    None,

    IDLE, // 평소 상태. 가만히 있거나, 주위만 서성거림.
    PATROL, // 순찰 상태. 플레이어의 추적을 놓쳤거나 근처에서 소리가 들릴 때 큰 범위를 돌아다님.
    TRACE, // 추적 상태. 플레이어를 발견했을 때 플레이어를 쫒아감.
    ATT, // 공격 상태. 플레이어를 공격함.
    RUN, // 도망 상태. 체력이 적거나 플레이어의 무기가 강하다고 판단할 시 현 자리를 일시적으로 벗어남.
    Hit, // 맞은 상태.
    DEAD, // 죽은 상태.
    AGGRO, 

    END
}

public class Enemy : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField]
    protected int maxHP;
    [SerializeField]
    protected int currentHP;
    public float walkSpeed;
    public float runSpeed;
    public float acceleration;
    [SerializeField]
    protected int attackDamage;
    [SerializeField]
    protected float attackDelay;
    [SerializeField]
    protected float attackRange;
    [SerializeField]
    [Range(0, 360)]
    protected float viewAngle;
    [SerializeField]
    protected float viewDistance;
    public float hearingAbility;

    [Header("MapInfo")]
    public Room currentRoom;

    [Header("LayerMask")]
    [SerializeField]
    protected LayerMask playerLayer;
    public LayerMask obstacleLayer;
    [SerializeField]
    protected LayerMask excludeLayer;
    protected int exludeEnemyLayer;
    public EnemyStates state;

    [HideInInspector]
    public Animator animator;
    protected NavMeshAgent navMeshAgent;

    protected EnemyState _state;
    protected EnemyStateHandle _stateHandle;

    private CapsuleCollider capsuleCollider;
    public virtual Vector3 CenterPos { get => capsuleCollider.bounds.center; }
    protected bool _canAtt = true;
    public bool CanAtt { get => _canAtt; set { _canAtt = value; if (!_canAtt) StartCoroutine(AttDelayCoroutine()); } }

    public int CurrentHP { get => currentHP; }

    IEnumerator AttDelayCoroutine() { yield return new WaitForSeconds(attackDelay); _canAtt = true; }

    [HideInInspector]
    public Vector3 aggroFlaskPos;
    public float chaseTime;
    public bool isChaseTime;

    [SerializeField]
    private BoxCollider attackCollider;

    [HideInInspector]
    public AudioSource audioSource;
    [SerializeField]
    protected AudioClip footstepClip;
    [SerializeField]
    protected AudioClip hitSound;

    public AudioClip patrolSound;
    public Vector3 startPos;

    private GameObject bloodeffect;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        HandleInput(EnemyStates.IDLE);
        state = EnemyStates.IDLE;
        _state.Enter(this);
        exludeEnemyLayer = ~excludeLayer;
        capsuleCollider = GetComponent<CapsuleCollider>();
        startPos = transform.position;
        bloodeffect = Resources.Load<GameObject>("BloodSprayFX_Extra");
    }

    protected virtual void FixedUpdate()
    {
        _state.Update(this);
    }

    // Enemy 상태 변경 함수
    public void HandleInput(EnemyStates es)
    {
#if UNITY_EDITOR
        Debug.Log($"current: {_state}, es: {es}");
#endif

        if (_state == null)
            _state = _stateHandle.HandleInput(EnemyStates.IDLE);

        es = _state.HandleInput(es, this.state);
        EnemyState state = _stateHandle.HandleInput(es);

        if (state != null)
        {
            _state.Exit(this);
            _state = null;
            _state = state;
            _state.Enter(this);
        }
    }

    public void SwitchAttackCollider()
    {
        attackCollider.enabled = !attackCollider.enabled;
    }

    public virtual void Hit(int dmg, bool stiffness = false)
    {
        currentHP -= dmg;
        audioSource.PlayOneShot(hitSound);
        HandleInput(EnemyStates.Hit);
    }

    // 설정 범위 내에 플레이어가 있을 시 true를 반환하는 함수.
    public Transform FindPlayer()
    {
        // 전방 viewAngle 각도에 해당하는 플레이어를 찾음.
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewDistance, playerLayer);
        Vector3 targetDir;
        RaycastHit hit;
        Vector3 lookDir = transform.forward;

        for (int i = 0; i < colliders.Length; i++)
        {
            targetDir = (colliders[i].bounds.center - CenterPos).normalized;

            if (Physics.Raycast(CenterPos, targetDir, out hit, Mathf.Infinity, exludeEnemyLayer))
            {
                if (Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg <= viewAngle * 0.5f && hit.transform.CompareTag("Player"))
                {
                    return colliders[i].transform;
                }
            }
        }

        return null;
    }

    public void SetDestination(Vector3 pos, bool isChaising)
    {
        navMeshAgent.updateRotation = !isChaising;

        if (isChaising)
            transform.LookAt(pos);

        navMeshAgent.SetDestination(pos);
    }

    public void StopNav()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    public void StartNav()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.updatePosition = true;
        navMeshAgent.updateRotation = true;
    }

    public void DisableNavemshAgent()
    {
        navMeshAgent.enabled = false;
    }

    // 목적지에 도착했는지 체크하는 함수
    public bool isArriveDestination() 
    { 
        if (!navMeshAgent.enabled) return false; 
        if (navMeshAgent.pathPending) return false; 
        return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.5f; 
    }
    // 플레이어가 공격 범위 안에 있는지 체크하는 함수
    public bool CheckAttackRange()
    {
        RaycastHit hit;
        if (Physics.Raycast(CenterPos, (GameManager.Instance.player.CenterPos() - CenterPos).normalized, out hit, attackRange, exludeEnemyLayer))
        {
            if (hit.transform.CompareTag("Player"))
            {
                if (!Physics.Raycast(CenterPos, (GameManager.Instance.player.CenterPos() - CenterPos).normalized, 
                    Vector3.Distance(GameManager.Instance.player.CenterPos(), CenterPos), obstacleLayer))
                {
                    return true;
                }
            }
        }

        return false;
    }
    public float getDestinationDistance() { if (navMeshAgent.hasPath) return navMeshAgent.remainingDistance; else return 1000f; }

    public void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = acceleration;
    }

    public void SetAggro(Vector3 pos)
    {
        if (state == EnemyStates.IDLE || state == EnemyStates.PATROL)
        {
            aggroFlaskPos = pos;
            HandleInput(EnemyStates.AGGRO);
        }
    }

    public void Stop()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", true);
    }

    public void Walk()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", true);
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public virtual IEnumerator PlayWalkSound()
    {
        while (true) 
        {
            yield return new WaitForSeconds(0.6f);
            audioSource.PlayOneShot(footstepClip);
        }
    }

    public void PlayRunSound()
    {
        audioSource.clip = footstepClip;
        audioSource.Play();
        audioSource.loop = true;
    }
    void ShowBloodEffect(Collision collision)
    {
        Vector3 pos = collision.contacts[0].point; //무기의 최초 포지션
        Vector3 _nomal = collision.contacts[0].point;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _nomal); //무기의 공격 방향

        GameObject blood = Instantiate<GameObject>(bloodeffect, pos, rot); //방향에 혈흔 이펙트 생성
        Destroy(blood, 1.0f); //삭제
    }
}
