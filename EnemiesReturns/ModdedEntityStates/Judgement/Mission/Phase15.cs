using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class Phase15 : BaseState
    {
        public static string phaseControllerChildString = "Phase15";

        public static int healthReductionItemCount => Mathf.CeilToInt(0.5f / Items.ReduceHP.ReduceHP.healthReductionFraction);

        public static int reduceDamageItemCount => Mathf.CeilToInt(0.2f / 0.01f);

        private ChildLocator childLocator;

        private GameObject phaseControllerObject;

        private Xoroshiro128Plus rng;

        private Inventory inventory;

        private List<ScriptedCombatEncounter> combatEncounters = new List<ScriptedCombatEncounter>();

        private GameObject effectInstance;

        public override void OnEnter()
        {
            rng = new Xoroshiro128Plus(Run.instance.spawnRng.nextUlong);

            Utils.KillAllMonsters();
            base.OnEnter();
            inventory = GetComponent<Inventory>();
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                phaseControllerObject = childLocator.FindChild(phaseControllerChildString).gameObject;
                if (phaseControllerObject)
                {
                    var phaseObjects = phaseControllerObject.transform.Find("PhaseObjects");

                    this.childLocator = phaseObjects.GetComponent<ChildLocator>();

                    GenerateSpawns();

                    phaseObjects.gameObject.SetActive(true);

                    foreach(var encounter in combatEncounters)
                    {
                        encounter.BeginEncounter();
                    }

                    var phaseCounter = this.childLocator.FindChild("PhaseCounter");
                    if(phaseCounter && phaseCounter.TryGetComponent<PhaseCounter>(out var counter))
                    {
                        counter.phase = 3;
                    }
                }
            }

            On.RoR2.KarmaManager.GetEffectiveKarma += KarmaManager_GetEffectiveKarma;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active && fixedAge > 10)
            {
                bool finished = true;
                foreach(var encounter in combatEncounters)
                {
                    finished = finished && encounter && encounter.combatSquad.memberCount == 0;
                }
                if (finished)
                {
                    outer.SetNextState(new PrePhase2());
                }
            }
        }

        private void CombatSquad_onMemberAddedServer(CharacterMaster characterMaster)
        {
            if (!characterMaster)
            {
                return;
            }

            var inventory = characterMaster.GetComponent<Inventory>();
            if (!inventory)
            {
                return;
            }

            if (this.inventory)
            {
                inventory.AddItemsFrom(this.inventory);
            }

            //inventory.GiveItemPermanent(Content.Items.ReduceHP.itemIndex, healthReductionItemCount);
            inventory.GiveItemPermanent(DLC4Content.Items.ReduceDamage.itemIndex, reduceDamageItemCount);
            inventory.GiveItemPermanent(RoR2Content.Items.InvadingDoppelganger, 1);
            // Giving 90% health reduction to combat doppelganger plus 1 to cut the remaining in half
            inventory.GiveItemPermanent(Content.Items.ReduceHP.itemIndex, 19);
        }

        private int KarmaManager_GetEffectiveKarma(On.RoR2.KarmaManager.orig_GetEffectiveKarma orig, KarmaManager self)
        {
            return 0;
        }

        private void GenerateSpawns()
        {
            List<CharacterSpawnCard> list = new List<CharacterSpawnCard>();
            list.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Brother.cscBrother_asset).WaitForCompletion());
            if (Run.instance.IsExpansionEnabled(DLC2Content.DLC2))
            {
                list.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_FalseSonBoss.cscFalseSonBoss_asset).WaitForCompletion());
            }
            if (Run.instance.IsExpansionEnabled(DLC1Content.DLC1))
            {
                list.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_VoidRaidCrab.cscMiniVoidRaidCrabPhase1_asset).WaitForCompletion());
            }
            if (Run.instance.IsExpansionEnabled(DLC3Content.DLC3))
            {
                list.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC3_VultureHunter.cscVultureHunter_asset).WaitForCompletion());
            }
            if (Run.instance.IsExpansionEnabled(DLC4Content.DLC4))
            {
                list.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC4_ReaperBoss.cscReaperBoss_asset).WaitForCompletion());
                list.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC4.cscProvidence_asset).WaitForCompletion());
            }

            var array = list.ToArray();
            rng.Shuffle<CharacterSpawnCard>(array);

            SetupEncounter(this.childLocator.FindChild("Boss1"), array, 0);
            SetupEncounter(this.childLocator.FindChild("Boss2"), array, 1);

            void SetupEncounter(Transform child, CharacterSpawnCard[] spawnCards, int index)
            {
                if (child)
                {
                    var combatEncounter = child.gameObject.GetComponent<ScriptedCombatEncounter>();
                    if (combatEncounter)
                    {
                        if (combatEncounter.spawns != null && combatEncounter.spawns.Length > 0)
                        {
                            combatEncounter.spawns[0].spawnCard = spawnCards[index % spawnCards.Length];
                        }
                        var combatSquad = child.gameObject.GetComponent<CombatSquad>();
                        if (combatSquad)
                        {
                            combatSquad.onMemberAddedServer += CombatSquad_onMemberAddedServer;
                        }
                        combatEncounters.Add(combatEncounter);
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            foreach(var encounter in combatEncounters)
            {
                if (encounter.combatSquad)
                {
                    encounter.combatSquad.onMemberAddedServer -= CombatSquad_onMemberAddedServer;
                }
            }
            if (effectInstance)
            {
                UnityEngine.Object.Destroy(effectInstance);
            }
            combatEncounters = null;
            phaseControllerObject.SetActive(false);
            On.RoR2.KarmaManager.GetEffectiveKarma -= KarmaManager_GetEffectiveKarma;
        }
    }
}
