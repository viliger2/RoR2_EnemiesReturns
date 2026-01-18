using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.SandCrab.Snip
{
    [RegisterEntityState]
    public class HoldSnip : BaseSkillState
    {
        public static float maxDuration => Configuration.SandCrab.SnipHoldMaxDuration.Value;

        private Transform attackCheckHitbox;

        private SphereSearch sphereSearch;

        private readonly List<HurtBox> hurtBoxesList = new List<HurtBox>();

        public override void OnEnter()
        {
            base.OnEnter();
            sphereSearch = new SphereSearch()
            {
                mask = LayerIndex.entityPrecise.mask,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                radius = 3.5f
            };

            this.activatorSkillSlot = skillLocator.primary; // we assume this is always primary;
            PlayAnimation("Gesture, Override, Mask", "HoldSnip");

            attackCheckHitbox = FindModelChild("ChargeAttackHitbox");
            Util.PlaySound("ER_SandCrab_Melee_Hold_Play", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority)
            {
                return;
            }

            if (fixedAge > maxDuration)
            {
                outer.SetNextState(new FireSnip());
            }

            if (characterBody && characterBody.isPlayerControlled)
            {
                if (!this.IsKeyDownAuthority(skillLocator, inputBank))
                {
                    outer.SetNextState(new FireSnip());
                }
            }
            else
            {
                if (!attackCheckHitbox)
                {
                    return;
                }

                var position = attackCheckHitbox.position;

                sphereSearch.origin = position;
                sphereSearch.RefreshCandidates();
                sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
                sphereSearch.GetHurtBoxes(hurtBoxesList);
                sphereSearch.ClearCandidates();

                foreach (var hurtBox in hurtBoxesList)
                {
                    if (!hurtBox || !hurtBox.healthComponent || !hurtBox.healthComponent.body)
                    {
                        continue;
                    }

                    if (characterBody.teamComponent.teamIndex == hurtBox.teamIndex)
                    {
                        continue;
                    }

                    outer.SetNextState(new FireSnip());
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override, Mask", "BufferEmpty");
            Util.PlaySound("ER_SandCrab_Melee_Hold_Stop", base.gameObject);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
