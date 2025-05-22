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
using HIEU_NL.Manager;

[FirestoreData]
public class User
{
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public int CurrentMaxLevelIndex { get; set; }
    [FirestoreProperty] public bool PuzzleUnlocked { get; set; }
    [FirestoreProperty] public Bag Bag { get; set; }
    [FirestoreProperty] public int CurrentLevelIndex { get; set; }
    [FirestoreProperty] public int CurrentWeaponIndex { get; set; }
    [FirestoreProperty] public int CurrentCharacterIndex { get; set; }
    [FirestoreProperty] public int CurrentCoin { get; set; }
    [FirestoreProperty] public Mission Mission { get; set; }
    
    /*[FirestoreProperty] public int CurrentTicket_DoubleDamage { get; set; }
    [FirestoreProperty] public int CurrentTicket_DoubleReward { get; set; }
    [FirestoreProperty] public int CurrentTicket_Magnet { get; set; }
    
    [FirestoreProperty] public int CurrentTicket_HealthRegeneration { get; set; }
    [FirestoreProperty] public int CurrentTicket_EnergyRegeneration { get; set; }*/
    
    /*[FirestoreProperty] public int HealthPotionCollectedAmount { get; set; }
    [FirestoreProperty] public int EnergyPotionCollectedAmount { get; set; }
    [FirestoreProperty] public int WeaponPurchasedAmount { get; set; }
    [FirestoreProperty] public int BossesKilledAmount { get; set; }*/
    
    public User(string name = STRING_EMPTY, int currentMaxLevelIndex = 0, bool puzzleUnlocked = false)
    {
        Name = name;
        CurrentMaxLevelIndex = currentMaxLevelIndex;
        PuzzleUnlocked = puzzleUnlocked;
        Bag = new Bag();
        CurrentLevelIndex = 0;
        CurrentWeaponIndex = 0;
        CurrentCharacterIndex = 0;
        CurrentCoin = 0;
        
        Mission = new Mission();

    }

}

[FirestoreData]
public class Bag
{
    [FirestoreProperty] public List<int> Weapon { get; set; }
    [FirestoreProperty] public List<int> Character { get; set; }

    public Bag()
    {
        Weapon = new List<int> { 0 };
        Character = new List<int> { 0 };
    }
}

[FirestoreData]
public class Mission
{
    [FirestoreProperty] public List<int> Amount { get; set; }
    [FirestoreProperty] public List<bool> State { get; set; }

