using EnemiesReturns.Enemies.LynxTribe.Storm;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    public class SummonStorm : BaseState
    {
        public static float baseDuration = 4f;

        public static float baseStoneEffectDuration = 3.24f;

        public static float baseStaffEffectDuration = 0.56f;

        public static float minDistance => Configuration.LynxTribe.LynxTotem.SummonStormMinRange.Value;
        public static float maxDistance => Configuration.LynxTribe.LynxTotem.SummonStormMaxRange.Value;

        public static int maxStormCount => Configuration.LynxTribe.LynxTotem.SummonStormMaxCount.Value;

        public static float playableMaxDistance = 1000f;

        public static float stormAttentionDuration => Configuration.LynxTribe.LynxTotem.SummmonStormLifetime.Value; // same as lifetime

        public static float effectSpawn = 0.35f;

        public static GameObject staffEffect;

        public static GameObject stoneEffectPrefab;

        public static GameObject eyeEffect;

        public static CharacterSpawnCard cscStorm;

        private float duration;

        private float stoneEffectDuration;

        private float staffEffectDuration;

        private bool staffEffectSpawned;

        private bool stonesSpawned;

        private float effectTimer;

        private Transform orbOrigin;

        private Transform stoneParticlesOrigin;

        private GameObject[] storms = Array.Empty<GameObject>();

        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            stoneEffectDuration = baseStoneEffectDuration / attackSpeedStat;
            staffEffectDuration = baseStaffEffectDuration / attackSpeedStat;
            PlayAnimation("Gesture, Override", "SummonTornados", "summonTornados.playbackDuration", duration);

            childLocator = GetModelChildLocator();
            orbOrigin = childLocator.FindChild("SummonStormsOrbOrigin");
            stoneParticlesOrigin = childLocator.FindChild("ShakeStoneParticlesOrigin");
            SummonStorms();
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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                effectTimer += GetDeltaTime();
                if (orbOrigin && effectTimer > effectSpawn)
                {
                    for (int i = 0; i < storms.Length; i++)
                    {
                        var orb = new LynxStormOrb();
                        orb.origin = orbOrigin.position;
                        orb.targetObject = storms[i];
                        orb.duration = 0.4f;
                        OrbManager.instance.AddOrb(orb);
                    }
                    effectTimer -= effectSpawn;
                }
            }

            if(fixedAge >= staffEffectDuration && !staffEffectSpawned)
            {
                if (staffEffect && orbOrigin)
                {
                    var effectData = new EffectData()
                    {
                        rootObject = this.gameObject,
                        modelChildIndex = (short)childLocator.FindChildIndex(orbOrigin),
                        origin = orbOrigin.position
                    };
                    EffectManager.SpawnEffect(staffEffect, effectData, false);
                }
                staffEffectSpawned = true;
            }

            if(fixedAge >= stoneEffectDuration && !stonesSpawned)
            {
                if (stoneParticlesOrigin && stoneEffectPrefab)
                {
                    EffectManager.SimpleEffect(stoneEffectPrefab, stoneParticlesOrigin.position, stoneParticlesOrigin.rotation, false);
                }
                stonesSpawned = true;
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

                    List<GameObject> stormList = new List<GameObject>();

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

                        stormList.Add(SummonStormAI(playerCharacterMaster));
                        stormCount++;
                    }
                    storms = stormList.ToArray();
                }
            }
        }

        private GameObject SummonStormAI(PlayerCharacterMasterController playerMasterController)
        {
            if (!playerMasterController.master)
            {
                return null;
            }

            var body = playerMasterController.master.GetBody();
            if (!body)
            {
                return null;
            }

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(cscStorm, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = minDistance,
                maxDistance = maxDistance,
                spawnOnTarget = body.transform
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
                        baseAI.currentEnemy.gameObject = body.gameObject;
                    }
                }
            };

            return DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
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
