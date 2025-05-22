using System;
using HIEU_NL.ObjectPool.Profile;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HIEU_NL.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class SelectProfileUI_LoginCanvas : RyoMonoBehaviour
{
    [SerializeField] private ProfileSingleUI_LoginCanvas _profileSinglePrefab;
    
    [SerializeField] private ScrollRect _scrollView;

    [SerializeField] private Button _exitButton;
    [SerializeField] private GameObject _LoadingObject;

    private List<ProfileSingleUI_LoginCanvas> _profileList = new();

    protected override void Awake()
    {
        base.Awake();

        _exitButton.onClick.AddListener(() =>
        {
            Exit();
        });

    }

    /*
     * 
     */

    private async UniTask UpdateVisual()
    {
        List<User> userList = await FirebaseManager.Instance.GetAllUsersData();

        for (int i = 0;  i < userList.Count; i++)
        {
            CreateNewProfile(i + 1, userList[i]);
            UniTask.Yield();
        }

        //##

        void CreateNewProfile(int rankingIndex, User user)
        {
            ProfileSingleUI_LoginCanvas profileSingle = Instantiate(_profileSinglePrefab, parent: _scrollView.content);

            if (profileSingle.IsValid())
            {
                profileSingle.UpdateVisual(rankingIndex, user);

                _profileList.Add(profileSingle);
            }
        }
    }
    
    private void ClearVisual()
    {
        if (_profileList.IsNullOrEmpty()) return;
        
        foreach (ProfileSingleUI_LoginCanvas userProfile in _profileList)
        {
            try
            {
                Destroy(userProfile.gameObject);
            }
            catch (Exception e)
            { }
        }

        _profileList.Clear();
    }

    private void Exit()
    {
        Hide();
    }

    /*
     * 
     */

    public async UniTask Show()
    {
        await UpdateVisual();

        HideLoadingEffect();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        
        ClearVisual();
        ShowLoadingEffect();
    }
    
    public void ShowLoadingEffect()
    {
        _LoadingObject.SetActive(true);
    }

    public void HideLoadingEffect()
    {
        _LoadingObject.SetActive(false);
    }
    
    

}
