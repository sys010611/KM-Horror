using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

// Enemy 상태를 관장하는 클래스
public class EnemyState
{
    public static Transform player;
    public static Vector3 flaskPos;
    protected Vector3 playerLastPos;

    public EnemyState() { }

    public EnemyState(Enemy enemy) { }

    public virtual EnemyStates HandleInput(EnemyStates es, EnemyStates enemy)
    {
        if (es.Equals(enemy))
            if (!es.Equals(EnemyStates.Hit))
                return EnemyStates.None;
        //다를 경우 받은 상태로 state 전환
        return es;
    }

    public virtual void Enter(Enemy enemy) { }
    public virtual void Exit(Enemy enemy) { }
    public virtual void Update(Enemy enemy) { }
    public virtual void FixedUpdate(Enemy enemy) { }

    protected bool FindPlayer(Enemy enemy)
    {
        var t = enemy.FindPlayer();

        if (t != null)
        {
            return true;
        }

        return false;
    }

    protected bool HearPlayerSound(Enemy enemy, float rates = 1f) 
    {
        if (Vector3.Distance(player.position, enemy.transform.position) <= enemy.hearingAbility * rates * GameManager.Instance.GetPlayerSound())
        {
            return true;
        }

        return false;
    }

    protected bool HearPlayerSound(Enemy enemy, out float soundAmount, float rates = 1f)
    {
        if (player == null)
        {
            soundAmount = 0;
            return false;
        }

        float distance = Vector3.Distance(player.position, enemy.transform.position);
        float amount = enemy.hearingAbility * rates * GameManager.Instance.GetPlayerSound();

        soundAmount = amount - distance;

        if (distance <= amount)
        {
            return true;
        }

        return false;
    }
}

public class EnemyStateHandle
{
    private Idle _idle;
    private Patrol _patrol;
    private Trace _trace;
    private Att _att;
    private Run _run;
    private Hit _hit;
    private Dead _dead;
    private Aggro _aggro;

    public EnemyStateHandle()
    {
        _idle = new Idle();
        _patrol = new Patrol();
        _trace = new Trace();
        _att = new Att();
        _hit = new Hit();
        _dead = new Dead();
        _aggro = new Aggro();
    }

    public EnemyStateHandle(Idle idle, Patrol patrol, Trace trace, Att att, Run run, Hit hit, Dead dead, Aggro aggro) 
    {
        _idle = idle;
        _patrol = patrol;
        _trace = trace;
        _att = att;
        _hit = hit;
        _dead = dead;
        _aggro = aggro;
        _run = run;
    }

    public EnemyState HandleInput(EnemyStates es)
    {
        //다를 경우 받은 상태로 state 전환
        switch (es)
        {
            case EnemyStates.IDLE:
                return _idle;
            case EnemyStates.PATROL:
                return _patrol;
            case EnemyStates.TRACE:
                return _trace;
            case EnemyStates.ATT:
                return _att;
            case EnemyStates.RUN:
                return _run;
            case EnemyStates.Hit:
                return _hit;
            case EnemyStates.DEAD:
                return _dead;
            case EnemyStates.AGGRO: 
                return _aggro;
            case EnemyStates.END:
            default:
                return null;
        }
    }
}

public class Idle : EnemyState 
{
    protected Coroutine walkSoundCoroutine;
    protected Coroutine _idleCoroutine;

    public override void Enter(Enemy enemy)
    {
        enemy.state = EnemyStates.IDLE;
        enemy.SetSpeed(enemy.walkSpeed);
        enemy.animator.SetBool("Idle", true);
    }

    public override void Update(Enemy enemy)
    {
        if (HearPlayerSound(enemy))
        {
            enemy.HandleInput(EnemyStates.PATROL);
            return; 
        }
    }

    public override void Exit(Enemy enemy)
    {
        if (walkSoundCoroutine != null) enemy.StopCoroutine(walkSoundCoroutine);
        if (_idleCoroutine != null) enemy.StopCoroutine(_idleCoroutine);
        enemy.animator.SetBool("Idle", false);
        enemy.animator.SetBool("Walk", false);
    }

