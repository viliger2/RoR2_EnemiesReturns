using EnemiesReturns.Enemies.Ifrit;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class SummonStormSkill : BaseState
    {
        public static float baseDuration => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormCastTime.Value;
        public static float minDistance => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormMinRange.Value;
        public static float maxDistance => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormMaxRange.Value;
        public static float rechargeOnFailure => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormRechargeOnFailure.Value;
        public static CharacterSpawnCard cscStorm;
        public static int stormCount => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormCount.Value;

        private float duration;

        private GameObject[] storms = Array.Empty<GameObject>();

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "SummonStorm", "SummonStorm.playbackRate", duration, 0.1f);
            SummonStorms();
        }

        private void SummonStorms()
        {
            if (NetworkServer.active)
            {
                storms = new GameObject[stormCount];
                for(int i = 0; i < stormCount; i++)
                {
                    foreach (var ai in characterBody.master.aiComponents)
                    {
                        if (!ai.currentEnemy.characterBody)
                        {
                            continue;
                        }

                        storms[i] = SummonStorm(ai.currentEnemy.characterBody.transform);
                    }
                }
            }
        }

        private GameObject SummonStorm(Transform transform)
        {
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(cscStorm, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = minDistance,
                maxDistance = maxDistance,
                spawnOnTarget = transform
            }, RoR2Application.rng);

            directorSpawnRequest.summonerBodyObject = base.gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = (SpawnCard.SpawnResult spawnResult) =>
            {
                if (!spawnResult.success)
                {
                    SummonStorm(transform); // surely this won't break anything
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
                        inventory.CopyItemsFrom(base.characterBody.inventory);
                    }
                }
            };
            return DirectorCore.instance?.TrySpawnObject(directorSpawnRequest);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);
            if(!(nextState is EntityStates.GenericCharacterMain))
            {
                if (NetworkServer.active)
                {
                    foreach (var gameObject in storms)
                    {
                        if (gameObject && gameObject.TryGetComponent<CharacterMaster>(out var master))
                        {
                            master.TrueKill();
                        }
                    }
                }
                skillLocator.special.RunRecharge(30f * rechargeOnFailure); // TODO: config
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            storms = null;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
