using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : RyoMonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _selectLevelButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _logoutButton;

    [SerializeField] private TextMeshProUGUI _userNameText;

    protected override void Awake()
    {
        base.Awake();

        this._startGameButton.onClick.AddListener(() =>
        {
            this.StartGame();
        });

        this._selectLevelButton.onClick.AddListener(() =>
        {
            this.SelectLevel();
        });
        
        this._shopButton.onClick.AddListener(() =>
        {
            this.Shop();
        });

        this._optionsButton.onClick.AddListener(() =>
        {
            this.Options();
        });

        this._logoutButton.onClick.AddListener(() =>
        {
            this.LogoutButton();
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

        if (this._startGameButton == null)
        {
            this._startGameButton = this.transform.Find("StartGameButton")?.GetComponent<Button>();
        }

        if (this._selectLevelButton == null)
        {
            this._selectLevelButton = this.transform.Find("SelectLevelButton")?.GetComponent<Button>();
        }
        
        if (this._shopButton == null)
        {
            this._shopButton = this.transform.Find("ShopButton")?.GetComponent<Button>();
        }

        if (this._optionsButton == null)
        {
            this._optionsButton = this.transform.Find("OptionsButton")?.GetComponent<Button>();
        }

        if (this._logoutButton == null)
        {
            this._logoutButton = this.transform.Find("LogoutButton")?.GetComponent<Button>();
        }
        
        if (this._userNameText == null)
        {
            this._userNameText = this.transform.Find("UserNameText")?.GetComponent<TextMeshProUGUI>();
        }

    }

    #region BUTTON

    private void StartGame()
    {
        Debug.Log("StartGame");
    }

    private void SelectLevel()
    {
        Debug.Log("SelectLevel");
    }

    private void Shop()
    {
        Debug.Log("Shop");
    }

    private void Options()
    {
        MainMenuCanvas.Instance.ShowOptionsUI();
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
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    #endregion

}
