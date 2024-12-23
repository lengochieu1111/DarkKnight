using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;

public class PuzzleCanvas : Singleton<PuzzleCanvas>
{
    [SerializeField] private PuzzleUI _puzzleUI;
    [SerializeField] private PauseGameUI_PuzzleCanvas _pauseGameUI;
    [SerializeField] private TransitionUI _transitionUI;

    protected override void Start()
    {
        base.Start();

        GameManager_Puzzle.Instance.OnChangedState += GameManager_Puzzle_OnChangedState;
        GameManager_Puzzle.Instance.OnPausegame += GameManager_OnPausegame;
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._puzzleUI == null)
        {
            this._puzzleUI = this.GetComponentInChildren<PuzzleUI>(true);
        }

        if (this._pauseGameUI == null)
        {
            this._pauseGameUI = this.GetComponentInChildren<PauseGameUI_PuzzleCanvas>(true);
        }
        
        if (this._transitionUI == null)
        {
            this._transitionUI = this.GetComponentInChildren<TransitionUI>(true);
        }

    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        this.ShowPuzzleUI();
        this.HidePauseGameUI();
        this.HideTransitionUI();
    }

    /*
     * 
     */

    private void GameManager_Puzzle_OnChangedState(object sender, System.EventArgs e)
    {
        if (GameManager_Puzzle.Instance.IsGameOver())
        {
            this.ShowTransitionUI();
        }
        else
        {
            this.HideTransitionUI();
        }
    }

    private void GameManager_OnPausegame(object sender, System.EventArgs e)
    {
        if (GameManager_Puzzle.Instance.IsGamePaused())
        {
            this.ShowPauseGameUI();
        }
        else
        {
            this.HidePauseGameUI();
        }
    }

    /*
     * 
     */

    public void ShowPuzzleUI()
    {
        this._puzzleUI.Show();
    }

    public void HidePuzzleUI()
    {
        this._puzzleUI.Hide();
    }
    
    public void ShowPauseGameUI()
    {
        this._pauseGameUI.Show();
    }

    public void HidePauseGameUI()
    {
        this._pauseGameUI.Hide();
    }
    
    public void ShowTransitionUI()
    {
        this._transitionUI.Show();
    }

    public void HideTransitionUI()
    {
        this._transitionUI.Hide();
    }
}
