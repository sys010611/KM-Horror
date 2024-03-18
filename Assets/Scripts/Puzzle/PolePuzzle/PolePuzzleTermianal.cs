using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class PolePuzzleTerminal : Item
{
    [SerializeField]
    protected Transform LeftPole;
    [SerializeField]
    protected Transform RightPole;
    protected bool isRotating = false;

    private static bool isFirstEncounter = true;

    [SerializeField] 
    protected AudioClip sound;

    protected override void Get()
    {
        if (!isRotating)
        {
            GameManager.Instance.player.interActionQueue.Enqueue(Get);
            StartCoroutine(Rotate());
        }

        if(isFirstEncounter && GameManager.Instance.HasPlayerReadPuzzleInfo)
        {
            isFirstEncounter = false;
            UIManager.Instance.SetConversation(14, 0);
        }
    }

    protected abstract IEnumerator Rotate();
}
