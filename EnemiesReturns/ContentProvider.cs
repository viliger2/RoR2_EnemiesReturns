﻿using EnemiesReturns.Behaviors.Judgement.BrokenTeleporter;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Ifrit.Pillar;
using EnemiesReturns.Enemies.Judgement;
using EnemiesReturns.Enemies.LynxTribe;
using EnemiesReturns.Enemies.LynxTribe.Archer;
using EnemiesReturns.Enemies.LynxTribe.Hunter;
using EnemiesReturns.Enemies.LynxTribe.Scout;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using EnemiesReturns.Enemies.LynxTribe.Totem;
using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.LynxFetish;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using RoR2.Projectile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Rewired.Utils.Classes.Utility;
using System.Reflection;
using EnemiesReturns.Reflection;
using EnemiesReturns.Enemies.Judgement.Arraign;

namespace EnemiesReturns
{
    public class ContentProvider : IContentPackProvider
    {
        public string identifier => EnemiesReturnsPlugin.GUID + "." + nameof(ContentProvider);

        private readonly ContentPack _contentPack = new ContentPack();

        public const string AssetBundleName = "enemiesreturns";
        public const string AssetBundleStagesName = "enemiesreturnsstagesscenes";
        public const string AssetBundleStagesAssetsName = "enemiesreturnsstagesassets";
        public const string AssetBundleFolder = "AssetBundles";

        public const string SoundbankFolder = "Soundbanks";
        public const string SoundsSoundBankFileName = "EnemiesReturnsSounds";

        private readonly List<GameObject> bodyList = new List<GameObject>();
        private readonly List<GameObject> masterList = new List<GameObject>();
        private readonly List<Type> stateList = new List<Type>();
        private readonly List<SkillFamily> sfList = new List<SkillFamily>();
        private readonly List<SkillDef> sdList = new List<SkillDef>();
        private readonly List<EffectDef> effectsList = new List<EffectDef>();
        private readonly List<GameObject> projectilesList = new List<GameObject>();
        private readonly List<UnlockableDef> unlockablesList = new List<UnlockableDef>();
        private readonly List<ItemDef> itemList = new List<ItemDef>();
        private readonly List<NetworkSoundEventDef> nseList = new List<NetworkSoundEventDef>();
        private readonly List<GameObject> nopList = new List<GameObject>();
        private readonly List<BuffDef> bdList = new List<BuffDef>();

        public static readonly Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbedror2/base/shaders/hgstandard", "RoR2/Base/Shaders/HGStandard.shader"},
            {"stubbedror2/base/shaders/hgsnowtopped", "RoR2/Base/Shaders/HGSnowTopped.shader"},
            {"stubbedror2/base/shaders/hgtriplanarterrainblend", "RoR2/Base/Shaders/HGTriplanarTerrainBlend.shader"},
            {"stubbedror2/base/shaders/hgintersectioncloudremap", "RoR2/Base/Shaders/HGIntersectionCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgcloudremap", "RoR2/Base/Shaders/HGCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgopaquecloudremap", "RoR2/Base/Shaders/HGOpaqueCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgdistortion", "RoR2/Base/Shaders/HGDistortion.shader" },
            {"stubbedcalm water/calmwater - dx11 - doublesided", "Calm Water/CalmWater - DX11 - DoubleSided.shader" },
            {"stubbedcalm water/calmwater - dx11", "Calm Water/CalmWater - DX11.shader" },
            {"stubbednature/speedtree", "RoR2/Base/Shaders/SpeedTreeCustom.shader"},
            {"stubbeddecalicious/decaliciousdeferreddecal", "Decalicious/DecaliciousDeferredDecal.shader" },
            {"stubbedror2/base/shaders/hgdamagenumber", "RoR2/Base/Shaders/HGDamageNumber.shader" },
            {"stubbedror2/base/shaders/hguianimatealpha", "RoR2/Base/Shaders/HGUIAnimateAlpha.shader" }
        };

        public static Dictionary<string, Material> MaterialCache = new Dictionary<string, Material>(); //apparently you need it because reasons?

        public static ItemDef VoidMegaCrabItem = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/VoidMegaCrabItem.asset").WaitForCompletion();

        public struct MonsterCategories
        {
            public const string Champions = "Champions";
            public const string Minibosses = "Minibosses";
            public const string BasicMonsters = "Basic Monsters";
            public const string Special = "Special";
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(_contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            _contentPack.identifier = identifier;

            Content.ItemRelationshipProviders.ModdedContagiousItemProvider = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
            (Content.ItemRelationshipProviders.ModdedContagiousItemProvider as ScriptableObject).name = "EnemiesReturnsContagiousItemProvider";
            Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationshipType = Addressables.LoadAssetAsync<ItemRelationshipType>("RoR2/DLC1/Common/ContagiousItem.asset").WaitForCompletion();
            Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationships = Array.Empty<ItemDef.Pair>();

            Stopwatch segmentStopWatch = new Stopwatch();
            segmentStopWatch.Start();
            string soundbanksFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), SoundbankFolder);
            LoadSoundBanks(soundbanksFolderPath);
            segmentStopWatch.Stop();
            //TimeSpan ts = stopwatch.elapsedSeconds;
            Log.Info("Soundbanks loaded in " + segmentStopWatch.elapsedSeconds);

