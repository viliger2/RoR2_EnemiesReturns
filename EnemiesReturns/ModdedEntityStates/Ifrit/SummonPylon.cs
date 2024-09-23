using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class SummonPylon : BaseState
    {
        public static float baseDuration = 3f; // TODO: config

        public static float baseSummonTimer = 1f;

        public static GameObject screamPrefab;

        public static SpawnCard scPylon => Enemies.Ifrit.IfritPylonFactory.scIfritPylon;

        private float duration;

        private float summonTimer;

        private Transform muzzleMouth;

        private bool hasSummoned;

        public override void OnEnter()
        {
            base.OnEnter();
            hasSummoned = false;
            duration = baseDuration / attackSpeedStat;
            summonTimer = baseSummonTimer / attackSpeedStat;
            muzzleMouth = FindModelChild("MuzzleMouth");
            PlayCrossfade("Body", "PillarSummon", "PillarSummon.playbackRate", duration, 0.2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= summonTimer && !hasSummoned)
            {
                hasSummoned = true;
                if (NetworkServer.active)
                {
                    SummonPillar();
                }
                if(muzzleMouth)
                {
                    SpawnEffect(muzzleMouth);
                }
            }

            if(fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void SpawnEffect(Transform position)
        {
            EffectManager.SpawnEffect(screamPrefab, new EffectData { rootObject = muzzleMouth.gameObject }, false);
        }

        private void SummonPillar()
        {
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(scPylon, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 30f,
                maxDistance = 50f, // TODO
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
