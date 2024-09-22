using EntityStates;
using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.FlameCharge
{
    public class FlameCharge : BaseState
    {
        public static GameObject flamethrowerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/FlamebreathEffect.prefab").WaitForCompletion();

        public static float tickFrequency = 8f; // TODO: lods of config

        public static float chargeDuration = 5f;

        public static float turnSmoothTime = 0.01f;

        public static float turnSpeed = 300f;

        public static float chargeMovementSpeedCoefficient = 2f;

        public static float chargeDamageCoefficient = 2f;

        public static float chargeForce = 5000f;

        public static float chargeProcCoef = 1.0f;

        public static float flameDamageCoefficient = 5f;

        public static float flameIgnitePercentChance = 100f;

        public static float flameForce = 0f;

        public static float flameProcCoef = 0.2f;

        public static string muzzleString = "MuzzleMouth";

        private Vector3 targetMoveVector;

        private Animator animator;

        private Transform muzzleMouth;

        private Transform flamethrowerEffectInstance;

        private Transform ledgeHandling;

        private EffectManagerHelper _emh_flamethrowerEffectInstance;

        private OverlapAttack flameAttack;

        private OverlapAttack chargeAttack;

        private float bulletAttackStopwatch;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            var childLocator = GetModelChildLocator();
            animator = GetModelAnimator();
            muzzleMouth = FindModelChild(muzzleString);
            ledgeHandling = FindModelChild("LedgeHandling");
            PlayCrossfade("Gesture,Override", "FlameBlastFiring", 0.2f);
            bool isCrit = RollCrit();
            SetupFlameAttack(modelTransform, isCrit);
            SetupChargeAttack(modelTransform, isCrit);
            SpawnEffect();

        }

        private void SetupChargeAttack(Transform modelTransform, bool isCrit)
        {
            chargeAttack = new OverlapAttack();
            chargeAttack.attacker = base.gameObject;
            chargeAttack.inflictor = base.gameObject;
            chargeAttack.teamIndex = TeamComponent.GetObjectTeam(chargeAttack.attacker);
            chargeAttack.damage = chargeDamageCoefficient * damageStat;
            //attack.hitEffectPrefab = hitEffectPrefab; // TODO: bison
            chargeAttack.isCrit = isCrit;
            chargeAttack.forceVector = Vector3.forward * chargeForce;
            chargeAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "BodyCharge");
            chargeAttack.procCoefficient = chargeProcCoef;
            chargeAttack.damageType = DamageType.Generic;
        }

        private void SetupFlameAttack(Transform modelTransform, bool isCrit)
        {
            flameAttack = new OverlapAttack();
            flameAttack.attacker = base.gameObject;
            flameAttack.inflictor = base.gameObject;
            flameAttack.teamIndex = TeamComponent.GetObjectTeam(flameAttack.attacker);
            flameAttack.damage = flameDamageCoefficient * damageStat;
            //attack.hitEffectPrefab = hitEffectPrefab; // TODO: lemurianbruiser
            flameAttack.isCrit = isCrit;
            flameAttack.forceVector = Vector3.forward * flameForce;
            flameAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "FlameCharge");
            flameAttack.procCoefficient = flameProcCoef;
            flameAttack.damageType = (Util.CheckRoll(flameIgnitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic);
        }

        public override void FixedUpdate()
        {
            Vector3 targetMoveVelocity = Vector3.zero; 
            targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(targetMoveVector, base.inputBank.aimDirection, ref targetMoveVelocity, turnSmoothTime, turnSpeed), Vector3.up).normalized;
            base.characterDirection.moveVector = targetMoveVector;
            Vector3 forward = base.characterDirection.forward;
            float value = moveSpeedStat * chargeMovementSpeedCoefficient;
            base.characterMotor.moveDirection = forward * chargeMovementSpeedCoefficient;
            animator.SetFloat(AnimationParameters.forwardSpeed, value);
            if(isAuthority)
            {
                chargeAttack.Fire();
                bulletAttackStopwatch += Time.fixedDeltaTime;
                if(bulletAttackStopwatch > 1f / tickFrequency)
                {
                    bulletAttackStopwatch -= 1f / tickFrequency;
                    flameAttack.Fire();

                    if (ledgeHandling)
                    {
                        var result = Physics.Raycast(ledgeHandling.position, Vector3.down, out var hitinfo, Mathf.Infinity, LayerIndex.world.mask);
                        if (!result || hitinfo.distance > 20f)
                        {
                            outer.SetNextState(new FlameChargeEnd());
                        }
                    }
                }
            }

            if(fixedAge >= chargeDuration && isAuthority)
            {
                outer.SetNextState(new FlameChargeEnd());
            }

            base.FixedUpdate();
        }

        private void SpawnEffect()
        {
            if(!EffectManager.ShouldUsePooledEffect(flamethrowerEffectPrefab))
            {
                flamethrowerEffectInstance = UnityEngine.Object.Instantiate(flamethrowerEffectPrefab, muzzleMouth).transform;
            } else
            {
                _emh_flamethrowerEffectInstance = EffectManager.GetAndActivatePooledEffect(flamethrowerEffectPrefab, muzzleMouth, true);
                flamethrowerEffectInstance = _emh_flamethrowerEffectInstance.gameObject.transform;
            }
            flamethrowerEffectInstance.transform.localPosition = Vector3.zero;
        }

        private void DestroyEffect()
        {
            if(flamethrowerEffectInstance != null)
            {
                if(_emh_flamethrowerEffectInstance != null && _emh_flamethrowerEffectInstance.OwningPool != null)
                {
                    _emh_flamethrowerEffectInstance.OwningPool.ReturnObject(_emh_flamethrowerEffectInstance);
                } else
                {
                    EntityState.Destroy(flamethrowerEffectInstance.gameObject);
                }
                flamethrowerEffectInstance = null;
                _emh_flamethrowerEffectInstance = null;
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);
            DestroyEffect();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
