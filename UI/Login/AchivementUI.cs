using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HIEU_NL.ObjectPool.Profile;
using HIEU_NL.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AchivementUI : RyoMonoBehaviour
{
    [SerializeField] private AchivementSingleUI_LoginCanvas _achivementSinglePrefab;
    
    [SerializeField] private ScrollRect _scrollView;

    [SerializeField] private Button _exitButton;
    [SerializeField] private GameObject _LoadingObject;

    private List<AchivementSingleUI_LoginCanvas> _achivementList = new();

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
        
        userList = userList.OrderByDescending(user => user.CurrentMaxLevelIndex).ToList();

        for (int i = 0;  i < userList.Count; i++)
        {
            CreateNewProfile(i + 1, userList[i]);
            UniTask.Yield();
        }
        
        //##

        void CreateNewProfile(int rankingIndex, User user)
        {
            AchivementSingleUI_LoginCanvas profileSingle = Instantiate(_achivementSinglePrefab, parent: _scrollView.content);
            
            if (profileSingle.IsValid())
            {
                profileSingle.UpdateVisual(rankingIndex, user);

                _achivementList.Add(profileSingle);
            }
        }
    }
    
    private void ClearVisual()
    {
        if (_achivementList.IsNullOrEmpty()) return;
        
        foreach (AchivementSingleUI_LoginCanvas userProfile in _achivementList)
        {
            Destroy(userProfile.gameObject);
        }

        _achivementList.Clear();
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
