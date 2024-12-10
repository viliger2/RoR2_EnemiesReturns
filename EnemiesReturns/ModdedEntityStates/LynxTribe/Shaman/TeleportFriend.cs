using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class TeleportFriend : BaseState
    {
        public static float range = 200f;

        public static HashSet<BodyIndex> blacklist; 

        public override void OnEnter()
        {
            base.OnEnter();
            if(blacklist == null)
            {
                blacklist = new HashSet<BodyIndex>();
            }

        }

        private GameObject FindFriendlyToTeleport()
        {
            var sphereSearch = new SphereSearch()
            {
                mask = LayerIndex.entityPrecise.mask,
                origin = transform.position,
                radius = range,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
            };
            var team = new TeamMask();
            team.AddTeam(teamComponent.teamIndex);

            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(team);
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            var hurtboxes = sphereSearch.GetHurtBoxes();
            foreach(var hurtbox in hurtboxes)
            {
                if (!blacklist.Contains(hurtbox.healthComponent.body.bodyIndex))
                {
                    return hurtbox.healthComponent.body.gameObject;
                }
            }

            return null;
        }

        private CharacterBody FindCurentTarget()
        {
            foreach (var ai in characterBody.master.aiComponents)
            {
                if (!ai.currentEnemy.characterBody)
                {
                    continue;
                }

                return ai.currentEnemy.characterBody;
            }

            return null;
        }

    }
}
