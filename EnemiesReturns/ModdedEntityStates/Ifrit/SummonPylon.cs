using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class SummonPylon : BaseState
    {
        public static float baseDuration = 3f; // TODO: config

        public static float pillarSummonDuration = 2.5f; // TODO: config

        public static SpawnCard scPylon => Enemies.Ifrit.IfritPylonFactory.scIfritPylon;

        private float duration;



        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Body", "PillarSummon", 0.5f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration)
            {
                if(NetworkServer.active)
                {
                    SummonPillar();
                }
                if(base.isAuthority)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        private void SummonPillar()
        {
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(scPylon, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 30f,
                maxDistance = 50f,
                spawnOnTarget = transform
            }, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = base.gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = (SpawnCard.SpawnResult spawnResult) =>
            {
                if (spawnResult.success && spawnResult.spawnedInstance && base.characterBody)
                {
                    if(spawnResult.spawnedInstance.TryGetComponent<CharacterMaster>(out var deployableMaster))
                    {
                        var body = deployableMaster.GetBody();
                        if(body && body.gameObject.TryGetComponent<Deployable>(out var deployable))
                        {
                            deployable.onUndeploy.AddListener(deployableMaster.TrueKill);
                            characterBody.master.AddDeployable(deployable, Enemies.Ifrit.IfritFactory.PylonDeployable);
                        }
                    }
                }
            };
            DirectorCore.instance?.TrySpawnObject(directorSpawnRequest);
        }
    }
}
