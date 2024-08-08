using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    public class StompL : StompBase
    {
        public override void OnEnter()
        {
            animationStateName = "StompL";
            targetMuzzle = "FootL";
            base.OnEnter();
            attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), element => element.groupName == "LeftStomp");
        }
    }
}
