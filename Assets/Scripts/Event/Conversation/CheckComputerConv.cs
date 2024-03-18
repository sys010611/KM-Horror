using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckComputerConv : ConversationEvent
{
    override protected void Event()
    {
        UIManager.Instance.SetConversation(11, 0);
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
