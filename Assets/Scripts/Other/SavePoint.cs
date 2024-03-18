using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private AudioClip saveSound;
    bool isInteractive;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.player.interActionQueue.Enqueue(Save);
            UIManager.Instance.SetInteractionText("SAVE");
            UIManager.Instance.ActivateInteractionUI();
            isInteractive = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != Save));
            UIManager.Instance.DeactivateInteractionUI();
            isInteractive = false;
        }
    }

    private void Save()
    {
        isInteractive = false;
        UIManager.Instance.DeactivateInteractionUI();
        SaveManager.Instance.Save(GameManager.Instance.inventory, GameManager.Instance.weaponManager, MapManager.Instance.PlayerCurrentSector.sceneName, MapManager.Instance.PlayerCurrentSector.neighborSectorSceneName);
        SoundManager.Instance.PlaySFX(saveSound);
        UIManager.Instance.SetConversation("Your current location information has been saved.");
    }
}
