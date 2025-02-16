using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerUI : RyoMonoBehaviour
{
    public void Show()
    {
        // UpdateVisual();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
