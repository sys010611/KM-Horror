using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creature : Enemy
{
    // 크리처마다 갖고있는 고유 번호 만약 맵매니저의 donDestroyObj에 같은 번호가 있다면 생성된 크리처는 삭제.
    public int serialNum;
    [SerializeField]
    private bool difficultMonster; // 해당 bool 변수가 true라면 게임매니저의 diffuclt가 true일 때만 활성화 됨.
    [SerializeField]
    private AudioClip breathClip;
    [SerializeField]
    private AudioClip shoutClip;

    public Transform runPoint;

    public bool isFind;

    public int hitCount = 0;

    public event Action<Vector3> OnEnableEvent;
    public Vector3 eventPos;

    protected override void Awake()
    {
        _stateHandle = new EnemyStateHandle(new CreatureIdle(), new CreaturePatrol(), new CreatureTrace(),
    new CreatureAttack(), new CreatureRun(), new CreatureHit(), new CreatureDead(), new CreatureAggro());
        animator = GetComponent<Animator>();
        base.Awake();
    }

    protected void Start()
    {
        StartCoroutine(PlayGrawlSound());
    }

    private void OnDestroy()
    {
        GameManager.Instance.activatedCreatureList.Remove(this);
    }

    public override void Hit(int dmg, bool stiffness)
    {
        if (state == EnemyStates.RUN)
        {
            return;
        }

        ++hitCount;
        currentHP -= dmg;
        audioSource.PlayOneShot(hitSound);
        HandleInput(EnemyStates.Hit);
    }

    private void OnEnable()
    {
        if (difficultMonster)
        {
            if (GameManager.Instance.difficult)
            {
                GameManager.Instance.activatedCreatureList.Add(this);
                InvokeRepeating(nameof(InformPlayerPos), 45f, 120);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            GameManager.Instance.activatedCreatureList.Add(this);
            InvokeRepeating(nameof(InformPlayerPos), 45f, 120);
        }

        OnEnableEvent?.Invoke(eventPos);
    }

    private void OnDisable()
    {
        GameManager.Instance.activatedCreatureList.Remove(this);
        CancelInvoke(nameof(InformPlayerPos));
    }

    public void PlayBreathSound()
    {
        audioSource.PlayOneShot(breathClip);
    }

    public void PlayShoutSound()
    {
        audioSource.spatialBlend = 0.5f;
        audioSource.PlayOneShot(shoutClip);
        this.Invoke(() => audioSource.spatialBlend = 1, shoutClip.length - 0.05f);
    }

    // 일정 주기마다 크리처에게 플레이어 근처 방들을 알려줘 순회하도록 함.
    private void InformPlayerPos()
    {
        if (state.Equals(EnemyStates.IDLE) || state.Equals(EnemyStates.AGGRO))
            if (currentRoom.sector == MapManager.Instance.PlayerCurrentSector)
                HandleInput(EnemyStates.PATROL);

    }

    IEnumerator PlayGrawlSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(15f, 25f));
            if (state == EnemyStates.IDLE)
            {
                SoundManager.Instance.PlaySound(transform.position, patrolSound);
            }
        }
    }
}
