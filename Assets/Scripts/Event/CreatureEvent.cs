using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEvent : Event
{
    [SerializeField]
    private GameObject creature;
    [SerializeField]
    private AudioClip audioClip1;
    [SerializeField]
    private AudioClip audioClip2;
    [SerializeField]
    private BoxCollider[] neighborColliders;
    [SerializeField]
    private float distance;

    protected override void StartEvent()
    {
        for (int i = 0; i < neighborColliders.Length; i++)
        {
            neighborColliders[i].enabled = false;
        }

        creature.SetActive(true);
        creature.GetComponent<Animator>().SetBool("Walk", true);
        EventManager.Instance.CreatrueEvent(creature, distance, audioClip1, audioClip2);
    }
}
