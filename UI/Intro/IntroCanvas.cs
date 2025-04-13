using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using UnityEngine;

public class IntroCanvas : RyoMonoBehaviour
{
    protected override void Start()
    {
        FirebaseManager.Instance.OnGetDataCompleted += FirebaseManager_OnGetDataCompleted;
    }

    private void FirebaseManager_OnGetDataCompleted(object sender, System.EventArgs e)
    {
        if (FirebaseManager.Instance.CurrentUser != null)
        {
            SceneTransitionManager.Instance.LoadScene(EScene.MainMenu);
        }
        else
        {
            SceneTransitionManager.Instance.LoadScene(EScene.Login);
        }
    }

}
