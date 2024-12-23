

using System.Collections.Generic;
using System;
using UnityEngine;

namespace HIEU_NL.Utilities
{
    public static class ParameterExtensions
    {

        public static class SYSTEM_CONFIG
        {
            public static string filePath_ = Application.persistentDataPath + "/SystemConfigData.json";
        }

        public const string STRING_EMPTY = "";

        public static class Window
        {
            public const int WINDOW_HEIGHT = 1080;
            public const int WINDOW_WIDTH = 1920;
        }

        public static class FirestoreDatabase
        {
            public const string COLLECTION_Users = "Users";

            //##
            public const string USER_FIELD_Name = "Name";
            public const string USER_FIELD_CurrentLevelIndex = "CurrentLevelIndex";

            //##

            public const string COLLECTION_Application = "Application";

            //### user saved
            public const string APPLICATION_DOCUMENT_UserSaved = "UserSaved";

            //### audio
            public const string APPLICATION_DOCUMENT_Audio = "Audio";

            public const string AUDIO_FIELD_Music = "Music";
            public const string AUDIO_FIELD_Sound = "Sound";
        }

        public static class Audio
        {
            public const string AUDIO_MIXER_PARAMETER_MusicVolume = "MusicVolume";
            public const string AUDIO_MIXER_PARAMETER_SoundVolume = "SoundVolume";

            public enum VolumeState
            {
                Mute,
                Low,
                Medium,
                High
            }

            public readonly static List<Tuple<VolumeState, float>> VOLUME_STATES = new List<Tuple<VolumeState, float>>
            {
                new Tuple<VolumeState, float>(VolumeState.Mute, 0.001f),
                new Tuple<VolumeState, float>(VolumeState.Low, 0.25f),
                new Tuple<VolumeState, float>(VolumeState.Medium, 0.5f),
                new Tuple<VolumeState, float>(VolumeState.High, 1f)
            };
        }

        public static class Animation
        {
            public const int Y_AXIS_ROTATE_LEFT = -180;
            public const int Y_AXIS_ROTATE_RIGHT = 180;
            
            //

            public static readonly int ANIM_HASH_Idle = Animator.StringToHash("Idle");
            public static readonly int ANIM_HASH_Walk = Animator.StringToHash("Walk");
            public static readonly int ANIM_HASH_Run = Animator.StringToHash("Run");
            public static readonly int ANIM_HASH_Dash = Animator.StringToHash("Dash");

            public static readonly int ANIM_HASH_JumpStart = Animator.StringToHash("JumpStart");
            public static readonly int ANIM_HASH_JumpLoop = Animator.StringToHash("JumpLoop");
            public static readonly int ANIM_HASH_Landing = Animator.StringToHash("Landing");

            public static readonly int ANIM_HASH_Attack = Animator.StringToHash("Attack");

            public static readonly int ANIM_HASH_Pain = Animator.StringToHash("Pain");

            public static readonly int ANIM_HASH_Dead = Animator.StringToHash("Dead");

        }

    }

}
