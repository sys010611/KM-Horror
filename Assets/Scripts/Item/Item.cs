using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Item : MonoBehaviour
{
    [SerializeField]
    protected AudioClip equipSound;
    private static bool showHotbarTooltip; //인벤토리 툴팁
    protected bool isInteractive;

    private void Awake()
    {
        showHotbarTooltip = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isInteractive)
        {
            GameManager.Instance.player.interActionQueue.Enqueue(Get);
            if (GetType() == typeof(KeycardReader))
                UIManager.Instance.SetInteractionText("OPEN");
            else if (this.CompareTag("Letter"))
                UIManager.Instance.SetInteractionText("READ");
            else if (this.CompareTag("Puzzle"))
                UIManager.Instance.SetInteractionText("USE");
            else
                UIManager.Instance.SetInteractionText("GET");
            UIManager.Instance.ActivateInteractionUI();
            isInteractive = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != Get));
            UIManager.Instance.DeactivateInteractionUI();
            isInteractive = false;
        }
    }

    private void OnDisable() //아이템을 획득하면 상호작용UI 해제
    {
        if (UIManager.Instance == null)
            return;

        UIManager.Instance.DeactivateInteractionUI();
        GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != Get));

        if (showHotbarTooltip)
        {
            UIManager.Instance.SetToolTip("Press TAB to check inventory");
            showHotbarTooltip = false;
        }
    }

    protected abstract void Get();
}
