using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvas : RyoMonoBehaviour
{
    [SerializeField] private bool _isFirstUpdate;

    private void Update()
    {
        if (!this._isFirstUpdate)
        {
            this._isFirstUpdate = true;

            TransitionManager.Instance.LoadCallback();
        }
    }

}
