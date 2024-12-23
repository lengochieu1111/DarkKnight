using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Pool;
using HIEU_NL.DesignPatterns.ObjectPool.Single;
using HIEU_NL.DesignPatterns.Singleton;

namespace HIEU_NL.ObjectPool.Audio
{
    public class SoundPool : PersistentSingleObjectPool <SoundEmitter, SoundData, SoundBuilder>
    {
        protected override SoundBuilder GetNewPoolBuilder(SoundEmitter prefab, SoundData data)
        {
            return new SoundBuilder(prefab, data);

        }

    }

}

