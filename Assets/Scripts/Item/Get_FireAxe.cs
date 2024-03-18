using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_FireAxe : Item
{
    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.weaponManager.weaponList[3] = true;
        UIManager.Instance.hotBar.GetItme(3);
        gameObject.SetActive(false);
    }
}
