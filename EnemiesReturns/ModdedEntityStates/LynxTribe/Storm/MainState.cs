using EnemiesReturns.Enemies.LynxTribe.Storm;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Storm
{
    public class MainState : GenericCharacterMain
    {
        public static float stormRadius => EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonStormRadius.Value;

        public static float pullStrength => EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonStormPullStrength.Value;

        public static float lifetime => EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummmonStormLifetime.Value;

        public static float stormGrabRange => EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonStormGrabRange.Value;

        public static float outerRangeCoeff = 0.5f;

        public static float maxPullDistanceCoefficient = 0.66f;

        private SphereSearch pullSphereSearch;

        private Transform cachedModelTransform;

        private float maxPullDistance;

        public override void OnEnter()
        {
            base.OnEnter();
            pullSphereSearch = new SphereSearch();
            cachedModelTransform = (base.modelLocator ? base.modelLocator.modelTransform : null);
            maxPullDistance = stormRadius * maxPullDistanceCoefficient;
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
                    if (targetBody.hasEffectiveAuthority && !targetBody.HasBuff(LynxStormStuff.StormImmunity) && !targetBody.gameObject.GetComponent<LynxStormComponent>())
                    {
                        var component = targetBody.GetComponent<IDisplacementReceiver>();
                        if (component != null)
                        {
                            var pullCoeff = Vector3.Distance(position, targetBody.transform.position) > maxPullDistance ? outerRangeCoeff : 1f;
                            component.AddDisplacement((position - targetBody.transform.position).normalized * pullStrength * pullCoeff * GetDeltaTime());
                        }
                        if (Vector3.Distance(position, targetBody.transform.position) < stormGrabRange) // TODO: CHECK NETWORKING, also fix for larger bodies, probably by adding body size or something!!!
                        {
                            targetBody.gameObject.AddComponent<LynxStormComponent>().SetStormTransform(this.gameObject);
                        }
                    }
                }
            }

            if(fixedAge >= lifetime)
            {
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyMaster();
                    DestroyBody();
                }
            }
        }

        private void DestroyBody()
        {
            if (base.gameObject)
            {
                NetworkServer.Destroy(base.gameObject);
            }
        }

        private void DestroyMaster()
        {
            if (base.characterBody && base.characterBody.master)
            {
                NetworkServer.Destroy(base.characterBody.masterObject);
            }
        }

        private void DestroyModel()
        {
            if ((bool)cachedModelTransform)
            {
                EntityState.Destroy(cachedModelTransform.gameObject);
                cachedModelTransform = null;
            }
        }

        private HurtBox[] SearchForTargets()
        {
            HurtBox[] result;

            pullSphereSearch.mask = LayerIndex.entityPrecise.mask;
            pullSphereSearch.origin = transform.position;
            pullSphereSearch.radius = stormRadius;
            pullSphereSearch.queryTriggerInteraction = UnityEngine.QueryTriggerInteraction.UseGlobal;
            pullSphereSearch.RefreshCandidates();
            pullSphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(characterBody.teamComponent.teamIndex));
            pullSphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            result = pullSphereSearch.GetHurtBoxes();
            pullSphereSearch.ClearCandidates();

            return result;
        }

    }
}
