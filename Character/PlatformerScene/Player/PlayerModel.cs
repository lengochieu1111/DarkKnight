using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Player
{
    using Architecture.MVC;
    using HIEU_NL.Platformer.SO;

    public class PlayerModel : MVC_Model<Player>
    {
        [field: SerializeField] public PlayerMovementStats MovementStats { get; private set; }
        [field: SerializeField] public PlayerAttackStats AttackStats { get; private set; }
    }
}
