using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlaskEvent : Event
{
    [SerializeField]
    private Creature creature;

    [SerializeField]
    private Transform flaskPos;
    [SerializeField]
    private AudioClip flaskAudio;

    protected override void StartEvent()
    {
        EventManager.Instance.FlaskEvent(flaskPos.position, flaskAudio, creature);
    }
}
