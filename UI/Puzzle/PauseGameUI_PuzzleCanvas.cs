using System.Collections;
using System.Collections.Generic;
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

        this._restartButton.onClick.AddListener(() =>
        {
            this.Restart();
        });
        
        this._musicButton.onClick.AddListener(() =>
        {
            this.Music();
        });

        this._soundButton.onClick.AddListener(() =>
        {
            this.Sound();
        });
        
        this._mainMenuButton.onClick.AddListener(() =>
        {
            this.MainMenu();
        });

        this._exitButton.onClick.AddListener(() =>
        {
            this.Exit();
        });

    }

    protected override void Start()
    {
        base.Start();

        this.UpdateVisual();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._restartButton == null)
        {
            this._restartButton = this.transform.Find("RestartButton")?.GetComponent<Button>();
        }
        
        if (this._musicButton == null)
        {
            this._musicButton = this.transform.Find("MusicButton")?.GetComponent<Button>();
        }

        if (this._soundButton == null)
        {
            this._soundButton = this.transform.Find("SoundButton")?.GetComponent<Button>();
        }
        
        if (this._mainMenuButton == null)
        {
            this._mainMenuButton = this.transform.Find("MainMenuButton")?.GetComponent<Button>();
        }

        if (this._exitButton == null)
        {
            this._exitButton = this.transform.Find("ExitButton")?.GetComponent<Button>();
        }

        if (this._musicText == null)
        {
            this._musicText = this.transform.Find("MusicText")?.GetComponent<TextMeshProUGUI>();
        }

        if (this._soundText == null)
        {
            this._soundText = this.transform.Find("SoundText")?.GetComponent<TextMeshProUGUI>();
        }

    }

    /*
     * 
     */

    private void Restart()
    {
        Debug.Log("Restart");
    }
    
    private void Music()
    {
        AudioMixerManager.Instance.NextMusicVolumeState();

        this.UpdateVisual();
    }

    private void Sound()
    {
        AudioMixerManager.Instance.NextSoundVolumeState();

        this.UpdateVisual();
    }
    
    private void MainMenu()
    {
        Debug.Log("MainMenu");
    }

    private void Exit()
    {
        if (GameManager_Puzzle.Instance.IsGamePaused())
        {
            GameManager_Puzzle.Instance.TogglePauseGame();
        }

        this.Hide();
    }

    /*
     * 
     */

    private void UpdateVisual()
    {
        this._musicText.text = AudioMixerManager.Instance.GetCurrentMusicVolumeStateString();
        this._soundText.text = AudioMixerManager.Instance.GetCurrentSoundVolumeStateString();
    }

    /*
     * 
     */

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
