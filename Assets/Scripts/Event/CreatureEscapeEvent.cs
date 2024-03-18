using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEscapeEvent : Event
{
    [SerializeField] GameObject creature;
    [SerializeField] GameObject vat;
    [SerializeField] AudioClip vatCrackAudio;
    [SerializeField] AudioClip walkingAudio;
    [SerializeField] AudioClip suspense;

    public bool playerSawCreature = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && playerSawCreature)
            StartEvent();
    }

    protected override void StartEvent()
    {
        Vector3 soundPos = creature.transform.position;

        EventManager.Instance.CreatureEscapeEvent(soundPos, vatCrackAudio, walkingAudio, suspense, creature, vat);

        Destroy(this);
    }
}
