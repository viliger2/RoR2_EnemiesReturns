using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.HeadLaserBarrage
{
    public class HeadLaserBarrageStart : BaseState
    {
        public static float baseDuration = 5.5f;

        private float duration;

        private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

        private static readonly int aimPitchCycleHash = Animator.StringToHash("aimPitchCycle");

        public static float targetPitch = EnemiesReturnsConfiguration.Colossus.LaserBarrageHeadPitch.Value;

        private float startYaw;

        private float startPitch;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = GetModelAnimator();
            if(modelAnimator)
            {
                startYaw = modelAnimator.GetFloat(aimYawCycleHash);
                startPitch = modelAnimator.GetFloat(aimPitchCycleHash);
            }
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Body", "LaserBeamStart", "Laser.playbackrate", duration, 0.1f);
        }

        public override void Update()
        {
            base.Update();
            if (modelAnimator)
            {
                modelAnimator.SetFloat(aimYawCycleHash, Mathf.Clamp(Mathf.Lerp(startYaw, 0f, age / duration), 0f, 0.99f));
                modelAnimator.SetFloat(aimPitchCycleHash, Mathf.Clamp(Mathf.Lerp(startPitch, targetPitch, age / duration), 0f, 0.99f));
            }
        }

        // TODO: MOVE ANIMATION TO UPDATE, WHAT THE FUCK WAS I THINKING
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new HeadLaserBarrageAttack());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
