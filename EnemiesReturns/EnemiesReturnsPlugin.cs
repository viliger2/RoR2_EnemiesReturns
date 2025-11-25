using BepInEx;
using BepInEx.Configuration;
using EnemiesReturns.Configuration;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using EnemiesReturns.Enemies.MechanicalSpider.Drone;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.LynxFetish;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using EnemiesReturns.ModCompats;
using HarmonyLib;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Networking;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    // TODO: flowers visuals when aeonian elite dies, careful with performance
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(AdvancedPredictionCompat.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(AncientScepterCompat.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(EliteReworksCompat.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(MoreStatsCompat.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(RiskyArtifafactsCompat.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
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
    [BepInDependency(ReviveAPI.ReviveAPI.ModGuid)]
    [BepInDependency("JaceDaDorito.LocationsOfPrecipitation")]
    public class EnemiesReturnsPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "EnemiesReturns";
        public const string Version = "0.7.13";
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
            RoR2.CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            ColossalKnurlFactory.Hooks();
            IfritStuff.Hooks();
            SpawnPillarOnChampionKillFactory.Hooks();
            MechanicalSpiderVictoryDanceController.Hooks();
            MechanicalSpiderDroneOnPurchaseEvents.Hooks();
            LynxFetishFactory.Hooks();
            Enemies.LynxTribe.LynxShrineChatMessage.Hooks();
            Behaviors.SubjectParamsChatMessage.Hooks();

            On.RoR2.MusicController.StartIntroMusic += MusicController_StartIntroMusic;
            Enemies.Judgement.SetupJudgementPath.Hooks();
            Enemies.Judgement.AnointedSkins.Hooks();
            Items.LunarFlower.LunarFlowerItemBehaviour.Hooks();

            if (MoreStatsCompat.enabled)
            {
                MoreStatsCompat.Hooks();
            }
            else
            {
                ShamanStuff.Hooks();
            }
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (Configuration.Judgement.Judgement.Enabled.Value)
            {
                if (equipmentDef.equipmentIndex == Content.Equipment.MithrixHammer.equipmentIndex)
                {
                    return Equipment.MithrixHammer.MithrixHammer.EquipmentSlot_PerformEquipmentAction(orig, self, equipmentDef);
                }
                else if (equipmentDef.equipmentIndex == Content.Equipment.VoidlingWeapon.equipmentIndex)
                {
                    return Equipment.VoidlingWeapon.VoidlingWeapon.EquipmentSlot_PerformEquipmentAction(orig, self, equipmentDef);
                }
            }
            return orig(self, equipmentDef);
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            EnemiesReturns.Equipment.MithrixHammer.MithrixHammer.CharacterBody_onBodyInventoryChangedGlobal(body);
            EnemiesReturns.Equipment.VoidlingWeapon.VoidlingWeapon.CharacterBody_onBodyInventoryChangedGlobal(body);
            EnemiesReturns.Items.LynxFetish.LynxFetishFactory.CharacterBody_onBodyInventoryChangedGlobal(body);
            EnemiesReturns.Items.SpawnPillarOnChampionKill.SpawnPillarOnChampionKillFactory.CharacterBody_onBodyInventoryChangedGlobal(body);
            EnemiesReturns.Items.LunarFlower.LunarFlowerItemBehaviour.CharacterBody_onBodyInventoryChangedGlobal(body);
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