    // Idle 상태일 때 Enemy가 짧은 거리를 이동하거나 주위를 둘러보도록 함.
    protected virtual IEnumerator IdleCoroutine(Enemy enemy)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(6f, 12f));

            float angle = Random.Range(0, 360);
            Vector3 dir = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.forward;

            walkSoundCoroutine = enemy.StartCoroutine("PlayWalkSound");
            enemy.animator.SetBool("Idle", false);
            enemy.animator.SetBool("Walk", true);

            enemy.SetDestination(enemy.transform.position + dir * 8.5f, false);

            yield return new WaitUntil(enemy.isArriveDestination);

            enemy.StopCoroutine(walkSoundCoroutine);
            enemy.animator.SetBool("Idle", true);
            enemy.animator.SetBool("Walk", false);
        }
    }
}

public class Patrol : EnemyState 
{
    protected List<Vector3> destinations = new List<Vector3>();
    protected int idx;
    protected Coroutine walkSoundCoroutine;

    public override void Enter(Enemy enemy)
    {
        
    }

    public override void Update(Enemy enemy)
    {
        if (HearPlayerSound(enemy) && !GameManager.Instance.player.isHide)
        {
            if (destinations.Count != 0)
                destinations.RemoveAt(0);
            destinations.Insert(0, player.position);
            idx = 0;
            enemy.SetDestination(destinations[idx++], false);
        }
    }


    public override void Exit(Enemy enemy)
    {
        enemy.animator.SetBool("Walk", false);
        if(walkSoundCoroutine != null)
            enemy.StopCoroutine(walkSoundCoroutine);
    }
}

public class Trace : EnemyState 
{
    protected bool isLooking;
    protected Coroutine chaseTimeCoroutine;

    public override void Enter(Enemy enemy)
    {
        enemy.SetSpeed(enemy.runSpeed);
        enemy.animator.SetBool("Run", true);
        enemy.PlayRunSound();
        isLooking = false;

        if (chaseTimeCoroutine == null)
        {
            enemy.isChaseTime = true;
            chaseTimeCoroutine = enemy.StartCoroutine(ChaseTimeCoroutine(enemy));
        }
    }

    public override void Update(Enemy enemy)
    {


    }

    public override void Exit(Enemy enemy)
    {
        enemy.SetDestination(enemy.transform.position, false);
        enemy.animator.SetBool("Run", false);
        enemy.StopSound();
    }


    protected IEnumerator ChaseTimeCoroutine(Enemy enemy)
    {
        yield return new WaitForSeconds(enemy.chaseTime);
        enemy.isChaseTime = false;
        chaseTimeCoroutine = null;
    }
}

public class Att : EnemyState 
{
    public Att() {}
    protected bool isAttacking;
    protected Coroutine attCoroutine;

    public override void Enter(Enemy enemy)
    {
        enemy.state = EnemyStates.ATT;
        enemy.StopNav();
        attCoroutine = enemy.StartCoroutine(AttCoroutine(enemy));
    }

    public override void Update(Enemy enemy)
    {
        if (enemy.CanAtt)
        {
            isAttacking = true;
            if (attCoroutine != null) enemy.StopCoroutine(attCoroutine);
            attCoroutine = enemy.StartCoroutine(AttCoroutine(enemy));
        }
    }

    protected virtual IEnumerator AttCoroutine(Enemy enemy)
    {
        yield return null;
    }

    public override void Exit(Enemy enemy)
    {
        enemy.StartNav();
        if (attCoroutine != null)
        {
            enemy.StopCoroutine(attCoroutine);
        }
    }
}
public class Run : EnemyState 
{
    public override void Enter(Enemy enemy)
    {
        enemy.SetSpeed(enemy.runSpeed);
        enemy.animator.SetBool("Run", true);
        enemy.PlayRunSound();

        ((Creature)enemy).PlayShoutSound();
        MapManager.Instance.BuildNavPath(enemy.currentRoom.sector, player.position);
        enemy.SetDestination(((Creature)enemy).runPoint.position ,false);

        enemy.state = EnemyStates.RUN;
    }

    public override void Update(Enemy enemy)
    {
        if (enemy.isArriveDestination())
        {
            enemy.DisableNavemshAgent();
            enemy.animator.SetTrigger("Jump");
            enemy.animator.SetBool("Run", false);
            enemy.StopSound();
            DOTween.Sequence().
                AppendInterval(0.75f)
                .Append(enemy.transform.DOMoveY(enemy.transform.position.y + 4, 0.7f));
        }
    }

    public override void Exit(Enemy enemy)
    {
        enemy.SetDestination(enemy.transform.position, false);
        enemy.animator.SetBool("Run", false);
        enemy.StopSound();
    }
}
public class Hit : EnemyState 
{
    protected Coroutine coroutine;

