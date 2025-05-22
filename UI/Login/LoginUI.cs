using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : RyoMonoBehaviour
{
    [SerializeField] private Button _newProfileButton;
    [SerializeField] private Button _selectProfileButton;
    [SerializeField] private Button _achievementsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitGameButton;

    protected override void Awake()
    {
        base.Awake();

        this._newProfileButton.onClick.AddListener(() =>
        {
            this.NewProfile();
        });

        this._selectProfileButton.onClick.AddListener(() =>
        {
            this.SelectProfile();
        });

        this._achievementsButton.onClick.AddListener(() =>
        {
            this.Achievements();
        });

        this._creditsButton.onClick.AddListener(() =>
        {
            this.Credits();
        });

        this._quitGameButton.onClick.AddListener(() =>
        {
            this.QuitGame();
        });

    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._newProfileButton == null)
        {
            this._newProfileButton = this.transform.Find("NewProfileButton")?.GetComponent<Button>();
        }

        if (this._selectProfileButton == null)
        {
            this._selectProfileButton = this.transform.Find("SelectProfileButton")?.GetComponent<Button>();
        }

        if (this._achievementsButton == null)
        {
            this._achievementsButton = this.transform.Find("AchievementsButton")?.GetComponent<Button>();
        }

        if (this._creditsButton == null)
        {
            this._creditsButton = this.transform.Find("CreditsButton")?.GetComponent<Button>();
        }

        if (this._quitGameButton == null)
        {
            this._quitGameButton = this.transform.Find("QuitGameButton")?.GetComponent<Button>();
        }

    }

    /*
     * 
     */

    private void NewProfile()
    {
        LoginCanvas.Instance.ShowNewProfileUI();
    }

    private void SelectProfile()
    {
        LoginCanvas.Instance.ShowSelectProfileUI();
    }

    private void Achievements()
    {
        LoginCanvas.Instance.ShowAchivementUI();
    }

    private void Credits()
    {
        LoginCanvas.Instance.ShowCreditUI();
    }

    private void QuitGame()
    {
        Application.Quit();
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
