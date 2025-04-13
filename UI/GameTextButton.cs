using HIEU_NL.Manager;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.ObjectPool.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTextButton : RyoMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Transform _buttonPointer;

    protected override void Start()
    {
        base.Start();

        HideButtonPointer();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (_buttonPointer == null)
        {
            _buttonPointer = transform.Find("ButtonPointer");
        }
    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        _buttonPointer.gameObject.SetActive(false);
    }

    /*
     * 
     */

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowButtonPointer();

        ((SoundManager)SoundManager.Instance).PlaySound(SoundType.Button_Menu_Highlight);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideButtonPointer();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ((SoundManager)SoundManager.Instance)?.PlaySound(SoundType.Button_Menu_Confirm);
    }

    /*
     * 
     */

    private void ShowButtonPointer()
    {
        _buttonPointer?.gameObject.SetActive(true);
    }
    
    private void HideButtonPointer()
    {
        _buttonPointer?.gameObject.SetActive(false);
    }


}
