using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Enemy> activatedCreatureList = new List<Enemy>();
    public List<Enemy> activatedZombieList = new List<Enemy>();
    public PlayerControl player;
    private Inventory _inventory;
    public bool islighton; //라이트 활성 체크 변수

    private KeycardReader currentOpeningDoor; //현재 플레이어가 열고자 하는 문
    public KeycardReader CurrentOpeningDoor { set{ currentOpeningDoor = value; } }

    public UnityAction OpenDoorAction;
    public UnityAction ResetHotbar;

    /// <summary>
    /// 플레이어가 내는 소리 추가치 (기본값: 0)
    /// </summary>
    public float playerSoundIncrement = 0;

    public Inventory inventory
    { 
        get 
        {
            if (_inventory == null)
                _inventory = FindObjectOfType<Inventory>(true);
            return _inventory; 
        }
        set => _inventory = value;
    }
    public WeaponManager weaponManager;
    public bool difficult;

    public enum PasswordType
    {
        NONE        = 0,
        SHOTGUN     = 2,
        ENDING_ITEM = 3,
        PICTURE     = 4,
    }
    //비밀번호를 알고있는지 여부 저장, 열거형 번호가 hashSet에 그대로 들어감
    public HashSet<PasswordType> checkedPasswords = new HashSet<PasswordType>();

    private bool hasPlayerReadPuzzleInfo = false; // 퍼즐 설명 편지 읽었는지 여부
    public bool HasPlayerReadPuzzleInfo { get { return hasPlayerReadPuzzleInfo; } set { hasPlayerReadPuzzleInfo = value; } } // 해당 프로퍼티

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OpenDoorAction += OpenDoor;
    }

    // 플레이어의 상태에 따라 적들이 감지하는 거리 리턴
    public float GetPlayerSound()
    {
        switch (player.PState)
        {
            case PlayerControl.Playerstates.IDLE:
                return 0 + playerSoundIncrement;
            case PlayerControl.Playerstates.Walk:
                return 15 + playerSoundIncrement;
            case PlayerControl.Playerstates.Run:
                return 40 + playerSoundIncrement;
            case PlayerControl.Playerstates.Crouch:
                return 6 + playerSoundIncrement;
            case PlayerControl.Playerstates.Jump:
                return 20 + playerSoundIncrement;
            case PlayerControl.Playerstates.InterAction:
                return 1 + playerSoundIncrement;
            case PlayerControl.Playerstates.Attack:
                return 30 + playerSoundIncrement;
            case PlayerControl.Playerstates.Shot:
                return 40 + playerSoundIncrement;
            default:
                return -1 + playerSoundIncrement;
        }
    }

    public bool CheckEnemySeeingPlayer()
    {
        for (int i = 0; i < activatedCreatureList.Count; i++)
            if (activatedCreatureList[i].FindPlayer())
                return true;

        return false;
    }

    private void OpenDoor()
    {
        if (currentOpeningDoor is null)
            return;
        currentOpeningDoor.OpenDoorEvent.Invoke();
    }
}

public static class Utility
{
    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}
