using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    [RegisterEntityState]
    public class ExitSkyLeap : BaseExitSkyLeap
    {
        public static GameObject firstAttackEffectStatic;

        public static GameObject secondAttackEffectStatic;

        public static GameObject waveProjectileStatic;

        public override GameObject firstAttackEffect => firstAttackEffectStatic;

        public override GameObject secondAttackEffect => secondAttackEffectStatic;

        public override float baseDuration => 3.5f;

        public override string soundString => "";

        public override float firstAttackDamage => Configuration.Judgement.ArraignP1.SkyDropFirstExplosionDamage.Value;

        public override float secondAttackDamage => Configuration.Judgement.ArraignP1.SkyDropSecondExplosionDamage.Value;

        public override float attackForce => Configuration.Judgement.ArraignP1.SkyDropExplosionForce.Value;

        public override float blastAttackRadius => Configuration.Judgement.ArraignP1.SkyDropExplosionRadius.Value;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackParamName => "SkyLeap.playbackRate";

        public override string firstAttackParamName => "SkyLeap.firstAttack";

        public override string secondAttackParamName => "SkyLeap.secondAttack";

        public override GameObject waveProjectile => waveProjectileStatic;

        public override float waveProjectileDamage => Configuration.Judgement.ArraignP1.SkyDropWavesDamage.Value;

        public override int waveCount => Configuration.Judgement.ArraignP1.SkyDropWavesCount.Value;

        public override float waveProjectileForce => Configuration.Judgement.ArraignP1.SkyDropWavesForce.Value;
    }
}
