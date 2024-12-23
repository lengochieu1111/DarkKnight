using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;

public class MainMenuCanvas : Singleton<MainMenuCanvas>
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private OptionsUI_MainMenuCanvas _optionsUI;

    #region SETUP COMPONENT/VALUES

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._mainMenuUI == null)
        {
            this._mainMenuUI = this.GetComponentInChildren<MainMenuUI>(true);
        }

        if (this._optionsUI == null)
        {
            this._optionsUI = this.GetComponentInChildren<OptionsUI_MainMenuCanvas>(true);
        }

    }


    protected override void ResetComponents()
    {
        base.SetupComponents();

        this.ShowMainMenuUI();
        this.HideOptionsUI();
    }

    #endregion

    #region SHOW/HIDE UI

    public void ShowOptionsUI()
    {
        this._optionsUI.Show();
    }
    
    public void HideOptionsUI()
    {
        this._optionsUI.Hide();
    }
    
    public void ShowMainMenuUI()
    {
        this._mainMenuUI.Show();
    }
    
    public void HideMainMenuUI()
    {
        this._mainMenuUI.Hide();
    }

    #endregion

}
