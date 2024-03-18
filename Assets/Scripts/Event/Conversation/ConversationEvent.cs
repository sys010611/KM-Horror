using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConversationEvent : MonoBehaviour
{
    protected int id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Event();
        }
    }

    protected abstract void Event();
}
