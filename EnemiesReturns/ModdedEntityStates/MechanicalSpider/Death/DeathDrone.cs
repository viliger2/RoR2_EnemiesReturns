using EnemiesReturns.Helpers;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using System.Linq;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    internal class DeathDrone : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if(NetworkServer.active)
            {
                SpawnDrone();
            }

            DestroyModel();
            if (NetworkServer.active)
            {
                DestroyBodyAsapServer();
            }
        }

        private void SpawnDrone()
        {
            var placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                position = characterBody.footPosition
            };

            var result = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(EnemiesReturns.Enemies.MechanicalSpider.MechanicalSpiderFactory.SpawnCards.iscMechanicalSpiderBroken, placementRule, Run.instance.spawnRng));
            if (result)
            {
                // TODO: add inventory copy to some component so spawned monster has elite equips and such
                var inventory = result.GetComponent<Inventory>();
                if (inventory && characterBody.inventory)
                {
                    inventory.CopyEquipmentFrom(characterBody.inventory);
                    inventory.CopyItemsFrom(characterBody.inventory);
                }
                var setEliteRamp = result.GetComponent<SetEliteRampOnShader>();
                if (setEliteRamp && inventory)
                {
                    setEliteRamp.SetEliteRampIndex(inventory.GetEquipment(0).equipmentDef?.passiveBuffDef?.eliteDef?.shaderEliteRampIndex ?? -1); // surely, SURELY it won't break
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
