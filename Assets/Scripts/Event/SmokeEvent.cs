using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEvent : Event
{
    [SerializeField]
    private ParticleSystem[] smokes;
    [SerializeField]
    private AudioClip audioClip;

    protected override void StartEvent()
    {
        EventManager.Instance.SmokeEvent(smokes[0].transform.position, audioClip, smokes);
    }
}
