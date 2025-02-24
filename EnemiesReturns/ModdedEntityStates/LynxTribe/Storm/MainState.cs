using EnemiesReturns.Enemies.LynxTribe.Storm;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
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

        private CharacterMaster owner;

        public override void OnEnter()
        {
            base.OnEnter();
            pullSphereSearch = new SphereSearch();
            cachedModelTransform = (base.modelLocator ? base.modelLocator.modelTransform : null);
            maxPullDistance = stormRadius * maxPullDistanceCoefficient;
            var aiOwnership = characterBody.master.GetComponent<AIOwnership>();
            if (aiOwnership)
            {
                owner = aiOwnership.ownerMaster;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active) 
            {
                if (owner)
                {
                    var ownerBody = owner.GetBody();
                    if (!ownerBody)
                    {
                        // if we have master, but master has no body, means the owner is dead\doesn't exist in game space
                        fixedAge += lifetime;
                    }

                    if(ownerBody.healthComponent && !ownerBody.healthComponent.alive)
                    {
                        fixedAge += lifetime; 
                    }
                }
            }

            var position = transform.position;
            var hurtBoxes = SearchForTargets();
            foreach (var hurtbox in hurtBoxes)
            {
                if (hurtbox && hurtbox.healthComponent && hurtbox.healthComponent.body)
                {
                    var targetBody = hurtbox.healthComponent.body;
                    if (targetBody.hasEffectiveAuthority && !targetBody.HasBuff(Content.Buffs.LynxStormImmunity) && !targetBody.gameObject.GetComponent<LynxStormComponent>())
                    {
                        var component = targetBody.GetComponent<IDisplacementReceiver>();
                        if (component != null)
                        {
                            var pullCoeff = Vector3.Distance(position, targetBody.transform.position) > maxPullDistance ? outerRangeCoeff : 1f;
                            component.AddDisplacement((position - targetBody.transform.position).normalized * pullStrength * pullCoeff * GetDeltaTime());
                        }
                        if (Vector3.Distance(position, targetBody.transform.position) < (stormGrabRange - 1) + targetBody.radius) // -1 is because it was tested on commando before adding body radius and commando is about 1 unity stones in radius
                        {
                            targetBody.gameObject.AddComponent<LynxStormComponent>().SetStormTransform(this.gameObject);
                        }
                    }
                }
            }

            if (fixedAge >= lifetime && NetworkServer.active)
            {
                characterBody.healthComponent.Suicide();
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
