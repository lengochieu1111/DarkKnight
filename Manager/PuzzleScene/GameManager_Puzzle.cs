using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;
using System;
using HIEU_NL.Puzzle.Script.Entity.Player;

public class GameManager_Puzzle : Singleton<GameManager_Puzzle>
{
    public event EventHandler OnChangedState;
    public event EventHandler OnPausegame;

    private enum State
    {
        WaitingToStart,
        GamePlaying,
        GameOver
    }

    [Header("State")]
    [SerializeField] private State _state;
    [SerializeField] private float _waitingTimeToStartTimer = 1f;
    [SerializeField] private float _gamePlayTimer = 30f;
    [SerializeField] private int _gameActionCounter = 20;

    [SerializeField] private bool _isGamePaused;

    [Header("Level")]
    [SerializeField] private int _levelIndex;


    protected override void Start()
    {
        base.Start();

        Player_Puzzle.OnPlayerActed += Player_Puzzle_OnPlayerActed;
        Player_Puzzle.OnPlayerPause += Player_Puzzle_OnPlayerPause;
        Player_Puzzle.OnPlayerLoses += Player_Puzzle_OnPlayerLoses;

        //
        this.SpawnLevel();
    }

    private void Update()
    {
        switch (this._state)
        {
            case State.WaitingToStart:
                this._waitingTimeToStartTimer -= Time.deltaTime;

                if (this._waitingTimeToStartTimer < 0f)
                {
                    this._state = State.GamePlaying;

                    OnChangedState?.Invoke(this, EventArgs.Empty);
                }

                break; 
            case State.GamePlaying:
                this._gamePlayTimer -= Time.deltaTime;

                if (this._gamePlayTimer < 0f || this._gameActionCounter < 0)
                {
                    this._state = State.GameOver;

                    OnChangedState?.Invoke(this, EventArgs.Empty);
                }

                break; 
            case State.GameOver:
                break; 
            default:
                break;
        }

    }

    protected override void SetupValues()
    {
        base.SetupValues();

        LevelSO_Puzzle levelSO = LevelManager_Puzzle.Instance.GetLevelSOWithIndex(this._levelIndex);
        //this._gamePlayTimer = levelSO.MaxTime;
        //this._gameActionCounter = levelSO.MaxAction;

    }

    /*
     * 
     */

    private void Player_Puzzle_OnPlayerActed(object sender, EventArgs e)
    {
        this._gameActionCounter--;
    }

    private void Player_Puzzle_OnPlayerPause(object sender, EventArgs e)
    {
        this.TogglePauseGame();
    }

    private void Player_Puzzle_OnPlayerLoses(object sender, EventArgs e)
    {
        this._gameActionCounter = -1;
    }

    /*
     * 
     */

    private void SpawnLevel()
    {
        LevelManager_Puzzle.Instance.SpawnLevel(this._levelIndex);
    }

    public void TogglePauseGame()
    {
        this._isGamePaused = !this._isGamePaused;

        if (this._isGamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        OnPausegame?.Invoke(this, EventArgs.Empty);
    }

    /*
     * 
     */

    public int GetGameActionCounter()
    {
        return this._gameActionCounter;
    }
    
    public float GetGamePlayTimer()
    {
        return this._gamePlayTimer;
    }
    
    public int GetLevelIndex()
    {
        return this._levelIndex;
    }

    public bool IsGamePaused()
    {
        return this._isGamePaused;
    }
    
    public bool IsGamePlaying()
    {
        return this._state is State.GamePlaying;
    }
    
    public bool IsGameOver()
    {
        return this._state is State.GameOver;
    }

}
