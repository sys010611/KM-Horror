using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PicturePuzzle : Puzzle
{
    public enum PICTURE
    {
        PICTURE1    = 1,
        PICTURE2    = 2,
        PICTURE3    = 3,
        PICTURE4    = 4
    }

    [SerializeField]
    private GameObject pictureObj;
    [SerializeField]
    private PICTURE pictureType;

    public bool pictureInserted = false;

    protected override void EnablePuzzle()
    {
        base.EnablePuzzle();

        int count = 0;
        switch (pictureType)
        {
            case PICTURE.PICTURE1:
                count = GameManager.Instance.inventory.picture1;
                break;
            case PICTURE.PICTURE2:
                count = GameManager.Instance.inventory.picture2;
                break;
            case PICTURE.PICTURE3:
                count = GameManager.Instance.inventory.picture3;
                break;
            case PICTURE.PICTURE4:
                count = GameManager.Instance.inventory.picture4;
                break;
        }

        if (count > 0 /* 플레이어가 그림 아이템을 갖고있는지 확인 */)
        {
            GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != EnablePuzzle));
            UIManager.Instance.DeactivateInteractionUI();

            pictureObj.SetActive(true);
            Vector3 pos = pictureObj.transform.position;
            pictureObj.transform.DOLocalMoveZ(-5, 1f).From(true);
            pictureInserted = true;
            PicturePuzzleSystem.Instance.checkIfAllInserted();
            SoundManager.Instance.PlaySFX(successSound);
            isClear = true;
        }
        else
        {
            UIManager.Instance.SetConversation("You don't have the corresponding picture.");
        }
    }
}
