using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Manager;
using HIEU_NL.Puzzle.Script.Game;

public class PuzzleCanvas : Singleton<PuzzleCanvas>
{
    [SerializeField] private PuzzleUI _puzzleUI;
    [SerializeField] private PauseGameUI_PuzzleCanvas _pauseGameUI;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        ShowPuzzleUI();
        HidePauseGameUI();
        
        //##
        MusicManager.Instance.PlayMusic_Puzzle();
    }

    protected override void Start()
    {
        base.Start();

        GameMode_Puzzle.Instance.OnChangedState += GameManager_OnChangedState;
        GameMode_Puzzle.Instance.OnPauseGame += GameManager_OnPauseGame;
    }

    /*
     * 
     */

    private void GameManager_OnChangedState(object sender, System.EventArgs e)
    {
        if (!GameMode_Puzzle.Instance.IsGameOver()) return;

        if (GameMode_Puzzle.Instance.IsGameWon())
        {
            SceneTransitionManager.Instance.LoadScene(EScene.Platformer);
        }
        else
        {
            SceneTransitionManager.Instance.ReloadCurrentScene();
        }
        
    }

    private void GameManager_OnPauseGame(object sender, System.EventArgs e)
    {
        if (GameMode_Puzzle.Instance.IsGamePaused())
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
        _puzzleUI.Show();
    }

    public void HidePuzzleUI()
    {
        _puzzleUI.Hide();
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
