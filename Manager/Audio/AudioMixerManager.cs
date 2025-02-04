using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static HIEU_NL.Utilities.ParameterExtensions.Audio;
using System;
using HIEU_NL.DesignPatterns.Singleton;

public class AudioMixerManager : PersistentSingleton<AudioMixerManager>
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private VolumeState _currentMusicVolumeState;
    [SerializeField] private VolumeState _currentSoundVolumeState;

    protected override void Start()
    {
        base.Start();

        FirebaseManager.Instance.OnGetDataCompleted += FirebaseManager_OnGetAudioSuccess;
    }

    private void FirebaseManager_OnGetAudioSuccess(object sender, EventArgs e)
    {
        VolumeState musicVolumeState = (VolumeState)Enum.Parse(typeof(VolumeState), FirebaseManager.Instance.CurrentAudio.Music);
        VolumeState soundVolumeState = (VolumeState)Enum.Parse(typeof(VolumeState), FirebaseManager.Instance.CurrentAudio.Sound);

        int musicIndex = GetCurentIndexOfCurrentVolumeState(musicVolumeState);
        int soundIndex = GetCurentIndexOfCurrentVolumeState(soundVolumeState);

        _currentMusicVolumeState = musicVolumeState;
        _currentSoundVolumeState = soundVolumeState;

        SetMusicVolume(VOLUME_STATES[musicIndex].Item2);
        SetSoundVolume(VOLUME_STATES[soundIndex].Item2);

        MusicManager.Instance.PlayMusic();
    }

    /*
     * 
     */

    public void NextMusicVolumeState()
    {
        int nextIndex = GetNextIndexOfCurrentVolumeState(_currentMusicVolumeState);
        _currentMusicVolumeState = VOLUME_STATES[nextIndex].Item1;
        float volumeValue = VOLUME_STATES[nextIndex].Item2;

        SetMusicVolume(volumeValue);

        FirebaseManager.Instance.ChangeAudioVolume(_currentMusicVolumeState, _currentSoundVolumeState);
    }

    public void NextSoundVolumeState()
    {
        int nextIndex = GetNextIndexOfCurrentVolumeState(_currentSoundVolumeState);
        _currentSoundVolumeState = VOLUME_STATES[nextIndex].Item1;
        float volumeValue = VOLUME_STATES[nextIndex].Item2;

        SetSoundVolume(volumeValue);

        FirebaseManager.Instance.ChangeAudioVolume(_currentMusicVolumeState, _currentSoundVolumeState);
    }

    /*
     * 
     */

    private void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat(AUDIO_MIXER_PARAMETER_MusicVolume, Mathf.Log10(volume) * 20f);
    }

    private void SetSoundVolume(float volume)
    {
        _audioMixer.SetFloat(AUDIO_MIXER_PARAMETER_SoundVolume, Mathf.Log10(volume) * 20f);
    }

    private int GetNextIndexOfCurrentVolumeState(VolumeState volumeState)
    {
        string[] volumeStateNames = Enum.GetNames(typeof(VolumeState));
        int currentIndex = Array.IndexOf(volumeStateNames, volumeState.ToString());
        return (currentIndex + 1) % volumeStateNames.Length;
    }

    private int GetCurentIndexOfCurrentVolumeState(VolumeState volumeState)
    {
        string[] volumeStateNames = Enum.GetNames(typeof(VolumeState));
        int currentIndex = Array.IndexOf(volumeStateNames, volumeState.ToString());
        return currentIndex;
    }

    /*
     * 
     */

    public string GetCurrentMusicVolumeStateString()
    {
        return _currentMusicVolumeState.ToString();
    }

    public string GetCurrentSoundVolumeStateString()
    {
        return _currentSoundVolumeState.ToString();
    }

}
