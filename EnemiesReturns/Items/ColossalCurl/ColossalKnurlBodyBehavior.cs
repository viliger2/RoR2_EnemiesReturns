using RoR2;
using R2API;
using RoR2.Items;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Resources;
using static EnemiesReturns.EnemiesReturnsConfiguration.Colossus;

namespace EnemiesReturns.Items.ColossalKnurl
{
    public class ColossalKnurlBodyBehavior : CharacterBody.ItemBehavior
    {
        public CharacterMaster master;

        private DeployableMinionSpawner golemAllySpawner;

        private void Awake()
        {
            enabled = false;
        }
        private void OnEnable()
        {
            master = body.master;
            golemAllySpawner = new DeployableMinionSpawner(master, ColossalKnurlFactory.deployableSlot, RoR2Application.rng)
            {
                respawnInterval = 10f,
                spawnCard = ColossalKnurlFactory.cscGolemAlly
            };
            golemAllySpawner.onMinionSpawnedServer += OnTitanAllySpawned;
        }

        private void OnDisable()
        {
            golemAllySpawner?.Dispose();
            golemAllySpawner = null;
        }

        private void OnTitanAllySpawned(SpawnCard.SpawnResult result)
        {
            var spawnedObject = result.spawnedInstance;
            if(spawnedObject)
            {
                var master = spawnedObject.GetComponent<CharacterMaster>();
                if(master)
                {
                    master.inventory.GiveItem(RoR2Content.Items.BoostDamage,
                        KnurlGolemAllyDamageModifier.Value + KnurlGolemAllyDamageModifierPerStack.Value * (stack - 1));
                    master.inventory.GiveItem(RoR2Content.Items.BoostHp,
                        KnurlGolemAllyHealthModifier.Value + KnurlGolemAllyHealthModifierPerStack.Value * (stack - 1));
                    master.inventory.GiveItem(RoR2Content.Items.Hoof, 
                        KnurlGolemAllySpeedModifier.Value + KnurlGolemAllySpeedModifierPerStack.Value * (stack - 1)); // TODO: maybe too much
                    var deployable = master.GetComponent<Deployable>();
                    if(deployable)
                    {
                        body.master.AddDeployable(deployable, ColossalKnurlFactory.deployableSlot);
                    }
                }
            }
        }

        public static int GetModdedCount(CharacterMaster self, int count)
        {
            return count;
        }
    }
}
