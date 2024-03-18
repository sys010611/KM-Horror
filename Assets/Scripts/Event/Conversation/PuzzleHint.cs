using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleHint : ConversationEvent
{
    private void Start()
    {
        id = 0;
    }

    protected override void Event()
    {
        UIManager.Instance.SetConversation(id, 0);
        UIManager.Instance.SetToolTip("Press M to open minimap");
    }
}
