using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam
{
    public abstract class BaseBeam : BaseState
    {
        public abstract GameObject pushBackEffect { get; }

        public static float pushRadius = 20f;

        public static float pushStrength = 15f;

        private GameObject pushBackEffectInstance;

        private SphereSearch pushSphereSearch;

        public override void OnEnter()
        {
            base.OnEnter();
            pushSphereSearch = new SphereSearch();
            pushBackEffectInstance = UnityEngine.Object.Instantiate(pushBackEffect, transform.position, Quaternion.identity);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            var position = transform.position;
            var hurtBoxes = SearchForTargets();
            foreach (var hurtbox in hurtBoxes)
            {
                if (hurtbox && hurtbox.healthComponent && hurtbox.healthComponent.body)
                {
                    var targetBody = hurtbox.healthComponent.body;
                    if (targetBody.hasEffectiveAuthority)
                    {
                        var component = targetBody.GetComponent<IDisplacementReceiver>();
                        if (component != null)
                        {
                            component.AddDisplacement(-(position - targetBody.transform.position).normalized * pushStrength * GetDeltaTime());
                        }
                    }
                }
            }
        }


        public override void OnExit()
        {
            base.OnExit();
            if (pushBackEffectInstance)
            {
                UnityEngine.Object.Destroy(pushBackEffectInstance);
            }
        }

        private HurtBox[] SearchForTargets()
        {
            HurtBox[] result;

            pushSphereSearch.mask = LayerIndex.entityPrecise.mask;
            pushSphereSearch.origin = transform.position;
            pushSphereSearch.radius = pushRadius;
            pushSphereSearch.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
            pushSphereSearch.RefreshCandidates();
            pushSphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(characterBody.teamComponent.teamIndex));
            pushSphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            result = pushSphereSearch.GetHurtBoxes();
            pushSphereSearch.ClearCandidates();

            return result;
        }

    }
}
