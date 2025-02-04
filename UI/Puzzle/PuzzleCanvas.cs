using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Puzzle.Script.Game;

public class PuzzleCanvas : Singleton<PuzzleCanvas>
{
    [SerializeField] private PuzzleUI _puzzleUI;
    [SerializeField] private PauseGameUI_PuzzleCanvas _pauseGameUI;
    [SerializeField] private TransitionUI _transitionUI;

    protected override void Start()
    {
        base.Start();

        GameMode_Puzzle.Instance.OnChangedState += GameManager_Puzzle_OnChangedState;
        GameMode_Puzzle.Instance.OnPauseGame += GameManagerOnPauseGame;
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
        
        if (_transitionUI == null)
        {
            _transitionUI = GetComponentInChildren<TransitionUI>(true);
        }

    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        ShowPuzzleUI();
        HidePauseGameUI();
        HideTransitionUI();
    }

    /*
     * 
     */

    private void GameManager_Puzzle_OnChangedState(object sender, System.EventArgs e)
    {
        if (GameMode_Puzzle.Instance.IsGameOver())
        {
            ShowTransitionUI();
        }
        else
        {
            HideTransitionUI();
        }
    }

    private void GameManagerOnPauseGame(object sender, System.EventArgs e)
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
    
    public void ShowTransitionUI()
    {
        _transitionUI.Show();
    }

    public void HideTransitionUI()
    {
        _transitionUI.Hide();
    }
}
