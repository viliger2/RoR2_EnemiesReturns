using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo
{
    [RegisterEntityState]
    public class Slash3 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash3;

        public static float blastAttackRadius => Configuration.Judgement.ArraignP2.ThreeHitComboExplosionRadius.Value;

        public static float blastAttackDamage => Configuration.Judgement.ArraignP2.ThreeHitComboExplosionDamage.Value;

        public static float blastAttackForce => Configuration.Judgement.ArraignP2.ThreeHitComboExplosionForce.Value;

        public static float blastAttackProcCoefficient => Configuration.Judgement.ArraignP2.ThreeHitComboExplosionProcCoefficient.Value;

        public static GameObject explosionEffect;

        public static GameObject swingEffect;

        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/OmniImpactVFXHuntress.prefab").WaitForCompletion();

        public static GameObject waveProjectile;

        public static float waveProjectileDamage => Configuration.Judgement.ArraignP2.ThreeHitComboWavesDamage.Value;

        public static int waveCount => Configuration.Judgement.ArraignP2.ThreeHitComboWavesCount.Value;

        public static float waveProjectileForce => Configuration.Judgement.ArraignP2.ThreeHitComboWavesForce.Value;

        private Vector3 desiredDirection;

        private bool firedWaves;

        private Transform explosionEffectMuzzle;

        public override void OnEnter()
        {
            this.baseDuration = 2.4f;
            base.damageCoefficient = Configuration.Judgement.ArraignP2.ThreeHitComboThirdSwingDamage.Value;
            base.hitBoxGroupName = "SwordNormal";
            base.hitEffectPrefab = hitEffect;
            base.procCoefficient = Configuration.Judgement.ArraignP2.ThreeHitComboThirdSwingProcCoefficient.Value;
            base.pushAwayForce = Configuration.Judgement.ArraignP2.ThreeHitComboThirdSwingForce.Value;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            base.swingEffectPrefab = swingEffect;
            base.swingEffectMuzzleString = "Swing3EffectMuzzle";
            base.mecanimHitboxActiveParameter = "Slash3.attack";
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "ER_Arraign_ThreeHitComboSwingP2_Play";
            base.forceForwardVelocity = true;
            base.forwardVelocityCurve = acdSlash3;
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;

            base.OnEnter();

            desiredDirection = inputBank.aimDirection;

            explosionEffectMuzzle = FindModelChild("SwingCombo3ExplosionMuzzle");
            if (!explosionEffectMuzzle)
            {
                explosionEffectMuzzle = base.transform;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 targetMoveVelocity = Vector3.zero;
            characterDirection.forward = Vector3.SmoothDamp(characterDirection.forward, desiredDirection, ref targetMoveVelocity, 0.01f, 45f);
            if (animator.GetFloat("Slash3.slam") > 0.9f && !firedWaves)
            {
                if (isAuthority)
                {
                    FireBlastAttackAuthority();
                    FireRingAuthority();
                }
                var effectData = new EffectData
                {
                    origin = explosionEffectMuzzle.position,
                    scale = 7f
                };
                EffectManager.SpawnEffect(explosionEffect, effectData, false);
                Util.PlaySound("ER_Arraign_ThreeHitComboExplosion_Play", base.gameObject);
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
                        damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Secondary),
                        crit = crit
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType.damageSource = DamageSource.Secondary;
        }

        private void FireBlastAttackAuthority()
        {
            var blastAttack = new BlastAttack
            {
                radius = blastAttackRadius,
                procCoefficient = blastAttackProcCoefficient,
                position = explosionEffectMuzzle.position,
                attacker = base.gameObject,
                crit = RollCrit(),
                baseDamage = blastAttackDamage * damageStat,
                canRejectForce = false,
                falloffModel = BlastAttack.FalloffModel.SweetSpot,
                baseForce = blastAttackForce,
                teamIndex = teamComponent.teamIndex,
                damageType = DamageSource.Secondary,
                attackerFiltering = AttackerFiltering.Default
            };
            blastAttack.Fire();
        }
    }
}
