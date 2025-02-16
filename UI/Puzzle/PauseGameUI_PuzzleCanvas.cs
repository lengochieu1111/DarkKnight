using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseGameUI_PuzzleCanvas : RyoMonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _exitButton;

    [SerializeField] private TextMeshProUGUI _musicText;
    [SerializeField] private TextMeshProUGUI _soundText;

    protected override void Awake()
    {
        base.Awake();

        _restartButton.onClick.AddListener(() =>
        {
            Restart();
        });
        
        _musicButton.onClick.AddListener(() =>
        {
            Music();
        });

        _soundButton.onClick.AddListener(() =>
        {
            Sound();
        });
        
        _mainMenuButton.onClick.AddListener(() =>
        {
            MainMenu();
        });

        _exitButton.onClick.AddListener(() =>
        {
            Exit();
        });

    }

    protected override void Start()
    {
        base.Start();

        UpdateVisual();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_restartButton == null)
        {
            _restartButton = transform.Find("RestartButton")?.GetComponent<Button>();
        }
        
        if (_musicButton == null)
        {
            _musicButton = transform.Find("MusicButton")?.GetComponent<Button>();
        }

        if (_soundButton == null)
        {
            _soundButton = transform.Find("SoundButton")?.GetComponent<Button>();
        }
        
        if (_mainMenuButton == null)
        {
            _mainMenuButton = transform.Find("MainMenuButton")?.GetComponent<Button>();
        }

        if (_exitButton == null)
        {
            _exitButton = transform.Find("ExitButton")?.GetComponent<Button>();
        }

        if (_musicText == null)
        {
            _musicText = transform.Find("MusicText")?.GetComponent<TextMeshProUGUI>();
        }

        if (_soundText == null)
        {
            _soundText = transform.Find("SoundText")?.GetComponent<TextMeshProUGUI>();
        }

    }

    /*
     * 
     */

    private void Restart()
    {
        TransitionManager.Instance.LoadScene(Scene.Puzzle, true);
    }
    
    private void Music()
    {
        AudioMixerManager.Instance.NextMusicVolumeState();

        UpdateVisual();
    }

    private void Sound()
    {
        AudioMixerManager.Instance.NextSoundVolumeState();

        UpdateVisual();
    }
    
    private void MainMenu()
    {
        TransitionManager.Instance.LoadScene(Scene.MainMenu);
    }

    private void Exit()
    {
        if (GameMode_Puzzle.Instance.IsGamePaused())
        {
            GameMode_Puzzle.Instance.TogglePauseGame();
        }

        Hide();
    }

    /*
     * 
     */

    private void UpdateVisual()
    {
        _musicText.text = AudioMixerManager.Instance.GetCurrentMusicVolumeStateString();
        _soundText.text = AudioMixerManager.Instance.GetCurrentSoundVolumeStateString();
    }

    /*
     * 
     */

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
