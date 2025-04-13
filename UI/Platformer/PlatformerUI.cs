using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.Platformer.Script.Game;
using HIEU_NL.Platformer.Script.Interface;
using NaughtyAttributes;
using UnityEngine;

public class PlatformerUI : RyoMonoBehaviour
{
    [SerializeField, BoxGroup("Bar")] private Bar_PlatformerUI _healthBar;
    [SerializeField, BoxGroup("Bar")] private Bar_PlatformerUI _energyBar;
    private Player_Platformer _player;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        Player_Platformer.OnHealthChange += Player_OnHealthChange;
        Player_Platformer.OnEnergyChange += Player_OnEnergyChange;
    }

    #region Event Action

    private void Player_OnHealthChange(object sender, float e)
    {
        _healthBar.Update_Bar(e);
    }
    
    private void Player_OnEnergyChange(object sender, float e)
    {
        _energyBar.Update_Bar(e);
    }
    
    #endregion
    
    //#
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    

    
}
