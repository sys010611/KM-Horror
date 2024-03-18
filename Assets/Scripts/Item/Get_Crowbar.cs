using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Crowbar : Item
{
    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.weaponManager.weaponList[2] = true;
        UIManager.Instance.hotBar.GetItme(2);
        gameObject.SetActive(false);
    }
}
