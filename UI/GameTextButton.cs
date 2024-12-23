using HIEU_NL.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTextButton : RyoMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Transform _buttonPointer;

    protected override void Start()
    {
        base.Start();

        this.HideButtonPointer();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._buttonPointer == null)
        {
            this._buttonPointer = this.transform.Find("ButtonPointer");
        }
    }

    protected override void ResetComponents()
    {
        base.ResetComponents();

        this._buttonPointer.gameObject.SetActive(false);
    }

    /*
     * 
     */

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.ShowButtonPointer();

        SoundManager.Instance.PlaySound(HIEU_NL.ObjectPool.Audio.SoundType.Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.HideButtonPointer();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(HIEU_NL.ObjectPool.Audio.SoundType.Click);

    }

    /*
     * 
     */

    private void ShowButtonPointer()
    {
        this._buttonPointer.gameObject.SetActive(true);
    }
    
    private void HideButtonPointer()
    {
        this._buttonPointer.gameObject.SetActive(false);
    }


}
