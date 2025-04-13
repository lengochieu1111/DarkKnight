 using HIEU_NL.DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]

public class MusicManager : PersistentSingleton<MusicManager>
{
    [SerializeField] private AudioSource _music_AudioSource;
    [SerializeField] private AudioClip _loginMusic_AudioClip;
    [SerializeField] private AudioClip _mainMenuMusic_AudioClip;
    [SerializeField] private AudioClip _puzzleMusic_AudioClip;
    [SerializeField] private AudioClip _platformerMusic_AudioClip;

    #region MAIN

    public void PlayMusic_Login()
    {
        PlayMusic(_loginMusic_AudioClip);
    }
    
    public void PlayMusic_MainMenu()
    {
        PlayMusic(_mainMenuMusic_AudioClip);
    }
    
    public void PlayMusic_Puzzle()
    {
        PlayMusic(_puzzleMusic_AudioClip);
    }
    
    public void PlayMusic_Platformer()
    {
        PlayMusic(_platformerMusic_AudioClip);
    }
    
    private void PlayMusic(AudioClip audioClip)
    {
        _music_AudioSource.clip = audioClip;
        _music_AudioSource.Play();
    }

    #endregion
    
    

}
