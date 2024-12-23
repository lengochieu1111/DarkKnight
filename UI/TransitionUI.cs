using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HIEU_NL.Utilities.ParameterExtensions.Window;

public class TransitionUI : RyoMonoBehaviour
{
    [SerializeField] private Transform _transitionTransform;

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (this._transitionTransform == null)
        {
            this._transitionTransform = this.transform.Find("Transition");
        }
    }

    /*
     * 
     */

    public void Show()
    {
        this._transitionTransform.DOMoveY(-WINDOW_HEIGHT, 0);

        this.gameObject.SetActive(true);

        this._transitionTransform.DOMoveY(WINDOW_HEIGHT / 2, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Debug.Log("Transition Completed");
        });

    }

    public void Hide()
    {
        this._transitionTransform.DOMoveY(-WINDOW_HEIGHT, 0);

        this.gameObject.SetActive(false);
    }


}