    public override void Enter(Enemy enemy)
    {
        enemy.state = EnemyStates.Hit;
        enemy.StopNav();

        float dot = Vector3.Dot(enemy.transform.forward, (player.position - enemy.transform.position).normalized);

        switch (dot)
        {
            case < -0.5f:
                enemy.animator.SetTrigger("Hit Back");
                break;
            case < 0:
                enemy.animator.SetTrigger("Hit Right");
                break;
            case < 0.5f:
                enemy.animator.SetTrigger("Hit Left");
                break;
            default:
                enemy.animator.SetTrigger("Hit Front");
                break;
        }
        coroutine = enemy.StartCoroutine(TraceCoroutine(enemy));
    }

    public override void Exit(Enemy enemy)
    {
        enemy.StartNav();
        enemy.StopCoroutine(coroutine);
    }

    protected IEnumerator TraceCoroutine(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);
        enemy.HandleInput(EnemyStates.TRACE);    }  
}
public class Dead : EnemyState 
{
    public override void Enter(Enemy enemy)
    {
        enemy.state = EnemyStates.DEAD;
        enemy.StopNav();

        float dot = Vector3.Dot(enemy.transform.forward, (player.position - enemy.transform.position).normalized);

        switch (dot)
        {
            case < -0.5f:
                enemy.animator.SetBool("Dead Front", true);
                break;
            case < 0:
                enemy.animator.SetBool("Dead Left", true);
                break;
            case < 0.5f:
                enemy.animator.SetBool("Dead Right", true);
                break;
            default:
                enemy.animator.SetBool("Dead Back", true);
                break;
        }

        enemy.GetComponent<CapsuleCollider>().enabled = false;
        enemy.GetComponent<BoxCollider>().enabled = false;
        enemy.enabled = false;
    }
}

public class Aggro : EnemyState
{
    public override void Enter(Enemy enemy)
    {
        enemy.state = EnemyStates.AGGRO;
        enemy.SetDestination(enemy.aggroFlaskPos ,false);
    }

    public override void Update(Enemy enemy)
    {
        if (enemy.isArriveDestination())
        {
            // 모든 방들을 다 순찰했으나 플레이어를 발견하지 못했으므로 Idle 상태로 전환
            enemy.HandleInput(EnemyStates.IDLE);
            return;
        }
    }
}

public class ZombieIdle : Idle 
{
    Coroutine returnCoroutine;
    float switchStateGague;
    bool playSound;

    public override void Enter(Enemy enemy)
    {
        if (enemy.state.Equals(EnemyStates.IDLE))
            _idleCoroutine = enemy.StartCoroutine(IdleCoroutine(enemy));
        else
            returnCoroutine = enemy.StartCoroutine(RetrunCoroutine(enemy));

        switchStateGague = 0;

        base.Enter(enemy);
    }

    public override void Update(Enemy enemy)
    {
        float soundAmount;

        if (HearPlayerSound(enemy, out soundAmount, 1))
        {
            switchStateGague += soundAmount * Time.deltaTime;
            if (!playSound)
            {
                playSound = true;
                ((Zombie)enemy).PlayDeadSound();
            }

            if (switchStateGague >= 5) 
                enemy.HandleInput(EnemyStates.PATROL);

            return;
        }
        else
        {
            switchStateGague = Mathf.Max(switchStateGague - Time.deltaTime, 0);
            if (switchStateGague <= 0)
                playSound = false;
        };
    }

    public override void Exit(Enemy enemy)
    {
        base.Exit(enemy);
        if (returnCoroutine != null) enemy.StopCoroutine(returnCoroutine);
        enemy.animator.SetBool("Idle", false);
        enemy.animator.SetBool("Walk", false);
        if (walkSoundCoroutine != null) enemy.StopCoroutine(walkSoundCoroutine);
    }

