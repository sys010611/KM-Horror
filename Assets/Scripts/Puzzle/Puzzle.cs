using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    protected bool isClear;

    [SerializeField]
    protected AudioClip successSound;
    [SerializeField]
    protected AudioClip failureSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isClear)
        {
            GameManager.Instance.player.interActionQueue.Enqueue(EnablePuzzle);
            UIManager.Instance.SetInteractionText("USE");
            UIManager.Instance.ActivateInteractionUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isClear)
        {
            GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != EnablePuzzle));
            UIManager.Instance.DeactivateInteractionUI();
        }
    }

    protected virtual void EnablePuzzle()
    {
        GameManager.Instance.player.interActionQueue.Enqueue(EnablePuzzle);
        UIManager.Instance.currentPuzzle = this;
    }
}
