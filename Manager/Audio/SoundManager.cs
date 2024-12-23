using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.ObjectPool.Audio;

namespace HIEU_NL.Manager
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        [SerializeField] private SoundDataListSO soundDataListSO;

        public void PlaySound(SoundType soundType)
        {
            SoundAsset soundAsset = soundDataListSO.SoundDataList.FirstOrDefault(data => data.SoundType == soundType);

            if (soundAsset != null)
            {
                SoundPool.Instance.CreatePoolBuilder(soundAsset.SoundData)
                .WithRandomPitch()
                .WithParent(transform)
                .WithPosition(transform.position)
                .Activate();
            }

        }

    }
}

