using System;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.Platformer.Script.Game;
using HIEU_NL.Platformer.Script.Interface;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_PlaformerCanvas : RyoMonoBehaviour
{
    [SerializeField] private Image _healthBarImage;
    private Player_Platformer _player;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        //##
        GameMode_Platformer.Instance.OnChangedState += GameMode_OnOnChangedState;
    }

    private void GameMode_OnOnChangedState(object sender, EventArgs e)
    {
        _player = GameMode_Platformer.Instance.Player;
        _player.OnTakeDamage += Player_OnOnTakeDamage;
    }

    private void Player_OnOnTakeDamage(object sender, HitData e)
    {
        Update_HealthBar();
    }

    public void Update_HealthBar()
    {
        _healthBarImage.fillAmount = _player.GetHealthPercentage();
    }
    
}
