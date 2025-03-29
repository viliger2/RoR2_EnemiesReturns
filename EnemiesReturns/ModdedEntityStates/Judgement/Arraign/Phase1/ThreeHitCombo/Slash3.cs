using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo
{
    public class Slash3 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash3;

        public static float fireWaves = 0.36f;

        public static GameObject waveProjectile;

        public static float waveProjectileDamage = 2f;

        public static int waveCount = 8;

        public static float waveProjectileForce = 0f;

        private Vector3 desiredDirection;

        private bool firedWaves;

        public override void OnEnter()
        {
            this.baseDuration = 2.4f;
            base.damageCoefficient = 2f;
            base.hitBoxGroupName = "Sword";
            //base.hitEffectPrefab = 
            base.procCoefficient = 1f;
            base.pushAwayForce = 6000f;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            //base.swingEffectMuzzleString = "";
            //base.mecanimHitboxActiveParameter = "";
            base.shorthopVelocityFromHit = 0f;
            //base.impactSound = "";
            base.forceForwardVelocity = true;
            base.forwardVelocityCurve = acdSlash3;
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;

            base.OnEnter();

            desiredDirection = inputBank.aimDirection;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 targetMoveVelocity = Vector3.zero;
            characterDirection.forward = Vector3.SmoothDamp(characterDirection.forward, desiredDirection, ref targetMoveVelocity, 0.01f, 45f);
            if(fixedAge > fireWaves && !firedWaves && isAuthority)
            {
                FireRingAuthority();
                firedWaves = true;
            }
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash3", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
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
                        damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary),
                        crit = crit
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }
    }
}
