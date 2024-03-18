using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Events;

public class KeycardReader : Item
{
    private Inventory inventory;

    public Transform L_door; //왼쪽 문
    public Transform R_door; // 오른쪽 문

    public UnityEvent OpenDoorEvent = new UnityEvent();

    [SerializeField]
    private AudioClip openSound;
    [SerializeField]
    private AudioClip closeSound1;
    [SerializeField]
    private AudioClip closeSound2;

    [SerializeField]
    private AudioClip successSound;
    [SerializeField]
    private AudioClip errorSound;

    BoxCollider boxCollider;

    public bool LightDoor = false;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        inventory = GameManager.Instance.player.GetComponent<Inventory>();
        OpenDoorEvent.AddListener(OpenDoor);
    }

    protected override void Get()
    {
        
        UIManager.Instance.DeactivateInteractionUI();

        if (gameObject.CompareTag("Keycard_Red") && inventory.keycard_Red != 0) //A카드키
        {
            KeycardVerified();
        }
        else if (gameObject.CompareTag("Keycard_Blue") && inventory.keycard_Blue != 0) //C카드키
        {
            KeycardVerified();
        }
        else if (gameObject.CompareTag("Keycard_Green") && inventory.keycard_Green != 0)
        {
            KeycardVerified();
        }
        else if (gameObject.CompareTag("Keycard_Yellow") && inventory.keycard_Yellow != 0) //B카드키
        {
            if (!GameManager.Instance.islighton && LightDoor) //불이 꺼져있고 트리거 도어일때
            {
                UIManager.Instance.SetConversation("The door's power system is off");
                //사운드는 비활성 상태이기 때문에 소리가 안남
            }
            else 
            {
                KeycardVerified();
            } 
        }
        else
        {
            SoundManager.Instance.PlaySFX(errorSound);
            UIManager.Instance.SetConversation("You don't have keycard for this door");
        }
    }

    private void KeycardVerified()
    {
        SoundManager.Instance.PlaySFX(successSound);

        bool allLoaded = true;
        foreach(Sector sector in FindObjectsOfType<Sector>())
        {
            if (sector?.isLoading == true)
            {
                allLoaded = false;
            }
        }

        //이미 로딩 완료 시: 바로 문 열기
        //로딩 미완료 시: 게임매니저에 등록만 해두고 Sector의 이벤트를 통해 OpenDoor 수행
        if (allLoaded)
            OpenDoor();
        else
            GameManager.Instance.CurrentOpeningDoor = this;
    }

    private void OpenDoor()
    {
        GameManager.Instance.CurrentOpeningDoor = null;

        var l = L_door.transform.localPosition.y;
        var r = R_door.transform.localPosition.y;

        boxCollider.enabled = false;

        DOTween.Sequence()
                //.AppendInterval(2f)
                .AppendCallback(() => SoundManager.Instance.PlaySound(transform.position, openSound))
                .Append(L_door.DOLocalMoveY(0.55f, 2f))
                .Join(R_door.DOLocalMoveY(-0.08f, 2f))
                .AppendInterval(20f)
                .AppendCallback(() => SoundManager.Instance.PlaySound(transform.position, closeSound1))
                .Append(L_door.DOLocalMoveY(l, 2f))
                .Join(R_door.DOLocalMoveY(r, 1.9f))
                .AppendCallback(() => boxCollider.enabled = true)
                .InsertCallback(27.7f, () => SoundManager.Instance.PlaySound(transform.position, closeSound2));
    }
}
