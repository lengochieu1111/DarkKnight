using HIEU_NL.Platformer.Script.Entity.Enemy.Boss;
using HIEU_NL.Platformer.Script.Entity.Player;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class BossBattleUI_PlatformerCanvas : RyoMonoBehaviour
{
    [SerializeField, BoxGroup("Background")] private Image _backgroundImage;
    
    [SerializeField, BoxGroup("Player Bar")] private Bar_PlatformerUI _playerHealthBar;
    [SerializeField, BoxGroup("Player Bar")] private Bar_PlatformerUI _playerEnergyBar;
    
    [SerializeField, BoxGroup("Boss Bar")] private Bar_PlatformerUI _bossHealthBar;
    [SerializeField, BoxGroup("Boss Bar")] private Bar_PlatformerUI _bossEnergyBar;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        Player_Platformer.OnHealthChange += Player_OnHealthChange;
        Player_Platformer.OnEnergyChange += Player_OnEnergyChange;
        BaseBoss.OnHealthChange += Boss_OnHealthChange;
        BaseBoss.OnEnergyChange += Boss_OnEnergyChange;
    }

    #region Event Action

    private void Player_OnHealthChange(object sender, float e)
    {
        _playerHealthBar.Update_Bar(e);
    }
    
    private void Player_OnEnergyChange(object sender, float e)
    {
        _playerEnergyBar.Update_Bar(e);
    }
    
    private void Boss_OnHealthChange(object sender, float e)
    {
        _bossHealthBar.Update_Bar(e);
    }
    
    private void Boss_OnEnergyChange(object sender, float e)
    {
        _bossEnergyBar.Update_Bar(e);
    }
    
    #endregion
    
    //#

    private void ShowEffect()
    {
        
    }
    
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
