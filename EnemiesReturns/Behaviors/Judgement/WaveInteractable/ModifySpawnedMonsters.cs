using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.WaveInteractable
{
    public class ModifySpawnedMonsters : MonoBehaviour
    {
        public CombatDirector combatDirector;

        public Inventory inventory;

        public EliteDef eliteType;

        public static float rewardMultiplier => Configuration.Judgement.Judgement.AeonianEliteGoldMultiplier.Value;

        private void Awake()
        {
            if (!combatDirector)
            {
                combatDirector = GetComponent<CombatDirector>();
            }
        }

        private void OnEnable()
        {
            combatDirector.onSpawnedServer.AddListener(Modify);
        }

        private void OnDisable()
        {
            combatDirector.onSpawnedServer.RemoveListener(Modify);
        }

        public void Modify(GameObject spawnedMonster)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            CharacterMaster component = spawnedMonster.GetComponent<CharacterMaster>();
            BaseAI ai = component.GetComponent<BaseAI>();
            if ((bool)ai)
            {
                ai.onBodyDiscovered += OnBodyDiscovered;
            }
            if (inventory)
            {
                component.inventory.AddItemsFrom(inventory);
            }

            var healthBoost = eliteType?.healthBoostCoefficient ?? 1f;
            var damageBoost = eliteType?.damageBoostCoefficient ?? 1f;

            var equipmentIndex = eliteType?.eliteEquipmentDef?.equipmentIndex ?? EquipmentIndex.None;
            if (equipmentIndex != EquipmentIndex.None)
            {
                component.inventory.SetEquipmentIndex(equipmentIndex, false);
            }

            component.inventory.GiveItemPermanent(RoR2Content.Items.BoostHp, Mathf.RoundToInt((healthBoost - 1) * 10f));
            component.inventory.GiveItemPermanent(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((damageBoost - 1) * 10f));

            void OnBodyDiscovered(CharacterBody newBody)
            {
                ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();

                if (newBody.gameObject.TryGetComponent<DeathRewards>(out var deathRewards))
                {
                    float num3 = newBody.cost * rewardMultiplier * 0.2f;
                    deathRewards.spawnValue = (int)Mathf.Max(1f, num3);
                    if (num3 > Mathf.Epsilon)
                    {
                        deathRewards.expReward = (uint)Mathf.Max(1f, num3 * Run.instance.compensatedDifficultyCoefficient);
                        deathRewards.goldReward = (uint)Mathf.Max(1f, num3 * 2f * Run.instance.compensatedDifficultyCoefficient); // 2 is magic number from combat director
                    }
                    else
                    {
                        deathRewards.expReward = 0u;
                        deathRewards.goldReward = 0u;
                    }
                }

                ai.onBodyDiscovered -= OnBodyDiscovered;
            }
        }
    }
}
