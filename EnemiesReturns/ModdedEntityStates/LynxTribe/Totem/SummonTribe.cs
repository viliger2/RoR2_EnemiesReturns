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

        public static float baseSummonDuration = 0.916f;

        public static float summonDistance = 4f; // distance from body for summon

        public static int summonCount => EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonTribeSummonCount.Value;

        public static int retryCount = 3;

        public static SpawnCard[] spawnCards = Array.Empty<SpawnCard>(); // TODO: spawn all cards as long as summonCount is more than spawnCards.lengh

        public static GameObject summonEffect;

        public static GameObject stoneEffectPrefab;

        public static GameObject eyeEffect;

        private float duration;

        private float summonDuration;

        private bool tribeSummoned;

        private Transform stoneParticlesOrigin;

        private Transform summonEffectTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            summonDuration = baseSummonDuration / attackSpeedStat;

            var childLocator = GetModelChildLocator();
            summonEffectTransform = childLocator.FindChild("SummonTribeSpawnEffect");
            stoneParticlesOrigin = childLocator.FindChild("SummonTribeStoneParticlesOrigin");

            if (eyeEffect)
            {
                EffectManager.SpawnEffect(eyeEffect, new EffectData()
                {
                    rootObject = base.gameObject,
                    modelChildIndex = (short)childLocator.FindChildIndex("StoneEyeL")
                }, false);
                EffectManager.SpawnEffect(eyeEffect, new EffectData()
                {
                    rootObject = base.gameObject,
                    modelChildIndex = (short)childLocator.FindChildIndex("StoneEyeR")
                }, false);
            }

            PlayAnimation("Gesture, Override", "SummonTribe", "summonTribe.playbackDuration", duration);
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
                            if(SummonTribesman(i, base.transform.position + new Vector3(x + 2 * i, 0 + i, z + 2 * i), retryCount))
                            {
                                break;
                            }
                            retryCount++;
                        }
                    }
                }
                if (summonEffect && summonEffectTransform) 
                {
                    EffectManager.SimpleEffect(summonEffect, summonEffectTransform.position, Quaternion.identity, false);
                }
                if (stoneEffectPrefab && stoneParticlesOrigin)
                {
                    EffectManager.SimpleEffect(stoneEffectPrefab, stoneParticlesOrigin.position, stoneParticlesOrigin.rotation, false);
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
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        private bool SummonTribesman(int bodyNum, Vector3 approximateSpawnPosition, int retryCount)
        {
            SpawnCard spawnCard;
            if (bodyNum + 1 <= summonCount - (summonCount % spawnCards.Length))
            {
                spawnCard = spawnCards[bodyNum % spawnCards.Length];
            }
            else
            {
                spawnCard = spawnCards[RoR2Application.rng.RangeInt(0, spawnCards.Length)];
            }
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
