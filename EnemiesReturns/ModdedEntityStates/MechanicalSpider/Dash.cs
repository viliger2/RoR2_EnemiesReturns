﻿using EnemiesReturns.ModdedEntityStates.Ifrit.FlameCharge;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    public class Dash : BaseState
    {
        public static AnimationCurve forwardSpeedCoefficientCurve;

        public static float heightCheck => EnemiesReturns.Configuration.MechanicalSpider.DashHeightCheck.Value;

        public static float duration => EnemiesReturns.Configuration.MechanicalSpider.DashDuration.Value;

        private bool startedStateGrounded;

        private Vector3 forwardDirection;

        private Transform ledgeHandling;

        public override void OnEnter()
        {
            base.OnEnter();
            if (inputBank && characterDirection)
            {
                characterDirection.forward = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
            }

            if (characterMotor)
            {
                startedStateGrounded = characterMotor.isGrounded;
            }

            if (!startedStateGrounded)
            {
                Vector3 velocity = characterMotor.velocity;
                velocity.y = characterBody.jumpPower;
                characterMotor.velocity = velocity;
            }
            PlayCrossfade("Body", "Dash", 0.2f);

            ledgeHandling = FindModelChild("LedgeHandling");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (inputBank && characterDirection)
                {
                    characterDirection.moveVector = inputBank.moveVector;
                    forwardDirection = characterDirection.forward;
                }
                if (characterMotor)
                {
                    var num2 = !startedStateGrounded ? forwardSpeedCoefficientCurve.Evaluate(fixedAge / duration) : forwardSpeedCoefficientCurve.Evaluate(fixedAge / duration); // TODO: maybe separate
                    characterMotor.rootMotion += num2 * moveSpeedStat * forwardDirection * GetDeltaTime();
#if DEBUG || NOWEAVER
                    if (ledgeHandling)
#else
                    if (ledgeHandling && !characterBody.isPlayerControlled)
#endif
                    {
                        var result = Physics.Raycast(ledgeHandling.position, Vector3.down, out var hitinfo, Mathf.Infinity, LayerIndex.world.mask);
                        if (!result || hitinfo.distance > heightCheck)
                        {
                            outer.SetNextStateToMain();
                        }
                    }
                }
                if (fixedAge >= duration)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Body", "DashStop", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}