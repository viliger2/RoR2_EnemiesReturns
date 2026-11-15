using EnemiesReturns.Enemies.ContactLight;
using EnemiesReturns.Enemies.Judgement;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class Phase3 : BaseState
    {
        public static string phaseControllerChildString = "Phase3";

        public static float bossSpawnDelay = 4f;

        public static float templeGuardDirectorDelay = 15f;

        private ScriptedCombatEncounter combatEncounter;

        private GameObject phaseControllerObject;

        private GameObject templeGuardsCombatDirector;

        private bool hasSpawned;

        private bool hasEnabledTempleGuards;

        public override void OnEnter()
        {
            Utils.KillAllMonsters();
            base.OnEnter();
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                phaseControllerObject = childLocator.FindChild(phaseControllerChildString).gameObject;
                if (phaseControllerObject)
                {
                    phaseControllerObject.SetActive(true);
                    var phaseChildLocator = phaseControllerObject.GetComponent<ChildLocator>();

                    var combatEncounterTransform = phaseChildLocator.FindChild("CombatEncounter");
                    if (combatEncounterTransform)
                    {
                        combatEncounter = combatEncounterTransform.gameObject.GetComponent<ScriptedCombatEncounter>();
                    }
                    var templeGuardsCombatDirectorTransform = phaseChildLocator.FindChild("TempleGuardCombatDirector");
                    if (templeGuardsCombatDirectorTransform)
                    {
                        templeGuardsCombatDirector = templeGuardsCombatDirectorTransform.gameObject;

                        var combatDirector = templeGuardsCombatDirector.GetComponent<CombatDirector>();
                        var placementArray = templeGuardsCombatDirector.GetComponent<CustomPlacement_Array>();
                        if(combatDirector && placementArray)
                        {
                            combatDirector.customPlacementBase = placementArray;
                        }
                    }
                }
            }
            ClearCorpses();
            BossGroup.onBossGroupStartServer += SpawnGoldTitanOnProvi;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!hasSpawned && fixedAge > bossSpawnDelay)
            {
                hasSpawned = true;
                BeginEncounter();
            }

            if(!hasEnabledTempleGuards && fixedAge > templeGuardDirectorDelay) 
            {
                templeGuardsCombatDirector.SetActive(true);
                hasEnabledTempleGuards = true;
            }

            if (NetworkServer.active && fixedAge > bossSpawnDelay + 10 && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Phase4());
            }
        }

        private void BeginEncounter()
        {
            if (NetworkServer.active)
            {
                combatEncounter.BeginEncounter();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Utils.KillAllMonsters();
            if (phaseControllerObject)
            {
                phaseControllerObject.SetActive(false);
            }
            BossGroup.onBossGroupStartServer -= SpawnGoldTitanOnProvi;
        }

        public void ClearCorpses()
        {
            for (int num3 = RoR2.Corpse.instancesList.Count - 1; num3 >= 0; num3--)
            {
                RoR2.Corpse.DestroyCorpse(RoR2.Corpse.instancesList[num3]);
            }
        }

        private static void SpawnGoldTitanOnProvi(BossGroup bossGroup)
        {
            CombatSquad combatSquad = bossGroup.combatSquad;
            bool flag = false;
            foreach (CharacterMaster readOnlyMembers in combatSquad.readOnlyMembersList)
            {
                if (readOnlyMembers.masterIndex == SetupContactLight.ProvidenceP2Master)
                {
                    flag = true;
                    break;
                }
            }
            float timer;
            if (flag)
            {
                timer = 2f;
                RoR2Application.onFixedUpdate += Check;
            }
            void Check()
            {
                bool flag2 = true;
                Vector3 approximatePosition = Vector3.zero;
                try
                {
                    if ((bool)combatSquad)
                    {
                        ReadOnlyCollection<CharacterMaster> readOnlyMembersList = combatSquad.readOnlyMembersList;
                        for (int i = 0; i < readOnlyMembersList.Count; i++)
                        {
                            CharacterMaster characterMaster = readOnlyMembersList[i];
                            if ((bool)characterMaster)
                            {
                                CharacterBody body = characterMaster.GetBody();
                                if (body)
                                {
                                    approximatePosition = body.transform.position;
                                }
                                if (body.HasBuff(RoR2Content.Buffs.HiddenInvincibility) || body.outOfCombat)
                                {
                                    flag2 = false;
                                }
                                else
                                {
                                    timer -= Time.fixedDeltaTime;
                                    if (timer > 0f)
                                    {
                                        flag2 = false;
                                    }
                                }
                            }
                        }
                        if (flag2)
                        {
                            GoldTitanManager.TryStartChannelingTitansServer(combatSquad, approximatePosition);
                        }
                    }
                }
                catch (Exception)
                {
                }
                if (flag2)
                {
                    RoR2Application.onFixedUpdate -= Check;
                }
            }
        }

    }
}
