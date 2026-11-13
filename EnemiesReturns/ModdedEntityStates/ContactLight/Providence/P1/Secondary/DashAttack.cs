using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Secondary
{
    [RegisterEntityState]
    public class DashAttack : BasicMeleeAttack
    {
        public static GameObject swingEffect;

        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion();

        private Vector3 desiredDirection;

        private Transform dashEffect;

        public override void OnEnter()
        {
            this.baseDuration = ProvidencePhase1.P1SecondaryDuration.Value;
            base.damageCoefficient = ProvidencePhase1.P1PrimaryDamage.Value;
            base.hitBoxGroupName = "DashStabHitbox";
            base.hitEffectPrefab = hitEffect;
            base.procCoefficient = ProvidencePhase1.P1PrimaryProcCoefficient.Value;
            base.pushAwayForce = ProvidencePhase1.P1PrimaryPushAwayForce.Value;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            base.swingEffectPrefab = swingEffect;
            base.swingEffectMuzzleString = "";
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "";
            //base.impactSound = 
            base.forceForwardVelocity = true;
            var coefficient = ProvidencePhase1.P1SecondaryForwardSpeedCoef.Value;
            base.forwardVelocityCurve = AnimationCurve.Linear(0f, coefficient, 0f, coefficient);
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;
            base.duration = base.baseDuration / attackSpeedStat;

            dashEffect = FindModelChild("DashEffect");
            if (dashEffect)
            {
                dashEffect.gameObject.SetActive(true);
            }

            base.OnEnter();

            desiredDirection = inputBank.aimDirection;
            Util.PlaySound("ER_Arraign_SpearThrowProjectileLoop_Play", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 targetMoveVelocity = Vector3.zero;
            characterDirection.forward = Vector3.SmoothDamp(characterDirection.forward, desiredDirection, ref targetMoveVelocity, 0.01f, 120f);
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "DashStab", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            if (dashEffect)
            {
                dashEffect.gameObject.SetActive(false);
            }
            Util.PlaySound("ER_Arraign_SpearThrowProjectileLoop_Stop", base.gameObject);
            base.OnExit();
        }

        public override void AuthorityOnFinish()
        {
            outer.SetNextState(new DashEnd());
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

