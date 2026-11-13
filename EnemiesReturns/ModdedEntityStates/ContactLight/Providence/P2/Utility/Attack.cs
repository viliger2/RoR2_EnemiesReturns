using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override float baseDuration => ProvidencePhase2.P2UtilityDuration.Value;

        public override float earlyExit => ProvidencePhase2.P2UtilityEarlyExit.Value;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "Leap";

        public override string playbackRateParams => "Leap.playbackRate";

        public override string animatorAttackParam => "Leap.attack";

        public override int orbCount => ProvidencePhase2.P2UtilityOrbCount.Value;

        public override float orbProjectileDamage => ProvidencePhase2.P2UtilityDamage.Value;

        public override byte comboCount => ProvidencePhase2.P2UtilityOrbJumps.Value;

        public override float orbSpeedOverride => ProvidencePhase2.P2UtilityOrbSpeed.Value;

        public override float orbInitialAngle => ProvidencePhase2.P2UtilityOrbInitialAngle.Value;

        public override float landingDamage => ProvidencePhase2.P2UtilityDamage.Value;

        public override float landingRadius => ProvidencePhase2.P2UtilityRadius.Value;
    }
}
