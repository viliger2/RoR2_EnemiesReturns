using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override float baseDuration => ProvidencePhase3.P3UtilityDuration.Value;

        public override float earlyExit => ProvidencePhase3.P3UtilityEarlyExit.Value;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "Leap";

        public override string playbackRateParams => "Leap.playbackRate";

        public override string animatorAttackParam => "Leap.attack";

        public override int orbCount => ProvidencePhase3.P3UtilityOrbCount.Value;

        public override float orbProjectileDamage => ProvidencePhase3.P3UtilityOrbProjectileDamage.Value;

        public override byte comboCount => ProvidencePhase3.P3UtilityOrbJumps.Value;

        public override float orbSpeedOverride => ProvidencePhase3.P3UtilityOrbSpeed.Value;

        public override float orbInitialAngle => ProvidencePhase3.P3UtilityOrbInitialAngle.Value;

        public override float landingDamage => ProvidencePhase3.P3UtilityDamage.Value;

        public override float landingRadius => ProvidencePhase3.P3UtilityRadius.Value;
    }
}
