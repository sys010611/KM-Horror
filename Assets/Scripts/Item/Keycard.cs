using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : Item
{
    private Inventory inventory;

    private void Start()
    {
        inventory = GameManager.Instance.player.GetComponent<Inventory>();
    }

    protected override void Get()
   {
        SoundManager.Instance.PlaySFX(equipSound);

        if (gameObject.CompareTag("Keycard_Red"))
        {
            inventory.keycard_Red++;
            UIManager.Instance.hotBar.GetKeycard(0);
        }
        else if (gameObject.CompareTag("Keycard_Yellow"))
        {
            inventory.keycard_Yellow++;
            UIManager.Instance.hotBar.GetKeycard(1);
        }
        else if (gameObject.CompareTag("Keycard_Blue"))
        {
            inventory.keycard_Blue++;
            UIManager.Instance.hotBar.GetKeycard(2);
        }

        gameObject.SetActive(false);
   }
}
