using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Game;
using UnityEngine;

public class PlatformerCanvas : Singleton<PlatformerCanvas>
{
    public event EventHandler OnBossComing;
    
    [SerializeField] private PlatformerUI _platformerUI;
    [SerializeField] private BossBattleUI_PlatformerCanvas _bossBattleUI;
    [SerializeField] private EndGameUI_PlatformerCanvas _endGameUI;
    [SerializeField] private PauseGameUI_PlatformerCanvas _pauseGameUI;
    
    [Header("Boss Coming Effects")]
    [SerializeField] private GameObject _bossComingObject;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 1.0f;
    [SerializeField] private Ease _fadeInEase = Ease.InOutSine;
    [SerializeField] private Ease _fadeOutEase = Ease.InOutSine;
    [SerializeField] private float _delayBetweenFades = 1.0f;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        ShowPlatformerUI();
        HideBossBattleUI();
        HidePauseGameUI();
        HideEndGameUI();
        HideBossComingEffect();
        
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
        ShowBossComingEffect();
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
    
    private void ShowBossComingEffect()
    {
        _bossComingObject.SetActive(true);

        // Tạo tween để fade in từ 0 đến 1
        _canvasGroup.DOFade(1f, _fadeDuration)
            .SetEase(_fadeInEase)
            .OnComplete(() => 
            {
                //
                OnBossComing?.Invoke(this, EventArgs.Empty);
                
                ShowBossBattleUI();
                HidePlatformerUI();
                
                UT_ShowBossComingEffect().Forget();
            });

        async UniTask UT_ShowBossComingEffect()
        {
            await UniTask.WaitForSeconds(_delayBetweenFades);
            
            // Tạo tween để fade out từ 1 đến 0
            _canvasGroup.DOFade(0f, _fadeDuration / 2f)
                .SetEase(_fadeOutEase)
                .OnComplete(() =>
                {
                    HideBossComingEffect();
                });
        }
        
    }

    private void HideBossComingEffect()
    {
        _canvasGroup.alpha = 0f;
        _bossComingObject.SetActive(false);
    }
    
}