    public Mission()
    {
        Amount = new List<int> { 0, 0, 0, 0 };
        State = new List<bool> { false, false, false, false };
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


    // Aplication
    private CollectionReference _applicationCollectionReference;
    public Audio CurrentAudio { get; private set; }

    #region SETUP/RESET
    
    protected override void ResetValues()
    {
        base.ResetValues();
        
        //##
        CurrentUser = null;
        CurrentAudio = null;
    }
    
    #endregion


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
        await GetUserSaved();

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
                { USER_FIELD_CurrentMaxLevelIndex, CurrentUser.CurrentMaxLevelIndex },
                { USER_FIELD_PuzzleUnlocked, CurrentUser.PuzzleUnlocked },
                { USER_FIELD_Bag, CurrentUser.Bag },
                { USER_FIELD_CurrentLevelIndex, CurrentUser.CurrentLevelIndex },
                { USER_FIELD_CurrentWeaponIndex, CurrentUser.CurrentWeaponIndex },
                { USER_FIELD_CurrentCharacterIndex, CurrentUser.CurrentCharacterIndex },
                { USER_FIELD_CurrentCoin, CurrentUser.CurrentCoin },
                { USER_FIELD_Mission, CurrentUser.Mission },
            });

            //
            LoginUser();

            return true;
        }

    }

    public void UpdateUser(User user)
    {
        _userCollectionReference.Document(user.Name).UpdateAsync(new Dictionary<string, object>
        {
            { USER_FIELD_Name, user.Name },
            { USER_FIELD_CurrentMaxLevelIndex, user.CurrentMaxLevelIndex },
            { USER_FIELD_PuzzleUnlocked , user.PuzzleUnlocked },
            { USER_FIELD_Bag, user.Bag },
            { USER_FIELD_CurrentLevelIndex, user.CurrentLevelIndex },
            { USER_FIELD_CurrentWeaponIndex, user.CurrentWeaponIndex },
            { USER_FIELD_CurrentCharacterIndex, user.CurrentCharacterIndex },
            { USER_FIELD_CurrentCoin, user.CurrentCoin },
            { USER_FIELD_Mission, CurrentUser.Mission },

        });
    }
    
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

                List<int> weaponList = new List<int>();
                List<int> characterList = new List<int>();
                
                if (userData.ContainsKey(USER_FIELD_Bag))
                {
                    Dictionary<string, object> bagData = (Dictionary<string, object>)userData[USER_FIELD_Bag];
                    if (bagData.ContainsKey(USER_FIELD_Bag_Weapon) && bagData[USER_FIELD_Bag_Weapon] is List<object> weaponObjectList)
                    {
                        weaponList = weaponObjectList.ConvertAll(weapon => Convert.ToInt32(weapon));
                    }
                    
                    if (bagData.ContainsKey(USER_FIELD_Bag_Character) && bagData[USER_FIELD_Bag_Character] is List<object> characterObjectList)
                    {
                        characterList = characterObjectList.ConvertAll(character => Convert.ToInt32(character));
                    }
                }
                
                List<int> amountList = new List<int>();
                List<bool> stateList = new List<bool>();
                
                if (userData.ContainsKey(USER_FIELD_Mission))
                {
                    Dictionary<string, object> missionData = (Dictionary<string, object>)userData[USER_FIELD_Mission];
                    if (missionData.ContainsKey(USER_FIELD_Mission_Amount) && missionData[USER_FIELD_Mission_Amount] is List<object> amountObjectList)
                    {
                        amountList = amountObjectList.ConvertAll(amount => Convert.ToInt32(amount));
                    }
                    
                    if (missionData.ContainsKey(USER_FIELD_Mission_State) && missionData[USER_FIELD_Mission_State] is List<object> stateObjectList)
                    {
                        stateList = stateObjectList.ConvertAll(state => Convert.ToBoolean(state));
                    }
                }

                User user = new User
                {
                    Name = userData.ContainsKey(USER_FIELD_Name) ? userData[USER_FIELD_Name].ToString() : string.Empty,
                    CurrentMaxLevelIndex = userData.ContainsKey(USER_FIELD_CurrentMaxLevelIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentMaxLevelIndex]) : 0,
                    PuzzleUnlocked = userData.ContainsKey(USER_FIELD_PuzzleUnlocked) ? Convert.ToBoolean(userData[USER_FIELD_PuzzleUnlocked]) : false,
                    Bag = new Bag()
                    {
                        Weapon = weaponList,
                        Character = characterList,
                    } ,
                    CurrentLevelIndex = userData.ContainsKey(USER_FIELD_CurrentLevelIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentLevelIndex]) : 0,
                    CurrentWeaponIndex = userData.ContainsKey(USER_FIELD_CurrentWeaponIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentWeaponIndex]) : 0,
                    CurrentCharacterIndex = userData.ContainsKey(USER_FIELD_CurrentCharacterIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentCharacterIndex]) : 0,
                    CurrentCoin = userData.ContainsKey(USER_FIELD_CurrentCoin) ? Convert.ToInt32(userData[USER_FIELD_CurrentCoin]) : 0,
                    
                    Mission = new Mission()
                    {
                        Amount = amountList,
                        State = stateList,
                    }
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

    private async Task GetUserSaved()
    {
        await _applicationCollectionReference.Document(APPLICATION_DOCUMENT_UserSaved).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    Dictionary<string, object> userData = task.Result.ToDictionary();
                    
                    List<int> weaponList = new List<int>();
                    List<int> characterList = new List<int>();
                
                    if (userData.ContainsKey(USER_FIELD_Bag))
                    {
                        Dictionary<string, object> bagData = (Dictionary<string, object>)userData[USER_FIELD_Bag];
                        if (bagData.ContainsKey(USER_FIELD_Bag_Weapon) && bagData[USER_FIELD_Bag_Weapon] is List<object> weaponObjectList)
                        {
                            weaponList = weaponObjectList.ConvertAll(weapon => Convert.ToInt32(weapon));
                        }
                    
                        if (bagData.ContainsKey(USER_FIELD_Bag_Character) && bagData[USER_FIELD_Bag_Character] is List<object> characterObjectList)
                        {
                            characterList = characterObjectList.ConvertAll(character => Convert.ToInt32(character));
                        }
                    }
                    
                    List<int> amountList = new List<int>();
                    List<bool> stateList = new List<bool>();
                
                    if (userData.ContainsKey(USER_FIELD_Mission))
                    {
                        Dictionary<string, object> missionData = (Dictionary<string, object>)userData[USER_FIELD_Mission];
                        if (missionData.ContainsKey(USER_FIELD_Mission_Amount) && missionData[USER_FIELD_Mission_Amount] is List<object> amountObjectList)
                        {
                            amountList = amountObjectList.ConvertAll(amount => Convert.ToInt32(amount));
                        }
                    
                        if (missionData.ContainsKey(USER_FIELD_Mission_State) && missionData[USER_FIELD_Mission_State] is List<object> stateObjectList)
                        {
                            stateList = stateObjectList.ConvertAll(state => Convert.ToBoolean(state));
                        }
                    }

                    CurrentUser = new User
                    {
                        Name = userData.ContainsKey(USER_FIELD_Name) ? userData[USER_FIELD_Name].ToString() : string.Empty,
                        CurrentMaxLevelIndex = userData.ContainsKey(USER_FIELD_CurrentMaxLevelIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentMaxLevelIndex]) : 0,
                        PuzzleUnlocked = userData.ContainsKey(USER_FIELD_PuzzleUnlocked) ? Convert.ToBoolean(userData[USER_FIELD_PuzzleUnlocked]) : false,
                        Bag = new Bag()
                        {
                            Weapon = weaponList,
                            Character = characterList,
                        },
                        CurrentLevelIndex = userData.ContainsKey(USER_FIELD_CurrentLevelIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentLevelIndex]) : 0,
                        CurrentWeaponIndex = userData.ContainsKey(USER_FIELD_CurrentWeaponIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentWeaponIndex]) : 0,
                        CurrentCharacterIndex = userData.ContainsKey(USER_FIELD_CurrentCharacterIndex) ? Convert.ToInt32(userData[USER_FIELD_CurrentCharacterIndex]) : 0,
                        CurrentCoin = userData.ContainsKey(USER_FIELD_CurrentCoin) ? Convert.ToInt32(userData[USER_FIELD_CurrentCoin]) : 0,
                        Mission = new Mission()
                        {
                            Amount = amountList,
                            State = stateList,
                        }
                    };

                    if (string.IsNullOrEmpty(CurrentUser.Name))
                    {
                        Debug.Log("Data null!");
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
    }
    
    private void UpdateUserSaved()
    {
        _applicationCollectionReference.Document(APPLICATION_DOCUMENT_UserSaved).UpdateAsync(new Dictionary<string, object>
        {
            { USER_FIELD_Name , CurrentUser.Name },
            { USER_FIELD_CurrentMaxLevelIndex , CurrentUser.CurrentMaxLevelIndex },
            { USER_FIELD_PuzzleUnlocked , CurrentUser.PuzzleUnlocked },
            { USER_FIELD_Bag, CurrentUser.Bag },
            { USER_FIELD_CurrentLevelIndex, CurrentUser.CurrentLevelIndex },
            { USER_FIELD_CurrentWeaponIndex, CurrentUser.CurrentWeaponIndex },
            { USER_FIELD_CurrentCharacterIndex, CurrentUser.CurrentCharacterIndex },
            { USER_FIELD_CurrentCoin, CurrentUser.CurrentCoin },
            { USER_FIELD_Mission, CurrentUser.Mission },
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
            { AUDIO_FIELD_Music , CurrentAudio.Music },
            { AUDIO_FIELD_Sound , CurrentAudio.Sound },
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

        SceneTransitionManager.Instance.LoadScene(EScene.MainMenu);

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

        SceneTransitionManager.Instance.LoadScene(EScene.Login);
    }

    /*
     * CHANGE AUDIO
     */

    public void ChangeAudioVolume(VolumeState musicVolumeState, VolumeState soundVolumeState)
    {
        CurrentAudio = new Audio(musicVolumeState.ToString(), soundVolumeState.ToString());

        UpdateAudio();
    }
    
    /*
     * CHANGE USER SAVED
     */

    private void ChangeUserSaved(string name = STRING_EMPTY, int currentMaxLevelIndex = -1, bool puzzleUnlocked = false
        , Bag bag = null, int currentLevelIndex = -1, int currentWeaponIndex = -1, int currentCharacterIndex = -1,
        int currentCoin = -1, Mission mission = null)
    {
        CurrentUser = new User
        {
            Name = name.Equals(STRING_EMPTY) ? CurrentUser.Name : name,
            CurrentMaxLevelIndex = currentMaxLevelIndex.Equals(-1) ? CurrentUser.CurrentMaxLevelIndex : currentMaxLevelIndex,
            PuzzleUnlocked = puzzleUnlocked,
            Bag = bag ?? CurrentUser.Bag,
            CurrentLevelIndex = currentLevelIndex.Equals(-1) ? CurrentUser.CurrentLevelIndex : currentLevelIndex,
            CurrentWeaponIndex = currentWeaponIndex.Equals(-1) ? CurrentUser.CurrentWeaponIndex : currentWeaponIndex,
            CurrentCharacterIndex = currentCharacterIndex.Equals(-1) ? CurrentUser.CurrentCharacterIndex : currentCharacterIndex,
            CurrentCoin = currentCoin.Equals(-1) ? CurrentUser.CurrentCoin : currentCoin,
            Mission = mission ?? CurrentUser.Mission,

        };
        
        UpdateUserSaved();
        UpdateUser(CurrentUser);
    }

    public void UnlockPuzzleUserSaved()
    {
        ChangeUserSaved(puzzleUnlocked:true);
    }
    
    public void UpgradeLevel(int nextLevelIndex)
    {
        ChangeUserSaved(currentMaxLevelIndex: nextLevelIndex, puzzleUnlocked:false);
    }
    
    public void BuyWeapon(int weaponIndex, int reductCoin)
    {
        CurrentUser.Bag.Weapon.Add(weaponIndex);
        CurrentUser.CurrentCoin -= reductCoin;
        
        ChangeUserSaved();
    }
    
    public void BuyCharacter(int characterIndex, int reductCoin)
    {
        CurrentUser.Bag.Character.Add(characterIndex);
        CurrentUser.CurrentCoin -= reductCoin;

        ChangeUserSaved();
    }   
    
    public void PaymentCoin(int addCoin)
    {
        CurrentUser.CurrentCoin += addCoin;
        
        ChangeUserSaved();
    }
    
    public void UseLevel(int currentLevelIndex)
    {
        ChangeUserSaved(currentLevelIndex:currentLevelIndex);
    }
    
    
    public void UseWeapon(int currentWeaponIndex)
    {
        ChangeUserSaved(currentWeaponIndex:currentWeaponIndex);
    }
    
    public void UseCharacter(int currentCharacterIndex)
    {
        ChangeUserSaved(currentCharacterIndex:currentCharacterIndex);
    }
    
    public void UpdateMissionAmount(int missionIndex, int addMissionAmount)
    {
        CurrentUser.Mission.Amount[missionIndex] += addMissionAmount;

        ChangeUserSaved();
    }
    
    public void UpdateMissionState(int missionIndex, int rewardMission)
    {
        CurrentUser.Mission.State[missionIndex] = true;
        CurrentUser.CurrentCoin += rewardMission;

        ChangeUserSaved();
    }
    
    /*
     * 
     */

    /*public void SetCurrentSelectedLevel(int currentSelectedLevel)
    {
        CurrentSelectedLevel = currentSelectedLevel;
    }*/
    
    #endregion

}

