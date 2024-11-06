using EnemiesReturns.Helpers;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using System.Linq;
using static EntityStates.Drone.DeathState;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    internal class DeathDrone : GenericCharacterDeath
    {
        public static float deathDuration = 4f;

        private bool startedGrounded = true;

        private bool droneSpawned = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if(isVoidDeath)
            {
                return;
            }
            if (characterMotor)
            {
                if (characterMotor.isGrounded)
                {
                    if (NetworkServer.active)
                    {
                        SpawnDrone(base.characterBody.footPosition);
                        Explode();
                    }
                    DestroyModel();
                }
                else
                {
                    startedGrounded = false;
                }
            }
            
            Util.PlaySound("ER_Spider_Death_Drone_Play", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!startedGrounded && characterMotor && characterMotor.isGrounded)
            {
                if (NetworkServer.active && !droneSpawned)
                {
                    SpawnDrone(base.characterBody.footPosition);
                    Explode();
                    droneSpawned = true;
                }
                DestroyModel();
            }
            if (fixedAge >= deathDuration && NetworkServer.active)
            {
                Explode();
            }
        }

        public void Explode()
        {
            EntityState.Destroy(base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            Explode();
            DestroyModel();
        }

        private void SpawnDrone(Vector3 spawnPosition)
        {
            var placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                position = spawnPosition
            };

            var result = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(EnemiesReturns.Enemies.MechanicalSpider.MechanicalSpiderFactory.SpawnCards.iscMechanicalSpiderBroken, placementRule, Run.instance.spawnRng));
            if (result)
            {
                var inventory = result.GetComponent<Inventory>();
                if (inventory && characterBody.inventory)
                {
                    inventory.CopyEquipmentFrom(characterBody.inventory);
                    inventory.CopyItemsFrom(characterBody.inventory);
                }
                var setEliteRamp = result.GetComponent<SetEliteRampOnShader>();
                if (setEliteRamp && inventory)
                {
                    setEliteRamp.SetEliteRampIndex(inventory.GetEquipment(0).equipmentDef?.passiveBuffDef?.eliteDef?.shaderEliteRampIndex ?? -1, inventory.GetEquipment(0).equipmentDef?.passiveBuffDef?.eliteDef?.eliteIndex ?? EliteIndex.None); // surely, SURELY it won't break
                }
                var purchaseInteraction = result.GetComponent<PurchaseInteraction>();
                if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money && inventory)
                {
                    var eliteDef = inventory.GetEquipment(0).equipmentDef?.passiveBuffDef?.eliteDef ?? null;
                    float eliteModifier = 1f;
                    if (eliteDef)
                    {
                        var eliteTier = CombatDirector.eliteTiers.First(tier => tier.eliteTypes.Contains(eliteDef));
                        eliteModifier = eliteTier.costMultiplier * EnemiesReturns.Configuration.MechanicalSpider.DroneEliteConstMultiplier.Value;
                    }
                    purchaseInteraction.Networkcost = Run.instance.GetDifficultyScaledCost((int)(purchaseInteraction.cost * eliteModifier));
                }
            }
        }
    }
}
