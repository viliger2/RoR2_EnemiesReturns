﻿using BepInEx;
using BepInEx.Configuration;
using EnemiesReturns.Behaviors.Judgement;
using EnemiesReturns.Configuration;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.LynxFetish;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using RoR2.Skills;

// TODO: recheck all enemies with 30 syringes so that their attacks work at high attack speeds
[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency("com.Viliger.RandyBobandyBrokeMyGamandy", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.score.AdvancedPrediction", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency(R2API.DeployableAPI.PluginGUID)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.EliteAPI.PluginGUID)]
    [BepInDependency(R2API.ProcTypeAPI.PluginGUID)]
    [BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID)]
    [BepInDependency(R2API.DamageAPI.PluginGUID)]
    [BepInDependency(R2API.TempVisualEffectAPI.PluginGUID)]
    [BepInDependency(R2API.OrbAPI.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency("JaceDaDorito.LocationsOfPrecipitation")]
    public class EnemiesReturnsPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "EnemiesReturns";
        public const string Version = "0.5.6";
        public const string GUID = "com." + Author + "." + ModName;

        private void Awake()
        {
#if DEBUG == true
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
#endif
            var configs = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && !type.IsInterface && typeof(IConfiguration).IsAssignableFrom(type));
            Log.Init(Logger);
#if DEBUG == false && NOWEAVER == false
            var UseConfigFile = Config.Bind<bool>("Config", "Use Config File", false, "Use config file for storring config. Each enemy gets their own config file. Due to mod being currently unfinished and unbalanced, we deploy rapid changes to values. So this way we can still have configs, but without the issue of people having those values saved.");

            if (UseConfigFile.Value)
            {
                foreach(var configType in configs)
                {
                    var config = (IConfiguration)System.Activator.CreateInstance(configType);
                    config.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.{config.GetType().Name}.cfg"), true));
                }
            }
            else
            {
                MakeNonConfigs(configs);
            }
#else
            MakeNonConfigs(configs);
#endif
            EnemiesReturns.Configuration.General.PopulateConfig(Config);

            RegisterStuff();

            Hooks();
            void MakeNonConfigs(IEnumerable<System.Type> configs)
            {
                var notSavedConfigFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, "Config"), false)
                {
                    SaveOnConfigSet = false,
                };

                foreach (var configType in configs)
                {
                    var config = (IConfiguration)System.Activator.CreateInstance(configType);
                    config.PopulateConfig(notSavedConfigFile);
                }
            }
        }

        private void RegisterStuff()
        {
            R2API.OrbAPI.AddOrb(typeof(LynxStormOrb));
        }

        private void Hooks()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            RoR2.Language.onCurrentLanguageChanged += Language.Language_onCurrentLanguageChanged;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            ColossalKnurlFactory.Hooks();
            IfritStuff.Hooks();
            SpawnPillarOnChampionKillFactory.Hooks();
            MechanicalSpiderVictoryDanceController.Hooks();
            Enemies.MechanicalSpider.MechanicalSpiderDroneOnPurchaseEvents.Hooks();
            LynxFetishFactory.Hooks();
            IL.RoR2.HealthComponent.Heal += ShamanStuff.HealthComponent_Heal;
            Enemies.LynxTribe.LynxShrineChatMessage.Hooks();

            Equipment.MithrixHammer.MithrixHammer.Hooks();
            Enemies.Judgement.SetupJudgementPath.Hooks();
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.ItemEnabled.Value)
            {
                LynxFetishFactory.RecalculateStatsAPI_GetStatCoefficients(sender, args);
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            var damageInfo = obj.damageInfo;
            var victim = obj.victim.gameObject;

            //ShamanStuff.OnHitEnemy(damageInfo, null, victim);

            if (!damageInfo.attacker || !damageInfo.attacker.TryGetComponent<CharacterBody>(out var attackerBody))
            {
                return;
            }

            if (!attackerBody.master)
            {
                return;
            }
            if (EnemiesReturns.Configuration.Colossus.ItemEnabled.Value)
            {
                ColossalKnurlFactory.OnHitEnemy(damageInfo, attackerBody, victim);
            }
        }

        [ConCommand(commandName = "returns_spawn_titans", flags = ConVarFlags.None, helpText = "Spawns all Titan variants")]
        private static void CCSpawnTitans(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanBlackBeach.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanDampCave.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGolemPlains.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGold.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_spitters", flags = ConVarFlags.None, helpText = "Spawns all Spitter variants")]
        private static void CCSpawnSpitters(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(SpitterBody.SpawnCards.cscSpitterDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(SpitterBody.SpawnCards.cscSpitterLakes, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(SpitterBody.SpawnCards.cscSpitterDepths, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(SpitterBody.SpawnCards.cscSpitterSulfur, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_colossi", flags = ConVarFlags.None, helpText = "Spawns all Colossus variants")]
        private static void CCSpawnColossi(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(ColossusBody.SpawnCards.cscColossusDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusGrassy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusSnowy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusSandy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusSkyMeadow, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusCastle, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_spiders", flags = ConVarFlags.None, helpText = "Spawns all Mechanical Spider variants")]
        private static void CCPocketSpiders(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_lynx", flags = ConVarFlags.None, helpText = "Spawns all Lynx Tribe enemies (including allies)")]
        private static void CCSpawnLynx(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Archer.ArcherBodyAlly.SpawnCards.cscLynxArcherAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Scout.ScoutBody.SpawnCards.cscLynxScoutDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Scout.ScoutBodyAlly.SpawnCards.cscLynxScoutAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Hunter.HunterBodyAlly.SpawnCards.cscLynxHunterAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Shaman.ShamanBody.SpawnCards.cscLynxShamanDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Shaman.ShamanBodyAlly.SpawnCards.cscLynxShamanAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Totem.TotemBody.SpawnCards.cscLynxTotemDefault, localPlayer.modelLocator.modelBaseTransform.position);
        }

        private static void SpawnMonster(CharacterSpawnCard card, Vector3 position)
        {
            var spawnRequest = new DirectorSpawnRequest(
                card,
                new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                    position = position
                },
                RoR2Application.rng
                );
            spawnRequest.teamIndexOverride = TeamIndex.Monster;
            spawnRequest.ignoreTeamMemberLimit = true;

            DirectorCore.instance.TrySpawnObject(spawnRequest);
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentProvider());
        }

        private void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), Language.LanguageFolder));
        }
    }
}
