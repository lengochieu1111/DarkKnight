using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Game;
using UnityEngine;

public class PlatformerCanvas : Singleton<PlatformerCanvas>
{
    [SerializeField] private PlatformerUI _platformerUI;
    [SerializeField] private PauseGameUI_PlatformerCanvas _pauseGameUI;

    protected override void Start()
    {
        base.Start();

        GameMode_Platformer.Instance.OnChangedState += GameManager_OnChangedState;
        GameMode_Platformer.Instance.OnPauseGame += GameManager_OnPauseGame;
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_platformerUI == null)
        {
            _platformerUI = GetComponentInChildren<PlatformerUI>(true);
        }

        if (_pauseGameUI == null)
        {
            _pauseGameUI = GetComponentInChildren<PauseGameUI_PlatformerCanvas>(true);
        }
        
    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        ShowPuzzleUI();
        HidePauseGameUI();
    }

    /*
     *
     */

    private void GameManager_OnChangedState(object sender, System.EventArgs e)
    {
        if (!GameMode_Platformer.Instance.IsGameOver()) return;

        if (GameMode_Platformer.Instance.IsGameWon)
        {
            TransitionManager.Instance.LoadScene(Scene.MainMenu);
            // Choice : next level - main menu
        }
        else
        {
            TransitionManager.Instance.LoadScene(Scene.Platformer, true);
        }
        
    }

    private void GameManager_OnPauseGame(object sender, System.EventArgs e)
    {
        if (GameMode_Platformer.Instance.IsGamePaused)
        {
            ShowPauseGameUI();
        }
        else
        {
            HidePauseGameUI();
        }
    }

    /*
     *
     */

    public void ShowPuzzleUI()
    {
        _platformerUI.Show();
    }

    public void HidePuzzleUI()
    {
        _platformerUI.Hide();
    }
    
    public void ShowPauseGameUI()
    {
        _pauseGameUI.Show();
    }

    public void HidePauseGameUI()
    {
        _pauseGameUI.Hide();
    }
}
