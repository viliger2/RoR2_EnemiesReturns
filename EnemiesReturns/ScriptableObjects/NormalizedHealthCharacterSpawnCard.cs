using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ScriptableObjects
{
    public class NormalizedHealthCharacterSpawnCard : CharacterSpawnCard
    {
        public float targetHealth;

        public NormalizedHealthCharacterSpawnCard() { }

        public NormalizedHealthCharacterSpawnCard(CharacterSpawnCard origin)
        {

        }

        public NormalizedHealthCharacterSpawnCard FillFromCharacterSpawnCard(CharacterSpawnCard card)
        {
            (this as ScriptableObject).name = card.name + "NormalizedHealth";
            this.prefab = card.prefab;
            this.sendOverNetwork = card.sendOverNetwork;
            this.hullSize= card.hullSize;
            this.nodeGraphType = card.nodeGraphType;
            this.requiredFlags= card.requiredFlags;
            this.forbiddenAsBoss= card.forbiddenAsBoss;
            this.directorCreditCost = card.directorCreditCost;
            this.occupyPosition = card.occupyPosition;
            this.eliteRules = card.eliteRules;
            this.noElites = card.noElites;
            this.forbiddenAsBoss = card.forbiddenAsBoss;
            this.loadout = card.loadout;

            return this;
        }

        public override void Spawn(Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest, ref SpawnResult result)
        {
            base.Spawn(position, rotation, directorSpawnRequest, ref result);
            if (result.success)
            {
                var master = result.spawnedInstance.GetComponent<CharacterMaster>();
                if (master && master.bodyPrefab)
                {
                    float healthFraction = 0f;
                    int itemsToAdd = 0;

                    var body = master.bodyPrefab.GetComponent<CharacterBody>();
                    if (body)
                    {
                        healthFraction = Mathf.Max(1 - targetHealth / body.baseMaxHealth, 0);
                    }
                    if(healthFraction > 0)
                    {
                        itemsToAdd = Mathf.CeilToInt(healthFraction / Items.ReduceHP.ReduceHP.healthReductionFraction);
                    }

                    master.inventory.GiveItemPermanent(Content.Items.ReduceHP, itemsToAdd);
                }
            }
        }
    }
}
