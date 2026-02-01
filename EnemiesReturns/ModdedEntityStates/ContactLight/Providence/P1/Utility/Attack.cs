using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackRateParams => "SkyLeap.playbackRate";

        public override string animatorAttackParam => "SkyLeap.firstAttack";

        public override float baseDuration => Configuration.General.ProvidenceP1UtilityAttackDuraion.Value;

        public override float earlyExit => Configuration.General.ProvidenceP1UtilityEarlyExit.Value;

        public override int waveCount => 4;

        public override float waveProjectileDamage => 2f;

        public override float waveProjectileForce => 500f;
    }
}
