using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Manager;
using HIEU_NL.Platformer.Script.Game;
using UnityEngine;

public class PlatformerCanvas : Singleton<PlatformerCanvas>
{
    [SerializeField] private PlatformerUI _platformerUI;
    [SerializeField] private BossBattleUI_PlatformerCanvas _bossBattleUI;
    [SerializeField] private EndGameUI_PlatformerCanvas _endGameUI;
    [SerializeField] private PauseGameUI_PlatformerCanvas _pauseGameUI;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        ShowPlatformerUI();
        HideBossBattleUI();
        HidePauseGameUI();
        HideEndGameUI();
        
        //##
        MusicManager.Instance.PlayMusic_Platformer();
    }

    protected override void Start()
    {
        base.Start();

        GameMode_Platformer.Instance.OnChangedState += GameManager_OnChangedState;
        GameMode_Platformer.Instance.OnBossBattle += GameManager_OnBossBattle;
        GameMode_Platformer.Instance.OnPauseGame += GameManager_OnPauseGame;
    }


    /*
     *
     */

    private void GameManager_OnChangedState(object sender, System.EventArgs e)
    {
        if (GameMode_Platformer.Instance.IsGameOver())
        {
            ShowEndGameUI();
        }
    }

    private void GameManager_OnBossBattle(object sender, System.EventArgs e)
    {
        ShowBossBattleUI();
        HidePlatformerUI();
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

    private void ShowPlatformerUI()
    {
        _platformerUI.Show();
    }

    private void HidePlatformerUI()
    {
        _platformerUI.Hide();
    }
    
    private void ShowPauseGameUI()
    {
        _pauseGameUI.Show();
    }

    private void HidePauseGameUI()
    {
        _pauseGameUI.Hide();
    }
    
    private void ShowBossBattleUI()
    {
        _bossBattleUI.Show();
    }

    private void HideBossBattleUI()
    {
        _bossBattleUI.Hide();
    }
    
    private void ShowEndGameUI()
    {
        _endGameUI.Show();
    }

    private void HideEndGameUI()
    {
        _endGameUI.Hide();
    }
    
}
