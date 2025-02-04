using System.Collections;
using UnityEngine;
using System.Linq;
using HIEU_NL.DesignPatterns.ObjectPool.Single;
using HIEU_NL.ObjectPool.Audio;
using NaughtyAttributes;

namespace HIEU_NL.Manager
{
    public class SoundManager : PersistentSingleObjectPool <SoundEmitter, SoundData, SoundBuilder>
    {
        [SerializeField, BoxGroup("SOUND DATA")] private SoundDataListSO _soundDataListSO;

        public void PlaySound(SoundType soundType)
        {
            SoundAsset soundAsset = _soundDataListSO.SoundDataList.FirstOrDefault(data => data.SoundType == soundType);

            if (soundAsset != null)
            {
                CreatePoolBuilder(soundAsset.SoundData)
                .WithRandomPitch()
                .WithParent(transform)
                .WithPosition(transform.position)
                .Activate();
            }

        }
        
        protected override SoundBuilder GetNewPoolBuilder(SoundEmitter prefab, SoundData data)
        {
            return new SoundBuilder(prefab, data);
        }

    }
}

