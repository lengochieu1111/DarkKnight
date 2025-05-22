

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
            public static int WindowHeight = Screen.height; // int windowWidth = Screen.currentResolution.width;
            public static int WindowWidth = Screen.width;
        }

        public static class FirestoreDatabase
        {
            public const string COLLECTION_Users = "Users";

            //##
            public const string USER_FIELD_Name = "Name";
            public const string USER_FIELD_CurrentMaxLevelIndex = "CurrentMaxLevelIndex";
            public const string USER_FIELD_PuzzleUnlocked = "PuzzleUnlocked";
            
            public const string USER_FIELD_Bag = "Bag";
            public const string USER_FIELD_Bag_Weapon = "Weapon";
            public const string USER_FIELD_Bag_Character = "Character";
            
            public const string USER_FIELD_CurrentLevelIndex = "CurrentLevelIndex";
            public const string USER_FIELD_CurrentWeaponIndex = "CurrentWeaponIndex";
            public const string USER_FIELD_CurrentCharacterIndex = "CurrentCharacterIndex";
            public const string USER_FIELD_CurrentCoin = "CurrentCoin";
            
            public const string USER_FIELD_Mission = "Mission";
            public const string USER_FIELD_Mission_Amount = "Amount";
            public const string USER_FIELD_Mission_State = "State";

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
            public const int Y_AXIS_0 = 0;
            public const int Y_AXIS_180 = 180;
            
            //

            public static readonly int ANIM_HASH_Idle = Animator.StringToHash("Idle");
            public static readonly int ANIM_HASH_Walk = Animator.StringToHash("Walk");
            public static readonly int ANIM_HASH_Run = Animator.StringToHash("Run");
            public static readonly int ANIM_HASH_Dash = Animator.StringToHash("Dash");

            public static readonly int ANIM_HASH_JumpStart = Animator.StringToHash("JumpStart");
            public static readonly int ANIM_HASH_JumpLoop = Animator.StringToHash("JumpLoop");
            public static readonly int ANIM_HASH_JumpEnd = Animator.StringToHash("Landing");

            public static readonly int ANIM_HASH_Attack = Animator.StringToHash("Attack");

            public static readonly int ANIM_HASH_Pain = Animator.StringToHash("Pain");

            public static readonly int ANIM_HASH_Dead = Animator.StringToHash("Dead");
            
            public static readonly int ANIM_HASH_Fly = Animator.StringToHash("Fly");
            
            //##
            
            public static readonly int ANIM_HASH_Idle_Enhanced = Animator.StringToHash("Idle_Enhanced");
            public static readonly int ANIM_HASH_Walk_Enhanced = Animator.StringToHash("Walk_Enhanced");
            public static readonly int ANIM_HASH_Run_Enhanced = Animator.StringToHash("Run_Enhanced");
            public static readonly int ANIM_HASH_Dash_Enhanced = Animator.StringToHash("Dash_Enhanced");

            public static readonly int ANIM_HASH_JumpStart_Enhanced = Animator.StringToHash("JumpStart_Enhanced");
            public static readonly int ANIM_HASH_JumpLoop_Enhanced = Animator.StringToHash("JumpLoop_Enhanced");
            public static readonly int ANIM_HASH_JumpEnd_Enhanced = Animator.StringToHash("Landing_Enhanced");

            public static readonly int ANIM_HASH_Attack_Enhanced = Animator.StringToHash("Attack_Enhanced");

            public static readonly int ANIM_HASH_Pain_Enhanced = Animator.StringToHash("Pain_Enhanced");

            public static readonly int ANIM_HASH_Dead_Enhanced = Animator.StringToHash("Dead_Enhanced");
            
            public static readonly int ANIM_HASH_Fly_Enhanced = Animator.StringToHash("Fly_Enhanced");
            
            
            
            // Advance
            
            public static readonly int ANIM_HASH_TeleportStart = Animator.StringToHash("TeleportStart");
            public static readonly int ANIM_HASH_TeleportEnd = Animator.StringToHash("TeleportEnd");
            
            public static readonly int ANIM_HASH_Stun = Animator.StringToHash("Stun");
            
            public static readonly int ANIM_HASH_Dead_1 = Animator.StringToHash("Dead_1");
            public static readonly int ANIM_HASH_Dead_2 = Animator.StringToHash("Dead_2");
            
            public static readonly int ANIM_HASH_Enhanced = Animator.StringToHash("Enhanced");
            
            public static readonly int ANIM_HASH_Defense = Animator.StringToHash("Defense");

            public enum AnimationType
            {
                ____NORMAL____,
                Idle,
                Walk,
                Run,
                Dash,
                
                JumpStart,
                JumpLoop,
                JumpEnd,
                
                Attack,
                Pain,
                Dead,
                
                TeleportStart,
                TeleportEnd,
                
                ____OTHER____,
                Attack_1,
                Attack_2,
                Attack_3,
                Attack_4,
                Attack_5,
                
                Attack_Special_1,
                Attack_Special_2,
                Attack_Special_3,
                Attack_Special_4,
                Attack_Special_5,
                
                Attack_1_Enhanced,
                Attack_2_Enhanced,
                Attack_3_Enhanced,
                Attack_4_Enhanced,
                Attack_5_Enhanced,
                
                Attack_Special_1_Enhanced,
                Attack_Special_2_Enhanced,
                Attack_Special_3_Enhanced,
                Attack_Special_4_Enhanced,
                Attack_Special_5_Enhanced,
                
                Defense,
                
            }
            
            public static Dictionary<AnimationType, int> ALL_ANIM_HASH = new Dictionary<AnimationType, int>()
            {
                { AnimationType.Idle, Animator.StringToHash("Idle") },
                { AnimationType.Walk, Animator.StringToHash("Walk") },
                { AnimationType.Run, Animator.StringToHash("Run") },
                { AnimationType.Dash, Animator.StringToHash("Dash") },
                
                { AnimationType.JumpStart, Animator.StringToHash("JumpStart") },
                { AnimationType.JumpLoop, Animator.StringToHash("JumpLoop") },
                { AnimationType.JumpEnd, Animator.StringToHash("Landing") },
                
                { AnimationType.Attack, Animator.StringToHash("Attack") },
                
                { AnimationType.Pain, Animator.StringToHash("Pain") },
                
                { AnimationType.Dead, Animator.StringToHash("Dead") },
                
                //#
                { AnimationType.Attack_1, Animator.StringToHash("Attack_1") },
                { AnimationType.Attack_2, Animator.StringToHash("Attack_2") },
                { AnimationType.Attack_3, Animator.StringToHash("Attack_3") },
                { AnimationType.Attack_4, Animator.StringToHash("Attack_4") },
                
                { AnimationType.Attack_Special_1, Animator.StringToHash("Attack_Special_1") },
                { AnimationType.Attack_Special_2, Animator.StringToHash("Attack_Special_2") },
                { AnimationType.Attack_Special_3, Animator.StringToHash("Attack_Special_3") },
                { AnimationType.Attack_Special_4, Animator.StringToHash("Attack_Special_4") },
                
                { AnimationType.Attack_1_Enhanced, Animator.StringToHash("Attack_1_Enhanced") },
                { AnimationType.Attack_2_Enhanced, Animator.StringToHash("Attack_2_Enhanced") },
                { AnimationType.Attack_3_Enhanced, Animator.StringToHash("Attack_3_Enhanced") },
                { AnimationType.Attack_4_Enhanced, Animator.StringToHash("Attack_4_Enhanced") },
                
                { AnimationType.Attack_Special_1_Enhanced, Animator.StringToHash("Attack_Special_1_Enhanced") },
                { AnimationType.Attack_Special_2_Enhanced, Animator.StringToHash("Attack_Special_2_Enhanced") },
                { AnimationType.Attack_Special_3_Enhanced, Animator.StringToHash("Attack_Special_3_Enhanced") },
                { AnimationType.Attack_Special_4_Enhanced, Animator.StringToHash("Attack_Special_4_Enhanced") },
                
            };

        }

        public static class Layers
        {
            public static readonly int Default = LayerMask.GetMask("Default");
            public static readonly int TransparentFX = LayerMask.GetMask("TransparentFX");
            public static readonly int Ignore_Raycast = LayerMask.GetMask("Ignore Raycast");
            
            public static readonly int Water = LayerMask.GetMask("Water");
            public static readonly int UI = LayerMask.GetMask("UI");
            
            public static readonly int Terrain = LayerMask.GetMask("Terrain");
            
            public static readonly int Player = LayerMask.GetMask("Player");
            public static readonly int Bot = LayerMask.GetMask("Bot");
            public static readonly int Effect = LayerMask.GetMask("Effect");
            public static readonly int Effect_Attack = LayerMask.GetMask("Effect_Attack");
            
        }


    }

}
