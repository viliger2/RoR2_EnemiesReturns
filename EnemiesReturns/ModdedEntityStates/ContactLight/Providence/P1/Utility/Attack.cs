using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override string layerName => "Gesture, Override";

        public override string animationStateName => "Leap";

        public override string playbackRateParams => "Leap.playbackRate";

        public override string animatorAttackParam => "Leap.attack";

        public override float baseDuration => ProvidencePhase1.P1UtilityDuration.Value;

        public override float earlyExit => ProvidencePhase1.P1UtilityEarlyExit.Value;

        public override float landingDamage => ProvidencePhase1.P1UtilityDamage.Value;

        public override float landingRadius => ProvidencePhase1.P1UtilityRadius.Value;

        public override int orbCount => ProvidencePhase1.P1UtilityOrbCount.Value;

        public override float orbProjectileDamage => ProvidencePhase1.P1UtilityOrbProjectileDamage.Value;

        public override byte comboCount => ProvidencePhase1.P1UtilityOrbJumps.Value;

        public override float orbSpeedOverride => ProvidencePhase1.P1UtilityOrbSpeed.Value;

        public override float orbInitialAngle => ProvidencePhase1.P1UtilityOrbInitialAngle.Value;
    }
}
