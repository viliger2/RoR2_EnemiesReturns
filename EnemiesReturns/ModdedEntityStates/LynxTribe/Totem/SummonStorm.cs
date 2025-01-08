using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    public class SummonStorm : BaseState
    {
        public static float baseDuration => 4f;

        public static float minDistance => Configuration.LynxTribe.LynxShaman.SummonStormMinRange.Value;
        public static float maxDistance => Configuration.LynxTribe.LynxShaman.SummonStormMaxRange.Value;

        public static int maxStormCount = -1;

        public static float playableMaxDistance = 1000f;

        public static float stormAttentionDuration = 30f; // same as lifetime

        public static CharacterSpawnCard cscStorm;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            SummonStorms();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void SummonStorms()
        {
            if (isAuthority)
            {
                if (characterBody.isPlayerControlled)
                {
                    if (Physics.Raycast(GetAimRay(), out var hitInfo, playableMaxDistance, LayerIndex.world.mask))
                    {
                        SummonStormPlayer(hitInfo.point);
                    }
                } else
                {
                    if (!NetworkServer.active) // TODO: networking visuals somehow
                    {
                        return;
                    }

                    var stormCount = 0;
                    foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
                    {
                        if (!playerCharacterMaster.isConnected)
                        {
                            continue;
                        }

                        if (maxStormCount >= 0 && stormCount >= maxStormCount)
                        {
                            return;
                        }

                        SummonStormAI(playerCharacterMaster.body.gameObject.transform, playerCharacterMaster.body);
                        stormCount++;
                    }
                }
            }
        }

        private void SummonStormAI(Transform transform, CharacterBody target)
        {
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(cscStorm, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = minDistance,
                maxDistance = maxDistance,
                spawnOnTarget = transform
            }, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = (spawnResult) =>
            {
                if (spawnResult.success && spawnResult.spawnedInstance && characterBody)
                {
                    var aiownership = spawnResult.spawnedInstance.GetComponent<AIOwnership>();
                    if (aiownership)
                    {
                        aiownership.ownerMaster = characterBody.master;
                    }
                    var inventory = spawnResult.spawnedInstance.GetComponent<Inventory>();
                    if (inventory)
                    {
                        inventory.CopyEquipmentFrom(characterBody.inventory);
                        inventory.CopyItemsFrom(characterBody.inventory);
                    }
                    var baseAI = spawnResult.spawnedInstance.GetComponent<BaseAI>();
                    if (baseAI)
                    {
                        baseAI.enemyAttentionDuration = stormAttentionDuration;
                        baseAI.currentEnemy.gameObject = target.gameObject;
                    }
                }
            };

            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
        }

        private void SummonStormPlayer(Vector3 position)
        {
            if (characterBody)
            {
                characterBody.SendConstructTurret(characterBody, position, Quaternion.identity, MasterCatalog.FindMasterIndex(cscStorm.prefab));
            }
        }
    }
}
