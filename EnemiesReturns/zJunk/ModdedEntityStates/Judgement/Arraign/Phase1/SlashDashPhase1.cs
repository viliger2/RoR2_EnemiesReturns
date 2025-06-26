namespace EnemiesReturns.zJunk.ModdedEntityStates.Judgement.Arraign.Phase1
{
    public class SlashDashPhase1 : BaseSlashDash
    {
        public override float baseDuration => 1f;

        public override float damageCoefficient => 2f;

        public override float procCoefficient => 1f;

        public override float turnSpeed => 100f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SlashDash";

        public override string playbackParamName => "SlashDash.playbackRate";

        public override string hitBoxGroupName => "Sword";
    }
}
