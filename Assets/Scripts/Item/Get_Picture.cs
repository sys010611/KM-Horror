using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Get_Picture : Item
{
    [SerializeField]
    PicturePuzzle.PICTURE pictureInfo;

    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        switch (pictureInfo)
        {
            case PicturePuzzle.PICTURE.PICTURE1:
                ++GameManager.Instance.inventory.picture1;
                break;
            case PicturePuzzle.PICTURE.PICTURE2:
                ++GameManager.Instance.inventory.picture2;
                break;
            case PicturePuzzle.PICTURE.PICTURE3:
                ++GameManager.Instance.inventory.picture3;
                break;
            case PicturePuzzle.PICTURE.PICTURE4:
                ++GameManager.Instance.inventory.picture4;
                break;
        }
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (UIManager.Instance == null)
            return;

        UIManager.Instance.DeactivateInteractionUI();
        foreach (PictureHint pictureHint in Array.FindAll(FindObjectsOfType<PictureHint>(), x => x.pictureType == pictureInfo))
        {
            pictureHint.gameObject.SetActive(false);
        }
        //UIManager의 DisablePictureIconOnMap(), HotBarUI의 SetPictureText() 델리게이션
        UIManager.Instance.OnGetPicture.Invoke((int)pictureInfo);
    }
}
