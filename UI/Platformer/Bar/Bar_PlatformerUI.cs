using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Bar_PlatformerUI : RyoMonoBehaviour
{
    [SerializeField, BoxGroup] private Image[] _barImageArray;
    [SerializeField, BoxGroup] private Image[] _imageArray;
    [SerializeField, BoxGroup] private bool _onlyShowHealthBarWhenLowHealth;
    [SerializeField, BoxGroup] private float _hideBarTime = 4f;
    private const float MIN_HEALTH = 0;
    private const float MAX_HEALTH = 1;
    
    private Coroutine _hideBarCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        Update_Bar(MAX_HEALTH);
        
        if (_onlyShowHealthBarWhenLowHealth)
        {
            Hide();
        }
    }

    public void Update_Bar(float percentage)
    {
        try
        {
            if(_barImageArray.IsNullOrEmpty()) return;

            foreach (Image barImage in _barImageArray)
            {
                barImage.fillAmount = Mathf.Clamp(percentage, MIN_HEALTH, MAX_HEALTH);
            }

            if (_onlyShowHealthBarWhenLowHealth && percentage < MAX_HEALTH)
            {
                Show();
                
                if (_hideBarCoroutine != null)
                {
                    StopCoroutine(_hideBarCoroutine);
                }
                
                _hideBarCoroutine = StartCoroutine(IE_HideBarCoroutine());
            }
        
        }
        catch (Exception e)
        {

        }
        
    }
    
    private void Show()
    {
        if(_imageArray.IsNullOrEmpty()) return;
        
        foreach (Image barImage in _imageArray)
        {
            barImage.enabled = true;
        }
    }
    
    public void Hide()
    {
        if(_imageArray.IsNullOrEmpty()) return;

        foreach (Image barImage in _imageArray)
        {
            barImage.enabled = false;
        }
    }

    private IEnumerator IE_HideBarCoroutine()
    {
        yield return new WaitForSeconds(_hideBarTime);
        Hide();
    }
    
}
