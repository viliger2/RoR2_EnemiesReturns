using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.SandCrab.Snip
{
    [RegisterEntityState]
    public class HoldSnip : BaseSkillState
    {
        public static float maxDuration = 5f;

        private Transform attackCheckHitbox;

        public override void OnEnter()
        {
            base.OnEnter();
            this.activatorSkillSlot = skillLocator.primary; // we assume this is always primary;
            PlayAnimation("Gesture, Override, Mask", "HoldSnip");

            attackCheckHitbox = FindModelChild("ChargeAttackHitbox");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority)
            {
                return;
            }

            if(fixedAge > maxDuration)
            {
                outer.SetNextState(new FireSnip());
            }

            if (characterBody && characterBody.isPlayerControlled)
            {
                if (!this.IsKeyDownAuthority(skillLocator, inputBank))
                {
                    outer.SetNextState(new FireSnip());
                }
            } else
            {
                if (!attackCheckHitbox)
                {
                    return;
                }

                var position = attackCheckHitbox.position;

                Collider[] colliders;

                int num = HGPhysics.OverlapSphere(out colliders, position, 3.5f, LayerIndex.entityPrecise.mask);
                for (int i = 0; i < num; i++)
                {
                    if (!colliders[i])
                    {
                        HGPhysics.ReturnResults(colliders);
                        continue;
                    }

                    var hurtBox = colliders[i].GetComponent<HurtBox>();
                    if (!hurtBox || !hurtBox.healthComponent || !hurtBox.healthComponent.body)
                    {
                        HGPhysics.ReturnResults(colliders);
                        continue;
                    }

                    var body = hurtBox.healthComponent.body;
                    if (characterBody.teamComponent.teamIndex == hurtBox.teamIndex)
                    {
                        HGPhysics.ReturnResults(colliders);
                        continue;
                    }

                    HGPhysics.ReturnResults(colliders);
                    outer.SetNextState(new FireSnip());
                }
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
