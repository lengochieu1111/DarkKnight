using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Pool;
using HIEU_NL.DesignPatterns.ObjectPool.Single;

namespace HIEU_NL.ObjectPool.Audio
{
    public class SoundBuilder : PoolBuilder<SoundEmitter, SoundData>
    {
        private bool _randomPitch;

        public SoundBuilder(SoundEmitter prefab, SoundData data, Vector3 position = default, Quaternion rotation = default, Transform parent = null) : base(prefab, data, position, rotation, parent)
        {
        }

        public SoundBuilder WithRandomPitch()
        {
            this._randomPitch = true;
            return this;
        }

        protected override void SetupPoolObject()
        {
            base.SetupPoolObject();

            if (_randomPitch)
            {
                _prefab.WithRandomPitch();
            }
        }

    }

}

