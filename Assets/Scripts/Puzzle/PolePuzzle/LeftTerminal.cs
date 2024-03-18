using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LeftTerminal : PolePuzzleTerminal
{
    protected override IEnumerator Rotate()
    {
        isRotating = true;

        SoundManager.Instance.PlaySound(transform.position, sound);

        Vector3 leftPoleRotation = LeftPole.localEulerAngles; //현재 각도 계산
        Vector3 rightPoleRotation = RightPole.localEulerAngles;
        leftPoleRotation += new Vector3(0, 179.99f, 0); //시계방향 회전 (179.99 -> 역회전 방지)
        rightPoleRotation += new Vector3(0, 90, 0);
        LeftPole.DOLocalRotate(leftPoleRotation, 1f).SetEase(Ease.Linear);
        RightPole.DOLocalRotate(rightPoleRotation, 1f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(1f);

        isRotating = false;
        if (isInteractive)
        {
            GameManager.Instance.player.interActionQueue.Enqueue(Get);
        }
    }
}
