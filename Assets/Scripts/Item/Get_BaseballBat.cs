using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_BaseballBat : Item
{
    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.weaponManager.weaponList[1] = true;
        UIManager.Instance.hotBar.GetItme(1);
        gameObject.SetActive(false);
    }
}
