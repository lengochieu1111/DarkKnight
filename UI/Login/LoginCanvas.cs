using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;

public class LoginCanvas : Singleton<LoginCanvas>
{
    [SerializeField] private LoginUI _loginUI;
    [SerializeField] private NewProfileUI_LoginCanvas _newProfileUI;
    [SerializeField] private SelectProfileUI_LoginCanvas _selectProfileUI;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        ShowLoginUI();
        HideNewProfileUI();
        HideSelectProfileUI();
        
        //##
        MusicManager.Instance.PlayMusic_Login();
    }

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
