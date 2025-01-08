using EntityStates;
using RoR2;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    public class SummonTribe : BaseState
    {
        public static float baseDuration = 1.6f;

        public static float baseSummonDuration = 1f;

        public static float summonDistance = 4f; // distance from body for summon

        public static int summonCount = 4;

        public static int retryCount = 3;

        public static SpawnCard[] spawnCards = Array.Empty<SpawnCard>();

        private float duration;

        private float summonDuration;

        private bool tribeSummoned;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            summonDuration = baseSummonDuration / attackSpeedStat;
            PlayAnimation("Gesture", "SummonTribe", "summonTribe.playbackDuration", duration);
            if(spawnCards.Length == 0)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > summonDuration && !tribeSummoned)
            {
                if (NetworkServer.active)
                {
                    float angle = 360 / summonCount;
                    for (int i = 0; i < summonCount; i++)
                    {
                        var currentRetryCount = 0;
                        var x = summonDistance * Mathf.Cos(angle * i * Mathf.Deg2Rad);
                        var z = summonDistance * Mathf.Sin(angle * i * Mathf.Deg2Rad);
                        while(currentRetryCount < retryCount) 
                        {
                            if(SummonTribesman(base.transform.position + new Vector3(x + 2 * i, 0 + i, z + 2 * i), retryCount))
                            {
                                break;
                            }
                            retryCount++;
                        }
                    }
                }
                tribeSummoned = true;
            }

            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private bool SummonTribesman(Vector3 approximateSpawnPosition, int retryCount)
        {
            var spawnCard = spawnCards[RoR2Application.rng.RangeInt(0, spawnCards.Length)];
            var directorSpawnRequest = new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 0f,
                maxDistance = summonDistance * (1 + retryCount), // just to be safe
                position = approximateSpawnPosition
            }, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = base.gameObject;
            directorSpawnRequest.onSpawnedServer = OnCardSpawned;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.teamIndexOverride = teamComponent.teamIndex;
            return DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

            void OnCardSpawned(SpawnCard.SpawnResult result)
            {
                if(result.success && result.spawnedInstance && characterBody)
                {
                    var inventory = result.spawnedInstance.GetComponent<Inventory>();
                    if (inventory)
                    {
                        inventory.CopyEquipmentFrom(base.characterBody.inventory);
                        if(characterBody.inventory.GetItemCount(RoR2Content.Items.Ghost) > 0)
                        {
                            inventory.GiveItem(RoR2Content.Items.Ghost);
                            inventory.GiveItem(RoR2Content.Items.HealthDecay, 30);
                            inventory.GiveItem(RoR2Content.Items.BoostDamage, 150);
                        }
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
