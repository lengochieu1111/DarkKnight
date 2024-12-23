using Architecture.MVC;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Player
{
    public class PlayerView : MVC_View<Player>
    {
        public void AE_RequestSlash()
        {
            controller.SpawnSlash();
        }

        public void AE_EndAttack()
        {
            controller.EndAttack();
        }

    }

}
