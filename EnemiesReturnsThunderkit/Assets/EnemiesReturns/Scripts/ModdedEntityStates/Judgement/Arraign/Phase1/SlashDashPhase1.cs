using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1
{
    public class SlashDashPhase1 : BaseSlashDash
    {
        public override float baseDuration => 0.5f;

        public override float damageCoefficient => 2f;

        public override float procCoefficient => 1f;

        public override float turnSpeed => 100f;

        public override float dashMoveSpeedCoefficient => 3f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SlashDash";

        public override string playbackParamName => "SlashDash.playbackRate";

        public override string hitBoxGroupName => "Sword";
    }
}
