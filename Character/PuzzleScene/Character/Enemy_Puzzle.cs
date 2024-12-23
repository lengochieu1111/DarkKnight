using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity.Character;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Enemy
{
    public class Enemy_Puzzle : DynamicEntity
    {

        protected override void MoveCompleted()
        {
            base.MoveCompleted();

            if (this.TryInteractWithStaticEntities(out List<StaticEntity> staticEntitis))
            {
                foreach (StaticEntity entity in staticEntitis)
                {
                    if (entity is Trap_Puzzle)
                    {
                        this.HandleDead();

                        return;
                    }
                }
            }
        }

        protected override void HandleCannotMove()
        {
            base.HandleCannotMove();

            this.HandleDead();
        }

        public override void ReceiveInteract(BaseEntity senderEntity, Vector2 receverDirection)
        {
            base.ReceiveInteract(senderEntity, receverDirection);

            if (senderEntity is Player_Puzzle)
            {
                this.RequestAction(receverDirection);
            }
        }

        private void HandleDead()
        {
            this.isDead = true;

            this.DestroySelf();
        }

    }

}
