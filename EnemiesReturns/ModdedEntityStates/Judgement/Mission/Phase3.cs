using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using RoR2.CharacterSpeech;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class Phase3 : BaseState
    {
        public static float spawnDelay = 0f;

        public static string phaseControllerChildString = "Phase3";

        public static GameObject speechControllerPrefab;

        public static CharacterSpawnCard cscArraignHaunt;

        public static float hauntSpawnDelay = 30f;

        public static float healthBarDelay = 15f;

        private ScriptedCombatEncounter combatEncounter;

        private BossGroup phaseBossGroup;

        private ChildLocator childLocator;

        private GameObject phaseControllerObject;

        private CombatSquad combatSquad;

        private bool hasHauntSpawned;

        private bool hasArraignSpawned;

        private Run.FixedTimeStamp healthBarShowTime = Run.FixedTimeStamp.positiveInfinity;

        public override void OnEnter()
        {
            KillAllMonsters();
            base.OnEnter();
            childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                phaseControllerObject = childLocator.FindChild(phaseControllerChildString).gameObject;
                if (phaseControllerObject)
                {
                    combatEncounter = phaseControllerObject.GetComponent<ScriptedCombatEncounter>();
                    phaseBossGroup = phaseControllerObject.GetComponent<BossGroup>();

                    var phaseObjects = phaseControllerObject.transform.Find("PhaseObjects");
                    phaseObjects.gameObject.SetActive(true);

                    var musicOverrideObject = phaseObjects.Find("MusicOverride");
                    if (musicOverrideObject)
                    {
                        var musicOverride = musicOverrideObject.GetComponent<MusicTrackOverride>();
                        if (Configuration.General.EnableCustomPhase3Music.Value) // TODO: replace with Judgement config
                        {
                            musicOverride.track = Content.MusicTracks.TheOrigin;
                        }
                        else
                        {
                            musicOverride.track = Content.MusicTracks.UnknownBoss;
                        }

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
            healthBarShowTime = Run.FixedTimeStamp.now + healthBarDelay;
            ClearCorpses();
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

            if(fixedAge > hauntSpawnDelay && NetworkServer.active && !hasHauntSpawned)
            {
                if (SummonHaunt())
                {
                    hasHauntSpawned = true;
                }
            }

            if (!hasArraignSpawned && fixedAge > spawnDelay)
            {
                BeginEncounter();
            }
            phaseBossGroup.shouldDisplayHealthBarOnHud = healthBarShowTime.hasPassed;

            if (NetworkServer.active && fixedAge > spawnDelay + 2 && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Ending());
            }
        }

        private void BeginEncounter()
        {
            hasArraignSpawned = true;
            if (NetworkServer.active)
            {
                combatEncounter.BeginEncounter();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (phaseControllerObject)
            {
                phaseControllerObject.SetActive(false);
            }
            if (combatSquad)
            {
                combatSquad.onMemberAddedServer -= CombatSquad_onMemberAddedServer;
            }
        }

        private GameObject SummonHaunt()
        {
            var placementRule = new DirectorPlacementRule();
            placementRule.placementMode = DirectorPlacementRule.PlacementMode.NearestNode;
            placementRule.position = Vector3.zero;

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(cscArraignHaunt, placementRule, RoR2Application.rng);
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

        public void KillAllMonsters()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            foreach (TeamComponent item in new List<TeamComponent>(TeamComponent.GetTeamMembers(TeamIndex.Monster)))
            {
                if ((bool)item)
                {
                    HealthComponent component = item.GetComponent<HealthComponent>();
                    if ((bool)component)
                    {
                        component.Suicide();
                    }
                }
            }
        }

        public void ClearCorpses()
        {
            for (int num3 = RoR2.Corpse.instancesList.Count - 1; num3 >= 0; num3--)
            {
                RoR2.Corpse.DestroyCorpse(RoR2.Corpse.instancesList[num3]);
            }
        }
    }
}
