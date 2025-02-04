using HIEU_NL.ObjectPool.Profile;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class SelectProfileUI_LoginCanvas : RyoMonoBehaviour
{
    [SerializeField] private ProfileSingleUI_LoginCanvas _profileSinglePrefab;
    
    [SerializeField] private ScrollRect _scrollView;

    [SerializeField] private Button _exitButton;

    private List<ProfileSingleUI_LoginCanvas> _profileList = new List<ProfileSingleUI_LoginCanvas>();

    protected override void Awake()
    {
        base.Awake();

        _exitButton.onClick.AddListener(() =>
        {
            Exit();
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

        if (_scrollView == null)
        {
            _scrollView = GetComponentInChildren<ScrollRect>();
        }
        
        if (_exitButton == null)
        {
            _exitButton = transform.Find("ExitButton")?.GetComponent<Button>();
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
        foreach (ProfileSingleUI_LoginCanvas userProfile in _profileList)
        {
            Destroy(gameObject);
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

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
