using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 이벤트들 관리하는 Manager 클래스. 각종 이벤트들 실행
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private void Awake()
    {
        Instance = this;
    }

#if UNITY_EDITOR
    // 순간이동 테스용 코드들 키패드 1 ~ 9로 순간이동
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_WayOut", "PlayScene_1F_Room V1 (2)", "PlayScene_1F_Room V1" }, 
                unload, 
                new Vector3(-55, 0, 71)));
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_Room V1 (2)", "PlayScene_1F_WayOut", "PlayScene_1F_Rest" },
                unload, 
                new Vector3(-3, 0, 72)));
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_RoomV2 (1)", "PlayScene_1F_Stair" }, 
                unload, 
                new Vector3(100, 0, 79)));
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_Room V1", "PlayScene_1F_WayOut", "PlayScene_1F_RoomV2" }, 
                unload, 
                new Vector3(-80, 0, 0)));
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_Rest", "PlayScene_1F_Room V1 (2)", "PlayScene_1F_RoomV3" }, 
                unload, 
                new Vector3(1,0,1)));
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_Stair", "PlayScene_1F_RoomV2 (1)", "PlayScene_1F_Room V1 (1)" }, 
                unload, 
                new Vector3(83, 0, -20)));
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_RoomV2", "PlayScene_1F_Room V1" }, 
                unload, 
                new Vector3(-67, 0, -91)));
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_RoomV3", "PlayScene_1F_Rest", "PlayScene_1F_Room V1 (1)" }, 
                unload, 
                new Vector3(0,0,-72)));
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            string[] unload = new string[MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName.Length + 1];
            unload[0] = MapManager.Instance.PlayerCurrentSector.sceneName;
            for (int i = 1; i < unload.Length; i++)
                unload[i] = MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName[i - 1];

            MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(
                new string[] { "PlayScene_1F_Room V1 (1)", "PlayScene_1F_RoomV3", "PlayScene_1F_Stair" }, 
                unload, 
                new Vector3(79, 0, -72)));
        }
    }
