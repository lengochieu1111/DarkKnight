using System.Collections;
using System.Collections.Generic;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Entity.Player;
using UnityEngine;

public class GameMode : Singleton<GameMode>
{
    [SerializeField] public Player _player;

    // [SerializeField] private Map _map;
    // [SerializeField] private HUD _hud;
    
    public Player GetPlayer() => _player;

}
