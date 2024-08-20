using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    public class StompR : StompBase
    {
        public override void OnEnter()
        {
            animationStateName = "StompR";
            targetMuzzle = "StompSpawnR";
            base.OnEnter();
            attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), element => element.groupName == "RightStomp");
        }
    }
}
