using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;

public class HidableLocker : MonoBehaviour
{
    [SerializeField]
    private GameObject lockerDoor;
    [SerializeField]
    private Transform inPlace;
    [SerializeField]
    private Transform outPlace;
    [SerializeField]
    private BoxCollider doorCollider;

    [SerializeField]
    private AudioClip openAudio;
    [SerializeField]
    private AudioClip closeAudio;

    [SerializeField]
    private AudioClip hartSound;

    private bool isInteract;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteract)
        {
            UIManager.Instance.SetInteractionText("HIDE");
            UIManager.Instance.ActivateInteractionUI();
            GameManager.Instance.player.interActionQueue.Enqueue(Hide);
            isInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != Hide && x != Exit));
            UIManager.Instance.DeactivateInteractionUI();
            isInteract = false;
        }

    }

    private void Hide()
    {
        EventManager.Instance.deadAction += () => { lockerDoor.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f); };

        var player = GameManager.Instance.player;
        player.StopWalkSound();
        player.StopRunSound();
        player.weaponManager.toBareHands();

        bool successHide = true;
        var sequence = DOTween.Sequence();
        Action f = () => { sequence.Kill(); };
        sequence.OnStart(() => { player.CanControl = false; successHide = true; doorCollider.enabled = false; EventManager.Instance.deadAction += f; })
            .OnPlay(() =>
            {
                if (GameManager.Instance.CheckEnemySeeingPlayer() && successHide)
                    successHide = false;
            })
            .AppendCallback(() => SoundManager.Instance.PlaySFX(openAudio))
            .Append(lockerDoor.transform.DOLocalRotate(new Vector3(0, 110, 0), 0.5f))
            .AppendInterval(0.1f)
            .Append(player.transform.DOMove(inPlace.position, 0.5f))
            .Append(player.transform.DOLocalRotate(lockerDoor.transform.rotation.eulerAngles, 0.5f))
            .AppendCallback(() => SoundManager.Instance.PlaySFX(closeAudio))
            .Append(lockerDoor.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f))
            .OnComplete(() => 
            { 
                player.isHide = successHide;
                player.PState = PlayerControl.Playerstates.IDLE;
                GameManager.Instance.player.interActionQueue.Enqueue(Exit); 
                EventManager.Instance.deadAction -= f;
                SoundManager.Instance.PlaySubSubBGM(hartSound, true);
                UIManager.Instance.SetInteractionText("EXIT");
            });
    }

    private void Exit()
    {
        var player = GameManager.Instance.player;

        DOTween.Sequence()
            .AppendCallback(() => SoundManager.Instance.PlaySFX(openAudio))
            .Append(lockerDoor.transform.DOLocalRotate(new Vector3(0, 110, 0), 0.5f))
            .AppendInterval(0.1f)
            .Append(player.transform.DOMove(outPlace.position, 0.5f))
            .Append(player.transform.DOLocalRotate(lockerDoor.transform.rotation.eulerAngles + new Vector3(0, 180, 0), 0.5f))
            .AppendCallback(() => { SoundManager.Instance.PlaySFX(closeAudio); SoundManager.Instance.StopSubSubBgm(); })
            .Append(lockerDoor.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f))
            .OnComplete(() =>
            {
                player.CanControl = true;
                player.isHide = false;
                doorCollider.enabled = true;
                EventManager.Instance.deadAction = null;
                GameManager.Instance.player.interActionQueue.Enqueue(Hide);
                UIManager.Instance.SetInteractionText("HIDE");
            });
    }
}
