using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using HIEU_NL.ObjectPool.Profile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelUI : RyoMonoBehaviour
{
    [SerializeField] private LevelSOList_Puzzle levelSOList;

    [SerializeField] private ScrollRect _scrollView;

    [SerializeField] private Button _exitButton;

    private List<ProfilePrefab> _userProfileList = new List<ProfilePrefab>();

    protected override void Awake()
    {
        base.Awake();

        this._exitButton.onClick.AddListener(() =>
        {
            this.Exit();
        });

    }

    /*
     * 
     */

    private async void UpdateVisual()
    {
        List<User> userList = await FirebaseManager.Instance.GetAllUsersData();

        int rankingIndex = 0;
        foreach (User user in userList)
        {
            this.CreateNewProfile(rankingIndex + 1, user);
            rankingIndex++;
        }
    }

    private void ClearVisual()
    {
        foreach (ProfilePrefab userProfile in this._userProfileList)
        {
            userProfile.Deactivate();
        }
    }

    private void CreateNewProfile(int rankingIndex, User user)
    {
        //ProfilePrefab userProfile = ObjectPool.Instance.SpawnProfile(this._scrollView.content);
        //userProfile.UpdateVisual(rankingIndex, user);
        //userProfile.gameObject.SetActive(true);
        //this._userProfileList.Add(userProfile);
    }

    private void Exit()
    {
        this.Hide();
    }

    /*
     * 
     */

    public void Show()
    {
        this.UpdateVisual();

        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.ClearVisual();

        this.gameObject.SetActive(false);
    }

}
