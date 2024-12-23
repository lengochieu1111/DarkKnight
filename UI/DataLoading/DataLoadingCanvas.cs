using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoadingCanvas : RyoMonoBehaviour
{

    protected override void Start()
    {
        FirebaseManager.Instance.OnGetDataCompleted += FirebaseManager_OnGetDataCompleted;
    }

    private void FirebaseManager_OnGetDataCompleted(object sender, System.EventArgs e)
    {
        TransitionManager.Instance.Load_LoginScene();
    }

}
