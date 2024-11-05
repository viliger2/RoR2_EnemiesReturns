using BepInEx;
using BepInEx.Configuration;
using EnemiesReturns.Configuration;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Viliger.RandyBobandyBrokeMyGamandy", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.score.AdvancedPrediction", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.DeployableAPI.PluginGUID)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    public class EnemiesReturnsPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "EnemiesReturns";
        public const string Version = "0.3.2";
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
                EnemiesReturns.Configuration.General.PopulateConfig(Config);
                EnemiesReturns.Configuration.Spitter.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.Spitter.cfg"), true));
                EnemiesReturns.Configuration.Colossus.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.Colossus.cfg"), true));
                EnemiesReturns.Configuration.Ifrit.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.Ifrit.cfg"), true));
                EnemiesReturns.Configuration.MechanicalSpider.PopulateConfig(new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, $"com.{Author}.{ModName}.MechanicalSpider.cfg"), true));
            }
            else
            {
                var notSavedConfigFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, "Config"), false)
                {
                    SaveOnConfigSet = false,
                };

                EnemiesReturns.Configuration.General.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.Spitter.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.Colossus.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.Ifrit.PopulateConfig(notSavedConfigFile);
                EnemiesReturns.Configuration.MechanicalSpider.PopulateConfig(notSavedConfigFile);
            }

            Hooks();
        }

        private void Hooks()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            RoR2.Language.onCurrentLanguageChanged += Language.Language_onCurrentLanguageChanged;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            ColossalKnurlFactory.Hooks();
            IfritFactory.Hooks();
            SpawnPillarOnChampionKillFactory.Hooks();
            // using single R2API recalcstats hook for the sake of performance
            //R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            var damageInfo = obj.damageInfo;
            var victim = obj.victim.gameObject;

            if (!damageInfo.attacker || !damageInfo.attacker.TryGetComponent<CharacterBody>(out var attackerBody))
            {
                return;
            }

            if (!attackerBody.master)
            {
                return;
            }

            ColossalKnurlFactory.OnHitEnemy(damageInfo, attackerBody, victim);
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

            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterLakes, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterDepths, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterSulfur, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_colossi", flags = ConVarFlags.None, helpText = "Spawns all Colossus variants")]
        private static void CCSpawnColossi(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusGrassy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSnowy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSandy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSkyMeadow, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusCastle, localPlayer.modelLocator.modelBaseTransform.position);
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