    IEnumerator RetrunCoroutine(Enemy enemy)
    {
        yield return new WaitForSeconds(2);

        enemy.SetDestination(enemy.startPos, false);
        enemy.animator.SetBool("Idle", false);
        enemy.animator.SetBool("Walk", true);
        walkSoundCoroutine = enemy.StartCoroutine("PlayWalkSound");

        yield return new WaitUntil(enemy.isArriveDestination);
        enemy.animator.SetBool("Walk", false);
        enemy.animator.SetBool("Idle", true);
        enemy.StopCoroutine(walkSoundCoroutine);

        _idleCoroutine = enemy.StartCoroutine(IdleCoroutine(enemy));
    }
}
public class ZombiePatrol : Patrol 
{
    public override void Enter(Enemy enemy)
    {
        enemy.animator.SetBool("Run", true);
        enemy.SetSpeed(enemy.runSpeed);
        if (enemy.state.Equals(EnemyStates.IDLE))
            SoundManager.Instance.PlaySound(enemy.transform.position, enemy.patrolSound);
        enemy.PlayRunSound();
        destinations.Clear();
        destinations.Add(player.position);
        enemy.state = EnemyStates.PATROL;
    }

    public override void Update(Enemy enemy)
    {
        if (HearPlayerSound(enemy) && !GameManager.Instance.player.isHide)
        {
            if (destinations.Count != 0)
                destinations.RemoveAt(0);
            destinations.Insert(0, player.position);
            idx = 0;
            enemy.SetDestination(destinations[idx++], false);
        }

        if (!GameManager.Instance.player.isHide && HearPlayerSound(enemy, 0.5f))
        {
            enemy.HandleInput(EnemyStates.TRACE);
            return;
        }

        if (enemy.isArriveDestination())
        {
            if (idx.Equals(destinations.Count))
            {
                enemy.HandleInput(EnemyStates.IDLE);
                return;
            }
            else
            {
                enemy.SetDestination(destinations[idx++], false);
            }
        }
    }

    public override void Exit(Enemy enemy)
    {
        enemy.animator.SetBool("Run", false);
        enemy.StopSound();
    }
}
public class ZombieTrace : Trace 
{
    public override void Enter(Enemy enemy)
    {
        if (enemy.state != EnemyStates.ATT || enemy.state != EnemyStates.Hit) 
            ((Zombie)enemy).PlayGrowlSound();
        base.Enter(enemy);
        enemy.state = EnemyStates.TRACE;
        enemy.transform.DOLookAt(player.position, 0.5f).OnComplete(() => isLooking = true);
    }

    public override void Update(Enemy enemy)
    {
        if (enemy.CheckAttackRange() && enemy.CanAtt)
        {
            enemy.HandleInput(EnemyStates.ATT);
            return;
        }

        enemy.SetDestination(player.position, isLooking && FindPlayer(enemy).Equals(true));

        if ((HearPlayerSound(enemy, 0.5f).Equals(false) && !enemy.isChaseTime) || GameManager.Instance.player.isHide)
        {
            // 추격 실패. PATROL 상태로 전환.
            enemy.HandleInput(EnemyStates.PATROL);
            return;
        }
    }

    public override void Exit(Enemy enemy)
    {
        base.Exit(enemy);
        enemy.animator.SetBool("Run", false);
        enemy.StopSound();
    }
}
public class ZombieAttack : Att 
{
    public override void Update(Enemy enemy)
    {
        if (GameManager.Instance.player.isHide)
        {
            enemy.HandleInput(EnemyStates.PATROL);
            return;
        }

        if (!enemy.CheckAttackRange() && !isAttacking && HearPlayerSound(enemy, 0.5f).Equals(true))
        {
            enemy.HandleInput(EnemyStates.TRACE);
            return;
        }

        if (!enemy.CheckAttackRange() && !isAttacking && HearPlayerSound(enemy, 0.5f).Equals(false))
        {
            enemy.HandleInput(EnemyStates.PATROL);
            return;
        }

        if (enemy.CanAtt && enemy.CheckAttackRange())
        {
            isAttacking = true;
            if (attCoroutine != null) enemy.StopCoroutine(attCoroutine);
            attCoroutine = enemy.StartCoroutine(AttCoroutine(enemy));
        }
    }

