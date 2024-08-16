using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static RoR2.Console;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using RoR2;
using EnemiesReturns.Enemies.Spitter;
using R2API;
using EnemiesReturns.Enemies.Colossus;
using Rewired.Utils.Classes.Utility;
using EnemiesReturns.EditorHelpers;

namespace EnemiesReturns
{
    public class ContentProvider : IContentPackProvider
    {
        public string identifier => EnemiesReturnsPlugin.GUID + "." + nameof(ContentProvider);

        private readonly ContentPack _contentPack = new ContentPack();

        public const string AssetBundleName = "EnemiesReturns";
        public const string AssetBundleFolder = "AssetBundles";

        public const string SoundbankFolder = "Soundbanks";
        public const string SoundsSoundBankFileName = "EnemiesReturnsSounds";

        public static Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbedror2/base/shaders/hgstandard", "RoR2/Base/Shaders/HGStandard.shader"},
            {"stubbedror2/base/shaders/hgsnowtopped", "RoR2/Base/Shaders/HGSnowTopped.shader"},
            {"stubbedror2/base/shaders/hgtriplanarterrainblend", "RoR2/Base/Shaders/HGTriplanarTerrainBlend.shader"},
            {"stubbedror2/base/shaders/hgintersectioncloudremap", "RoR2/Base/Shaders/HGIntersectionCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgcloudremap", "RoR2/Base/Shaders/HGCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgdistortion", "RoR2/Base/Shaders/HGDistortion.shader" },
            {"stubbedcalm water/calmwater - dx11 - doublesided", "Calm Water/CalmWater - DX11 - DoubleSided.shader" },
            {"stubbedcalm water/calmwater - dx11", "Calm Water/CalmWater - DX11.shader" },
            {"stubbednature/speedtree", "RoR2/Base/Shaders/SpeedTreeCustom.shader"},
            {"stubbeddecalicious/decaliciousdeferreddecal", "Decalicious/DecaliciousDeferredDecal.shader" }
        };

        public static List<Material> MaterialCache = new List<Material>(); //apparently you need it because reasons?

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

