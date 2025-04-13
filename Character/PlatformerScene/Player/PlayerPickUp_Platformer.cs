using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.GameItem;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerPickUp_Platformer : RyoMonoBehaviour
{
    public event EventHandler<BaseGameItem_Platformer> OnPickUp;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.TryGetComponent(out BaseGameItem_Platformer gameItem))
        {
            OnPickUp?.Invoke(this, gameItem);
        }
    }
}
