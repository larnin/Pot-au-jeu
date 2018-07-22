using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlaySoundEvent : EventArgs
{
    public PlaySoundEvent(AudioClip _clip)
    {
        clip = _clip;
    }

    public AudioClip clip;
}