            string assetBundleFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), AssetBundleFolder);

            AssetBundle assetbundle = null;
            yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleName), args.progressReceiver, (resultAssetBundle) => assetbundle = resultAssetBundle);

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Material[]>)((assets) =>
            {
                SwapMaterial(assets);
            }));

            Dictionary<string, Sprite> iconLookup = new Dictionary<string, Sprite>();

            Dictionary<string, Texture2D> rampLookups = new Dictionary<string, Texture2D>();
            Texture2D texLavaCrackRound = null;
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Texture2D[]>)((textures) =>
            {
                texLavaCrackRound = textures.First(texture => texture.name == "texLavaCrackRound");
                rampLookups = textures.Where(texture => texture.name.StartsWith("texRamp")).ToDictionary(texture => texture.name, texture => texture);
            }));

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Sprite[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (var asset in assets)
                {
                    iconLookup.Add(asset.name, asset);
                }
                stopwatch.Stop();
                Log.Info("Icons loaded in " + stopwatch.elapsedSeconds);
            }));

            Dictionary<string, AnimationCurveDef> acdLookup = new Dictionary<string, AnimationCurveDef>();
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<AnimationCurveDef[]>)((assets) =>
            {
                acdLookup = assets.ToDictionary(item => item.name);
            }));

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                CreateSpitter(assets, iconLookup);

                CreateColossus(assets, iconLookup, acdLookup);

                CreateIfrit(assets, iconLookup, texLavaCrackRound, acdLookup);

                CreateMechanicalSpider(assets, iconLookup, acdLookup);

                CreateLynxTribe(assets, iconLookup, acdLookup, rampLookups);

                stopwatch.Stop();
                Log.Info("Characters created in " + stopwatch.elapsedSeconds);
            }));

            if (Configuration.Judgement.Enabled.Value)
            {
                AssetBundle assetBundleStagesAssets = null;
                yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleStagesAssetsName), args.progressReceiver, (resultAssetBundle) => assetBundleStagesAssets = resultAssetBundle);

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Sprite[]>)((assets) =>
                {
                    foreach (var asset in assets)
                    {
                        if(asset.name == "texAnointedSkinIcon")
                        {
                            Enemies.Judgement.SetupJudgementPath.AnointedSkinIcon = asset;
                        }
                        iconLookup.Add(asset.name, asset);
                    }
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Texture2D[]>)((textures) =>
                {
                    var thisbundleramps = textures.Where(texture => texture.name.StartsWith("texRamp")).ToDictionary(texture => texture.name, texture => texture);
                    foreach (var ramps in thisbundleramps)
                    {
                        rampLookups.Add(ramps.Key, ramps.Value);
                    }

                    Enemies.Judgement.SetupJudgementPath.aeonianEliteRamp = rampLookups["texRampAeonianElite"];
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<AnimationCurveDef[]>)((assets) =>
                {
                    zJunk.ModdedEntityStates.Judgement.Arraign.BaseSlashDash.speedCoefficientCurve = assets.First(acd => acd.name == "acdMoveSpeed").curve;

                    var acdOverlay = assets.First(acd => acd.name == "acdPreSlashMaterialAlpha").curve;
                    ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.PreSlash.acdOverlayAlpha = acdOverlay;
                    ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap.ExitSkyLeap.acdOverlayAlpha = acdOverlay;
                    ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash1.acdSlash1 = assets.First(acd => acd.name == "acdSlash1").curve;
                    ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash2.acdSlash2 = assets.First(acd => acd.name == "acdSlash2").curve;
                    ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.acdSlash3 = assets.First(acd => acd.name == "acdSlash3").curve;

                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash1.acdSlash1 = assets.First(acd => acd.name == "acdSlash1P2").curve;
                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash2.acdSlash2 = assets.First(acd => acd.name == "acdSlash2P2").curve;
                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash3.acdSlash3 = assets.First(acd => acd.name == "acdSlash3P2").curve;

                    ModdedEntityStates.Judgement.Arraign.Slide.BaseSlideState.speedCoefficientCurve = assets.First(acd => acd.name == "acdMithrixDash").curve;
                    foreach (var acd in assets)
                    {
                        acdLookup.Add(acd.name, acd);
                    }
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<EliteDef[]>)((assets) =>
                {
                    Content.Elites.Aeonian = assets.First(elitedef => elitedef.name == "EliteAeonian");
                    R2API.EliteRamp.AddRamp(Content.Elites.Aeonian, rampLookups["texRampAeonianElite"]);

                    _contentPack.eliteDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SceneDef[]>)((assets) =>
                {
                    _contentPack.sceneDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<BuffDef[]>)((assets) =>
                {
                    Content.Buffs.AffixAeoninan = assets.First(buff => buff.name == "bdAeonian");
                    Content.Buffs.ImmuneToHammer = assets.First(buff => buff.name == "bdImmuneToHammer");
                    Content.Buffs.ImmuneToAllDamageExceptHammer = assets.First(buff => buff.name == "bdImmuneToAllDamageExceptHammer");

                    _contentPack.buffDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Material[]>)((assets) =>
                {
                    SwapMaterial(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<GameObject[]>)((assets) =>
                {
                    _contentPack.bodyPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterBody>(out _)).ToArray());
                    _contentPack.masterPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterMaster>(out _)).ToArray());
                    _contentPack.projectilePrefabs.Add(assets.Where(asset => asset.TryGetComponent<ProjectileController>(out _)).ToArray());
                    _contentPack.effectDefs.Add(Array.ConvertAll(assets.Where(asset => asset.TryGetComponent<EffectComponent>(out _)).ToArray(), item => new EffectDef(item)));

                    var arraignStuff = new ArraignStuff();

                    SetupJudgementPath.JudgementInteractable = assets.First(asset => asset.name == "JudgementInteractable");
                    Behaviors.Judgement.WaveInteractable.JudgementSelectionController.modifiedPickerPanel = Enemies.Judgement.SetupJudgementPath.CloneOptionPickerPanel();
                    nopList.Add(SetupJudgementPath.JudgementInteractable);

                    SetupJudgementPath.PileOfDirt = assets.First(asset => asset.name == "PileOfDirtInteractable");
                    SetupJudgementPath.PileOfDirt = SetupJudgementPath.SetupLunarKey(SetupJudgementPath.PileOfDirt);
                    nopList.Add(SetupJudgementPath.PileOfDirt);

                    SetupJudgementPath.BrokenTeleporter = assets.First(asset => asset.name == "BrokenTeleporterInteractable");
                    SetupJudgementPath.BrokenTeleporter = SetupJudgementPath.SetupBrokenTeleporter(SetupJudgementPath.BrokenTeleporter);
                    nopList.Add(SetupJudgementPath.BrokenTeleporter);

                    var judgementTeleporter = assets.First(asset => asset.name == "PortalJudgement");
                    nopList.Add(judgementTeleporter);

                    Equipment.MithrixHammer.MithrixHammer.MithrixHammerController = assets.First(asset => asset.name == "MithrixHammerController");
                    nopList.Add(Equipment.MithrixHammer.MithrixHammer.MithrixHammerController);

                    ModdedEntityStates.Judgement.Mission.Phase2.speechControllerPrefab = assets.First(asset => asset.name == "ArraignP1SpeechController");
                    ModdedEntityStates.Judgement.Mission.Phase3.speechControllerPrefab = assets.First(asset => asset.name == "ArraignP2SpeechController");

                    ModdedEntityStates.Judgement.MithrixHammer.Fire.swingEffect = assets.First(asset => asset.name == "MithrixHammerSwingEffect");
                    ModdedEntityStates.Judgement.MithrixHammer.Fire.swingEffect = Equipment.MithrixHammer.MithrixHammer.SetupEffectMaterials(ModdedEntityStates.Judgement.MithrixHammer.Fire.swingEffect);

                    var explosionEffect = assets.First(asset => asset.name == "ArraignRemoveSwordEffect");
                    explosionEffect = arraignStuff.CreateSkyLeapRemoveSwordEffect(explosionEffect);
                    ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap.ExitSkyLeap.staticSecondAttackEffect = explosionEffect;
                    ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap.ExitSkyLeap.staticSecondAttackEffect = explosionEffect;

                    var waveProjectile = assets.First(asset => asset.name == "ArraignSlash3Wave");
                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash3.waveProjectile = waveProjectile;
                    ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap.ExitSkyLeap.waveProjectile = waveProjectile;

                    //var lightningProjectile = Enemies.Judgement.Arraign.ArraignStuff.SetupLightningStrikePrefab(assets.First(asset => asset.name == "ArraignPreLightningProjectile"));
                    var lightningProjectile = assets.First(asset => asset.name == "ArraignPreLightningProjectile");
                    ModdedEntityStates.Judgement.Arraign.Phase1.LightningStrikes.projectilePrefab = lightningProjectile;
                    ModdedEntityStates.Judgement.Arraign.Phase2.LeapingDash.LeapDash.projectilePrefab = lightningProjectile;

                    ModdedEntityStates.Judgement.Arraign.Phase2.ClockAttack.projectilePrefab = assets.First(asset => asset.name == "ArraignPreClockAttackProjectile");

                    ModdedEntityStates.Judgement.Arraign.Phase1.WeaponThrow.staticProjectilePrefab = assets.First(asset => asset.name == "ArraignSwordProjectile");
                    ModdedEntityStates.Judgement.Arraign.Phase2.SpearThrow.staticProjectilePrefab = assets.First(asset => asset.name == "ArraignSpearProjectile");

                    ModdedEntityStates.Judgement.Arraign.Phase2.ClockAttack.effectPrefab = assets.First(asset => asset.name == "ClockZoneEffect");

                    ModdedEntityStates.Judgement.Arraign.BaseSkyLeap.BaseHoldSkyLeap.dropEffectPrefab = arraignStuff.CreateSkyLeapDropPositionEffect(assets.First(asset => asset.name == "DropPositionEffect"));

                    ModdedEntityStates.Judgement.Arraign.Phase1.SwordBeam.SwordBeamStart.postProccessBeam = assets.First(asset => asset.name == "BeamPostProccess");
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ItemDef[]>)((assets) =>
                {
                    Content.Items.TradableRock = assets.First(item => item.name == "TradableRock");
                    Content.Items.TradableRock.pickupModelPrefab = SetupJudgementPath.SetupLunarKey(Content.Items.TradableRock.pickupModelPrefab);

                    Content.Items.LunarFlower = assets.First(item => item.name == "LunarFlower");
                    Content.Items.LunarFlower.pickupModelPrefab = SetupJudgementPath.SetupLunarFlower(Content.Items.LunarFlower.pickupModelPrefab);

                    _contentPack.itemDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<GameEndingDef[]>)((assets) =>
                {
                    Content.GameEndings.SurviveJudgement = assets.First(item => item.cachedName == "SurviveJudgement");

                    _contentPack.gameEndingDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ItemTierDef[]>)((assets) =>
                {
                    Content.ItemTiers.HiddenInLogbook = assets.First(item => item.name == "HiddenInLogbook");
                    Content.ItemTiers.HiddenInLogbook.highlightPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HighlightTier1Item.prefab").WaitForCompletion();
                    Content.ItemTiers.HiddenInLogbook.dropletDisplayPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/BossOrb.prefab").WaitForCompletion();
                    _contentPack.itemTierDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<EquipmentDef[]>)((assets) =>
                {
                    Content.Equipment.MithrixHammer = assets.First(equipment => equipment.name == "MithrixHammer");
                    Content.Equipment.MithrixHammer.pickupModelPrefab = Equipment.MithrixHammer.MithrixHammer.SetupPickupDisplay(Content.Equipment.MithrixHammer.pickupModelPrefab);

                    Content.Equipment.EliteAeonian = assets.First(equipment => equipment.name == "EliteAeonianEquipment");
                    _contentPack.equipmentDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<CharacterSpawnCard[]>)((assets) =>
                {
                    ModdedEntityStates.Judgement.Arraign.Phase2.SummonSkyLasers.cscSkyLaser = assets.First(asset => asset.name == "cscSkyLaser");
                    ModdedEntityStates.Judgement.Mission.Phase3.cscArraignHaunt = assets.First(asset => asset.name == "cscArraignHaunt");
                }));

                AssetBundle assetBundleStages = null;
                yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleStagesName), args.progressReceiver, (resultAssetBundle) => assetBundleStages = resultAssetBundle);

                CreateJudgement();
            }

            var entityStateTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && !type.IsInterface && type.GetCustomAttribute<RegisterEntityState>(false) != null).ToArray();
#if DEBUG || NOWEAVER
            foreach (var type in entityStateTypes)
            {
                Log.Info($"Found type {type} when searching for RegisterEntityState attribute.");
            }
#endif
            _contentPack.entityStateTypes.Add(entityStateTypes);
            _contentPack.bodyPrefabs.Add(bodyList.ToArray());
            _contentPack.masterPrefabs.Add(masterList.ToArray());
            _contentPack.skillDefs.Add(sdList.ToArray());
            _contentPack.skillFamilies.Add(sfList.ToArray());
            _contentPack.entityStateTypes.Add(stateList.ToArray());
            _contentPack.effectDefs.Add(effectsList.ToArray());
            _contentPack.projectilePrefabs.Add(projectilesList.ToArray());
            _contentPack.unlockableDefs.Add(unlockablesList.ToArray());
            _contentPack.itemDefs.Add(itemList.ToArray());
            _contentPack.networkSoundEventDefs.Add(nseList.ToArray());
            _contentPack.networkedObjectPrefabs.Add(nopList.ToArray());
            _contentPack.itemRelationshipProviders.Add(new[] { Content.ItemRelationshipProviders.ModdedContagiousItemProvider });
            _contentPack.buffDefs.Add(bdList.ToArray());
            totalStopwatch.Stop();
            Log.Info("Total loading time: " + totalStopwatch.elapsedSeconds);

            yield break;

            void SwapMaterial(Material[] assets)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var materials = assets;

                if (materials != null)
                {
                    foreach (Material material in materials)
                    {
                        if(!ShaderLookup.TryGetValue(material.shader.name.ToLower(), out var matName))
                        {
                            Log.Info($"Couldn't find replacement shader for {material.shader.name.ToLower()} in dictionary.");
                            continue;
                        }
                        var replacementShader = Addressables.LoadAssetAsync<Shader>(matName).WaitForCompletion();
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                        }
                        else
                        {
                            Log.Info("Couldn't find replacement shader for " + material.shader.name.ToLower());
                        }
                        MaterialCache.Add(material.name, material);
                    }
                }
                stopwatch.Stop();
                Log.Info("Materials swapped in " + stopwatch.elapsedSeconds);
            }
        }

        #region Content

        private void CreateIfrit(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Texture2D texLavaCrackRound, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            CreateIfritPillar(assets, iconLookup, acdLookup);
            ExplicitPickupDropTable dtIfrit = CreateIfritItem(assets, iconLookup);

            if (EnemiesReturns.Configuration.Ifrit.Enabled.Value)
            {
                var ifritStuff = new IfritStuff();

                ModdedEntityStates.Ifrit.DeathState.deathEffect = ifritStuff.CreateDeathEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.DeathState.deathEffect));

                ModdedEntityStates.Ifrit.SummonPylon.screamPrefab = ifritStuff.CreateBreathParticle();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.SummonPylon.screamPrefab));

                var nsedHellzoneRockFire = Utils.CreateNetworkSoundDef("ER_Ifrit_Hellzone_Rock_Play");
                nseList.Add(nsedHellzoneRockFire);

                var ifritSpawnEffect = assets.First(effect => effect.name == "IfritSpawnPortal");
                ModdedEntityStates.Ifrit.SpawnState.spawnEffect = ifritStuff.CreateSpawnEffect(ifritSpawnEffect, acdLookup["acdPortalPP"]);
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.SpawnState.spawnEffect));

                var ifritHellzonePillarProjectile = assets.First(projectile => projectile.name == "IfritHellzonePillarProjectile");
                var ifritHellzonePillarProjectileGhost = assets.First(projectile => projectile.name == "IfritHellzonePillarProjectileGhost");
                var IfritHellzoneVolcanoEffect = assets.First(projectile => projectile.name == "IfritHellzoneVolcanoEffect");
                var pillarProjectile = ifritStuff.CreateHellzonePillarProjectile(ifritHellzonePillarProjectile, ifritHellzonePillarProjectileGhost);
                var dotZoneProjectile = ifritStuff.CreateHellfireDotZoneProjectile(pillarProjectile, IfritHellzoneVolcanoEffect, texLavaCrackRound, nsedHellzoneRockFire);
                var hellzoneProjectile = ifritStuff.CreateHellzoneProjectile();
                var preProjectile = ifritStuff.CreateHellzonePredictionProjectile(dotZoneProjectile, texLavaCrackRound);

                projectilesList.Add(dotZoneProjectile);
                projectilesList.Add(hellzoneProjectile);
                projectilesList.Add(pillarProjectile);
                projectilesList.Add(preProjectile);

                ModdedEntityStates.Ifrit.FlameCharge.FlameCharge.flamethrowerEffectPrefab = ifritStuff.CreateFlameBreath();
                ModdedEntityStates.Ifrit.Hellzone.FireHellzoneFire.projectilePrefab = hellzoneProjectile;
                ModdedEntityStates.Ifrit.Hellzone.FireHellzoneFire.dotZoneProjectile = preProjectile;

                var ifritBody = new IfritBody();

                IfritBody.Skills.Hellzone = ifritBody.CreateHellzoneSkill();
                IfritBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("IfritSecondaryFamily", IfritBody.Skills.Hellzone);

                IfritBody.Skills.SummonPylon = ifritBody.CreateSummonPylonSkill();
                IfritBody.SkillFamilies.Special = Utils.CreateSkillFamily("IfritSpecialFamily", IfritBody.Skills.SummonPylon);

                IfritBody.Skills.FlameCharge = ifritBody.CreateFlameChargeSkill();
                IfritBody.SkillFamilies.Utility = Utils.CreateSkillFamily("IfritUtilityFamily", IfritBody.Skills.FlameCharge);

                IfritBody.Skills.Smash = ifritBody.CreateSmashSkill();
                IfritBody.SkillFamilies.Primary = Utils.CreateSkillFamily("IfritPrimaryFamily", IfritBody.Skills.Smash);

                var ifritLog = Utils.CreateUnlockableDef("Logs.IfritBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_IFRIT");
                unlockablesList.Add(ifritLog);

                IfritBody.BodyPrefab = ifritBody.AddBodyComponents(assets.First(body => body.name == "IfritBody"), iconLookup["texIconIfritBody"], ifritLog, dtIfrit);
                bodyList.Add(IfritBody.BodyPrefab);

                var ifritManePrefab = assets.First(mane => mane.name == "IfritManeFireParticle");
                ifritManePrefab.GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matIfritManeFire", ifritBody.CreateManeFiresMaterial);

                var ifritMaster = new IfritMaster();
                IfritMaster.MasterPrefab = ifritMaster.AddMasterComponents(assets.First(master => master.name == "IfritMaster"), IfritBody.BodyPrefab);
                masterList.Add(IfritMaster.MasterPrefab);

                IfritBody.SpawnCards.cscIfritDefault = ifritBody.CreateCard("cscIfritDefault", IfritMaster.MasterPrefab, IfritBody.SkinDefs.Default, IfritBody.BodyPrefab);
                var dcIfritDefault = new DirectorCard
                {
                    spawnCard = IfritBody.SpawnCards.cscIfritDefault,
                    selectionWeight = EnemiesReturns.Configuration.Ifrit.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Ifrit.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchIfritDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcIfritDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Ifrit.DefaultStageList.Value, dchIfritDefault);

                if (EnemiesReturns.Configuration.Ifrit.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
                {
                    ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(IfritBody.SpawnCards.cscIfritDefault, 2);
                }
            }
        }

        private ExplicitPickupDropTable CreateIfritItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtIfrit = null;
            if (EnemiesReturns.Configuration.Ifrit.ItemEnabled.Value)
            {
                var pillarItemFactory = new SpawnPillarOnChampionKillFactory();

                Content.Items.SpawnPillarOnChampionKill = pillarItemFactory.CreateItem(assets.First(item => item.name == "IfritItem"), iconLookup["texIconIfritItem"]);
                itemList.Add(Content.Items.SpawnPillarOnChampionKill);

                dtIfrit = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtIfrit as ScriptableObject).name = "epdtIfrit";
                dtIfrit.canDropBeReplaced = true;
                dtIfrit.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = Content.Items.SpawnPillarOnChampionKill
                        }
                };
                HG.ArrayUtils.ArrayAppend(ref Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = Content.Items.SpawnPillarOnChampionKill, itemDef2 = VoidMegaCrabItem });
            }

            return dtIfrit;
        }

        private void CreateIfritPillar(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (EnemiesReturns.Configuration.Ifrit.ItemEnabled.Value || EnemiesReturns.Configuration.Ifrit.Enabled.Value)
            {
                var pillarStuff = new PillarStuff();

                ModdedEntityStates.Ifrit.Pillar.SpawnState.burrowPrefab = pillarStuff.CreateSpawnEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.SpawnState.burrowPrefab));

                ModdedEntityStates.Ifrit.Pillar.BaseDeathState.fallEffect = pillarStuff.CreateDeathFallEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.BaseDeathState.fallEffect));

                var explosionEffect = pillarStuff.CreateExlosionEffectAlt();
                ModdedEntityStates.Ifrit.Pillar.BaseFireExplosion.explosionPrefab = explosionEffect;
                ModdedEntityStates.Ifrit.Pillar.Enemy.KilledDeathState.explosionPrefab = explosionEffect;
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.BaseFireExplosion.explosionPrefab));

                var pylonBody = assets.First(body => body.name == "IfritPylonBody");
                var pylonMaster = assets.First(master => master.name == "IfritPylonMaster");

                ModdedEntityStates.Ifrit.Pillar.Enemy.KilledDeathState.fireballYCurve = acdLookup["acdFireballFallCurve"].curve;

                PillarStuff.PillarExplosion = ProcTypeAPI.ReserveProcType();

                var pillarMaster = new PillarMaster();

                if (EnemiesReturns.Configuration.Ifrit.Enabled.Value)
                {
                    var pillarEnemyBody = new PillarEnemyBody();
                    PillarEnemyBody.BodyPrefab = pillarEnemyBody.AddBodyComponents(pylonBody.InstantiateClone("IfritPylonEnemyBody", false), iconLookup["texIconPillarEnemy"], acdLookup);
                    bodyList.Add(PillarEnemyBody.BodyPrefab);

                    PillarMaster.EnemyMasterPrefab = pillarMaster.AddMasterComponents(pylonMaster.InstantiateClone("IfritPylonEnemyMaster", false), PillarEnemyBody.BodyPrefab);
                    masterList.Add(PillarMaster.EnemyMasterPrefab);

                    PillarEnemyBody.SpawnCard = pillarEnemyBody.CreateCard("cscIfritEnemyPillar", PillarMaster.EnemyMasterPrefab);
                }

                if (EnemiesReturns.Configuration.Ifrit.ItemEnabled.Value)
                {
                    var pillarAllyBody = new PillarAllyBody();

                    PillarAllyBody.BodyPrefab = pillarAllyBody.AddBodyComponents(pylonBody.InstantiateClone("IfritPylonPlayerBody", false), iconLookup["texIconPillarAlly"], acdLookup);
                    bodyList.Add(PillarAllyBody.BodyPrefab);

                    PillarMaster.AllyMasterPrefab = pillarMaster.AddMasterComponents(pylonMaster.InstantiateClone("IfritPylonPlayerMaster", false), PillarAllyBody.BodyPrefab);
                    masterList.Add(PillarMaster.AllyMasterPrefab);

                    PillarAllyBody.SpawnCard = pillarAllyBody.CreateCard("cscIfritPlayerPillar", PillarMaster.AllyMasterPrefab);
                }
            }
        }

        private void CreateColossus(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            ExplicitPickupDropTable dtColossus = CreateColossusItem(assets, iconLookup);

            if (EnemiesReturns.Configuration.Colossus.Enabled.Value)
            {
                var colossusStuff = new ColossusStuff();

                var stompEffect = colossusStuff.CreateStompEffect();
                ModdedEntityStates.Colossus.Stomp.StompBase.stompEffectPrefab = stompEffect;
                effectsList.Add(new EffectDef(stompEffect));

                var stompProjectile = colossusStuff.CreateStompProjectile();
                ModdedEntityStates.Colossus.Stomp.StompBase.projectilePrefab = stompProjectile;
                projectilesList.Add(stompProjectile);

                var deathFallEffect = colossusStuff.CreateDeathFallEffect();
                effectsList.Add(new EffectDef(deathFallEffect));
                ModdedEntityStates.Colossus.Death.Death2.fallEffect = deathFallEffect;

                var death2Effect = colossusStuff.CreateDeath2Effect();
                effectsList.Add(new EffectDef(death2Effect));
                ModdedEntityStates.Colossus.Death.Death2.deathEffect = death2Effect;

                var clapEffect = colossusStuff.CreateClapEffect();
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.clapEffect = clapEffect;

                var flyingRockGhost = colossusStuff.CreateFlyingRocksGhost();
                FloatingRocksController.flyingRockPrefab = flyingRockGhost;
                var flyingRockProjectile = colossusStuff.CreateFlyingRockProjectile(flyingRockGhost);
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.projectilePrefab = flyingRockProjectile;
                projectilesList.Add(flyingRockProjectile);

                var laserBarrageProjectile = colossusStuff.CreateLaserBarrageProjectile();
                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.projectilePrefab = laserBarrageProjectile;
                projectilesList.Add(laserBarrageProjectile);

                var spawnEffect = colossusStuff.CreateSpawnEffect();
                ModdedEntityStates.Colossus.SpawnState.burrowPrefab = spawnEffect;
                effectsList.Add(new EffectDef(spawnEffect));

                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.intencityGraph = acdLookup["LaserBarrageLightIntencity"].curve;

                var colossusLog = Utils.CreateUnlockableDef("Logs.ColossusBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_COLOSSUS");
                unlockablesList.Add(colossusLog);

                var laserEffect = colossusStuff.CreateLaserEffect();
                Junk.ModdedEntityStates.Colossus.HeadLaser.HeadLaserAttack.beamPrefab = laserEffect;

                var colossusBody = new ColossusBody();
                ColossusBody.Skills.Stomp = colossusBody.CreateStompSkill();
                ColossusBody.Skills.StoneClap = colossusBody.CreateStoneClapSkill();
                ColossusBody.Skills.LaserBarrage = colossusBody.CreateLaserBarrageSkill();
                ColossusBody.Skills.HeadLaser = colossusBody.CreateHeadLaserSkill();
                sdList.Add(ColossusBody.Skills.Stomp);
                sdList.Add(ColossusBody.Skills.StoneClap);
                sdList.Add(ColossusBody.Skills.LaserBarrage);
                sdList.Add(ColossusBody.Skills.HeadLaser);

                ColossusBody.SkillFamilies.Primary = Utils.CreateSkillFamily("ColossusPrimaryFamily", ColossusBody.Skills.Stomp);
                ColossusBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("ColossusSecondaryFamily", ColossusBody.Skills.StoneClap);
                ColossusBody.SkillFamilies.Utility = Utils.CreateSkillFamily("ColossusUtilityFamily", ColossusBody.Skills.LaserBarrage);
                ColossusBody.SkillFamilies.Special = Utils.CreateSkillFamily("ColossusSpecialFamily", ColossusBody.Skills.HeadLaser);
                sfList.Add(ColossusBody.SkillFamilies.Primary);
                sfList.Add(ColossusBody.SkillFamilies.Secondary);
                sfList.Add(ColossusBody.SkillFamilies.Utility);
                sfList.Add(ColossusBody.SkillFamilies.Special);

                ColossusBody.BodyPrefab = colossusBody.AddBodyComponents(assets.First(body => body.name == "ColossusBody"), iconLookup["texColossusIcon"], colossusLog, dtColossus);
                bodyList.Add(ColossusBody.BodyPrefab);

                var colossusMaster = new ColossusMaster();
                ColossusMaster.MasterPrefab = colossusMaster.AddMasterComponents(assets.First(master => master.name == "ColossusMaster"), ColossusBody.BodyPrefab);
                masterList.Add(ColossusMaster.MasterPrefab);

                ColossusBody.SpawnCards.cscColossusDefault = colossusBody.CreateCard("cscColossusDefault", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Default, ColossusBody.BodyPrefab);
                DirectorCard dcColossusDefault = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusDefault,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusDefault, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion());
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Colossus.DefaultStageList.Value, dchColossusDefault);

                ColossusBody.SpawnCards.cscColossusSkyMeadow = colossusBody.CreateCard("cscColossusSkyMeadow", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.SkyMeadow, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSkyMeadow = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSkyMeadow,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSkyMeadow = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSkyMeadow,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Colossus.SkyMeadowStageList.Value, dchColossusSkyMeadow);

                ColossusBody.SpawnCards.cscColossusGrassy = colossusBody.CreateCard("cscColossusGrassy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Grassy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusGrassy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusGrassy,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusGrassy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusGrassy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusGrassy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilyNature").WaitForCompletion());
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Colossus.GrassyStageList.Value, dchColossusGrassy);

                ColossusBody.SpawnCards.cscColossusCastle = colossusBody.CreateCard("cscColossusCastle", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Castle, ColossusBody.BodyPrefab);
                DirectorCard dcColossusCastle = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusCastle,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusCastle = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusCastle,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Colossus.CastleStageList.Value, dchColossusCastle);

                ColossusBody.SpawnCards.cscColossusSandy = colossusBody.CreateCard("cscColossusSandy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Sandy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSandy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSandy,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSandy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSandy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusSandy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySandy.asset").WaitForCompletion());
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Colossus.SandyStageList.Value, dchColossusSandy);

                ColossusBody.SpawnCards.cscColossusSnowy = colossusBody.CreateCard("cscColossusSnowy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Snowy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSnowy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSnowy,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSnowy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSnowy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusSnowy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySnowy.asset").WaitForCompletion());
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Colossus.SnowyStageList.Value, dchColossusSnowy);

                if (EnemiesReturns.Configuration.Colossus.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
                {
                    ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(ColossusBody.SpawnCards.cscColossusGrassy, 3);
                }
            }
        }

        private ExplicitPickupDropTable CreateColossusItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtColossus = null;

            if (EnemiesReturns.Configuration.Colossus.ItemEnabled.Value)
            {
                Items.ColossalKnurl.ColossalKnurlFactory.ColossalFist = ProcTypeAPI.ReserveProcType();

                var knurlFactory = new ColossalKnurlFactory();

                EnemiesReturns.Content.Items.ColossalCurl = knurlFactory.CreateItem(assets.First(item => item.name == "PickupColossalCurl"), iconLookup["texColossalKnurlIcon"]);
                itemList.Add(EnemiesReturns.Content.Items.ColossalCurl);

                dtColossus = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtColossus as ScriptableObject).name = "epdtColossus";
                dtColossus.canDropBeReplaced = true;
                dtColossus.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = EnemiesReturns.Content.Items.ColossalCurl
                        }
                };

                var knurlProjectileGhost = assets.First(item => item.name == "ColossalKnurlFistProjectileGhost");
                knurlProjectileGhost = knurlFactory.CreateFistGhostPrefab(knurlProjectileGhost);

                var knurlProjectile = assets.First(item => item.name == "ColossalKnurlFistProjectile");
                ColossalKnurlFactory.projectilePrefab = knurlFactory.CreateFistProjectile(knurlProjectile, knurlProjectileGhost);

                projectilesList.Add(ColossalKnurlFactory.projectilePrefab);

                HG.ArrayUtils.ArrayAppend(ref Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = EnemiesReturns.Content.Items.ColossalCurl, itemDef2 = VoidMegaCrabItem });
            }

            return dtColossus;
        }

        private void CreateSpitter(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            if (EnemiesReturns.Configuration.Spitter.Enabled.Value)
            {
                var spitterStuff = new SpitterStuff();
                ModdedEntityStates.Spitter.Bite.biteEffectPrefab = spitterStuff.CreateBiteEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Spitter.Bite.biteEffectPrefab));

                var chargedSpitSmallDoTZone = spitterStuff.CreatedChargedSpitSmallDoTZone();
                var chargedSpitDoTZone = spitterStuff.CreateChargedSpitDoTZone();
                var chargedSpitChunkProjectile = spitterStuff.CreateChargedSpitSplitProjectile(chargedSpitSmallDoTZone);
                var chargedSpitProjectile = spitterStuff.CreateChargedSpitProjectile(chargedSpitDoTZone, chargedSpitChunkProjectile); ;
                ModdedEntityStates.Spitter.FireChargedSpit.projectilePrefab = chargedSpitProjectile;
                projectilesList.Add(chargedSpitProjectile);
                projectilesList.Add(chargedSpitSmallDoTZone);
                projectilesList.Add(chargedSpitDoTZone);
                projectilesList.Add(chargedSpitChunkProjectile);

                Junk.ModdedEntityStates.Spitter.NormalSpit.normalSpitProjectile = spitterStuff.CreateNormalSpitProjectile();
                projectilesList.Add(Junk.ModdedEntityStates.Spitter.NormalSpit.normalSpitProjectile);

                var spitterLog = Utils.CreateUnlockableDef("Logs.SpitterBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SPITTER");
                unlockablesList.Add(spitterLog);

                var spitterBody = new SpitterBody();
                SpitterBody.Skills.NormalSpit = spitterBody.CreateNormalSpitSkill();
                SpitterBody.Skills.ChargedSpit = spitterBody.CreateChargedSpitSkill();
                SpitterBody.Skills.Bite = spitterBody.CreateBiteSkill();

                sdList.Add(SpitterBody.Skills.NormalSpit);
                sdList.Add(SpitterBody.Skills.Bite);
                sdList.Add(SpitterBody.Skills.ChargedSpit);

                SpitterBody.SkillFamilies.Primary = Utils.CreateSkillFamily("SpitterPrimaryFamily", SpitterBody.Skills.NormalSpit);
                SpitterBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("SpitterSecondaryFamily", SpitterBody.Skills.Bite);
                SpitterBody.SkillFamilies.Special = Utils.CreateSkillFamily("SpitterSpecialFamily", SpitterBody.Skills.ChargedSpit);

                sfList.Add(SpitterBody.SkillFamilies.Primary);
                sfList.Add(SpitterBody.SkillFamilies.Secondary);
                sfList.Add(SpitterBody.SkillFamilies.Special);

                SpitterBody.BodyPrefab = spitterBody.AddBodyComponents(assets.First(body => body.name == "SpitterBody"), iconLookup["texSpitterIcon"], spitterLog);
                bodyList.Add(SpitterBody.BodyPrefab);

                SpitterMaster.MasterPrefab = new SpitterMaster().AddMasterComponents(assets.First(master => master.name == "SpitterMaster"), SpitterBody.BodyPrefab);
                masterList.Add(SpitterMaster.MasterPrefab);

                SpitterBody.SpawnCards.cscSpitterDefault = spitterBody.CreateCard("cscSpitterDefault", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Default, SpitterBody.BodyPrefab);
                var dcSpitterDefault = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterDefault,
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Spitter.DefaultStageList.Value, dchSpitterDefault);

                SpitterBody.SpawnCards.cscSpitterLakes = spitterBody.CreateCard("cscSpitterLakes", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Lakes, SpitterBody.BodyPrefab);
                var dcSpitterLakes = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterLakes,
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterLakes = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterLakes,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Spitter.LakesStageList.Value, dchSpitterLakes);

                SpitterBody.SpawnCards.cscSpitterSulfur = spitterBody.CreateCard("cscSpitterSulfur", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Sulfur, SpitterBody.BodyPrefab);
                var dcSpitterSulfur = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterSulfur,
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterSulfur = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterSulfur,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Spitter.SulfurStageList.Value, dchSpitterSulfur);

                SpitterBody.SpawnCards.cscSpitterDepths = spitterBody.CreateCard("cscSpitterDepths", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Depths, SpitterBody.BodyPrefab);
                var dcSpitterDepth = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterDepths,
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterDepths = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDepth,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.Spitter.DepthStageList.Value, dchSpitterDepths);
                if (EnemiesReturns.Configuration.Spitter.HelminthroostReplaceMushrum.Value)
                {
                    DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.MiniMushrum, DirectorAPI.Stage.HelminthHatchery);
                }
            }
        }

        private void CreateMechanicalSpider(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (EnemiesReturns.Configuration.MechanicalSpider.Enabled.Value)
            {
                var spiderLog = Utils.CreateUnlockableDef("Logs.MechanicalSpiderBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_MECHANICAL_SPIDER");
                unlockablesList.Add(spiderLog);

                var spiderStuff = new MechanicalSpiderStuff();

                var doubleShotEffect = spiderStuff.CreateDoubleShotImpactEffect();
                effectsList.Add(new EffectDef(doubleShotEffect));

                ModdedEntityStates.MechanicalSpider.DoubleShot.Fire.projectilePrefab = spiderStuff.CreateDoubleShotProjectilePrefab(doubleShotEffect);
                projectilesList.Add(ModdedEntityStates.MechanicalSpider.DoubleShot.Fire.projectilePrefab);

                ModdedEntityStates.MechanicalSpider.DoubleShot.ChargeFire.effectPrefab = spiderStuff.CreateDoubleShotChargeEffect();

                var spiderEnemyBody = new MechanicalSpiderEnemyBody();
                MechanicalSpiderEnemyBody.Skills.DoubleShot = spiderEnemyBody.CreateDoubleShotSkill();
                MechanicalSpiderEnemyBody.Skills.Dash = spiderEnemyBody.CreateDashSkill();

                ModdedEntityStates.MechanicalSpider.Dash.Dash.forwardSpeedCoefficientCurve = acdLookup["acdSpiderDash"].curve;

                sdList.Add(MechanicalSpiderEnemyBody.Skills.DoubleShot);
                sdList.Add(MechanicalSpiderEnemyBody.Skills.Dash);

                MechanicalSpiderEnemyBody.SkillFamilies.Primary = Utils.CreateSkillFamily("MechanicalSpiderPrimaryFamily", MechanicalSpiderEnemyBody.Skills.DoubleShot);
                MechanicalSpiderEnemyBody.SkillFamilies.Utility = Utils.CreateSkillFamily("MechanicalSpiderUtilityFamily", MechanicalSpiderEnemyBody.Skills.Dash);

                sfList.Add(MechanicalSpiderEnemyBody.SkillFamilies.Primary);
                sfList.Add(MechanicalSpiderEnemyBody.SkillFamilies.Utility);

                CreateMechanicalSpiderEnemy(assets, iconLookup, spiderLog, spiderEnemyBody);

                CreateMechanichalSpiderDrone(assets, iconLookup, spiderStuff);
            }
        }

        private void CreateMechanichalSpiderDrone(GameObject[] assets, Dictionary<string, Sprite> iconLookup, MechanicalSpiderStuff spiderStuff)
        {
            var spiderAllyBody = new MechanicalSpiderDroneBody();
            MechanicalSpiderDroneBody.BodyPrefab = spiderAllyBody.AddBodyComponents(assets.First(body => body.name == "MechanicalSpiderDroneBody"), iconLookup["texMechanicalSpiderAllyIcon"]);
            bodyList.Add(MechanicalSpiderDroneBody.BodyPrefab);

            var spiderAllyMaster = new MechanicalSpiderDroneMaster();
            MechanicalSpiderDroneMaster.MasterPrefab = spiderAllyMaster.AddMasterComponents(assets.First(master => master.name == "MechanicalSpiderDroneMaster"), MechanicalSpiderDroneBody.BodyPrefab);
            masterList.Add(MechanicalSpiderDroneMaster.MasterPrefab);

            MechanicalSpiderStuff.InteractablePrefab = spiderStuff.CreateInteractable(assets.First(interactable => interactable.name == "MechanicalSpiderBroken"), MechanicalSpiderDroneMaster.MasterPrefab);
            nopList.Add(MechanicalSpiderStuff.InteractablePrefab);

            MechanicalSpiderStuff.SpawnCards.iscMechanicalSpiderBroken = spiderStuff.CreateInteractableSpawnCard("iscMechanicalSpiderBroken", MechanicalSpiderStuff.InteractablePrefab);
        }

        private void CreateMechanicalSpiderEnemy(GameObject[] assets, Dictionary<string, Sprite> iconLookup, UnlockableDef spiderLog, MechanicalSpiderEnemyBody spiderEnemyBody)
        {
            MechanicalSpiderEnemyBody.BodyPrefab = spiderEnemyBody.AddBodyComponents(assets.First(body => body.name == "MechanicalSpiderBody"), iconLookup["texMechanicalSpiderEnemyIcon"], spiderLog);
            bodyList.Add(MechanicalSpiderEnemyBody.BodyPrefab);

            var spiderEnemyMaster = new MechanicalSpiderEnemyMaster();
            MechanicalSpiderEnemyMaster.MasterPrefab = spiderEnemyMaster.AddMasterComponents(assets.First(master => master.name == "MechanicalSpiderMaster"), MechanicalSpiderEnemyBody.BodyPrefab);
            masterList.Add(MechanicalSpiderEnemyMaster.MasterPrefab);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault = spiderEnemyBody.CreateCard("cscMechanicalSpiderDefault", MechanicalSpiderEnemyMaster.MasterPrefab);
            var dcMechanicalSpiderDefault = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault,
                selectionWeight = EnemiesReturns.Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(EnemiesReturns.Configuration.MechanicalSpider.DefaultStageList.Value, dchMechanicalSpiderDefault);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy = spiderEnemyBody.CreateCard("cscMechanicalSpiderGrassy", MechanicalSpiderEnemyMaster.MasterPrefab, MechanicalSpiderEnemyBody.SkinDefs.Grassy, MechanicalSpiderEnemyMaster.MasterPrefab);
            var dcMechanicalSpiderGrassy = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy,
                selectionWeight = EnemiesReturns.Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderGrassy = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderGrassy,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(EnemiesReturns.Configuration.MechanicalSpider.GrassyStageList.Value, dchMechanicalSpiderGrassy);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy = spiderEnemyBody.CreateCard("cscMechanicalSpiderSnowy", MechanicalSpiderEnemyMaster.MasterPrefab, MechanicalSpiderEnemyBody.SkinDefs.Snowy, MechanicalSpiderEnemyMaster.MasterPrefab);
            var dcMechanicalSpiderSnowy = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy,
                selectionWeight = EnemiesReturns.Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderSnowy = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderSnowy,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(EnemiesReturns.Configuration.MechanicalSpider.SnowyStageList.Value, dchMechanicalSpiderSnowy);
        }

        private void CreateLynxTribe(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, Dictionary<string, Texture2D> rampLookups)
        {
            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.Enabled.Value
                || EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                CreateLynxStorm(assets, acdLookup);
            }
            if (EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                CreateLynxShaman(assets, iconLookup, acdLookup, rampLookups);
            }
            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.Enabled.Value)
            {
                var dtLynxTotem = CreateLynxTotemItem(assets, iconLookup);
                CreateLynxScout(assets, iconLookup);
                CreateLynxHunter(assets, iconLookup);
                CreateLynxArcher(assets, iconLookup, rampLookups);
                CreateLynxTotem(assets, iconLookup, acdLookup, dtLynxTotem);
                Utils.AddMonsterFamilyToStages(EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value, new LynxTribeStuff().CreateLynxTribeFamily());
                if (EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineEnabled.Value)
                {
                    CreateLynxShrine(assets);
                }
                if (EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapEnabled.Value)
                {
                    CreateLynxTrap(assets);
                }
            }
        }

        public void CreateLynxTrap(GameObject[] assets)
        {
            var lynxStuff = new LynxTribeStuff();

            var nseLynxTribeTrapTwigSnap = Utils.CreateNetworkSoundDef("ER_LynxTrap_SnapTwig_Play");
            nseList.Add(nseLynxTribeTrapTwigSnap);

            var leavesPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/lemuriantemple/Assets/LTFallenLeaf.spm").WaitForCompletion();
            var material = leavesPrefab.transform.Find("LTFallenLeaf_LOD0").GetComponent<MeshRenderer>().material;
            var lynxTrapPrefab = assets.First(prefab => prefab.name == "LynxTrapPrefab");

            var defaultStages = EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value.Split(",");
            foreach (var stageString in defaultStages)
            {
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                GameObject lynxTrap = lynxTrapPrefab.InstantiateClone(lynxTrapPrefab.name + cleanStageString, false);
                switch (cleanStageString)
                {
                    case "blackbeach":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("SkyMeadowLeaves_LOD0", lynxStuff.CreateBlackBeachLeavesMaterialLOD0, material));
                        break;
                    case "skymeadow":
                    case "itskymeadow":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("SkyMeadowLeaves_LOD0", lynxStuff.CreateSkyMeadowLeavesMaterialLOD0, material));
                        break;
                    case "village":
                    case "villagenight":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("ShatteredAbodesLeaves_LOD0", lynxStuff.CreateShatteredAbodesLeavesMaterialLOD0, material));
                        break;
                    case "golemplains":
                    case "itgolemplains":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("TitanicPlainsLeaves_LOD0", lynxStuff.CreateTitanicPlanesLeavesMaterialLOD0, material));
                        break;
                    case "foggyswamp":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("WetlandsLeaves_LOD0", lynxStuff.CreateWetlandsLeavesMaterialLOD0, material));
                        break;
                    case "habitatfall":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("GoldenDiebackLeaves_LOD0", lynxStuff.CreateGoldemDiebackLeavesMaterialLOD0, material));
                        break;
                    default:
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, material);
                        break;
                }
                InteractableSpawnCard spawnCardTrap = lynxStuff.CreateLynxTrapSpawnCard(lynxTrap, cleanStageString);

                var dcLynxTrap = new DirectorCard
                {
                    spawnCard = spawnCardTrap,
                    selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapSelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = false
                };

                var holderTrap = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcLynxTrap,
                    InteractableCategory = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapSpawnCategory.Value
                };

                DirectorAPI.Helpers.AddNewInteractableToStage(holderTrap, DirectorAPI.ParseInternalStageName(cleanStageString), cleanStageString);

                nopList.Add(lynxTrap);
            }
        }

        public void CreateLynxShrine(GameObject[] assets)
        {
            var lynxStuff = new LynxTribeStuff();

            ModdedEntityStates.LynxTribe.Retreat.retreatEffectPrefab = lynxStuff.CreateRetreatEffect(assets.First(prefab => prefab.name == "LynxTribeRetreatEffect"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Retreat.retreatEffectPrefab));

            LynxTribeStuff.CustomHologramContent = lynxStuff.CustomCostHologramContentPrefab();

            var shrineEffect = lynxStuff.CreateShrineUseEffect();
            effectsList.Add(new EffectDef(shrineEffect));

            var nsedLynxShrineFailure = Utils.CreateNetworkSoundDef("ER_Lynx_Shrine_Failure_Play");
            nseList.Add(nsedLynxShrineFailure);

            var nsedLynxShrineSuccess = Utils.CreateNetworkSoundDef("ER_Lynx_Shrine_Success_Play");
            nseList.Add(nsedLynxShrineSuccess);

            var lynxShrine1 = lynxStuff.CreateShrinePrefab(assets.First(prefab => prefab.name == "LynxShrinePrefab"), shrineEffect, nsedLynxShrineFailure, nsedLynxShrineSuccess);
            var lynxShrine2 = lynxStuff.CreateShrinePrefab(assets.First(prefab => prefab.name == "LynxShrinePrefab2"), shrineEffect, nsedLynxShrineFailure, nsedLynxShrineSuccess);
            var lynxShrine3 = lynxStuff.CreateShrinePrefab(assets.First(prefab => prefab.name == "LynxShrinePrefab3"), shrineEffect, nsedLynxShrineFailure, nsedLynxShrineSuccess);

            DirectorAPI.DirectorCardHolder holderShrine1 = CreateCardHolderLynxShrine(lynxShrine1, "1");
            DirectorAPI.DirectorCardHolder holderShrine2 = CreateCardHolderLynxShrine(lynxShrine2, "2");
            DirectorAPI.DirectorCardHolder holderShrine3 = CreateCardHolderLynxShrine(lynxShrine3, "3");

            var defaultStages = EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value.Split(",");
            foreach (var stageString in defaultStages)
            {
                DirectorAPI.DirectorCardHolder shrineToSpawn;
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                switch (cleanStageString)
                {
                    case "blackbeach":
                    case "skymeadow":
                    case "itskymeadow":
                        shrineToSpawn = holderShrine1;
                        break;
                    case "village":
                    case "villagenight":
                    case "golemplains":
                    case "itgolemplains":
                        shrineToSpawn = holderShrine2;
                        break;
                    case "foggyswamp":
                    case "habitatfall":
                    default:
                        shrineToSpawn = holderShrine3;
                        break;
                }

                DirectorAPI.Helpers.AddNewInteractableToStage(shrineToSpawn, DirectorAPI.ParseInternalStageName(cleanStageString), cleanStageString);
            }

            nopList.Add(lynxShrine1);
            nopList.Add(lynxShrine2);
            nopList.Add(lynxShrine3);
        }

        private void CreateJudgement()
        {
            if (Configuration.Judgement.Enabled.Value)
            {
                nseList.Add(Utils.CreateNetworkSoundDef("Play_moonBrother_spawn")); // TODO: replce with different sounds, something heavy that lands in water, use wow sound like you usually do

                SetupJudgementPath.AddInteractabilityToNewt();
                SetupJudgementPath.AddWeaponDropToMithrix();

                Content.DamageTypes.EndGameBossWeapon = DamageAPI.ReserveDamageType();

                var arraignStuff = new EnemiesReturns.Enemies.Judgement.Arraign.ArraignStuff();
                ModdedEntityStates.Judgement.Arraign.BasePrimaryWeaponSwing.swingEffect = arraignStuff.CreateArraignSwingEffect();
                var swingComboEffect = arraignStuff.CreateArraignSwingComboEffect();
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash1.swingEffect = swingComboEffect;
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash2.swingEffect = swingComboEffect;
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.swingEffect = swingComboEffect;

                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.PreSlash.overlayMaterial = GetOrCreateMaterial("matArraignPreSlashOverlay", arraignStuff.CreatePreSlashWeaponOverlayMaterial);
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.explosionEffect = arraignStuff.CreateSlash3ExplosionEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.explosionEffect));

                ModdedEntityStates.Judgement.Arraign.BaseSkyLeap.BaseHoldSkyLeap.markEffect = arraignStuff.CreateSkyLeapMarktempVisualEffect();
                ModdedEntityStates.Judgement.Arraign.Phase1.SwordBeam.SwordBeamLoop.beamPrefab = arraignStuff.CreateBeamEffect();
            }
        }

        private DirectorAPI.DirectorCardHolder CreateCardHolderLynxShrine(GameObject lynxShrine1, string suffix)
        {
            var spawnCardShrine1 = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            (spawnCardShrine1 as ScriptableObject).name = "iscLynxShrine" + suffix;
            spawnCardShrine1.prefab = lynxShrine1;
            spawnCardShrine1.sendOverNetwork = true;
            spawnCardShrine1.hullSize = HullClassification.Golem;
            spawnCardShrine1.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCardShrine1.requiredFlags = RoR2.Navigation.NodeFlags.None;
            spawnCardShrine1.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn | RoR2.Navigation.NodeFlags.NoShrineSpawn;
            spawnCardShrine1.directorCreditCost = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineDirectorCost.Value;
            spawnCardShrine1.occupyPosition = true;
            spawnCardShrine1.eliteRules = SpawnCard.EliteRules.Default;
            spawnCardShrine1.orientToFloor = false;
            spawnCardShrine1.maxSpawnsPerStage = Configuration.LynxTribe.LynxStuff.LynxShrineMaxSpawnPerStage.Value;

            var dcLynxShrine1 = new DirectorCard
            {
                spawnCard = spawnCardShrine1,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineSelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false
            };

            var holderShrine1 = new DirectorAPI.DirectorCardHolder();
            holderShrine1.Card = dcLynxShrine1;
            holderShrine1.InteractableCategory = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineSpawnCategory.Value;
            return holderShrine1;
        }

        public void CreateLynxArcher(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, Texture2D> rampLookups)
        {
            var archerStuff = new ArcherStuff();
            var material = GetOrCreateMaterial("matLynxArcherArrow", archerStuff.CreateArcherArrowMaterial);

            ModdedEntityStates.LynxTribe.Archer.FireArrow.projectilePrefab = archerStuff.CreateArrowProjectile(
                assets.First(prefab => prefab.name == "ArrowProjectile"),
                archerStuff.CreateArrowProjectileGhost(assets.First(prefab => prefab.name == "ArrowProjectileGhost"), material),
                archerStuff.CreateArrowImpalePrefab(assets.First(prefab => prefab.name == "ArrowProjectileGhost"), material),
                archerStuff.CreateArrowLoopSoundDef());
            projectilesList.Add(ModdedEntityStates.LynxTribe.Archer.FireArrow.projectilePrefab);

            var archerBody = new ArcherBody();
            ArcherBody.Skills.Shot = archerBody.CreateShotSkill();
            sdList.Add(ArcherBody.Skills.Shot);

            ArcherBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxArcherPrimarySkillFamily", ArcherBody.Skills.Shot);
            sfList.Add(ArcherBody.SkillFamilies.Primary);

            ArcherBody.BodyPrefab = archerBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxArcherBody"), sprite: iconLookup["texLynxArcherIcon"]);
            bodyList.Add(ArcherBody.BodyPrefab);

            var archerMaster = new ArcherMaster();
            ArcherMaster.MasterPrefab = archerMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxArcherMaster"), ArcherBody.BodyPrefab);
            masterList.Add(ArcherMaster.MasterPrefab);

            ArcherBody.SpawnCards.cscLynxArcherDefault = archerBody.CreateCard("cscLynxArcherDefault", ArcherMaster.MasterPrefab, ArcherBody.SkinDefs.Default, ArcherBody.BodyPrefab);

            var dhLynxArcherDefault = new DirectorCard
            {
                spawnCard = ArcherBody.SpawnCards.cscLynxArcherDefault,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxArcher.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = 0
            };
            DirectorAPI.DirectorCardHolder dchLynxArcherDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxArcherDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell), dchLynxArcherDefault);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, ArcherBody.SpawnCards.cscLynxArcherDefault);

            var archerBodyAlly = new ArcherBodyAlly();
            Content.Buffs.LynxArcherDamage = archerBodyAlly.CreateDamageBuffDef(iconLookup["texLynxArcherBuff"]);
            bdList.Add(Content.Buffs.LynxArcherDamage);

            ArcherBodyAlly.BodyPrefab = archerBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxArcherAllyBody"), sprite: iconLookup["texArcherAllyIcon"]);
            bodyList.Add(ArcherBodyAlly.BodyPrefab);

            var archerMasterAlly = new ArcherMasterAlly();
            ArcherMasterAlly.MasterPrefab = archerMasterAlly.AddMasterComponents(assets.First(prefab => prefab.name == "LynxArcherAllyMaster"), ArcherBodyAlly.BodyPrefab);
            masterList.Add(ArcherMasterAlly.MasterPrefab);

            ArcherBodyAlly.SpawnCards.cscLynxArcherAlly = archerBodyAlly.CreateCard("cscLynxArcherAlly", ArcherMasterAlly.MasterPrefab, ArcherBodyAlly.SkinDefs.Ally, ArcherBodyAlly.BodyPrefab);
        }

        public void CreateLynxHunter(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            var hunterStuff = new HunterStuff();
            var wooshEffect = hunterStuff.CreateHunterAttackEffect(assets.First(prefab => prefab.name == "LynxHunterAttackEffect"));
            Junk.ModdedEntityStates.LynxTribe.Hunter.Stab.wooshEffect = wooshEffect;
            ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.wooshEffect = wooshEffect;
            effectsList.Add(new EffectDef(wooshEffect));

            ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.slideEffectPrefab = hunterStuff.CreateLungeSlideEffect();
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.slideEffectPrefab));

            var coneEffect = hunterStuff.CreateHunterAttackSpearTipEffect(assets.First(prefab => prefab.name == "LynxHunterSpearTipEffect"));
            Junk.ModdedEntityStates.LynxTribe.Hunter.Stab.coneEffect = coneEffect;
            ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.coneEffect = coneEffect;
            effectsList.Add(new EffectDef(coneEffect));

            var hunterBody = new HunterBody();
            HunterBody.Skills.Stab = hunterBody.CreateStabSkill();
            sdList.Add(HunterBody.Skills.Stab);

            HunterBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxHunterPrimarySkillFamily", HunterBody.Skills.Stab);
            sfList.Add(HunterBody.SkillFamilies.Primary);

            HunterBody.BodyPrefab = hunterBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxHunterBody"), sprite: iconLookup["texLynxHunterIcon"]);
            bodyList.Add(HunterBody.BodyPrefab);

            var hunterMaster = new HunterMaster();
            HunterMaster.MasterPrefab = hunterMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxHunterMaster"), HunterBody.BodyPrefab);
            masterList.Add(HunterMaster.MasterPrefab);

            HunterBody.SpawnCards.cscLynxHunterDefault = hunterBody.CreateCard("cscLynxHunterDefault", HunterMaster.MasterPrefab, HunterBody.SkinDefs.Default, HunterBody.BodyPrefab);

            var dhLynxHunterDefault = new DirectorCard
            {
                spawnCard = HunterBody.SpawnCards.cscLynxHunterDefault,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxHunter.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = 0
            };
            DirectorAPI.DirectorCardHolder dchLynxHunterDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxHunterDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell), dchLynxHunterDefault);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, HunterBody.SpawnCards.cscLynxHunterDefault);

            var hunterBodyAlly = new HunterBodyAlly();
            Content.Buffs.LynxHunterArmor = hunterBodyAlly.CreateArmorBuffDef(iconLookup["texLynxHunterBuff"]);
            bdList.Add(Content.Buffs.LynxHunterArmor);

            HunterBodyAlly.BodyPrefab = hunterBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxHunterAllyBody"), sprite: iconLookup["texLynxHunterAllyIcon"]);
            bodyList.Add(HunterBodyAlly.BodyPrefab);

            var hunterMasterAlly = new HunterMasterAlly();
            HunterMasterAlly.MasterPrefab = hunterMasterAlly.AddMasterComponents(assets.First(prefab => prefab.name == "LynxHunterAllyMaster"), HunterBodyAlly.BodyPrefab);
            masterList.Add(HunterMasterAlly.MasterPrefab);

            HunterBodyAlly.SpawnCards.cscLynxHunterAlly = hunterBodyAlly.CreateCard("cscLynxHunterAlly", HunterMasterAlly.MasterPrefab, HunterBodyAlly.SkinDefs.Ally, HunterBodyAlly.BodyPrefab);
        }

        public void CreateLynxScout(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            var scoutStuff = new ScoutStuff();

            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectLeft = scoutStuff.CreateDoubleSlashClawEffect(assets.First(prefab => prefab.name == "LynxScoutClawEffectLeft"));
            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectRight = scoutStuff.CreateDoubleSlashClawEffect(assets.First(prefab => prefab.name == "LynxScoutClawEffectRight"));
            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectLeft = scoutStuff.CreateDoubleSlashLeftHandSwingTrail();
            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectRight = scoutStuff.CreateDoubleSlashRightHandSwingTrail();

            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectLeft));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectRight));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectLeft));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectRight));

            var scoutBody = new ScoutBody();
            ScoutBody.Skills.DoubleSlash = scoutBody.CreateDoubleSlashSkill();

            sdList.Add(ScoutBody.Skills.DoubleSlash);

            ScoutBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxScoutPrimarySkillFamily", ScoutBody.Skills.DoubleSlash);

            sfList.Add(ScoutBody.SkillFamilies.Primary);

            ScoutBody.BodyPrefab = scoutBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxScoutBody"), sprite: iconLookup["texLynxScoutIcon"]);
            bodyList.Add(ScoutBody.BodyPrefab);

            var scoutMaster = new ScoutMaster();
            ScoutMaster.MasterPrefab = scoutMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxScoutMaster"), ScoutBody.BodyPrefab);
            masterList.Add(ScoutMaster.MasterPrefab);

            ScoutBody.SpawnCards.cscLynxScoutDefault = scoutBody.CreateCard("cscLynxScoutDefault", ScoutMaster.MasterPrefab, ScoutBody.SkinDefs.Default, ScoutBody.BodyPrefab);

            var dhLynxScoutDefault = new DirectorCard
            {
                spawnCard = ScoutBody.SpawnCards.cscLynxScoutDefault,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxScout.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = 0
            };
            DirectorAPI.DirectorCardHolder dchLynxScoutDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxScoutDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell), dchLynxScoutDefault);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, ScoutBody.SpawnCards.cscLynxScoutDefault);

            var scoutBodyAlly = new ScoutBodyAlly();
            Content.Buffs.LynxScoutSpeed = scoutBodyAlly.CreateSpeedBuffDef(iconLookup["texLynxScoutBuff"]);
            bdList.Add(Content.Buffs.LynxScoutSpeed);

            ScoutBodyAlly.BodyPrefab = scoutBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxScoutAllyBody"), sprite: iconLookup["texLynxScoutAllyIcon"]);
            bodyList.Add(ScoutBodyAlly.BodyPrefab);

            var scoutMasterAlly = new ScoutMasterAlly();
            ScoutMasterAlly.MasterPrefab = scoutMasterAlly.AddMasterComponents(assets.First(prefab => prefab.name == "LynxScoutAllyMaster"), ScoutBodyAlly.BodyPrefab);
            masterList.Add(ScoutMasterAlly.MasterPrefab);

            ScoutBodyAlly.SpawnCards.cscLynxScoutAlly = scoutBodyAlly.CreateCard("cscLynxScoutAlly", ScoutMasterAlly.MasterPrefab, ScoutBodyAlly.SkinDefs.Ally, ScoutBodyAlly.BodyPrefab);
        }

        private void CreateLynxTotem(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, ExplicitPickupDropTable dtLynxTotem)
        {
            var totemStuff = new TotemStuff();

            totemStuff.RegisterDeployableSlot();

            var shamanTotemSpawnEffect = totemStuff.CreateShamanTotemSpawnEffect(assets.First(prefab => prefab.name == "LynxSpawnParticles"));
            ModdedEntityStates.LynxTribe.Totem.SummonTribe.summonEffect = shamanTotemSpawnEffect;
            ModdedEntityStates.LynxTribe.Totem.SpawnState.leavesSpawnEffect = shamanTotemSpawnEffect;
            effectsList.Add(new EffectDef(shamanTotemSpawnEffect));

            var spawnEffect = totemStuff.CreateTribesmenSpawnEffect(assets.First(prefab => prefab.name == "LynxSpawnParticles"));
            ModdedEntityStates.LynxTribe.Scout.SpawnState.spawnEffect = spawnEffect;
            ModdedEntityStates.LynxTribe.Hunter.SpawnState.spawnEffect = spawnEffect;
            ModdedEntityStates.LynxTribe.Archer.SpawnState.spawnEffect = spawnEffect;
            effectsList.Add(new EffectDef(spawnEffect));

            Junk.ModdedEntityStates.LynxTribe.Totem.SummonFirewall.projectilePrefab = totemStuff.CreateFirewallProjectile(assets.First(prefab => prefab.name == "LynxTotemFireWallProjectile"), acdLookup["acdLynxTotemFirewall"]);
            projectilesList.Add(Junk.ModdedEntityStates.LynxTribe.Totem.SummonFirewall.projectilePrefab);

            var shakeEffect = totemStuff.CreateGroundpoundShakeEffect();
            Junk.ModdedEntityStates.LynxTribe.Totem.Groundpound.shakeEffect = shakeEffect;
            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.shakeEffect = shakeEffect;
            ModdedEntityStates.LynxTribe.Totem.SpawnStateFromShaman.shakeEffect = shakeEffect;
            effectsList.Add(new EffectDef(shakeEffect));

            var poundEffect = totemStuff.CreateGroundpoundPoundEffect();
            Junk.ModdedEntityStates.LynxTribe.Totem.Groundpound.poundEffect = poundEffect;
            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.poundEffect = poundEffect;
            ModdedEntityStates.LynxTribe.Totem.SpawnState.poundEffect = poundEffect;
            effectsList.Add(new EffectDef(poundEffect));

            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.stoneParticlesEffect = totemStuff.CreateStoneParticlesEffect(assets.First(prefab => prefab.name == "TotemShakeParticles"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.stoneParticlesEffect));

            var singleStoneEffect = totemStuff.CreateStoneParticlesEffect(assets.First(prefab => prefab.name == "TotemSingleStoneParticle"));
            ModdedEntityStates.LynxTribe.Totem.SummonStorm.stoneEffectPrefab = singleStoneEffect;
            ModdedEntityStates.LynxTribe.Totem.SummonTribe.stoneEffectPrefab = singleStoneEffect;
            effectsList.Add(new EffectDef(singleStoneEffect));

            ModdedEntityStates.LynxTribe.Totem.SummonTribe.eyeEffect = totemStuff.CreateEyeGlowEffect(assets.First(prefab => prefab.name == "TotemEyeGlowSummonTribe"), acdLookup["acdLynxTotemEyeGlowSummonTribe"], 1f);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.SummonTribe.eyeEffect));

            ModdedEntityStates.LynxTribe.Totem.SummonStorm.eyeEffect = totemStuff.CreateEyeGlowEffect(assets.First(prefab => prefab.name == "TotemEyeGlowSummonStorm"), acdLookup["acdLynxTotemEyeGlowSummonStorm"], 3.3f);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.SummonStorm.eyeEffect));

            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.eyeEffect = totemStuff.CreateEyeGlowEffect(assets.First(prefab => prefab.name == "TotemEyeGlowGroundpound"), acdLookup["acdLynxTotemEyeGlowGroundpound"], 1.8f);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.eyeEffect));

            ModdedEntityStates.LynxTribe.Totem.SummonStorm.staffEffect = totemStuff.CreateSummonStormsStaffParticle(acdLookup["acdLynxTotemSummonStormStaffEffectScale"]);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.SummonStorm.staffEffect));

            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.groundpoundProjectilePrefab = totemStuff.CreateGroundpoundProjectile(assets.First(prefab => prefab.name == "LynxTotemGroundpoundProjectile"));
            projectilesList.Add(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.groundpoundProjectilePrefab);

            Enemies.LynxTribe.Storm.LynxStormOrb.orbEffect = totemStuff.CreateStormSummonOrb(assets.First(prefab => prefab.name == "TotemSummonStormOrb"));
            effectsList.Add(new EffectDef(Enemies.LynxTribe.Storm.LynxStormOrb.orbEffect));

            var totemLog = Utils.CreateUnlockableDef("Logs.LynxTotemBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_LYNX_TOTEM");
            unlockablesList.Add(totemLog);

            var totemBody = new TotemBody();
            TotemBody.Skills.Burrow = totemBody.CreateBurrowSkill();
            TotemBody.Skills.SummonStorms = totemBody.CreateSummonStormsSkill();
            TotemBody.Skills.SummonTribe = totemBody.CreateSummonTribeSkill();
            TotemBody.Skills.SummonFirewall = totemBody.CreateSummonFirewallSkill();
            TotemBody.Skills.Groundpound = totemBody.CreateGroundpoundSkill();
            sdList.Add(TotemBody.Skills.Burrow);
            sdList.Add(TotemBody.Skills.SummonStorms);
            sdList.Add(TotemBody.Skills.SummonTribe);
            sdList.Add(TotemBody.Skills.SummonFirewall);
            sdList.Add(TotemBody.Skills.Groundpound);

            TotemBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxTotemPrimarySkillFamily", TotemBody.Skills.Groundpound);
            TotemBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("LynxTotemSecondarySkillFamily", TotemBody.Skills.SummonTribe);
            TotemBody.SkillFamilies.Utility = Utils.CreateSkillFamily("LynxTotemUtilitySkillFamily", TotemBody.Skills.Burrow);
            TotemBody.SkillFamilies.Special = Utils.CreateSkillFamily("LynxTotemSpecialSkillFamily", TotemBody.Skills.SummonStorms);
            sfList.Add(TotemBody.SkillFamilies.Primary);
            sfList.Add(TotemBody.SkillFamilies.Secondary);
            sfList.Add(TotemBody.SkillFamilies.Utility);
            sfList.Add(TotemBody.SkillFamilies.Special);

            ModdedEntityStates.LynxTribe.Totem.SummonStorm.cscStorm = LynxStormBody.cscLynxStorm;

            TotemBody.BodyPrefab = totemBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxTotemBody"), sprite: iconLookup["texLynxTotemIcon"], totemLog, dtLynxTotem);
            bodyList.Add(TotemBody.BodyPrefab);

            var totemMaster = new TotemMaster();
            TotemMaster.MasterPrefab = totemMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxTotemMaster"), TotemBody.BodyPrefab);
            masterList.Add(TotemMaster.MasterPrefab);

            TotemBody.SpawnCards.cscLynxTotemDefault = totemBody.CreateCard("cscLynxTotemDefault", TotemMaster.MasterPrefab, TotemBody.SkinDefs.Default, TotemBody.BodyPrefab);

            var dhLynxTotemDefault = new DirectorCard
            {
                spawnCard = TotemBody.SpawnCards.cscLynxTotemDefault,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxTotem.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.LynxTribe.LynxTotem.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxTotemDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxTotemDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
            };
            Utils.AddMonsterToStages(EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value, dchLynxTotemDefault);

            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
            {
                ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(Enemies.LynxTribe.Totem.TotemBody.SpawnCards.cscLynxTotemDefault, 1);
            }
        }

        private ExplicitPickupDropTable CreateLynxTotemItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtLynxTotem = null;

            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.ItemEnabled.Value)
            {
                var fetishFactory = new LynxFetishFactory();

                Content.Items.LynxFetish = fetishFactory.CreateItem(assets.First(item => item.name == "PickupLynxFetish"), iconLookup["texLynxFetishIcon"]);
                itemList.Add(Content.Items.LynxFetish);

                dtLynxTotem = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtLynxTotem as ScriptableObject).name = "epdtLynxTotem";
                dtLynxTotem.canDropBeReplaced = true;
                dtLynxTotem.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                    new ExplicitPickupDropTable.PickupDefEntry
                    {
                        pickupWeight = 1,
                        pickupDef = Content.Items.LynxFetish
                    }
                };

                HG.ArrayUtils.ArrayAppend(ref Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = Content.Items.LynxFetish, itemDef2 = VoidMegaCrabItem });
            }

            return dtLynxTotem;
        }

        private void CreateLynxShaman(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, Dictionary<string, Texture2D> rampLookups)
        {
            Content.DamageTypes.ApplyReducedHealing = DamageAPI.ReserveDamageType();
            var shamanStuff = new ShamanStuff();

            var shamanSpawnEffect = shamanStuff.CreateShamanSpawnEffect(assets.First(prefab => prefab.name == "LynxSpawnParticles"));
            ModdedEntityStates.LynxTribe.Shaman.SpawnState.spawnEffect = shamanSpawnEffect;
            effectsList.Add(new EffectDef(shamanSpawnEffect));

            Content.Buffs.ReduceHealing = shamanStuff.CreateReduceHealingBuff(iconLookup["texReducedHealingBuffColored"]);
            bdList.Add(Content.Buffs.ReduceHealing);

            Junk.ModdedEntityStates.LynxTribe.Shaman.SummonStormSkill.summonEffectPrefab = shamanStuff.CreateSummonStormParticles(assets.First(prefab => prefab.name == "ShamanSummonStormParticle"));
            Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport.Teleport.ghostMaskPrefab = shamanStuff.SetupShamanMaskMaterials(assets.First(prefab => prefab.name == "ShamanMask"));

            Junk.ModdedEntityStates.LynxTribe.Shaman.TeleportFriend.teleportEffect = shamanStuff.CreateShamanTeleportOut(assets.First(prefab => prefab.name == "ShamanTeleportEffectOut"));
            effectsList.Add(new EffectDef(Junk.ModdedEntityStates.LynxTribe.Shaman.TeleportFriend.teleportEffect));

            ModdedEntityStates.LynxTribe.Shaman.PushBack.summonPrefab = shamanStuff.CreateShamanPushBackSummonEffect(assets.First(prefab => prefab.name == "LynxShamanPushBackSummon"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Shaman.PushBack.summonPrefab));

            ModdedEntityStates.LynxTribe.Shaman.PushBack.explosionPrefab = shamanStuff.CreateShamanPushBackExplosionEffect(assets.First(prefab => prefab.name == "LynxShamanPushBackExplosion"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Shaman.PushBack.explosionPrefab));

            TempVisualEffectAPI.AddTemporaryVisualEffect(shamanStuff.CreateReduceHealingVisualEffect(), (body) => { return body.HasBuff(Content.Buffs.ReduceHealing); });

            var projectileImpactEffect = shamanStuff.CreateShamanProjectileImpactEffect(assets.First(prefab => prefab.name == "LynxShamanProjectileImpactEffect"), rampLookups["texRampLynxShamanProjectileImpact"]);
            effectsList.Add(new EffectDef(projectileImpactEffect));

            var skullProjectile = shamanStuff.CreateShamanTrackingProjectile(
                assets.First(prefab => prefab.name == "ShamanTrackingProjectile"),
                shamanStuff.CreateShamanTrackingProjectileGhost(),
                projectileImpactEffect,
                shamanStuff.CreateProjectileFlightSoundLoop()
            );
            Junk.ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesRapidFire.trackingProjectilePrefab = skullProjectile;
            ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesShotgun.trackingProjectilePrefab = skullProjectile;
            projectilesList.Add(skullProjectile);

            var projectilesSummonEffect = shamanStuff.CreateShamanTrackingProjectileSummonEffect(acdLookup["acdShamanSummonProjectilesScaling"]);
            ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesShotgun.summonEffect = projectilesSummonEffect;
            effectsList.Add(new EffectDef(projectilesSummonEffect));

            var shamanLog = Utils.CreateUnlockableDef("Logs.LynxShamanBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_LYNX_SHAMAN");
            unlockablesList.Add(shamanLog);

            var shamanBody = new ShamanBody();
            ShamanBody.Skills.Teleport = shamanBody.CreateTeleportSkill();
            ShamanBody.Skills.SummonStorm = shamanBody.CreateSummonStormSkill();
            ShamanBody.Skills.SummonProjectiles = shamanBody.CreateSummonProjectilesSkill();
            ShamanBody.Skills.TeleportFriend = shamanBody.CreateTeleportFriendSkill();
            ShamanBody.Skills.SummonLightning = shamanBody.CreateSummonLightningSkill();
            ShamanBody.Skills.PushBack = shamanBody.CreatePushBackSkill();

            sdList.Add(ShamanBody.Skills.Teleport);
            sdList.Add(ShamanBody.Skills.SummonStorm);
            sdList.Add(ShamanBody.Skills.SummonProjectiles);
            sdList.Add(ShamanBody.Skills.TeleportFriend);
            sdList.Add(ShamanBody.Skills.SummonLightning);
            sdList.Add(ShamanBody.Skills.PushBack);

            Junk.ModdedEntityStates.LynxTribe.Shaman.SummonStormSkill.cscStorm = LynxStormBody.cscLynxStorm;

            ShamanBody.SkillFamilies.Utility = Utils.CreateSkillFamily("LynxShamanUtilitySkillFamily", ShamanBody.Skills.Teleport);
            //ShamanBody.SkillFamilies.Utility = Utils.CreateSkillFamily("LynxShamanUtilitySkillFamily", ShamanBody.Skills.SummonLightning);
            ShamanBody.SkillFamilies.Special = Utils.CreateSkillFamily("LynxShamanSpecialSkillFamily", ShamanBody.Skills.SummonStorm);
            ShamanBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxShamanPrimarySkillFamily", ShamanBody.Skills.SummonProjectiles);
            //ShamanBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("LynxShamanSecondarySkillFamily", ShamanBody.Skills.TeleportFriend);
            ShamanBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("LynxShamanSecondarySkillFamily", ShamanBody.Skills.PushBack);

            sfList.Add(ShamanBody.SkillFamilies.Utility);
            sfList.Add(ShamanBody.SkillFamilies.Special);
            sfList.Add(ShamanBody.SkillFamilies.Primary);
            sfList.Add(ShamanBody.SkillFamilies.Secondary);

            ShamanBody.BodyPrefab = shamanBody.AddBodyComponents(assets.First(body => body.name == "LynxShamanBody"), iconLookup["texLynxShamanIcon"], shamanLog);
            bodyList.Add(ShamanBody.BodyPrefab);
            ShamanMaster.MasterPrefab = new ShamanMaster().AddMasterComponents(assets.First(master => master.name == "LynxShamanMaster"), ShamanBody.BodyPrefab);
            masterList.Add(ShamanMaster.MasterPrefab);

            ShamanBody.SpawnCards.cscLynxShamanDefault = shamanBody.CreateCard("cscLynxShamanDefault", ShamanMaster.MasterPrefab, ShamanBody.SkinDefs.Default, ShamanBody.BodyPrefab);

            var dhLynxShamanDefault = new DirectorCard
            {
                spawnCard = ShamanBody.SpawnCards.cscLynxShamanDefault,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.LynxTribe.LynxShaman.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxShamanDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxShamanDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(EnemiesReturns.Configuration.LynxTribe.LynxShaman.DefaultStageList.Value, dchLynxShamanDefault);

            var shamanBodyAlly = new ShamanBodyAlly();
            Content.Buffs.LynxShamanSpecialDamage = shamanBodyAlly.CreateSpecialBuffDef(iconLookup["texLynxShamanBuff"]);
            bdList.Add(Content.Buffs.LynxShamanSpecialDamage);

            ShamanBodyAlly.BodyPrefab = shamanBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxShamanAllyBody"), sprite: iconLookup["texLynxShamanAllyIcon"], log: null);
            bodyList.Add(ShamanBodyAlly.BodyPrefab);

            ShamanMasterAlly.MasterPrefab = new ShamanMasterAlly().AddMasterComponents(assets.First(prefab => prefab.name == "LynxShamanAllyMaster"), ShamanBodyAlly.BodyPrefab);
            masterList.Add(ShamanMasterAlly.MasterPrefab);

            ShamanBodyAlly.SpawnCards.cscLynxShamanAlly = shamanBodyAlly.CreateCard("cscLynxShamanAlly", ShamanMasterAlly.MasterPrefab, ShamanBodyAlly.SkinDefs.Ally, ShamanBodyAlly.BodyPrefab);
        }

        private void CreateLynxStorm(GameObject[] assets, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var stormStuff = new LynxStormStuff();
            Content.Buffs.LynxStormImmunity = stormStuff.CreateStormImmunityBuff();
            bdList.Add(Content.Buffs.LynxStormImmunity);

            Enemies.LynxTribe.Storm.LynxStormComponent.dotEffect = stormStuff.CreateStormThrowEffect();
            effectsList.Add(new EffectDef(Enemies.LynxTribe.Storm.LynxStormComponent.dotEffect));

            var stormBody = new LynxStormBody();
            LynxStormBody.BodyPrefab = stormBody.AddBodyComponents(assets.First(body => body.name == "StormBody"), acdLookup);
            bodyList.Add(LynxStormBody.BodyPrefab);

            LynxStormMaster.MasterPrefab = new LynxStormMaster().AddMasterComponents(assets.First(master => master.name == "StormMaster"), LynxStormBody.BodyPrefab);
            masterList.Add(LynxStormMaster.MasterPrefab);

            LynxStormBody.cscLynxStorm = stormBody.CreateCard("cscLynxStorm", LynxStormMaster.MasterPrefab);
        }
        #endregion

        private IEnumerator LoadAssetBundle(string assetBundleFullPath, IProgress<float> progress, Action<AssetBundle> onAssetBundleLoaded)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundleFullPath);
            while (!assetBundleCreateRequest.isDone)
            {
                progress.Report(assetBundleCreateRequest.progress);
                yield return null;
            }

            onAssetBundleLoaded(assetBundleCreateRequest.assetBundle);
            stopwatch.Stop();
            Log.Info("Asset bundle " + assetBundleFullPath + " loaded in " + stopwatch.elapsedSeconds);

            yield break;
        }

        private static IEnumerator LoadAllAssetsAsync<T>(AssetBundle assetBundle, IProgress<float> progress, Action<T[]> onAssetsLoaded) where T : UnityEngine.Object
        {
            var sceneDefsRequest = assetBundle.LoadAllAssetsAsync<T>();
            while (!sceneDefsRequest.isDone)
            {
                progress.Report(sceneDefsRequest.progress);
                yield return null;
            }

            onAssetsLoaded(sceneDefsRequest.allAssets.Cast<T>().ToArray());

            yield break;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Material> materialCreateFunc)
        {
            if (!ContentProvider.MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc();
                ContentProvider.MaterialCache.Add(materialName, material);
            }
            return material;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Texture2D, Material> materialCreateFunc, Texture2D texture)
        {
            if (!ContentProvider.MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc(texture);
                ContentProvider.MaterialCache.Add(materialName, material);
            }
            return material;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Material, Material> materialCreateFunc, Material materialOrig)
        {
            if (!ContentProvider.MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc(materialOrig);
                ContentProvider.MaterialCache.Add(materialName, material);
            }

            return material;
        }

        private void LoadSoundBanks(string soundbanksFolderPath)
        {
            var akResult = AkSoundEngine.AddBasePath(soundbanksFolderPath);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank base path : {soundbanksFolderPath}");
            }
            else
            {
                Log.Error(
                    $"Error adding base path : {soundbanksFolderPath} " +
                    $"Error code : {akResult}");
            }

            //akResult = AkSoundEngine.LoadBank(InitSoundBankFileName, out var _);
            //if (akResult == AKRESULT.AK_Success)
            //{
            //    Log.Info($"Added bank : {InitSoundBankFileName}");
            //}
            //else
            //{
            //    Log.Error(
            //        $"Error loading bank : {InitSoundBankFileName} " +
            //        $"Error code : {akResult}");
            //}

            //akResult = AkSoundEngine.LoadBank(MusicSoundBankFileName, out var _);
            //if (akResult == AKRESULT.AK_Success)
            //{
            //    Log.Info($"Added bank : {MusicSoundBankFileName}");
            //}
            //else
            //{
            //    Log.Error(
            //        $"Error loading bank : {MusicSoundBankFileName} " +
            //        $"Error code : {akResult}");
            //}

            akResult = AkSoundEngine.LoadBank(SoundsSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {SoundsSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {SoundsSoundBankFileName} " +
                    $"Error code : {akResult}");
            }
        }
    }
}
