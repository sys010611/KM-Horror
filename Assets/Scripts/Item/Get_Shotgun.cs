using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Shotgun : Item
{
    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.weaponManager.weaponList[5] = true;
        UIManager.Instance.hotBar.GetItme(5);
        UIManager.Instance.SetConversation(5, 0);
        gameObject.SetActive(false);
    }
}
