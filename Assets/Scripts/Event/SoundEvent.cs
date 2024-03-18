using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEvent : Event
{
    [SerializeField]
    private AudioClip sound;

    protected override void StartEvent()
    {
        SoundManager.Instance.PlayMainBGM(sound, true);
    }
}
