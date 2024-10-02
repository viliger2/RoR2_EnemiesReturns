using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class SummonPylon : BaseState
    {
        public static float baseDuration = 3f;

        public static float baseSummonTimer = 1f;

        public static GameObject screamPrefab;

        public static SpawnCard scPylon => Enemies.Ifrit.IfritPillarFactory.Enemy.scIfritPillar;

        public static float minSpawnDistance => EnemiesReturnsConfiguration.Ifrit.PillarMinSpawnDistance.Value;

        public static float maxSpawnDistance => EnemiesReturnsConfiguration.Ifrit.PillarMaxSpawnDistance.Value;

        public static int pillarCount => EnemiesReturnsConfiguration.Ifrit.PillarMaxInstances.Value;

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
            Util.PlaySound("ER_Ifrit_SummonPillar_Play", base.gameObject);
            PlayCrossfade("Body", "PillarSummon", "PillarSummon.playbackRate", duration, 0.2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= summonTimer && !hasSummoned)
            {
                hasSummoned = true;
                if (NetworkServer.active)
                {
                    for (int i = 0; i < pillarCount; i++)
                    {
                        SummonPillar();
                    }
                }
                if (muzzleMouth)
                {
                    SpawnEffect(muzzleMouth);
                }
            }

            if (fixedAge >= duration && base.isAuthority)
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
                minDistance = minSpawnDistance,
                maxDistance = maxSpawnDistance,
                spawnOnTarget = transform
            }, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = base.gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = (SpawnCard.SpawnResult spawnResult) =>
            {
                if (!spawnResult.success)
                {
                    SummonPillar(); // surely this won't break anything
                    return;
                }
                if (spawnResult.spawnedInstance && base.characterBody)
                {
                    var aiownership = spawnResult.spawnedInstance.GetComponent<AIOwnership>();
                    if (aiownership)
                    {
                        aiownership.ownerMaster = this.characterBody.master;
                    }
                    var inventory = spawnResult.spawnedInstance.GetComponent<Inventory>();
                    if (inventory)
                    {
                        inventory.CopyEquipmentFrom(base.characterBody.inventory);
                    }

                    if (spawnResult.spawnedInstance.TryGetComponent<CharacterMaster>(out var deployableMaster))
                    {
                        var body = deployableMaster.GetBody();
                        if (body && body.gameObject.TryGetComponent<Deployable>(out var deployable))
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
