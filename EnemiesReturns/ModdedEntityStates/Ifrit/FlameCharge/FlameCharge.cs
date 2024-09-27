using EntityStates;
using HG;
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

        public static GameObject bodyImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXLarge.prefab").WaitForCompletion();

        public static GameObject flameImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileExplosionVFX.prefab").WaitForCompletion();

        public static float chargeDuration => EnemiesReturnsConfiguration.Ifrit.FlameChargeDuration.Value;

        public static float turnSpeed => EnemiesReturnsConfiguration.Ifrit.TurnSpeed.Value;

        public static float chargeMovementSpeedCoefficient => EnemiesReturnsConfiguration.Ifrit.FlameChargeSpeedCoefficient.Value;

        public static float chargeDamageCoefficient => EnemiesReturnsConfiguration.Ifrit.FlameChargeDamage.Value;

        public static float chargeForce => EnemiesReturnsConfiguration.Ifrit.FlameChargeForce.Value;

        public static float chargeProcCoef => EnemiesReturnsConfiguration.Ifrit.FlameChargeProcCoefficient.Value;

        public static float flameTickFrequency => EnemiesReturnsConfiguration.Ifrit.FlameChargeFlameTickFrequency.Value;

        public static float flameDamageCoefficient => EnemiesReturnsConfiguration.Ifrit.FlameChargeFlameDamage.Value;

        public static float flameIgnitePercentChance => EnemiesReturnsConfiguration.Ifrit.FlameChargeFlameIgniteChance.Value;

        public static float flameForce => EnemiesReturnsConfiguration.Ifrit.FlameChargeFlameForce.Value;

        public static float flameProcCoef => EnemiesReturnsConfiguration.Ifrit.FlameChargeFlameProcCoefficient.Value;

        public static float heighCheck => EnemiesReturnsConfiguration.Ifrit.FlameChargeHeighCheck.Value;

        public static string muzzleString = "MuzzleMouth";

        private static float turnSmoothTime = 0.01f;

        private Vector3 targetMoveVector;

        private Animator animator;

        private Transform muzzleMouth;

        private Transform flamethrowerEffectInstance;

        private Transform ledgeHandling;

        private Transform sprintEffect;

        private EffectManagerHelper _emh_flamethrowerEffectInstance;

        private OverlapAttack flameAttack;

        private OverlapAttack chargeAttack;

        private float bulletAttackStopwatch;

        private DamageTrail fireTrail;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            var childLocator = GetModelChildLocator();
            animator = GetModelAnimator();
            muzzleMouth = FindModelChild(muzzleString);
            ledgeHandling = FindModelChild("LedgeHandling");
            sprintEffect = FindModelChild("SprintEffect");
            PlayCrossfade("Gesture,Override", "FlameBlastFiring", 0.2f);
            Util.PlaySound("ER_Ifrit_FireBreath_Play", base.gameObject);
            bool isCrit = RollCrit();
            SetupFlameAttack(modelTransform, isCrit);
            SetupChargeAttack(modelTransform, isCrit);
            SpawnEffect();
            SetSprintEffectState(true);

            fireTrail = UnityEngine.GameObject.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/FireTrail"), transform).GetComponent<DamageTrail>();
            fireTrail.transform.position = characterBody.footPosition;
            fireTrail.owner = base.gameObject;
            fireTrail.radius *= characterBody.radius;
            fireTrail.damagePerSecond = damageStat * 1.5f;
        }

        private void SetSprintEffectState(bool active)
        {
            if(sprintEffect)
            {
                sprintEffect.gameObject.SetActive(active);
            }
        }

        private void SetupChargeAttack(Transform modelTransform, bool isCrit)
        {
            chargeAttack = new OverlapAttack();
            chargeAttack.attacker = base.gameObject;
            chargeAttack.inflictor = base.gameObject;
            chargeAttack.teamIndex = TeamComponent.GetObjectTeam(chargeAttack.attacker);
            chargeAttack.damage = chargeDamageCoefficient * damageStat;
            chargeAttack.hitEffectPrefab = bodyImpactEffect;
            chargeAttack.isCrit = isCrit;
            chargeAttack.forceVector = Vector3.forward * chargeForce;
            chargeAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "BodyCharge");
            chargeAttack.procCoefficient = chargeProcCoef;
            chargeAttack.damageType = DamageType.Generic;
            chargeAttack.retriggerTimeout = 0.5f;
        }

        private void SetupFlameAttack(Transform modelTransform, bool isCrit)
        {
            flameAttack = new OverlapAttack();
            flameAttack.attacker = base.gameObject;
            flameAttack.inflictor = base.gameObject;
            flameAttack.teamIndex = TeamComponent.GetObjectTeam(flameAttack.attacker);
            flameAttack.damage = flameDamageCoefficient * damageStat;
            flameAttack.hitEffectPrefab = flameImpactEffect;
            flameAttack.isCrit = isCrit;
            flameAttack.forceVector = Vector3.forward * flameForce;
            flameAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "FlameCharge");
            flameAttack.procCoefficient = flameProcCoef;
            flameAttack.damageType.damageType = (Util.CheckRoll(flameIgnitePercentChance, base.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic);
            flameAttack.retriggerTimeout = (1 / flameTickFrequency) * 2;
        }

        public override void FixedUpdate()
        {
            characterBody.outOfCombatStopwatch = 0f;
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
                if(bulletAttackStopwatch > 1f / flameTickFrequency)
                {
                    bulletAttackStopwatch -= 1f / flameTickFrequency;
                    flameAttack.Fire();

                    if (ledgeHandling)
                    {
                        var result = Physics.Raycast(ledgeHandling.position, Vector3.down, out var hitinfo, Mathf.Infinity, LayerIndex.world.mask);
                        if (!result || hitinfo.distance > heighCheck)
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
            base.OnExit();
            Util.PlaySound("ER_Ifrit_FireBreath_Stop", base.gameObject);
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);
            DestroyEffect();
            SetSprintEffectState(false);
            UnityEngine.GameObject.Destroy(fireTrail.gameObject);
            fireTrail = null;
            base.characterMotor.moveDirection = Vector3.zero;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