    protected override IEnumerator AttCoroutine(Enemy enemy)
    {
        enemy.audioSource.PlayOneShot(((Zombie)enemy).attackClips[Random.Range(0, ((Zombie)enemy).attackClips.Length)]);

        isAttacking = true;
        enemy.transform.DOLookAt(player.position, 0.1f);
        enemy.CanAtt = false;
        enemy.animator.SetTrigger("Attack");
        ((Zombie)enemy).PlayAttackSound();
        yield return new WaitForSeconds(0.1f);
        enemy.SwitchAttackCollider();
        yield return new WaitForSeconds(0.4f);
        enemy.SwitchAttackCollider();
        yield return new WaitForSeconds(0.6f);
        isAttacking = false;
    }
}
public class ZombieRun : Run {}
public class ZombieHit : Hit {}
public class ZombieDead : Dead 
{
    public override void Enter(Enemy enemy)
    {
        base.Enter(enemy);
        ((Zombie)enemy).PlayDeadSound();
    }
}
public class ZombieAggro : Aggro 
{
    public override void Enter(Enemy enemy)
    {
        base.Enter(enemy);
        enemy.animator.SetBool("Run", true);
        SoundManager.Instance.PlaySound(enemy.transform.position, enemy.patrolSound);
        enemy.PlayRunSound();
        enemy.SetSpeed(enemy.runSpeed);
    }

    public override void Update(Enemy enemy)
    {
        base.Update(enemy);
    }

    public override void Exit(Enemy enemy)
    {
        base.Exit(enemy);
        enemy.animator.SetBool("Run", false); ;
        enemy.StopSound();
    }

}

public class CreatureIdle : Idle 
{
    public override void Enter(Enemy enemy)
    {
        base.Enter(enemy);
        _idleCoroutine = enemy.StartCoroutine(IdleCoroutine(enemy));
    }

    public override void Update(Enemy enemy)
    {
        if (FindPlayer(enemy))
        {
            enemy.HandleInput(EnemyStates.TRACE);
            return;
        }

        if (HearPlayerSound(enemy))
        {
            enemy.HandleInput(EnemyStates.PATROL);
            return;
        }
    }

    public override void Exit(Enemy enemy) 
    {
        base.Exit(enemy);
    }

    protected override IEnumerator IdleCoroutine(Enemy enemy)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));

            float angle = Random.Range(0, 360);

            walkSoundCoroutine = enemy.StartCoroutine("PlayWalkSound");
            enemy.animator.SetBool("Idle", false);
            enemy.animator.SetBool("Walk", true);

            var exludedRooms = Array.FindAll(enemy.currentRoom.sector.rooms, x => x != enemy.currentRoom);
            enemy.SetDestination(exludedRooms[Random.Range(0, exludedRooms.Length)].transform.position, false);

            yield return new WaitUntil(enemy.isArriveDestination);

            enemy.StopCoroutine(walkSoundCoroutine);
            enemy.animator.SetBool("Idle", true);
            enemy.animator.SetBool("Walk", false);
        }
    }
}
public class CreaturePatrol : Patrol 
{
    bool isStay;
    Coroutine stayCoroutine;
    Coroutine searchRoomCoroutine;
    bool isRun = false;

    public override void Enter(Enemy enemy)
    {
        idx = 0;
        isStay = false;
        destinations = MapManager.Instance.playerCurrentRoom.GetNearestRooms().Select(x => x.transform.position).ToList();

        // 방들을 순회하기 위해, 현재 위치나 playerLastPos 주위 방들의 좌표들을 얻어옴.
        if (enemy.state.Equals(EnemyStates.TRACE))
        {
            playerLastPos = player.position;
            enemy.SetDestination(playerLastPos, false);
            enemy.SetSpeed(enemy.runSpeed);
            enemy.animator.SetBool("Run", true);
            enemy.PlayRunSound();
            isRun = true;
        }
        else if (destinations.Count != 0)
        {
            enemy.animator.SetBool("Walk", true);
            enemy.SetSpeed(enemy.walkSpeed);
            enemy.SetDestination(destinations[idx++], false);
            walkSoundCoroutine = enemy.StartCoroutine("PlayWalkSound");
            isRun = false;
        }

        enemy.state = EnemyStates.PATROL;

        //SoundManager.Instance.PlaySound(enemy.transform.position, enemy.patrolSound);
    }

    public override void Update(Enemy enemy)
    {
        base.Update(enemy);

        if (!GameManager.Instance.player.isHide && (FindPlayer(enemy) || HearPlayerSound(enemy, 0.5f)) && !isRun)
        {
            enemy.HandleInput(EnemyStates.TRACE);
            return;
        }

        if (!isStay)
        {
            if (enemy.isArriveDestination())
            {
                if (isRun)
                {
                    enemy.StopSound();
                    enemy.SetSpeed(enemy.walkSpeed);
                    isRun = false;
                }

                if (idx.Equals(destinations.Count))
                {
                    // 모든 방들을 다 순찰했으나 플레이어를 발견하지 못했으므로 Idle 상태로 전환
                    enemy.HandleInput(EnemyStates.IDLE);
                    return;
                }
                else
                {
                    isStay = true;
                    enemy.Stop();
                    stayCoroutine = enemy.StartCoroutine(StayCoroutine((Creature)enemy));
                }
            }
        }
    }

