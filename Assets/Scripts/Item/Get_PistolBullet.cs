using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_PistolBullet : Item
{
    private Inventory inventory;
    private void Start()
    {
        inventory = GameManager.Instance.player.GetComponent<Inventory>();
    }

    protected override void Get()
    {
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        inventory.PistolBullet++;
        gameObject.SetActive(false);
    }
}
