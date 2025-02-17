using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Puzzle.Script.Game;

public class PuzzleCanvas : Singleton<PuzzleCanvas>
{
    [SerializeField] private PuzzleUI _puzzleUI;
    [SerializeField] private PauseGameUI_PuzzleCanvas _pauseGameUI;

    protected override void Start()
    {
        base.Start();

        GameMode_Puzzle.Instance.OnChangedState += GameManager_OnChangedState;
        GameMode_Puzzle.Instance.OnPauseGame += GameManager_OnPauseGame;
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_puzzleUI == null)
        {
            _puzzleUI = GetComponentInChildren<PuzzleUI>(true);
        }

        if (_pauseGameUI == null)
        {
            _pauseGameUI = GetComponentInChildren<PauseGameUI_PuzzleCanvas>(true);
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
        if (!GameMode_Puzzle.Instance.IsGameOver()) return;

        if (GameMode_Puzzle.Instance.IsGameWon())
        {
            TransitionManager.Instance.LoadScene(Scene.Platformer);
        }
        else
        {
            TransitionManager.Instance.LoadScene(Scene.Puzzle, true);
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
