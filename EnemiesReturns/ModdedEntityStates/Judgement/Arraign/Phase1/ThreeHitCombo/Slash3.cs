using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo
{
    [RegisterEntityState]
    public class Slash3 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash3;

        public static float blastAttackRadius => Configuration.Judgement.ArraignP1.ThreeHitComboExplosionRadius.Value;

        public static float blastAttackDamage => Configuration.Judgement.ArraignP1.ThreeHitComboExplosionDamage.Value;

        public static float blastAttackForce => Configuration.Judgement.ArraignP1.ThreeHitComboExplosionForce.Value;

        public static float blastAttackProcCoefficient => Configuration.Judgement.ArraignP1.ThreeHitComboExplosionProcCoefficient.Value;

        public static GameObject swingEffect;

        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion();

        public static GameObject explosionEffect;

        private Vector3 desiredDirection;

        private bool firedBlastAttack;

        private Transform explosionEffectMuzzle;

        public override void OnEnter()
        {
            this.baseDuration = 2.4f;
            base.damageCoefficient = Configuration.Judgement.ArraignP1.ThreeHitComboThirdSwingDamage.Value;
            base.hitBoxGroupName = "SwordNormal";
            base.hitEffectPrefab = hitEffect;
            base.procCoefficient = Configuration.Judgement.ArraignP1.ThreeHitComboThirdSwingProcCoefficient.Value;
            base.pushAwayForce = Configuration.Judgement.ArraignP1.ThreeHitComboThirdSwingForce.Value;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            base.swingEffectPrefab = swingEffect;
            base.swingEffectMuzzleString = "Swing3EffectMuzzle";
            base.mecanimHitboxActiveParameter = "Slash3.attack";
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "ER_Arraign_ThreeHitComboSwingP1_Play";
            //base.impactSound = "";
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
            if (animator.GetFloat("Slash3.slam") > 0.9f && !firedBlastAttack)
            {
                if (isAuthority)
                {
                    FireBlastAttackAuthority();
                }
                var effectData = new EffectData
                {
                    origin = explosionEffectMuzzle.position,
                    scale = 7f
                };
                Util.PlaySound("ER_Arraign_ThreeHitComboExplosion_Play", base.gameObject);
                EffectManager.SpawnEffect(explosionEffect, effectData, false);
                firedBlastAttack = true;
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
