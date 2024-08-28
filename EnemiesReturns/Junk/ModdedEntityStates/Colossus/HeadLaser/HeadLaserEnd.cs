using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.Colossus.HeadLaser
{
    public class HeadLaserEnd : BaseState
    {
        public static float baseDuration = 3.5f;

        private float duration;

        private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

        private static readonly int aimPitchCycleHash = Animator.StringToHash("aimPitchCycle");

        private float startYaw;

        private float startPitch;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                startYaw = modelAnimator.GetFloat(aimPitchCycleHash);
                startPitch = modelAnimator.GetFloat(aimPitchCycleHash);
            }
            PlayCrossfade("Body", "LaserBeamEnd", "Laser.playbackrate", duration, 0.1f);
        }

        public override void Update()
        {
            base.Update();
            if (modelAnimator)
            {
                modelAnimator.SetFloat(aimYawCycleHash, Mathf.Clamp(Mathf.Lerp(startYaw, 0.5f, age / duration), 0f, 0.99f));
                modelAnimator.SetFloat(aimPitchCycleHash, Mathf.Clamp(Mathf.Lerp(startPitch, 0.5f, age / duration), 0f, 0.99f));
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
