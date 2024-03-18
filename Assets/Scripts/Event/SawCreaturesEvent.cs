using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawCreaturesEvent : MonoBehaviour
{
    [SerializeField] CreatureEscapeEvent creatureEscapeEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            creatureEscapeEvent.playerSawCreature = true;

            Destroy(this);
        }
    }
}
