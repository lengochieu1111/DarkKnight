using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static HIEU_NL.Utilities.ParameterExtensions.FirestoreDatabase;
using static HIEU_NL.Utilities.ParameterExtensions.Audio;
using static HIEU_NL.Utilities.ParameterExtensions;

using Firebase.Firestore;
using Firebase.Extensions;
using Firebase;

using HIEU_NL.DesignPatterns.Singleton;
using System.Threading.Tasks;
using System;

[FirestoreData]
public class User
{
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public int CurrentLevelIndex { get; set; }

    public User(string name = STRING_EMPTY, int currentLevelIndex = 0)
    {
        Name = name;
        CurrentLevelIndex = currentLevelIndex;
    }

}

[FirestoreData]
public class Audio
{
    [FirestoreProperty] public string Music { get; set; }
    [FirestoreProperty] public string Sound { get; set; }

    public Audio(string music = STRING_EMPTY, string sound = STRING_EMPTY)
    {
        Music = music;
        Sound = sound;
    }

}

public class FirebaseManager : PersistentSingleton<FirebaseManager>
{
    public event EventHandler OnGetDataCompleted;

    private FirebaseFirestore _database;

    // User
    private CollectionReference _userCollectionReference;
    public User CurrentUser { get; private set; }
    public bool HasUserSaved { get; private set; }

    // Aplication
    private CollectionReference _applicationCollectionReference;
    public Audio CurrentAudio { get; private set; }

