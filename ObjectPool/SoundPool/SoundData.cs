using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HIEU_NL.DesignPatterns.ObjectPool.Single;
using System;
using UnityEngine.Audio;

namespace HIEU_NL.ObjectPool.Audio
{
    [Serializable]
    public class SoundData : PoolData
    {
        [field: SerializeField] public AudioClip Clip { get; private set; }
        [field: SerializeField] public AudioMixerGroup MixerGroup { get; private set; }
        [field:SerializeField] public bool Loop { get; private set; }
        [field:SerializeField] public bool PlayOnAwake { get; private set; }
    }

}

