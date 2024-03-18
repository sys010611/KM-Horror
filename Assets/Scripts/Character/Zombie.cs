using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField]
    public AudioClip[] attackClips;
    [SerializeField]
    private AudioClip deadClip;
    [SerializeField]
    private AudioClip growlClip;

    protected override void Awake()
    {
        _stateHandle = new EnemyStateHandle(new ZombieIdle(), new ZombiePatrol(), new ZombieTrace(),
    new ZombieAttack(), new ZombieRun(), new ZombieHit(), new ZombieDead(), new ZombieAggro());
        animator = GetComponent<Animator>();
        base.Awake();
    }


    private void OnEnable()
    {
        GameManager.Instance.activatedZombieList.Add(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.activatedZombieList.Remove(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance.activatedZombieList.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStat>().Hit(attackDamage);
        }
    }

    public override void Hit(int dmg, bool stiffness = false)
    {
        currentHP -= dmg;
        audioSource.PlayOneShot(hitSound);
        if (currentHP <= 0)
        {
            HandleInput(EnemyStates.DEAD);
        }
        else if(state != EnemyStates.ATT)
        {
            if (stiffness && state != EnemyStates.Hit)
            {
                HandleInput(EnemyStates.Hit);
            }
            else
            {
                HandleInput(EnemyStates.TRACE);
            }
        }
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackClips[Random.Range(0, attackClips.Length)]);
    }

    public void PlayDeadSound()
    {
        audioSource.PlayOneShot(deadClip);
    }

    public void PlayGrowlSound()
    {
        audioSource.PlayOneShot(growlClip);
    }
}
