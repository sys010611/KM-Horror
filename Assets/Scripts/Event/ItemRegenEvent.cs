using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRegenEvent : Event
{
    private Item[] items;

    private void Awake()
    {
        items = FindObjectsOfType<Item>(true);
    }

    protected override void StartEvent()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(true);
        }
    }
}
