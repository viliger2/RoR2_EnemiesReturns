using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    [RegisterEntityState]
    public class Dive : BaseState
    {
        public static float maxDuration = 5f; // TODO?

        public static float damageCoefficient = 2f;

        public static float turnSmoothTime = 1f;

        public static float turnSpeed = 200f;

        public static float diveSpeedCoefficient = 6f;

        public static float acceleration = 360f;

        private OverlapAttack diveAttack;

        private Transform sphereCheckTransform;

        private Vector3 targetMoveVector;

        private Vector3 targetMoveVectorVelocity;

        public override void OnEnter()
        {
            base.OnEnter();

            PlayCrossfade("Gesture, Override", "Dive", 0.1f);
            diveAttack = SetupOverlapAttack();
            sphereCheckTransform = FindModelChild("SphereCheckTransform");
            if (!sphereCheckTransform)
            {
                sphereCheckTransform = base.transform;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            targetMoveVector = Vector3.SmoothDamp(targetMoveVector, base.inputBank.aimDirection, ref targetMoveVectorVelocity, turnSmoothTime, turnSpeed).normalized;
            base.characterDirection.moveVector = targetMoveVector;

            //Vector3 forward = base.characterDirection.forward;
            //base.characterMotor.moveDirection = targetMoveVector * diveSpeedCoefficient;
            //base.characterMotor.velocity += targetMoveVector * moveSpeedStat * diveSpeedCoefficient * GetDeltaTime();
            base.characterMotor.rootMotion = targetMoveVector * moveSpeedStat * diveSpeedCoefficient * GetDeltaTime();

            if (isAuthority)
            {
                diveAttack.Fire();
                
                if(HGPhysics.DoesOverlapSphere(sphereCheckTransform.position, 0.1f, LayerIndex.world.mask) || base.characterMotor.isGrounded)
                {
                    outer.SetNextState(new DiveEnd());
                    return;
                }

                if(fixedAge >= maxDuration)
                {
                    outer.SetNextState(new DiveEnd());
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterMotor.moveDirection = Vector3.zero;
            base.characterDirection.moveVector = base.characterDirection.forward;
            PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        private OverlapAttack SetupOverlapAttack()
        {
            var attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
            attack.damage = damageStat * damageCoefficient;
            //attack.hitEffectPrefab = ; TODO
            attack.isCrit = RollCrit();
            attack.damageType = DamageTypeCombo.GenericPrimary;
            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Dive");
            }
            return attack;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