    protected override void Start()
    {
        base.Start();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                _database = FirebaseFirestore.DefaultInstance;

                _userCollectionReference = _database.Collection(COLLECTION_Users);

                _applicationCollectionReference = _database.Collection(COLLECTION_Application);

                GetDataApilication();

            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies.");
            }

        });

    }

    private async void GetDataApilication()
    {
        await GetAudio();
        HasUserSaved = await GetUserSaved();

        OnGetDataCompleted?.Invoke(this, EventArgs.Empty);

    }

    #region USER

    public async Task<bool> CheckUserExists(string userName)
    {
        QuerySnapshot snapshots = await _userCollectionReference.GetSnapshotAsync();

        foreach (DocumentSnapshot documentSnapshot in snapshots.Documents)
        {
            if (documentSnapshot.Exists)
            {
                if (documentSnapshot.Id.Equals(userName))
                {
                    return true;
                }
            }
        }

        Debug.Log("User not exists!");

        return false;
    }

    public async Task<bool> CreateNewUser(string userName)
    {
        bool userExists = await CheckUserExists(userName);

        if (userExists)
        {
            Debug.Log("Create new user failed!");

            return false;
        }
        else
        {
            CurrentUser = new User(userName);

            //
            await _userCollectionReference.Document(userName).SetAsync(new Dictionary<string, object>
            {
                { USER_FIELD_Name, CurrentUser.Name },
                { USER_FIELD_CurrentLevelIndex, CurrentUser.CurrentLevelIndex },
            });

            //
            LoginUser();

            return true;
        }

    }

    /*    public void UpdateUser()
    {
        _userCollectionReference.Document(_user.Name).UpdateAsync(new Dictionary<string, object>
        {
            { USER_FIELD_Name, _user.Name },
            { USER_FIELD_CurrentLevelIndex, _user.CurrentLevelIndex },
        });
    }*/
    
    public async Task<bool> DeleteUser(User user)
    {
        bool userExists = await CheckUserExists(user.Name);

        if (userExists)
        {
            await _userCollectionReference.Document(user.Name).DeleteAsync();

            return true;
        }
        else
        {
            Debug.Log("Delete user failed!");

            return false;
        }

    }

    /*
     * 
     */

    public async Task<List<User>> GetAllUsersData()
    {
        List<User> userList = new List<User>();

        QuerySnapshot snapshots = await _userCollectionReference.GetSnapshotAsync();

        foreach (DocumentSnapshot documentSnapshot in snapshots.Documents)
        {
            if (documentSnapshot.Exists)
            {
                Dictionary<string, object> userData = documentSnapshot.ToDictionary();

                User user = new User
                {
                    Name = userData.ContainsKey(USER_FIELD_Name) ? userData[USER_FIELD_Name].ToString() : string.Empty,
                    CurrentLevelIndex = userData.ContainsKey(USER_FIELD_CurrentLevelIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentLevelIndex]) : 0,
                };

                userList.Add(user);
            }
        }

        return userList;

    }

    #endregion

    #region APPLICATION

    /*
     * USER SAVED
     */

    private async Task<bool> GetUserSaved()
    {
        bool hasUserSaved = false;

        await _applicationCollectionReference.Document(APPLICATION_DOCUMENT_UserSaved).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    Dictionary<string, object> userData = task.Result.ToDictionary();

                    CurrentUser = new User
                    {
                        Name = userData.ContainsKey(USER_FIELD_Name) ? userData[USER_FIELD_Name].ToString() : string.Empty,
                        CurrentLevelIndex = userData.ContainsKey(USER_FIELD_CurrentLevelIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentLevelIndex]) : 0,
                    };

                    if (string.IsNullOrEmpty(CurrentUser.Name))
                    {
                        Debug.Log("Data null!");
                    }
                    else
                    {
                        hasUserSaved = true;
                    }

                }
                else
                {
                    Debug.Log("Data not exists!");
                }

            }
            else
            {
                Debug.Log("Get Data Not Completed!");
            }

        });

        return hasUserSaved;

    }
    
    private void UpdateUserSaved()
    {
        _applicationCollectionReference.Document(APPLICATION_DOCUMENT_UserSaved).UpdateAsync(new Dictionary<string, object>
        {
            {USER_FIELD_Name , CurrentUser.Name },
            {USER_FIELD_CurrentLevelIndex , CurrentUser.CurrentLevelIndex },
        });
    }

    /*
     * AUDIO
     */

    private async Task<bool> GetAudio()
    {
        bool getDataSuccess = false;

        await _applicationCollectionReference.Document(APPLICATION_DOCUMENT_Audio).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    getDataSuccess = true;

                    Dictionary<string, object> audioData = task.Result.ToDictionary();

                    CurrentAudio = new Audio
                    {
                        Music = audioData.ContainsKey(AUDIO_FIELD_Music) ? audioData[AUDIO_FIELD_Music].ToString() : VolumeState.Low.ToString(),
                        Sound = audioData.ContainsKey(AUDIO_FIELD_Sound) ? audioData[AUDIO_FIELD_Sound].ToString() : VolumeState.Low.ToString()
                    };

                    if (!Enum.IsDefined(typeof(VolumeState), CurrentAudio.Music) || !Enum.IsDefined(typeof(VolumeState), CurrentAudio.Sound))
                    {
                        CurrentAudio.Music = VolumeState.Low.ToString();
                        CurrentAudio.Sound = VolumeState.Low.ToString();
                    }

                    //
                    UpdateAudio();

                }
                else
                {
                    Debug.Log("Data not exists!");
                }

            }
            else
            {
                Debug.Log("Get Data Not Completed!");
            }

        });

        return getDataSuccess;

    }

    private void UpdateAudio()
    {
        _applicationCollectionReference.Document(APPLICATION_DOCUMENT_Audio).UpdateAsync(new Dictionary<string, object>
        {
            {AUDIO_FIELD_Music , CurrentAudio.Music },
            {AUDIO_FIELD_Sound , CurrentAudio.Sound },
        });
    }

    #endregion

    #region OTHERS

    /*
     * LOGIN
     */

    public void RequestLoginUser(User user)
    {
        CurrentUser = user;

        LoginUser();
    }
    
    private void LoginUser()
    {
        UpdateUserSaved();

        TransitionManager.Instance.Load_MainMenuScene();

        Debug.Log("LoginUser");

    }

    /*
     * LOGOUT
     */

    public void RequestLogoutUser()
    {
        CurrentUser = new User();

        LogoutUser();
    }

    private void LogoutUser()
    {
        UpdateUserSaved();

        TransitionManager.Instance.Load_LoginScene();
    }

    /*
     * CHANGE AUDIO
     */

    public void ChangeAudioVolume(VolumeState musicVolumeState, VolumeState soundVolumeState)
    {
        CurrentAudio = new Audio(musicVolumeState.ToString(), soundVolumeState.ToString());

        UpdateAudio();
    }

    #endregion

}