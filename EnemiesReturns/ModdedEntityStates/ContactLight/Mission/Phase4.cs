using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static EnemiesReturns.Content;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class Phase4 : BaseState
    {
        public static string phaseControllerChildString = "Phase4";

        public static float bossSpawnDelay = 10f;

        public static float healthBarDelay = 16f;

        public static MusicTrackDef musicTrack => Content.MusicTracks.DoubleFngRainbowReturns;

        public static MusicTrackDef musicTrackNone => Content.MusicTracks.None;

        private ScriptedCombatEncounter combatEncounter;

        private BossGroup bossGroup;

        private GameObject phaseControllerObject;

        private bool hasSpawned;

        private GameObject musicTrackOverrideNone;

        private ChildLocator phaseChildLocator;

        private bool healthBarDisplayed;

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
                    phaseChildLocator = phaseControllerObject.GetComponent<ChildLocator>();

                    var combatEncounterTransform = phaseChildLocator.FindChild("CombatEncounter");
                    if (combatEncounterTransform)
                    {
                        combatEncounter = combatEncounterTransform.gameObject.GetComponent<ScriptedCombatEncounter>();
                        bossGroup = combatEncounterTransform.gameObject.GetComponent<BossGroup>();
                    }

                    var musicTrackOverride = phaseChildLocator.FindChild("MusicTrackOverrideNone");
                    if (musicTrackOverride && musicTrackOverride.TryGetComponent<MusicTrackOverride>(out var musicComponent))
                    {
                        musicTrackOverrideNone = musicTrackOverride.gameObject;

                        musicComponent.track = musicTrackNone;
                        musicTrackOverride.gameObject.SetActive(true);
                    }
                }
            }
            ClearCorpses();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!hasSpawned && fixedAge > bossSpawnDelay)
            {
                hasSpawned = true;
                BeginEncounter();
                if (musicTrackOverrideNone)
                {
                    musicTrackOverrideNone.SetActive(false);
                }
                var musicTrackOverride = phaseChildLocator.FindChild("MusicTrackOverride");
                if (musicTrackOverride && musicTrackOverride.TryGetComponent<MusicTrackOverride>(out var musicComponent))
                {
                    musicComponent.track = musicTrack;
                    musicTrackOverride.gameObject.SetActive(true);
                }
            }

            if(!healthBarDisplayed && fixedAge > healthBarDelay)
            {
                if (bossGroup)
                {
                    bossGroup.shouldDisplayHealthBarOnHud = true;
                }
                healthBarDisplayed = true;
            }

            if (NetworkServer.active && fixedAge > bossSpawnDelay + 10f && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new PostFight());
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
