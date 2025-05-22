using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using HIEU_NL.Platformer.Script.Game;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI_PlatformerCanvas : RyoMonoBehaviour
{
    private const string RESULT_VICTORY = "VICTORY";
    private const string RESULT_DEFEAT = "DEFEAT";
    
    [SerializeField, BoxGroup] private Button _menuButton;
    [SerializeField, BoxGroup] private Button _actionButton;
    [SerializeField, BoxGroup] private TextMeshProUGUI _resultText;
    [SerializeField, BoxGroup] private TextMeshProUGUI _actionText;

    protected override void Awake()
    {
        base.Awake();
        
        //##
        _menuButton.onClick.AddListener(() =>
        {
            SceneTransitionManager.Instance.LoadScene(EScene.MainMenu);
        });
        
        _actionButton.onClick.AddListener(() =>
        {
            if (GameMode_Platformer.Instance.IsGameWon)
            {
                SceneTransitionManager.Instance.LoadScene(EScene.Puzzle);
            }
            else
            {
                SceneTransitionManager.Instance.ReloadCurrentScene();
            }
        });

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##

        UpdateVisuals();
    }
    
    //#
    

    private void UpdateVisuals()
    {
        if (GameMode_Platformer.Instance.IsGameWon)
        {
            _resultText.text = RESULT_VICTORY;
            _actionText.text = "Next Level";
        }
        else
        {
            _resultText.text = RESULT_DEFEAT;
            _actionText.text = "Restart Level";
        }
    }


    //#
    
    public void Show()
    {
        try
        {
            gameObject.SetActive(true);
        }
        catch (Exception e)
        { }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
