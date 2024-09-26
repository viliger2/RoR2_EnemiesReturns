using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Hellzone
{
    public class FireHellzoneStart : BaseState
    {
        public static float baseDuration = 1.8f;

        public static string attackString = "ER_Ifrit_BreathIn_Fireball_Play";

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            Util.PlayAttackSpeedSound(attackString, gameObject, attackSpeedStat);
            PlayAnimation("Gesture,Override", "FireballStart", "FireFireball.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new FireHellzoneFire());
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
