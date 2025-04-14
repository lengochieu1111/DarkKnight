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
    [SerializeField, BoxGroup] private Button _menuButton;
    [SerializeField, BoxGroup] private Button _nextButton;
    [SerializeField, BoxGroup] private TextMeshProUGUI _nextText;

    protected override void Awake()
    {
        base.Awake();
        
        //##
        _menuButton.onClick.AddListener(() =>
        {
            SceneTransitionManager.Instance.LoadScene(EScene.MainMenu);
        });
        
        _nextButton.onClick.AddListener(() =>
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
        if (GameMode_Platformer.Instance.IsGameWon)
        {
            _nextText.text = "Next Level";
        }
        else
        {
            _nextText.text = "Restart Level";
        }
        
    }


    //#
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
