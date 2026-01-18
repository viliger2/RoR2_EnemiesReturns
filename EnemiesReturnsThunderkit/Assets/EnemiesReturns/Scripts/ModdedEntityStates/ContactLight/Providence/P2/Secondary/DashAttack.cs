using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Linq;
using RoR2.Projectile;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Secondary
{
    [RegisterEntityState]
    public class DashAttack : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash1;

        public static GameObject swingEffect;

        public static GameObject projectileClone;

        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion();

        public static float dashDamageCoefficient = 3f;

        public static int minClones = 1;

        public static int maxClones = 3;

        private Vector3 desiredDirection;

        private int clonesCount;

        public override void OnEnter()
        {
            this.baseDuration = 1f;
            base.damageCoefficient = dashDamageCoefficient;
            base.hitBoxGroupName = "SecondaryProvidence";
            base.hitEffectPrefab = hitEffect;
            base.procCoefficient = 1f;
            base.pushAwayForce = 1000f;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            base.swingEffectPrefab = swingEffect;
            base.swingEffectMuzzleString = "SwingCombo1EffectMuzzle";
            base.mecanimHitboxActiveParameter = "Slash1.attack";
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "ER_Arraign_ThreeHitComboSwingP1_Play";
            //base.impactSound = "";
            base.forceForwardVelocity = true;
            base.forwardVelocityCurve = AnimationCurve.Linear(0f, 1f, 0f, 1f);
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;
            base.duration = base.baseDuration / attackSpeedStat;

            clonesCount = (int)Mathf.Min(maxClones, Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.25f, healthComponent.fullHealth, (float)maxClones, (float)minClones));

            base.OnEnter();

            desiredDirection = inputBank.aimDirection;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 targetMoveVelocity = Vector3.zero;
            characterDirection.forward = Vector3.SmoothDamp(characterDirection.forward, desiredDirection, ref targetMoveVelocity, 0.01f, 90f);
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash1", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            //skillLocator.secondary = skillLocator.allSkills.First(component => component.skillName == "FireOrbs");
            base.OnExit();
        }

        public override void AuthorityOnFinish()
        {
            outer.SetNextState(new DashEnd());
            FireProjectileAuthority();
        }

        private void FireProjectileAuthority()
        {
            if (!isAuthority)
            {
                return;
            }

            var info = new FireProjectileInfo()
            {
                comboNumber = (byte)(clonesCount - 1),
                crit = RollCrit(),
                damage = damageStat * dashDamageCoefficient,
                damageTypeOverride = DamageSource.Secondary,
                force = 1000f,
                owner = gameObject,
                position = transform.position,
                procChainMask = new ProcChainMask(),
                projectilePrefab = projectileClone,
                rotation = Util.QuaternionSafeLookRotation(inputBank.aimDirection),
                fuseOverride = 1f
            };

            ProjectileManager.instance.FireProjectile(info);
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType.damageSource = DamageSource.Secondary;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

