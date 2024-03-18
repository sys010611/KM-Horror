using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraningEvent : Event
{
    [SerializeField]
    private GameObject[] lights;
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private bool isLong = false;

    protected override void StartEvent()
    {
        EventManager.Instance.WarningEvent(this.transform.position, lights, audioClip, isLong);
    }
}
