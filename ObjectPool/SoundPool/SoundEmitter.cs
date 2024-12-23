using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using HIEU_NL.DesignPatterns.ObjectPool.Single;
using HIEU_NL.Manager;

namespace HIEU_NL.ObjectPool.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : PoolPrefab<SoundData>
    {
        [SerializeField] private AudioSource _audioSource;
        private Coroutine _playingCoroutine;
        public SoundData SoundData { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
            
        }

        //

        public override void Initialize(SoundData soundData)
        {
            base.Initialize(soundData);

            _audioSource.clip = soundData.Clip;
            _audioSource.outputAudioMixerGroup = soundData.MixerGroup;
            _audioSource.loop = soundData.Loop;
            _audioSource.playOnAwake = soundData.PlayOnAwake;
        }

        public override void Activate()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }

            _audioSource.Play();
            _playingCoroutine = StartCoroutine(WaitForsoundEnd());
        }

        public override void Deactivate()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _audioSource.Stop();
            SoundPool.Instance.ReturnToPool(this);
        }

        //

        private IEnumerator WaitForsoundEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);

            SoundPool.Instance.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            _audioSource.pitch += Random.Range(min, max);
        }

    }

}

