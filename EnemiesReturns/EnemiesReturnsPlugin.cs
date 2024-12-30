using BepInEx;
using BepInEx.Configuration;
using EnemiesReturns.Behaviors;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Newtonsoft.Json.Utilities;
using R2API.Networking;
using RewiredConsts;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency("com.Viliger.RandyBobandyBrokeMyGamandy", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.score.AdvancedPrediction", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.DeployableAPI.PluginGUID)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.EliteAPI.PluginGUID)]
    [BepInDependency(R2API.ProcTypeAPI.PluginGUID)]
    [BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID)]
    [BepInDependency(R2API.DamageAPI.PluginGUID)]
    [BepInDependency(R2API.TempVisualEffectAPI.PluginGUID)]
    public class EnemiesReturnsPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "EnemiesReturns";
        public const string Version = "0.4.3";
        public const string GUID = "com." + Author + "." + ModName;

        private void Awake()
        {
#if DEBUG == true
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
#endif
            var UseConfigFile = Config.Bind<bool>("Config", "Use Config File", false, "Use config file for storring config. Each enemy gets their own config file. Due to mod being currently unfinished and unbalanced, we deploy rapid changes to values. So this way we can still have configs, but without the issue of people having those values saved.");

            Log.Init(Logger);

            if (UseConfigFile.Value)
            {
                EnemiesReturns.Configuration.Spitter.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.Spitter.cfg"), true));
                EnemiesReturns.Configuration.Colossus.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.Colossus.cfg"), true));
                EnemiesReturns.Configuration.Ifrit.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.Ifrit.cfg"), true));
                EnemiesReturns.Configuration.MechanicalSpider.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.MechanicalSpider.cfg"), true));
                EnemiesReturns.Configuration.LynxTribe.LynxShaman.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.LynxShaman.cfg"), true));
            }
            else
            {
                var notSavedConfigFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, "Config"), false)
                {
                    SaveOnConfigSet = false,
                };

                EnemiesReturns.Configuration.Spitter.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.Colossus.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.Ifrit.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.MechanicalSpider.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.LynxTribe.LynxShaman.PopulateConfig(notSavedConfigFile);
            }
            EnemiesReturns.Configuration.General.PopulateConfig(Config);

            Hooks();
        }

        private void Hooks()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            RoR2.Language.onCurrentLanguageChanged += Language.Language_onCurrentLanguageChanged;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            ColossalKnurlFactory.Hooks();
            IfritStuff.Hooks();
            SpawnPillarOnChampionKillFactory.Hooks();
            MechanicalSpiderVictoryDanceController.Hooks();
            IL.RoR2.HealthComponent.Heal += ShamanStuff.HealthComponent_Heal;
            // using single R2API recalcstats hook for the sake of performance
            //R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
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
