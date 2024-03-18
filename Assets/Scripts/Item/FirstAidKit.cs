using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidKit : Item
{
    private Inventory inventory;

    private void Start()
    {
        inventory = GameManager.Instance.player.GetComponent<Inventory>();
    }

    protected override void Get()
    {
        SoundManager.Instance.PlaySFX(equipSound);
        inventory.FirstAidKit++;
        gameObject.SetActive(false);
    }
}
