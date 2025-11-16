using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.MechanicalSpider.Drone
{
    public class MechanicalSpiderVictoryDanceController : MonoBehaviour
    {
        public CharacterBody body;

        public static event Action onBossDeath;

        private void OnEnable()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (!body)
            {
                body = GetComponent<CharacterBody>();
            }

            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            onBossDeath += SpiderVictoryDanceController_onBossDeath;
            ModdedEntityStates.Judgement.Mission.Ending.onArraignDefeated += Ending_onArraignDefeated;
        }

        private void OnDisable()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            TeleporterInteraction.onTeleporterChargedGlobal -= TeleporterInteraction_onTeleporterChargedGlobal;
            onBossDeath -= SpiderVictoryDanceController_onBossDeath;
            ModdedEntityStates.Judgement.Mission.Ending.onArraignDefeated -= Ending_onArraignDefeated;
        }

        private void Ending_onArraignDefeated()
        {
            StartDancing();
        }

        private void SpiderVictoryDanceController_onBossDeath()
        {
            StartDancing();
        }

        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction obj)
        {
            StartDancing();
        }

        private void StartDancing()
        {
            if (body && body.hasEffectiveAuthority)
            {
                EntityStateMachine.FindByCustomName(body.gameObject, "Body").SetNextState(new ModdedEntityStates.MechanicalSpider.VictoryDance());
            }
        }

        public static void Hooks()
        {
            // wow I really hate this
            On.EntityStates.Missions.BrotherEncounter.BossDeath.OnEnter += BossDeath_OnEnter;
            On.EntityStates.MeridianEvent.MeridianEventCleared.OnEnter += MeridianEventCleared_OnEnter;
            On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += VoidRaidGauntletController_SpawnOutroPortal;
        }

        private static void VoidRaidGauntletController_SpawnOutroPortal(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, VoidRaidGauntletController self)
        {
            orig(self);
            onBossDeath?.Invoke();
        }

        private static void MeridianEventCleared_OnEnter(On.EntityStates.MeridianEvent.MeridianEventCleared.orig_OnEnter orig, EntityStates.MeridianEvent.MeridianEventCleared self)
        {
            orig(self);
            onBossDeath?.Invoke();
        }

        private static void BossDeath_OnEnter(On.EntityStates.Missions.BrotherEncounter.BossDeath.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.BossDeath self)
        {
            orig(self);
            onBossDeath?.Invoke();
        }
    }
}
