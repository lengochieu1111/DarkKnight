using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Entity.Enemy;
using HIEU_NL.Platformer.Script.Entity.Enemy.Boss;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.Platformer.Script.Game;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PlatformerUI : RyoMonoBehaviour
{
    [SerializeField, BoxGroup("Bar")] private Bar_PlatformerUI _healthBar;
    [SerializeField, BoxGroup("Bar")] private Bar_PlatformerUI _energyBar;
    [SerializeField, BoxGroup("Bar")] private Bar_PlatformerUI _enemyProcess;
    [SerializeField, BoxGroup("Bar")] private TextMeshProUGUI _enemyRemainingText;
    private Player_Platformer _player;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _enemyProcess.Update_Bar(0f);
        
        GameMode_Platformer.OnSetupSuccess += GameMode_OnSetupSuccess;
        Player_Platformer.OnHealthChange += Player_OnHealthChange;
        Player_Platformer.OnEnergyChange += Player_OnEnergyChange;
        BaseEnemy.OnAnyDeadEnemy += BaseEnemy_OnAnyDeadEnemy;
    }

    protected override void Start()
    {
        base.Start();
        
        //##
        
    }


    #region Event Action
    
    private void GameMode_OnSetupSuccess(object sender, int e)
    {
        _enemyRemainingText.text = (e - 1).ToString();
    }

    private void Player_OnHealthChange(object sender, float e)
    {
        _healthBar.Update_Bar(e);
    }
    
    private void Player_OnEnergyChange(object sender, float e)
    {
        _energyBar.Update_Bar(e);
    }
    
    private void BaseEnemy_OnAnyDeadEnemy(object sender, EventArgs e)
    {
        int enemyNormalCount = GameMode_Platformer.Instance.EnemyList.FindAll(enemy => !enemy.IsDead && enemy is not BaseBoss).Count;
        _enemyRemainingText.text = enemyNormalCount.ToString();
        
        float percent = 1 - ((float)enemyNormalCount / (GameMode_Platformer.Instance.EnemyList.Count - 1));
        _enemyProcess.Update_Bar(percent);
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
