using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Effect;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.Platformer.Script.Interface;
using NaughtyAttributes;
using UnityEngine;

public class Spike_Platformer : AttackEffect_Platformer
{
    [SerializeField, BoxGroup] private Transform _playerAppearTransformLeft;
    [SerializeField, BoxGroup] private Transform _playerAppearTransformRight;
    [SerializeField, BoxGroup] private int _damage = 20;

    protected override void SetupComponents()
    {
        base.SetupComponents();
        
        //##
        Setup(new HitData(damageCauser: this, damage: _damage));
    }
    
    protected override void ResetValues()
    {
        base.ResetValues();

        _isTracing = true;
    }

    protected override void HandleInteracted(Transform hitTransform)
    {
        base.HandleInteracted(hitTransform);
          
        //##
        
        if (hitTransform.TryGetComponent(out Player_Platformer player))
        {
            if (player.IsFlippingLeft)
            {
                hitTransform.position = _playerAppearTransformLeft.position;
            }
            else
            {
                hitTransform.position = _playerAppearTransformRight.position;
            }
            _hitedList.Clear();
        }

    }

}