#endif

    // 크리쳐한테 죽을 때
    public Action deadAction;
    public void DeadEvent(Enemy enemy)
    {
        PlayerControl player = GameManager.Instance.player;

        deadAction?.Invoke();
        SoundManager.Instance.StopSubSubBgm();
        SoundManager.Instance.StopSubBgm();
        SoundManager.Instance.StopMainBgm();
        player.CanControl = false;
        enemy.animator.SetTrigger("Attack");
        SoundManager.Instance.PlaySFX(SoundManager.Instance.crackAudio);
        if (player.PState == PlayerControl.Playerstates.Crouch)
        {
            player.transform.localScale = new Vector3(1, 0.94f, 1);
        }
        player.transform.DOLookAt(enemy.transform.position + Vector3.up * 0.6f, 800).SetSpeedBased();

        //GameManager.Instance.player.transform.DOMove(enemy.transform.position + enemy.transform.forward * 3.3f, 50).SetSpeedBased();
        this.Invoke(() => { SoundManager.Instance.PlaySFX(SoundManager.Instance.bloodyWoundSound); }, 0.8f);
        this.Invoke(() => { player.Hitfeedback(); SoundManager.Instance.PlaySFX(SoundManager.Instance.headExplosionSound); }, 1.5f);
        this.Invoke(() => 
        DOTween.Sequence()
            .AppendCallback(() => UIManager.Instance.FadeImage())
            .Append(player.transform.DOLocalRotate(new Vector3(0, 0, 90), 1f).SetRelative())
            .AppendInterval(3f)
            .AppendCallback(() => SceneManager.LoadScene("MainScene")), 1.5f);        
    }

    // 좀비한테 죽을 때
    public void DeadEvent(PlayerControl player)
    {
        deadAction?.Invoke();
        SoundManager.Instance.StopSubBgm();
        SoundManager.Instance.StopMainBgm();
        GameManager.Instance.player.CanControl = false;
        DOTween.Sequence()
            .AppendCallback(() => UIManager.Instance.FadeImage())
            .Append(player.transform.DOLocalRotate(new Vector3(0, 0, 90), 1f).SetRelative())
            .AppendInterval(3f)
            .AppendCallback(() => SceneManager.LoadScene("MainScene"));
    }

    // 경보 발생 이벤트
    public void WarningEvent(Vector3 pos, GameObject[] lights, AudioClip audioClip, bool isLong = false)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(true);
        }

        if(!isLong)
        {
            this.Invoke(() =>
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].SetActive(false);
                }
            }, audioClip.length * 4);
        }

        SoundManager.Instance.PlaySFX(SoundManager.Instance.pingAudio);

        if(!isLong)
            SoundManager.Instance.PlaySoundRepeat(pos, audioClip, 4);
        else
            SoundManager.Instance.PlaySoundRepeat(pos, audioClip, 6);

        for (int i = 0; i < GameManager.Instance.activatedCreatureList.Count; i++)
        {
            GameManager.Instance.activatedCreatureList[i].SetAggro(pos);
        }

        for (int i = 0; i < GameManager.Instance.activatedZombieList.Count; i++)
        {
            GameManager.Instance.activatedZombieList[i].SetAggro(pos);
        }
    }

    public void CreatrueEvent(GameObject creature, float distance, AudioClip audioClip1, AudioClip audioClip2)
    {
        creature.transform.DOLocalMoveZ(-distance, 5).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() => creature.SetActive(false));
        SoundManager.Instance.PlaySFX(audioClip1);
        SoundManager.Instance.PlaySFX(audioClip2);
    }

    public void FlaskEvent(Vector3 pos, AudioClip audioClip, Creature creature)
    {
        SoundManager.Instance.PlaySound(pos, audioClip);

        creature.eventPos = pos;
        creature.gameObject.SetActive(true);
        creature.OnEnableEvent += creature.SetAggro;
        this.Invoke(() => creature.gameObject.SetActive(false), 30);
    }

    public void SmokeEvent(Vector3 pos, AudioClip audioClip, ParticleSystem[] smokes)
    {
        for (int i = 0; i < smokes.Length; i++)
        {
            smokes[i].Play();
        }
        SoundManager.Instance.PlaySound(pos, audioClip);
        this.Invoke(() =>
        {
            for (int i = 0; i < smokes.Length; i++)
            {
                smokes[i].Stop();
            }
        }, audioClip.length);

        //for (int i = 0; i < GameManager.Instance.activatedCreatureList.Count; i++)
        //{
        //    GameManager.Instance.activatedCreatureList[i].SetAggro(pos);
        //}
    }

    public void PlayerCreatureInSameSectorEvent()
    {
        SoundManager.Instance.PlayMainBGM(SoundManager.Instance.sameSectorBGM, false, true);
    }

    private bool sameRoomCoolTime = false;
    public void PlayerCreatureInSameRoomEvent()
    {
        if (!sameRoomCoolTime)
        {
            sameRoomCoolTime = true;
            SoundManager.Instance.PlayMainBGM(SoundManager.Instance.suspenseSound, true);
            this.Invoke(() => sameRoomCoolTime = false, 30);
        }
    }

    private bool seeingCreatureCoolTime = false;
    public void PlayerSeeCreatureEvent()
    {
        if (!seeingCreatureCoolTime)
        {
            if (!GameManager.Instance.player.isHide && !GameManager.Instance.activatedCreatureList.Find(x => x.currentRoom == MapManager.Instance.playerCurrentRoom))
            {
                seeingCreatureCoolTime = true;
                SoundManager.Instance.PlaySFX(SoundManager.Instance.hitDarkSound);
            }
        }
    }

    public void CreatureEscapeEvent(Vector3 soundPos, AudioClip vatCrackSound, AudioClip walkingSound, AudioClip suspense, GameObject creature, GameObject vat)
    {
        SoundManager.Instance.PlaySound(soundPos, vatCrackSound);
        this.Invoke(() => SoundManager.Instance.PlaySoundRepeat(soundPos, walkingSound, 8), 0.3f);
        this.Invoke(() => SoundManager.Instance.PlaySFX(suspense), 0.3f);
        
        creature.SetActive(false);
        vat.transform.GetChild(0).gameObject.SetActive(false);
        vat.transform.GetChild(1).gameObject.SetActive(false);
        vat.transform.Rotate(new Vector3(-15f, 180f, 0f), Space.Self);
    }
}
