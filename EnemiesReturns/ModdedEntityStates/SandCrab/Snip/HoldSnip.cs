using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.SandCrab.Snip
{
    [RegisterEntityState]
    public class HoldSnip : BaseSkillState
    {
        public static float maxDuration = 5f;

        public override void OnEnter()
        {
            base.OnEnter();
            this.activatorSkillSlot = skillLocator.primary; // we assume this is always primary;
            PlayAnimation("Gesture, Override, Mask", "HoldSnip");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(characterBody && characterBody.isPlayerControlled)
            {
                if (!this.IsKeyDownAuthority(skillLocator, inputBank) && isAuthority)
                {
                    outer.SetNextState(new FireSnip());
                }
            } else
            {
                // write AI for finding target
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override, Mask", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
