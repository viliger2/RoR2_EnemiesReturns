using BepInEx;
using BepInEx.Configuration;
using EnemiesReturns.Configuration;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.LynxFetish;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency("JaceDaDorito.LocationsOfPrecipitation")]
    public class EnemiesReturnsPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "EnemiesReturns";
        public const string Version = "0.5.21";
        public const string GUID = "com." + Author + "." + ModName;

        public static bool ModIsLoaded = false;

        private void Awake()
        {
            var configs = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && !type.IsInterface && typeof(IConfiguration).IsAssignableFrom(type));
            Log.Init(Logger);
            EnemiesReturns.Configuration.General.PopulateConfig(Config);
#if DEBUG == false && NOWEAVER == false
            if (Configuration.General.UseConfigFile.Value)
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
            RegisterStuff();
            Hooks();
#if DEBUG == true
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
            Log.Error("EnemiesReturns is in a debug build! If you see this in release builds - report immediately since multiplayer will not work!");
#endif
            ModIsLoaded = true;

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
            ShamanStuff.Hooks();
            Enemies.LynxTribe.LynxShrineChatMessage.Hooks();
            Behaviors.SubjectParamsChatMessage.Hooks();

            On.RoR2.MusicController.StartIntroMusic += MusicController_StartIntroMusic;
            Equipment.MithrixHammer.MithrixHammer.Hooks();
            Enemies.Judgement.SetupJudgementPath.Hooks();
            Enemies.Judgement.AnointedSkins.Hooks();
        }

        private void MusicController_StartIntroMusic(On.RoR2.MusicController.orig_StartIntroMusic orig, MusicController self)
        {
            orig(self);
            AkSoundEngine.PostEvent("ER_Play_Music_System", self.gameObject);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.ItemEnabled.Value)
            {
                LynxFetishFactory.RecalculateStatsAPI_GetStatCoefficients(sender, args);
            }
            if (EnemiesReturns.Configuration.Judgement.Judgement.Enabled.Value)
            {
                Enemies.Judgement.SetupJudgementPath.RecalculateStatsAPI_GetStatCoefficients(sender, args);
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
