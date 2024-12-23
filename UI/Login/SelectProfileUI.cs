using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using HIEU_NL.ObjectPool.Profile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectProfileUI : RyoMonoBehaviour
{
    [SerializeField] private ScrollRect _scrollView;

    [SerializeField] private Button _exitButton;

    private List<ProfilePrefab> _profileList = new List<ProfilePrefab>();

    protected override void Awake()
    {
        base.Awake();

        this._exitButton.onClick.AddListener(() =>
        {
            this.Exit();
        });

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        UpdateVisual();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        ClearVisual();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._scrollView == null)
        {
            this._scrollView = this.GetComponentInChildren<ScrollRect>();
        }
        
        if (this._exitButton == null)
        {
            this._exitButton = this.transform.Find("ExitButton")?.GetComponent<Button>();
        }

    }

    /*
     * 
     */

    private async void UpdateVisual()
    {
        List<User> userList = await FirebaseManager.Instance.GetAllUsersData();

        for (int i = 0;  i < userList.Count; i++)
        {
            CreateNewProfile(i + 1, userList[i]);
        }

        //##

        void CreateNewProfile(int rankingIndex, User user)
        {
            PoolPrefab profilePoolPrefab = MultipleObjectPool.Instance.GetPoolObject(PoolPrefabType.Profile_LOGIN, parent: this._scrollView.content);

            if (profilePoolPrefab != null && profilePoolPrefab.TryGetComponent<ProfilePrefab>(out ProfilePrefab profile))
            {
                profile.UpdateVisual(rankingIndex, user);
                profile.Activate();

                _profileList.Add(profile);
            }
        }
    }
    
    private void ClearVisual()
    {
        foreach (ProfilePrefab userProfile in this._profileList)
        {
            userProfile.Deactivate();
        }

        _profileList.Clear();
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
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