            Dictionary<string, Material> skinsLookup = new Dictionary<string, Material>();

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Material[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var materials = assets;

                if (materials != null)
                {
                    foreach (Material material in materials)
                    {
                        skinsLookup.Add(material.name, material);

                        if (!material.shader.name.StartsWith("Stubbed")) { continue; }

                        var replacementShader = Addressables.LoadAssetAsync<Shader>(ShaderLookup[material.shader.name.ToLower()]).WaitForCompletion();
                        var oldName = material.shader.name.ToLower();
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                            MaterialCache.Add(material);
                        }
                        else
                        {
                            Log.Warning("Couldn't find replacement shader for " + material.shader.name.ToLower());
                        }
                    }
                }
                stopwatch.Stop();
                Log.Info("Materials swapped in " + stopwatch.elapsedSeconds);
            }));

            Texture2D spitterIcon = null;
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Texture2D[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                spitterIcon = assets.First(sprite => sprite.name == "texSpitterIcon");
                stopwatch.Stop();
                Log.Info("Icons loaded in " + stopwatch.elapsedSeconds);
            }));

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //var escList = new List<EntityStateConfiguration>();
                var bodyList = new List<GameObject>();
                var masterList = new List<GameObject>();
                var stateList = new List<Type>();
                var sfList = new List<SkillFamily>();
                var sdList = new List<SkillDef>();
                var effectsList = new List<EffectDef>();
                var projectilesList = new List<GameObject>();
                var unlockablesList = new List<UnlockableDef>();

                #region Spitter
                var spitterFactory = new SpitterFactory();

                var biteEffectPrefab = spitterFactory.CreateBiteEffect();
                ModdedEntityStates.Spitter.Bite.biteEffectPrefab = biteEffectPrefab;
                effectsList.Add(new EffectDef(biteEffectPrefab));

                //SpitterFactory.normalSpitProjectile = spitterFactory.CreateNormalSpitProjectile();
                var chargedSpitSmallDoTZone = spitterFactory.CreatedChargedSpitSmallDoTZone();
                var chargedSpitDoTZone = spitterFactory.CreateChargedSpitDoTZone();
                var chargedSpitChunkProjectile = spitterFactory.CreateChargedSpitSplitProjectile(chargedSpitSmallDoTZone);
                var chargedSpitProjectile = spitterFactory.CreateChargedSpitProjectile(chargedSpitDoTZone, chargedSpitChunkProjectile);;
                ModdedEntityStates.Spitter.FireChargedSpit.projectilePrefab = chargedSpitProjectile;
                //projectilesList.Add(SpitterFactory.normalSpitProjectile);
                projectilesList.Add(chargedSpitProjectile);
                projectilesList.Add(chargedSpitSmallDoTZone);
                projectilesList.Add(chargedSpitDoTZone);
                projectilesList.Add(chargedSpitChunkProjectile);

                var spitterLog = Utils.CreateUnlockableDef("Logs.SpitterBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SPITTER");
                unlockablesList.Add(spitterLog);

                //Skills.NormalSpit = spitterFactory.CreateNormalSpitSkill();
                SpitterFactory.Skills.Bite = spitterFactory.CreateBiteSkill();
                SpitterFactory.Skills.ChargedSpit = spitterFactory.CreateChargedSpitSkill();

                //sdList.Add(SpitterFactory.Skills.NormalSpit);
                sdList.Add(SpitterFactory.Skills.Bite);
                sdList.Add(SpitterFactory.Skills.ChargedSpit);

                //SkillFamilies.Primary = spitterFactory.CreateSkillFamily("SpitterPrimaryFamily", Skills.NormalSpit);
                SpitterFactory.SkillFamilies.Secondary = Utils.CreateSkillFamily("SpitterSecondaryFamily", SpitterFactory.Skills.Bite);
                SpitterFactory.SkillFamilies.Special = Utils.CreateSkillFamily("SpitterSpecialFamily", SpitterFactory.Skills.ChargedSpit);

                //sfList.Add(SpitterFactory.SkillFamilies.Primary);
                sfList.Add(SpitterFactory.SkillFamilies.Secondary);
                sfList.Add(SpitterFactory.SkillFamilies.Special);

                var spitterBody = assets.First(body => body.name == "SpitterBody");
                SpitterFactory.SpitterBody = spitterFactory.CreateSpitterBody(spitterBody, spitterIcon, spitterLog, skinsLookup);
                bodyList.Add(SpitterFactory.SpitterBody);

                var spitterMaster = assets.First(master => master.name == "SpitterMaster");
                SpitterFactory.SpitterMaster = spitterFactory.CreateSpitterMaster(spitterMaster, spitterBody);
                masterList.Add(SpitterFactory.SpitterMaster);

                SpitterFactory.SpawnCards.cscSpitterDefault = spitterFactory.CreateCard("cscSpitterDefault", spitterMaster, SpitterFactory.SkinDefs.Default, spitterBody);
                DirectorAPI.DirectorCardHolder dchSpitterDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = new DirectorCard
                    {
                        spawnCard = SpitterFactory.SpawnCards.cscSpitterDefault,
                        selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = true,
                        minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                    },
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.WetlandAspect);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.Custom, "FBLScene");
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.VoidCell);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.ArtifactReliquary);

                SpitterFactory.SpawnCards.cscSpitterLakes = spitterFactory.CreateCard("cscSpitterLakes", spitterMaster, SpitterFactory.SkinDefs.Lakes, spitterBody);
                DirectorAPI.DirectorCardHolder dchSpitterLakes = new DirectorAPI.DirectorCardHolder
                {
                    Card = new DirectorCard
                    {
                        spawnCard = SpitterFactory.SpawnCards.cscSpitterLakes,
                        selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = true,
                        minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                    },
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterLakes, false, DirectorAPI.Stage.VerdantFalls);

                SpitterFactory.SpawnCards.cscSpitterSulfur = spitterFactory.CreateCard("cscSpitterSulfur", spitterMaster, SpitterFactory.SkinDefs.Sulfur, spitterBody);
                DirectorAPI.DirectorCardHolder dhcSpitterSulfur = new DirectorAPI.DirectorCardHolder
                {
                    Card = new DirectorCard
                    {
                        spawnCard = SpitterFactory.SpawnCards.cscSpitterSulfur,
                        selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = true,
                        minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                    },
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dhcSpitterSulfur, false, DirectorAPI.Stage.SulfurPools);

                SpitterFactory.SpawnCards.cscSpitterDepths = spitterFactory.CreateCard("cscSpitterDepths", spitterMaster, SpitterFactory.SkinDefs.Depths, spitterBody);
                DirectorAPI.DirectorCardHolder dhcSpitterDepths = new DirectorAPI.DirectorCardHolder
                {
                    Card = new DirectorCard
                    {
                        spawnCard = SpitterFactory.SpawnCards.cscSpitterDepths,
                        selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = true,
                        minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                    },
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dhcSpitterDepths, false, DirectorAPI.Stage.AbyssalDepths);
                DirectorAPI.Helpers.AddNewMonsterToStage(dhcSpitterDepths, false, DirectorAPI.Stage.AbyssalDepthsSimulacrum);

                stateList.Add(typeof(ModdedEntityStates.Spitter.Bite));
                stateList.Add(typeof(ModdedEntityStates.Spitter.SpawnState));
                //stateList.Add(typeof(ModdedEntityStates.Spitter.NormalSpit));
                stateList.Add(typeof(ModdedEntityStates.Spitter.ChargeChargedSpit));
                stateList.Add(typeof(ModdedEntityStates.Spitter.FireChargedSpit));
                stateList.Add(typeof(ModdedEntityStates.Spitter.DeathDance));

                //escList.Add(spitterFactory.CreateSpawnStateConfiguration());
                //escList.Add(spitterFactory.CreateBiteStateConfiguration());
                #endregion

                #region Colossus
                var colossusFactory = new ColossusFactory();

                var stompProjectile = colossusFactory.CreateStompProjectile();
                ModdedEntityStates.Colossus.Stomp.StompBase.projectilePrefab = stompProjectile;
                projectilesList.Add(stompProjectile);

                var stompEffect = colossusFactory.CreateStompEffect();
                ModdedEntityStates.Colossus.Stomp.StompBase.stompEffectPrefab = stompEffect;
                effectsList.Add(new EffectDef(stompEffect));

                var deathFallEffect = colossusFactory.CreateDeathFallEffect();
                effectsList.Add(new EffectDef(deathFallEffect));
                OurAnimationEvents.effectDictionary.Add(0, deathFallEffect);

                var death2Effect = colossusFactory.CreateDeath2Effect();
                effectsList.Add(new EffectDef(death2Effect));
                OurAnimationEvents.effectDictionary.Add(1, death2Effect);

                var clapEffect = colossusFactory.CreateClapEffect();
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.clapEffect = clapEffect;

                var flyingRockGhost = colossusFactory.CreateFlyingRocksGhost();
                Enemies.Colossus.FloatingRocksController.flyingRockPrefab = flyingRockGhost;
                var flyingRockProjectile = colossusFactory.CreateFlyingRockProjectile(flyingRockGhost);
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.projectilePrefab = flyingRockProjectile;
                projectilesList.Add(flyingRockProjectile);

                var laserBarrageProjectile = colossusFactory.CreateLaserBarrageProjectile();
                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.projectilePrefab = laserBarrageProjectile;
                projectilesList.Add(laserBarrageProjectile);

                var spawnEffect = colossusFactory.CreateSpawnEffect();
                ModdedEntityStates.Colossus.SpawnState.burrowPrefab = spawnEffect;
                effectsList.Add(new EffectDef(spawnEffect));

                //var laserEffect = colossusFactory.CreateLaserEffect();
                //ModdedEntityStates.Junk.Colossus.HeadLaser.HeadLaserAttack.beamPrefab = laserEffect;

                ColossusFactory.Skills.Stomp = colossusFactory.CreateStompSkill();
                ColossusFactory.Skills.StoneClap = colossusFactory.CreateStoneClapSkill();
                ColossusFactory.Skills.LaserBarrage = colossusFactory.CreateLaserBarrageSkill();
                //ColossusFactory.Skills.HeadLaser = colossusFactory.CreateHeadLaserSkill();
                sdList.Add(ColossusFactory.Skills.Stomp);
                sdList.Add(ColossusFactory.Skills.StoneClap);
                sdList.Add(ColossusFactory.Skills.LaserBarrage);
                //sdList.Add(ColossusFactory.Skills.HeadLaser);

                ColossusFactory.SkillFamilies.Primary = Utils.CreateSkillFamily("ColossusPrimaryFamily", ColossusFactory.Skills.Stomp);
                ColossusFactory.SkillFamilies.Secondary = Utils.CreateSkillFamily("ColossusSecondaryFamily", ColossusFactory.Skills.StoneClap);
                ColossusFactory.SkillFamilies.Utility = Utils.CreateSkillFamily("ColossusUtilityFamily", ColossusFactory.Skills.LaserBarrage);
                //ColossusFactory.SkillFamilies.Special = Utils.CreateSkillFamily("ColossusSpecialFamily", ColossusFactory.Skills.HeadLaser);
                sfList.Add(ColossusFactory.SkillFamilies.Primary);
                sfList.Add(ColossusFactory.SkillFamilies.Secondary);
                sfList.Add(ColossusFactory.SkillFamilies.Utility);
                //sfList.Add(ColossusFactory.SkillFamilies.Special);

                var colossusBody = assets.First(body => body.name == "ColossusBody");
                ColossusFactory.ColossusBody = colossusFactory.CreateColossusBody(colossusBody, null, null, null);
                bodyList.Add(ColossusFactory.ColossusBody);

                var colossusMaster = assets.First(master => master.name == "ColossusMaster");
                ColossusFactory.ColossusMaster = colossusFactory.CreateColossusMaster(colossusMaster, colossusBody);
                masterList.Add(ColossusFactory.ColossusMaster);

                stateList.Add(typeof(ModdedEntityStates.Colossus.SpawnState));
                stateList.Add(typeof(ModdedEntityStates.Colossus.RockClap.RockClapEnd));
                stateList.Add(typeof(ModdedEntityStates.Colossus.RockClap.RockClapStart));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompEnter));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompBase));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompL));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompR));
                stateList.Add(typeof(ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageStart));
                stateList.Add(typeof(ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack));
                stateList.Add(typeof(ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageEnd));

                Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusDefault = colossusFactory.CreateCard("cscColossusDefault", colossusMaster);
                DirectorCard dcColossusDefault = new DirectorCard
                {
                    spawnCard = ColossusFactory.SpawnCards.cscColossusDefault,
                    selectionWeight = EnemiesReturnsConfiguration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Colossus.MinimumStageCompletion.Value
                };

                DirectorAPI.DirectorCardHolder dchColossusDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };

                //DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusDefault, false, DirectorAPI.Stage.WetlandAspect);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusDefault, false, DirectorAPI.Stage.SiphonedForest); // TODO: swap to snowy
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusDefault, false, DirectorAPI.Stage.TitanicPlains); // TODO: swap to grassy
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusDefault, false, DirectorAPI.Stage.AbandonedAqueduct); // TODO: swap to sandy
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusDefault, false, DirectorAPI.Stage.RallypointDelta); // TODO: swap to snowy

                // TODO: variants have their own family cards
                var golemFamilyCard = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion();
                int num = Utils.FindCategoryIndexByName(golemFamilyCard, "Champions");
                if (num >= 0)
                {
                    golemFamilyCard.AddCard(num, dcColossusDefault);
                }

                //stateList.Add(typeof(HeadLaserAttack));
                //stateList.Add(typeof(HeadLaserEnd));
                //stateList.Add(typeof(HeadLaserStart));
                #endregion

                _contentPack.bodyPrefabs.Add(bodyList.ToArray());
                _contentPack.masterPrefabs.Add(masterList.ToArray());
                _contentPack.skillDefs.Add(sdList.ToArray());
                _contentPack.skillFamilies.Add(sfList.ToArray());
                _contentPack.entityStateTypes.Add(stateList.ToArray());
                _contentPack.effectDefs.Add(effectsList.ToArray());
                _contentPack.projectilePrefabs.Add(projectilesList.ToArray());
                _contentPack.unlockableDefs.Add(unlockablesList.ToArray());
                //_contentPack.entityStateConfigurations.Add(escList.ToArray());
                stopwatch.Stop();
                Log.Info("Characters created in " + stopwatch.elapsedSeconds);
            }));

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<AnimationCurveDef[]>)((assets) =>
            {
                var acdLaserBarrage = assets.First(acd => acd.name == "LaserBarrageLightIntencity");
                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.intencityGraph = acdLaserBarrage.curve;
            }));

            totalStopwatch.Stop();
            Log.Info("Total loading time: " + totalStopwatch.elapsedSeconds);

            yield break;
        }

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

        private static void LoadSoundBanks(string soundbanksFolderPath)
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
