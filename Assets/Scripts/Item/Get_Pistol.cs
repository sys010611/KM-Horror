using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Pistol : Item
{
    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.weaponManager.weaponList[4] = true;
        UIManager.Instance.hotBar.GetItme(4);
        gameObject.SetActive(false);
    }
}
