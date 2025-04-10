using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap
{
    [RegisterEntityState]
    public class ExitSkyLeap : BaseExitSkyLeap
    {
        public static GameObject staticFirstAttackEffect;

        public static GameObject staticSecondAttackEffect;

        public static GameObject waveProjectile;

        public static int waveCount = 12;

        public static float waveProjectileDamage = 2.5f;

        public static float waveProjectileForce = 0f;

        public override GameObject firstAttackEffect => staticFirstAttackEffect;

        public override GameObject secondAttackEffect => staticSecondAttackEffect;

        public override float baseDuration => 2.3f;

        public override string soundString => "";

        public override float attackDamage => 3.2f;

        public override float attackForce => 1000f;

        public override float blastAttackRadius => 30f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackParamName => "SkyLeap.playbackRate";

        public override string firstAttackParamName => "SkyLeap.firstAttack";

        public override string secondAttackParamName => "SkyLeap.secondAttack";

        private bool firedWaves;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!firedWaves && modelAnimator.GetFloat(firstAttackParamName) > 0.9f)
            {
                if (isAuthority)
                {
                    FireRingAuthority();
                }
                firedWaves = true;
            }
        }

        private void FireRingAuthority()
        {
            float num = 360f / (float)waveCount;
            Vector3 vector = Vector3.ProjectOnPlane(base.inputBank.aimDirection, Vector3.up);
            Vector3 footPosition = base.characterBody.footPosition;
            bool crit = RollCrit();
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 forward = Quaternion.AngleAxis(num * (float)i, Vector3.up) * vector;
                if (base.isAuthority)
                {
                    var info = new FireProjectileInfo
                    {
                        projectilePrefab = waveProjectile,
                        position = footPosition,
                        rotation = Util.QuaternionSafeLookRotation(forward),
                        owner = base.gameObject,
                        damage = base.characterBody.damage * waveProjectileDamage,
                        force = waveProjectileForce,
                        damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Special),
                        crit = crit
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

    }
}
