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

        int musicIndex = this.GetCurentIndexOfCurrentVolumeState(musicVolumeState);
        int soundIndex = this.GetCurentIndexOfCurrentVolumeState(soundVolumeState);

        this._currentMusicVolumeState = musicVolumeState;
        this._currentSoundVolumeState = soundVolumeState;

        this.SetMusicVolume(VOLUME_STATES[musicIndex].Item2);
        this.SetSoundVolume(VOLUME_STATES[soundIndex].Item2);

        MusicManager.Instance.PlayMusic();
    }

    /*
     * 
     */

    public void NextMusicVolumeState()
    {
        int nextIndex = this.GetNextIndexOfCurrentVolumeState(this._currentMusicVolumeState);
        this._currentMusicVolumeState = VOLUME_STATES[nextIndex].Item1;
        float volumeValue = VOLUME_STATES[nextIndex].Item2;

        this.SetMusicVolume(volumeValue);

        FirebaseManager.Instance.ChangeAudioVolume(this._currentMusicVolumeState, this._currentSoundVolumeState);
    }

    public void NextSoundVolumeState()
    {
        int nextIndex = this.GetNextIndexOfCurrentVolumeState(this._currentSoundVolumeState);
        this._currentSoundVolumeState = VOLUME_STATES[nextIndex].Item1;
        float volumeValue = VOLUME_STATES[nextIndex].Item2;

        this.SetSoundVolume(volumeValue);

        FirebaseManager.Instance.ChangeAudioVolume(this._currentMusicVolumeState, this._currentSoundVolumeState);
    }

    /*
     * 
     */

    private void SetMusicVolume(float volume)
    {
        this._audioMixer.SetFloat(AUDIO_MIXER_PARAMETER_MusicVolume, Mathf.Log10(volume) * 20f);
    }

    private void SetSoundVolume(float volume)
    {
        this._audioMixer.SetFloat(AUDIO_MIXER_PARAMETER_SoundVolume, Mathf.Log10(volume) * 20f);
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
        return this._currentMusicVolumeState.ToString();
    }

    public string GetCurrentSoundVolumeStateString()
    {
        return this._currentSoundVolumeState.ToString();
    }

}
