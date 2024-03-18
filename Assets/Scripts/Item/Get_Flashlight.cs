using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Flashlight : Item
{
    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.weaponManager.weaponList[0] = true;
        UIManager.Instance.hotBar.GetItme(0);
        gameObject.SetActive(false);
    }
}
