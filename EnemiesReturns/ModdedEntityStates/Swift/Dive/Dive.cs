using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Swift.Dive
{
    [RegisterEntityState]
    public class Dive : BaseState
    {
        public static float maxDuration => Configuration.Swift.DiveMaxDuration.Value; 

        public static float damageCoefficient => Configuration.Swift.DiveDamage.Value;

        public static float turnSmoothTime = 1f;

        public static float turnSpeed => Configuration.Swift.DiveTurnSpeed.Value;

        public static float diveSpeedCoefficient => Configuration.Swift.DiveSpeedCoefficient.Value;

        public static float acceleration = 360f;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.OmniImpactVFX_prefab).WaitForCompletion();

        private OverlapAttack diveAttack;

        private Transform sphereCheckTransform;

        private Vector3 targetMoveVector;

        private Vector3 targetMoveVectorVelocity;

        private Transform speedLines;

        public override void OnEnter()
        {
            base.OnEnter();

            PlayCrossfade("Gesture, Override", "Dive", 0.1f);
            diveAttack = SetupOverlapAttack();
            sphereCheckTransform = FindModelChild("SphereCheckTransform");
            speedLines = FindModelChild("SprintEffect");
            if (speedLines)
            {
                speedLines.gameObject.SetActive(true);
            }

            if (!sphereCheckTransform)
            {
                sphereCheckTransform = transform;
            }
            Util.PlaySound("ER_Swift_DiveLoop_Play", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            targetMoveVector = Vector3.SmoothDamp(targetMoveVector, inputBank.aimDirection, ref targetMoveVectorVelocity, turnSmoothTime, turnSpeed).normalized;
            characterDirection.moveVector = targetMoveVector;
            characterMotor.rootMotion = targetMoveVector * moveSpeedStat * diveSpeedCoefficient * GetDeltaTime();

            if (isAuthority)
            {
                diveAttack.Fire();

                if (HGPhysics.DoesOverlapSphere(sphereCheckTransform.position, 0.1f, LayerIndex.world.mask) || characterMotor.isGrounded)
                {
                    outer.SetNextState(new DiveEnd());
                    return;
                }

                if (fixedAge >= maxDuration)
                {
                    outer.SetNextState(new DiveEndAir());
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            characterMotor.moveDirection = Vector3.zero;
            characterDirection.moveVector = characterDirection.forward;
            if (speedLines)
            {
                speedLines.gameObject.SetActive(false);
            }
            PlayAnimation("Gesture, Override", "BufferEmpty");
            Util.PlaySound("ER_Swift_DiveLoop_Stop", base.gameObject);
        }

        private OverlapAttack SetupOverlapAttack()
        {
            var attack = new OverlapAttack();
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            attack.damage = damageStat * damageCoefficient;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.isCrit = RollCrit();
            attack.damageType = DamageTypeCombo.GenericPrimary;
            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == "Dive");
            }
            return attack;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
