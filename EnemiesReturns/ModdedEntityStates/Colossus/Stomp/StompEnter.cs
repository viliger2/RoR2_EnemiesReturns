using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    [RegisterEntityState]
    public class StompEnter : BaseState
    {
        public static float searchRadius = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                var sphereSearch = new SphereSearch();

                var leftTransform = FindModelChild("StompLeftSearchPoint");
                List<HurtBox> lefttList = GetSphereSearchResult(sphereSearch, leftTransform.position);
                Transform closestLeftTransform = null;
                if (lefttList.Count > 0)
                {
                    closestLeftTransform = GetFirstTransform(lefttList[0]);
                }

                var rightTransform = FindModelChild("StompRightSearchPoint");
                List<HurtBox> rightList = GetSphereSearchResult(sphereSearch, rightTransform.position);
                Transform closestRightTransform = null;
                if (rightList.Count > 0)
                {
                    closestRightTransform = GetFirstTransform(rightList[0]);    
                }

                var resultL = (leftTransform.position - closestLeftTransform?.position)?.sqrMagnitude ?? float.PositiveInfinity;
                var resultR = (rightTransform.position - closestRightTransform?.position)?.sqrMagnitude ?? float.PositiveInfinity;
                if (resultL < resultR)
                {
                    outer.SetNextState(new StompL());
                }
                else
                {
                    outer.SetNextState(new StompR());
                }
            }

            Transform GetFirstTransform(HurtBox hurtBox)
            {
                if (!hurtBox) return null;
                if(!hurtBox.healthComponent) return null;
                if(!hurtBox.healthComponent.body) return null;
                if(!hurtBox.healthComponent.body.modelLocator) return null;
                return hurtBox.healthComponent.body.modelLocator.modelTransform;
            }
        }

        private List<HurtBox> GetSphereSearchResult(SphereSearch sphereSearch, Vector3 origin)
        {
            List<HurtBox> result = new List<HurtBox>();
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.origin = origin;
            sphereSearch.radius = searchRadius;
            sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            sphereSearch.OrderCandidatesByDistance();
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            sphereSearch.GetHurtBoxes(result);
            sphereSearch.ClearCandidates();

            return result;
        }
    }
}
