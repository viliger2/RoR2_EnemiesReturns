using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override float baseDuration => 1f;

        public override float earlyExit => 1f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackRateParams => "SkyLeap.playbackRate";

        public override string animatorAttackParam => "SkyLeap.firstAttack";

        public override int waveCount => 4;

        public override float waveProjectileDamage => 2f;

        public override float waveProjectileForce => 500f;
    }
}