    public override void Exit(Enemy enemy)
    {
        base.Exit(enemy);
        if (stayCoroutine != null)
        {
            enemy.StopCoroutine(stayCoroutine);
            stayCoroutine = null;
        }
        if (searchRoomCoroutine  != null)
        {
            enemy.StopCoroutine(searchRoomCoroutine);
            searchRoomCoroutine = null;
        }
        if (isRun)
        {
            enemy.animator.SetBool("Run", false);
            enemy.StopSound();
            isRun = false;
        }
        enemy.animator.SetBool("Idle", false);
    }

    IEnumerator StayCoroutine(Creature enemy)
    {
        if (walkSoundCoroutine != null)
        {
            enemy.StopCoroutine(walkSoundCoroutine);
            walkSoundCoroutine = null;
        }
        enemy.PlayBreathSound();
        yield return new WaitForSeconds(Random.Range(3, 6));
        walkSoundCoroutine = enemy.StartCoroutine("PlayWalkSound");
        enemy.Walk();

        if (enemy.currentRoom == MapManager.Instance.playerCurrentRoom && !GameManager.Instance.player.isHide)
        {
            yield return searchRoomCoroutine = enemy.StartCoroutine(SearchRoomCoroutine(enemy));
        }

        enemy.SetDestination(destinations[idx++], false);
        isStay = false;
    }

    IEnumerator SearchRoomCoroutine(Enemy enemy)
    {
        BoxCollider roomCollider = enemy.currentRoom.GetComponent<BoxCollider>();
        Vector3 extents = roomCollider.bounds.extents;
        enemy.SetSpeed(enemy.walkSpeed / 2);

        for (int i = 0; i < 2; i++)
        {
            enemy.SetDestination(roomCollider.bounds.center + new Vector3(Random.Range(-extents.x - 0.25f, extents.x + 0.25f), 0, Random.Range(-extents.z - 0.25f, extents.z + 0.25f)), false);
            yield return new WaitUntil(enemy.isArriveDestination);
        }

        for (int i = 3; i > 0; i--)
        {
            Vector3 playerNearPos;
            do
            {
                float seta = Random.Range(0, 2 * Mathf.PI);
                playerNearPos = player.position + i * new Vector3(Mathf.Cos(seta), 0, Mathf.Sin(seta));
            } while (!roomCollider.bounds.Contains(playerNearPos));
            enemy.SetDestination(roomCollider.bounds.center + new Vector3(Random.Range(-extents.x, extents.x), 0, Random.Range(-extents.z, extents.z)), false);
            yield return new WaitUntil(enemy.isArriveDestination);
        }

        enemy.SetSpeed(enemy.walkSpeed);
    }
}
public class CreatureTrace : Trace
{
    enum TraceStates
    {
        SIMPLE,
        DETOUR
    }
    TraceStates traceStates;
    bool _isEndDeTour;
    bool hasShouted = false;

    public override void Enter(Enemy enemy)
    {
        if (!((Creature)enemy).isFind && enemy.state != EnemyStates.Hit)
        {
            ((Creature)enemy).isFind = true;
            enemy.Invoke(() => ((Creature)enemy).isFind = false, 5f);
            SoundManager.Instance.PlayPingSound(SoundManager.Instance.pingAudio);
        }
        _isEndDeTour = false;

        if (enemy.CheckAttackRange() && enemy.CanAtt)
        {
            enemy.HandleInput(EnemyStates.ATT);
            return;
        }

        // Trace 종류: 단순 추적, 우회 (1 / 3 비율로 행동. 단, 전 State가 Attack이었거나 거리가 가까울 시 단순 추적하는 것으로)
        if (enemy.state != EnemyStates.ATT && Vector3.Distance(enemy.transform.position, player.transform.position) >= 20 && Random.Range(0f, 1f) <= 0.25f)
        {
            // 우회 추적
            traceStates = TraceStates.DETOUR;
        }
        else
        {
            // 단순 추적
            traceStates = TraceStates.SIMPLE;
            _isEndDeTour = true;
        }

        if (!hasShouted)
        {
            hasShouted = true;
            enemy.animator.SetTrigger("Shout");
            ((Creature)enemy).PlayShoutSound();
            enemy.StopNav();
            enemy.Invoke(() => { enemy.StartNav(); base.Enter(enemy); }, 2.2f);
            enemy.Invoke(() => hasShouted = false, 5f);
        }
        else
        {
            enemy.StartNav();
            base.Enter(enemy);
        }

        if (traceStates.Equals(TraceStates.DETOUR))
        {
            // 우회 추적
            MapManager.Instance.BuildNavPath(enemy.currentRoom.sector, (enemy.transform.position + player.position) * 0.5f);
            enemy.StartCoroutine(ChangeDetourState());
        }
        else
        {
            enemy.transform.DOLookAt(player.position, 0.5f).OnComplete(() => isLooking = true);
        }

        enemy.state = EnemyStates.TRACE;
        enemy.SwitchAttackCollider();
        enemy.animator.SetBool("Walk", false);
    }

