using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.LeapingDash
{

    public class LeapDash : BaseLeapDash
    {
        public override float damageCoefficient => 2f;

        public override float liftOffTimer => 1.7f / 1f; // 1 is animation speed

        public override float force => 0f;

        public override float procCoefficient => 1f;

        public override float blastAttackRadius => 10f;

        public override float upwardVelocity => 30f;

        public override float forwardVelocity => 70f;

        public override float minimumY => 0.05f;

        public override float aimVelocity => 0f;

        public override float airControl => 1f;

        public override float additionalGravity => 0f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SwordFlipBegin";

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new LeapDashExit());
        }
    }
}
