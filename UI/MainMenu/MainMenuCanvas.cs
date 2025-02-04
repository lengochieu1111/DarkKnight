using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;

public class MainMenuCanvas : Singleton<MainMenuCanvas>
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private OptionsUI_MainMenuCanvas _optionsUI;
    [SerializeField] private SelectMapUI_MainMenuCanvas _selectMapUI;

    #region SETUP COMPONENT/VALUES

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_mainMenuUI == null)
        {
            _mainMenuUI = GetComponentInChildren<MainMenuUI>(true);
        }

        if (_optionsUI == null)
        {
            _optionsUI = GetComponentInChildren<OptionsUI_MainMenuCanvas>(true);
        }

    }


    protected override void ResetComponents()
    {
        base.SetupComponents();

        ShowMainMenuUI();
        HideOptionsUI();
        HideSelectMapUI();
    }

    #endregion

    #region SHOW/HIDE UI

    public void ShowOptionsUI()
    {
        _optionsUI.Show();
    }
    
    public void HideOptionsUI()
    {
        _optionsUI.Hide();
    }
    
    public void ShowMainMenuUI()
    {
        _mainMenuUI.Show();
    }
    
    public void HideMainMenuUI()
    {
        _mainMenuUI.Hide();
    }
    
    public void ShowSelectMapUI()
    {
        _selectMapUI.Show();
    }
    
    public void HideSelectMapUI()
    {
        _selectMapUI.Hide();
    }

    #endregion

}
