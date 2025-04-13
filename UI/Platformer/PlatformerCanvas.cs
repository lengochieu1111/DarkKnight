using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Manager;
using HIEU_NL.Platformer.Script.Game;
using UnityEngine;

public class PlatformerCanvas : Singleton<PlatformerCanvas>
{
    [SerializeField] private PlatformerUI _platformerUI;
    [SerializeField] private PauseGameUI_PlatformerCanvas _pauseGameUI;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        ShowPlatformerUI();
        HidePauseGameUI();
        
        //##
        MusicManager.Instance.PlayMusic_Platformer();
    }

    protected override void Start()
    {
        base.Start();

        GameMode_Platformer.Instance.OnChangedState += GameManager_OnChangedState;
        GameMode_Platformer.Instance.OnPauseGame += GameManager_OnPauseGame;
    }


    /*
     *
     */

    private void GameManager_OnChangedState(object sender, System.EventArgs e)
    {
        if (GameMode_Platformer.Instance.IsGamePlaying()) return;

        if (GameMode_Platformer.Instance.IsGameWon)
        {
            SceneTransitionManager.Instance.LoadScene(EScene.MainMenu);
            // Choice : next level - main menu
        }
        else
        {
            SceneTransitionManager.Instance.ReloadCurrentScene();
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

    public void ShowPlatformerUI()
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
