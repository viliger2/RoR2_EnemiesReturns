using EnemiesReturns.Enemies.Judgement;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using RoR2.CharacterSpeech;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class Phase2 : BaseState
    {
        public static float spawnDelay = 3f;

        public static string phaseControllerChildString = "Phase2";

        public static float enableDirectorDelay = 9f;

        public static float healthBarDelay = 1f;

        public static float hauntSpawnDelay = 20f;

        public static int requiredKarma = 7;

        public static GameObject speechControllerPrefab;

        public static CharacterSpawnCard cscArraignHauntP1;

        private ScriptedCombatEncounter combatEncounter;

        private BossGroup phaseBossGroup;

        private ChildLocator childLocator;

        private GameObject phaseControllerObject;

        private bool hasSpawned;

        private bool hasHauntSpawned;

        private CombatSquad combatSquad;

        private bool shouldSummonHaunt;

        private Run.FixedTimeStamp healthBarShowTime = Run.FixedTimeStamp.positiveInfinity;

        public override void OnEnter()
        {
            Utils.KillAllMonsters();
            base.OnEnter();
            childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                phaseControllerObject = childLocator.FindChild(phaseControllerChildString).gameObject;
                if (phaseControllerObject)
                {
                    var phaseObjects = phaseControllerObject.transform.Find("PhaseObjects");
                    phaseObjects.gameObject.SetActive(true);

                    combatEncounter = phaseControllerObject.GetComponent<ScriptedCombatEncounter>();
                    phaseBossGroup = phaseControllerObject.GetComponent<BossGroup>();

                    var musicOverrideObject = phaseObjects.Find("MusicOverride");
                    if (musicOverrideObject)
                    {
                        var musicOverride = musicOverrideObject.GetComponent<MusicTrackOverride>();
                        musicOverride.track = Content.MusicTracks.UnknownBoss;

                        musicOverrideObject.gameObject.SetActive(true);
                    }
                }
            }
            if (phaseControllerObject)
            {
                combatSquad = phaseControllerObject.GetComponent<CombatSquad>();
                if (combatSquad)
                {
                    combatSquad.onMemberAddedServer += CombatSquad_onMemberAddedServer;
                }
            }
            shouldSummonHaunt = Utils.ArraignKarmaCheck(requiredKarma);

            healthBarShowTime = Run.FixedTimeStamp.now + healthBarDelay;

            BossGroup.onBossGroupStartServer += SpawnGoldTitanOnArraign;
        }

        private void CombatSquad_onMemberAddedServer(CharacterMaster master)
        {
            if (speechControllerPrefab)
            {
                UnityEngine.Object.Instantiate(speechControllerPrefab, master.transform).GetComponent<CharacterSpeechController>().characterMaster = master;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!hasSpawned)
            {
                if (fixedAge > spawnDelay)
                {
                    BeginEncounter();
                }
            }
            if (shouldSummonHaunt && fixedAge > hauntSpawnDelay && NetworkServer.active && !hasHauntSpawned)
            {
                if (SummonHaunt())
                {
                    hasHauntSpawned = true;
                }
            }

            phaseBossGroup.shouldDisplayHealthBarOnHud = healthBarShowTime.hasPassed;

            if (NetworkServer.active && fixedAge > spawnDelay + 2 && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Phase3());
            }
        }

        private void BeginEncounter()
        {
            hasSpawned = true;
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
            if (combatSquad)
            {
                combatSquad.onMemberAddedServer -= CombatSquad_onMemberAddedServer;
            }
            BossGroup.onBossGroupStartServer -= SpawnGoldTitanOnArraign;
        }

        private static void SpawnGoldTitanOnArraign(BossGroup bossGroup)
        {
            CombatSquad combatSquad = bossGroup.combatSquad;
            bool flag = false;
            foreach (CharacterMaster readOnlyMembers in combatSquad.readOnlyMembersList)
            {
                if (readOnlyMembers.masterIndex == SetupJudgementPath.ArraignP1MasterIndex)
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


        private GameObject SummonHaunt()
        {
            var placementRule = new DirectorPlacementRule();
            placementRule.placementMode = DirectorPlacementRule.PlacementMode.NearestNode;
            placementRule.position = Vector3.zero;

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(cscArraignHauntP1, placementRule, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.teamIndexOverride = TeamIndex.Monster;
            directorSpawnRequest.onSpawnedServer = (spawnResult) =>
            {
                if (spawnResult.success && spawnResult.spawnedInstance && characterBody)
                {
                    var baseAI = spawnResult.spawnedInstance.GetComponent<BaseAI>();
                    if (baseAI && baseAI.body)
                    {
                        baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                    }
                }
            };

            return DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
        }
    }
}
