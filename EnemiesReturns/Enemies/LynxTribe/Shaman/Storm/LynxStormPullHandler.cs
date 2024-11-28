using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman.Storm
{
    public class LynxStormPullHandler : NetworkBehaviour
    {
        //[Serializable]
        //public struct StormInfo
        //{
        //    public StormInfo(GameObject victim, GameObject inflictor)
        //    {
        //        this.victim = victim;
        //        this.inflictor = inflictor;
        //    }
        //    public GameObject victim;
        //    public GameObject inflictor;
        //}

        //public static float blacklistDuration = 10f;

        //public static float stormRadius => 20f;

        //public static float pullStrength => 5f;

        //private SphereSearch pullSphereSearch;

        //private CharacterBody characterBody;

        //private void Awake()
        //{
        //    pullSphereSearch = new SphereSearch();
        //    characterBody = GetComponent<CharacterBody>();
        //}

        //private void FixedUpdate()
        //{
        //    var position = transform.position;
        //    var hurtBoxes = SearchForTargets();

        //    foreach (var hurtbox in hurtBoxes)
        //    {
        //        if (hurtbox && hurtbox.healthComponent && hurtbox.healthComponent.body)
        //        {
        //            //if(blacklistEntries.TryGetValue(hurtbox.healthComponent, out float timer) && timer > 0f)
        //            //{
        //            //    continue;
        //            //}
        //            var targetBody = hurtbox.healthComponent.body;
        //            if (targetBody.hasEffectiveAuthority && !targetBody.gameObject.GetComponent<LynxStormComponent>())
        //            {
        //                var component = targetBody.GetComponent<IDisplacementReceiver>();
        //                if (component != null)
        //                {
        //                    component.AddDisplacement((position - targetBody.transform.position).normalized * pullStrength * Time.fixedDeltaTime);
        //                }
        //                // TODO: maybe move this stuff to 
        //                if (Vector3.Distance(position, targetBody.transform.position) < 3f) // TODO: CHECK NETWORKING!!!
        //                {
        //                    targetBody.gameObject.AddComponent<LynxStormComponent>().SetStormTransform(transform);
        //                    //if (!NetworkServer.active)
        //                    //{
        //                    //    CmdStart(new StormInfo(targetBody.gameObject, this.gameObject));
        //                    //} else
        //                    //{
        //                    //    SpawnStormController(targetBody.gameObject, this.gameObject);
        //                    //}
        //                    //blacklistEntries.Add(hurtbox.healthComponent, blacklistDuration);
        //                }
        //            }
        //        }
        //    }
        //}

        //[Command]
        //private void CmdStart(StormInfo info)
        //{
        //    SpawnStormController(info.victim, info.inflictor);
        //}

        //private void SpawnStormController(GameObject victim, GameObject inflictor)
        //{
        //    GameObject obj = UnityEngine.Object.Instantiate(EnemiesReturns.Enemies.LynxTribe.Shaman.Storm.LynxStormStuff.StormController);
        //    LynxStormController lynxStormController = obj.GetComponent<LynxStormController>();
        //    lynxStormController.SetVictim(victim);
        //    lynxStormController.SetOwner(inflictor);
        //    NetworkServer.Spawn(obj);
        //}


        //private HurtBox[] SearchForTargets()
        //{
        //    HurtBox[] result;

        //    pullSphereSearch.mask = LayerIndex.entityPrecise.mask;
        //    pullSphereSearch.origin = transform.position;
        //    pullSphereSearch.radius = stormRadius;
        //    pullSphereSearch.queryTriggerInteraction = UnityEngine.QueryTriggerInteraction.UseGlobal;
        //    pullSphereSearch.RefreshCandidates();
        //    pullSphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(characterBody.teamComponent.teamIndex));
        //    pullSphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
        //    result = pullSphereSearch.GetHurtBoxes();
        //    pullSphereSearch.ClearCandidates();

        //    return result;
        //}
    }
}
