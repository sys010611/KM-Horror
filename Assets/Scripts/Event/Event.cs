using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event : MonoBehaviour
{
    private BoxCollider eventCollider;

    private void Start()
    {
        eventCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartEvent();
            eventCollider.enabled = false;
        }
    }

    protected abstract void StartEvent();
}
