using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Syringe : Item
{
    protected override void Get()
    {
        UIManager.Instance.SetConversation(10,0);

        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.inventory.syringe++;
        gameObject.SetActive(false);
    }
}
