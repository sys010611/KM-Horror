using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Flask : Item
{
    private Inventory inventory;
    private static bool showFlaskTooltip;

    private void Awake()
    {
        showFlaskTooltip = true;
    }

    private void Start()
    {
        inventory = GameManager.Instance.player.GetComponent<Inventory>();
    }

    protected override void Get()
    {
        if (showFlaskTooltip)
        {
            UIManager.Instance.SetToolTip("Press G to throw flask \n\nYou can distract enemies by throwing flask");
            showFlaskTooltip = false;
        }
        GameManager.Instance.player.PState = PlayerControl.Playerstates.InterAction;
        SoundManager.Instance.PlaySFX(equipSound);
        inventory.Flask++;
        gameObject.SetActive(false);
    }
}
