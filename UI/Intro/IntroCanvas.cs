using System.Collections;
using System.Collections.Generic;
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
            TransitionManager.Instance.LoadScene(Scene.MainMenu);
        }
        else
        {
            TransitionManager.Instance.LoadScene(Scene.Login);
        }
    }

}
