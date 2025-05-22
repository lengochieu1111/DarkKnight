using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreditUI : RyoMonoBehaviour
{
    [SerializeField] private Button _exitButton;
    [SerializeField] private GameObject _LoadingObject;

    protected override void Awake()
    {
        base.Awake();

        _exitButton.onClick.AddListener(() =>
        {
            Exit();
        });

    }


    /*
     *
     */

    private void Exit()
    {
        Hide();
    }

    /*
     *
     */

    public async UniTask Show()
    {
        HideLoadingEffect();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ShowLoadingEffect();
    }
    
    private void ShowLoadingEffect()
    {
        _LoadingObject.SetActive(true);
    }

    private void HideLoadingEffect()
    { 
        _LoadingObject.SetActive(false);
    }
    
}
