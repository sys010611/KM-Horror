using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCreatureEvent : Event
{
    [SerializeField]
    private GameObject creature;

    protected override void StartEvent()
    {
        creature.SetActive(true);
        SoundManager.Instance.PlayMainBGM(SoundManager.Instance.sameSectorBGM);
    }
}
