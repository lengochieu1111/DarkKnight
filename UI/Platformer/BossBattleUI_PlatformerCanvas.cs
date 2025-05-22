using HIEU_NL.Platformer.Script.Entity.Enemy.Boss;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.SO.Map;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class BossBattleUI_PlatformerCanvas : RyoMonoBehaviour
{
    [SerializeField, BoxGroup("Background")] private MapDataListSO _mapDataListSO;
    [SerializeField, BoxGroup("Background")] private Image _backgroundImage;
    [SerializeField, BoxGroup("Background")] private Image _bossImage;
    
    [SerializeField, BoxGroup("Player Bar")] private Bar_PlatformerUI _playerHealthBar;
    [SerializeField, BoxGroup("Player Bar")] private Bar_PlatformerUI _playerEnergyBar;
    
    [SerializeField, BoxGroup("Boss Bar")] private Bar_PlatformerUI _bossHealthBar;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        Player_Platformer.OnHealthChange += Player_OnHealthChange;
        Player_Platformer.OnEnergyChange += Player_OnEnergyChange;
        BaseBoss.OnHealthChange += Boss_OnHealthChange;
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
    
    #endregion
    
    //#

    private void ShowEffect()
    {
        
    }
    
    //#
    
    public void Show()
    {
        int levelIndex = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;
        _bossImage.sprite = _mapDataListSO.MapAssetList[levelIndex].MapSprite;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
}
