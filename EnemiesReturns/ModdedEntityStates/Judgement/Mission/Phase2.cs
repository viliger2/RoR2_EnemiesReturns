using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using RoR2.CharacterSpeech;
using System.Collections.Generic;
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

        public static GameObject speechControllerPrefab;

        public static CharacterSpawnCard cscArraignHauntP1;

        private ScriptedCombatEncounter combatEncounter;

        private BossGroup phaseBossGroup;

        private ChildLocator childLocator;

        private GameObject phaseControllerObject;

        private bool hasSpawned;

        private bool hasHauntSpawned;

        private CombatSquad combatSquad;

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
            healthBarShowTime = Run.FixedTimeStamp.now + healthBarDelay;
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
            if (fixedAge > hauntSpawnDelay && NetworkServer.active && !hasHauntSpawned)
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
