using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity.Character;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Enemy
{
    public class Enemy_Puzzle : DynamicEntity_Puzzle
    {

        protected override void MoveCompleted()
        {
            base.MoveCompleted();

            if (this.TryInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntitis))
            {
                foreach (StaticEntity_Puzzle entity in staticEntitis)
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

        public override void ReceiveInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
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