    public override void Update(Enemy enemy)
    {
        //플레이어가 공격 범위안에 있는지, 공격할 수 있는지 체크
        if (enemy.CheckAttackRange() && enemy.CanAtt)
        {
            enemy.HandleInput(EnemyStates.ATT);
            return;
        }

        enemy.SetDestination(player.position, isLooking && FindPlayer(enemy).Equals(true));

        if ((!FindPlayer(enemy) && !HearPlayerSound(enemy, 0.5f) && !enemy.isChaseTime && _isEndDeTour) || GameManager.Instance.player.isHide)
        {
            // 추격 실패. Patrol 상태로 전환.
            enemy.HandleInput(EnemyStates.PATROL);
            return;
        }

        // Detour 상태일 시 일정 시간이 지나거나 플레이어의 거리가 가까워지면 Simple로 전환
        if (traceStates.Equals(TraceStates.DETOUR) && (enemy.getDestinationDistance() <= 15 || _isEndDeTour))
        {
            MapManager.Instance.BuildNavPath(enemy.currentRoom.sector, new Vector3(0, 50, 0));
            traceStates = TraceStates.SIMPLE;
        }
    }

    public override void Exit(Enemy enemy)
    {
        base.Exit(enemy);

        enemy.SwitchAttackCollider();

    }

    IEnumerator ChangeDetourState()
    {
        yield return new WaitForSeconds(4f);
        _isEndDeTour = true;
    }
}
public class CreatureAttack : Att 
{
    public override void Enter(Enemy enemy)
    {
        enemy.state = EnemyStates.ATT;
        enemy.StopNav();
        enemy.transform.DOLookAt(player.position, 0.1f);
        EventManager.Instance.DeadEvent(enemy);
    }

    public override void Update(Enemy enemy)
    {
        
    }
}
public class CreatureRun : Run {}
public class CreatureHit : Hit 
{
    public override void Enter(Enemy enemy)
    {
        enemy.state = EnemyStates.Hit;
        enemy.StopNav();
        enemy.animator.SetTrigger("Hit");

        if (((Creature)enemy).hitCount >= 2 && ((Creature)enemy).runPoint != null)
        {
            coroutine = enemy.StartCoroutine(RunCoroutine(enemy));
        }
        else
        {
            coroutine = enemy.StartCoroutine(TraceCoroutine(enemy));
        }
    }

    IEnumerator RunCoroutine(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);
        enemy.HandleInput(EnemyStates.RUN);
    }
}
public class CreatureDead : Dead {}
public class CreatureAggro : Aggro 
{
    Coroutine walkCoroutine;

    public override void Enter(Enemy enemy)
    {
        base.Enter(enemy);
        enemy.SetSpeed(enemy.walkSpeed);
        enemy.animator.SetBool("Walk", true);
        walkCoroutine = enemy.StartCoroutine("PlayWalkSound");
    }

    public override void Update(Enemy enemy)
    {
        if (!GameManager.Instance.player.isHide && FindPlayer(enemy))
        {
            enemy.HandleInput(EnemyStates.TRACE);
            return;
        }
        base.Update(enemy);
    }

    public override void Exit(Enemy enemy)
    {
        base.Exit(enemy);
        enemy.animator.SetBool("Walk", false);
        enemy.StopCoroutine(walkCoroutine);
    }
}
