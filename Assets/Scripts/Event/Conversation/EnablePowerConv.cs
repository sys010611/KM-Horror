using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePowerConv : ConversationEvent
{
    override protected void Event()
    {
        UIManager.Instance.SetConversation(12, 0);
        this.GetComponent<BoxCollider>().enabled = false;
    }
}
