using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override float baseDuration => 3.5f;

        public override float earlyExit => 1.25f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "Leap";

        public override string playbackRateParams => "Leap.playbackRate";

        public override string animatorAttackParam => "Leap.attack";

        public override int waveCount => 8;

        public override float waveProjectileDamage => 2f;

        public override float waveProjectileForce => 1000f;
    }
}
