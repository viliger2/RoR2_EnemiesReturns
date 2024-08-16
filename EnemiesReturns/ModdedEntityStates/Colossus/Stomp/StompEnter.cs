using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    public class StompEnter : BaseState
    {
        public static float searchRadius = 10f; 

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                var sphereSearch = new SphereSearch();

                var leftTransform = FindModelChild("StompLeftSearchPoint");
                List<HurtBox> lefttList = GetSphereSearchResult(sphereSearch, leftTransform.position);
                Transform closestLeftTransform = null;
                if (lefttList.Count > 0)
                {
                    closestLeftTransform = lefttList.First()?.healthComponent?.body.modelLocator.modelTransform ?? null;
                }

                var rightTransform = FindModelChild("StompRightSearchPoint");
                List<HurtBox> rightList = GetSphereSearchResult(sphereSearch, rightTransform.position);
                Transform closestRightTransform = null;
                if (rightList.Count > 0)
                {
                    closestRightTransform = rightList.First()?.healthComponent?.body.modelLocator.modelTransform ?? null;
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
