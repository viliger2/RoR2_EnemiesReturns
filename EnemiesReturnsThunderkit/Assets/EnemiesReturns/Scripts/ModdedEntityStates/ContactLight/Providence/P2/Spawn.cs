using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2
{
    [RegisterEntityState]
    public class Spawn : BaseState
    {
        public static float baseDuration = 4.2f;

        private HurtBoxGroup hurtboxGroup;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }

            PlayAnimation("Body", "Spawn2", "Spawn1.playbackRate", baseDuration);

            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter++;
            }
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, baseDuration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= baseDuration && isAuthority)
            {
                outer.SetNextState(new ModdedEntityStates.ContactLight.Providence.P2.Utility.FireClones());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter--;
            }
            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

    }
}
