 using HIEU_NL.DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]

public class MusicManager : PersistentSingleton<MusicManager>
{
    [SerializeField] private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();

        LoadComponents();

    }

    protected void LoadComponents()
    {

        if (_audioSource == null )
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayMusic()
    {
        _audioSource?.Play();
    }

}
