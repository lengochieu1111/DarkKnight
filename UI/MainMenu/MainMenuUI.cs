using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuUI : RyoMonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _selectMapButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _missionButton;
    [SerializeField] private Button _logoutButton;

    [SerializeField] private TextMeshProUGUI _userNameText;

    protected override void Awake()
    {
        base.Awake();

        _startGameButton.onClick.AddListener(() =>
        {
            StartGame();
        });

        _selectMapButton.onClick.AddListener(() =>
        {
            SelectLevel();
        });
        
        _shopButton.onClick.AddListener(() =>
        {
            Shop();
        });

        _optionsButton.onClick.AddListener(() =>
        {
            Options();
        });
        
        _missionButton.onClick.AddListener(() =>
        {
            Mission();
        });

        _logoutButton.onClick.AddListener(() =>
        {
            LogoutButton();
        });

    }

    protected override void Start()
    {
        base.Start();

        UpdateVisual_UserNameText();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_startGameButton == null)
        {
            _startGameButton = transform.Find("StartGameButton")?.GetComponent<Button>();
        }

        if (_selectMapButton == null)
        {
            _selectMapButton = transform.Find("SelectLevelButton")?.GetComponent<Button>();
        }
        
        if (_shopButton == null)
        {
            _shopButton = transform.Find("ShopButton")?.GetComponent<Button>();
        }

        if (_optionsButton == null)
        {
            _optionsButton = transform.Find("OptionsButton")?.GetComponent<Button>();
        }

        if (_logoutButton == null)
        {
            _logoutButton = transform.Find("LogoutButton")?.GetComponent<Button>();
        }
        
        if (_userNameText == null)
        {
            _userNameText = transform.Find("UserNameText")?.GetComponent<TextMeshProUGUI>();
        }

    }

    #region BUTTON

    private void StartGame()
    {
        if (FirebaseManager.Instance.CurrentUser.CurrentLevelIndex <
            FirebaseManager.Instance.CurrentUser.CurrentMaxLevelIndex)
        {
            SceneTransitionManager.Instance.LoadScene(EScene.Platformer);
        }
        else
        {
            if (FirebaseManager.Instance.CurrentUser.PuzzleUnlocked)
            {
                SceneTransitionManager.Instance.LoadScene(EScene.Platformer);
            }
            else
            {
                SceneTransitionManager.Instance.LoadScene(EScene.Puzzle);
            }
        }

    }

    private void SelectLevel()
    {
        MainMenuCanvas.Instance.ShowSelectMapUI();
    }

    private void Shop()
    {
        MainMenuCanvas.Instance.ShowShopUI();
    }

    private void Options()
    {
        MainMenuCanvas.Instance.ShowOptionsUI();
    }
    
    private void Mission()
    {
        MainMenuCanvas.Instance.ShowMissionUI();
    }
    
    private void LogoutButton()
    {
        FirebaseManager.Instance.RequestLogoutUser();
    }

    #endregion

    #region TEXT

    private void UpdateVisual_UserNameText()
    {
        _userNameText.text = FirebaseManager.Instance.CurrentUser.Name;
    }

    #endregion

    #region SHOW/HIDE UI

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion

}
