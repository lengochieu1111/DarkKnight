using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using UnityEngine.SceneManagement;

public class LoginCanvas : Singleton<LoginCanvas>
{
    [SerializeField] private LoginUI _loginUI;
    [SerializeField] private NewProfileUI_LoginCanvas _newProfileUI;
    [SerializeField] private SelectProfileUI_LoginCanvas _selectProfileUI;

    #region SETUP COMPONET/VALUES

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_loginUI == null)
        {
            _loginUI = GetComponentInChildren<LoginUI>(true);
        }
        
        if (_newProfileUI == null)
        {
            _newProfileUI = GetComponentInChildren<NewProfileUI_LoginCanvas>(true);
        }
        
        if (_selectProfileUI == null)
        {
            _selectProfileUI = GetComponentInChildren<SelectProfileUI_LoginCanvas>(true);
        }

    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        ShowLoginUI();
        HideNewProfileUI();
        HideSelectProfileUI();

    }

    #endregion

    #region SHOW/HIDE UI

    public void ShowLoginUI()
    {
        _loginUI.Show();
    }

    public void HideOptionsUI()
    {
        _loginUI.Hide();
    }

    // NEW PROFILE UI
    
    public void ShowNewProfileUI()
    {
        _newProfileUI.Show();
    }
    
    public void HideNewProfileUI()
    {
        _newProfileUI.Hide();
    }

    // SELECT PROFILE UI
    
    public void ShowSelectProfileUI()
    {
        _selectProfileUI.Show();
    }
    
    public void HideSelectProfileUI()
    {
        _selectProfileUI.Hide();
    }

    #endregion

}
