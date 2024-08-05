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
using static EnemiesReturns.Enemies.Spitter.SpitterFactory;
using EnemiesReturns.Enemies.Spitter;
using R2API;
using EnemiesReturns.Enemies.Colossus;

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
            _contentPack.identifier = identifier;

            string soundbanksFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), SoundbankFolder);
            LoadSoundBanks(soundbanksFolderPath);

            string assetBundleFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), AssetBundleFolder);

            AssetBundle assetbundle = null;
            yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleName), args.progressReceiver, (resultAssetBundle) => assetbundle = resultAssetBundle);

            Dictionary<string, Material> skinsLookup = new Dictionary<string, Material>();

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Material[]>)((assets) =>
            {
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
            }));

            #region Sprites
            Texture2D spitterIcon = null;
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Texture2D[]>)((assets) =>
            {
                spitterIcon = assets.First(sprite => sprite.name == "texSpitterIcon");
            }));
            #endregion
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
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

                SpitterFactory.biteEffectPrefab = spitterFactory.CreateBiteEffect();
                effectsList.Add(new EffectDef(SpitterFactory.biteEffectPrefab));

                //SpitterFactory.normalSpitProjectile = spitterFactory.CreateNormalSpitProjectile();
                var chargedSpitSmallDoTZone = spitterFactory.CreatedChargedSpitSmallDoTZone();
                var chargedSpitDoTZone = spitterFactory.CreateChargedSpitDoTZone();
                var chargedSpitChunkProjectile = spitterFactory.CreateChargedSpitSplitProjectile(chargedSpitSmallDoTZone);
                SpitterFactory.chargedSpitProjectile = spitterFactory.CreateChargedSpitProjectile(chargedSpitDoTZone, chargedSpitChunkProjectile);;
                //projectilesList.Add(SpitterFactory.normalSpitProjectile);
                projectilesList.Add(SpitterFactory.chargedSpitProjectile);
                projectilesList.Add(chargedSpitSmallDoTZone);
                projectilesList.Add(chargedSpitDoTZone);
                projectilesList.Add(chargedSpitChunkProjectile);

                var spitterLog = Utils.CreateUnlockableDef("Logs.SpitterBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SPITTER");
                unlockablesList.Add(spitterLog);

                //Skills.NormalSpit = spitterFactory.CreateNormalSpitSkill();
                Skills.Bite = spitterFactory.CreateBiteSkill();
                Skills.ChargedSpit = spitterFactory.CreateChargedSpitSkill();

                //sdList.Add(SpitterFactory.Skills.NormalSpit);
                sdList.Add(SpitterFactory.Skills.Bite);
                sdList.Add(SpitterFactory.Skills.ChargedSpit);

                //SkillFamilies.Primary = spitterFactory.CreateSkillFamily("SpitterPrimaryFamily", Skills.NormalSpit);
                SkillFamilies.Secondary = Utils.CreateSkillFamily("SpitterSecondaryFamily", Skills.Bite);
                SkillFamilies.Special = Utils.CreateSkillFamily("SpitterSpecialFamily", Skills.ChargedSpit);

                //sfList.Add(SpitterFactory.SkillFamilies.Primary);
                sfList.Add(SpitterFactory.SkillFamilies.Secondary);
                sfList.Add(SpitterFactory.SkillFamilies.Special);

                var spitterBody = assets.First(body => body.name == "SpitterBody");
                SpitterFactory.spitterBody = spitterFactory.CreateSpitterBody(spitterBody, spitterIcon, spitterLog, skinsLookup);
                bodyList.Add(SpitterFactory.spitterBody);

                var spitterMaster = assets.First(master => master.name == "SpitterMaster");
                spitterMaster = spitterFactory.CreateSpitterMaster(spitterMaster, spitterBody);
                masterList.Add(spitterMaster);

                SpitterFactory.SpawnCards.cscSpitterDefault = spitterFactory.CreateCard("cscSpitterDefault", spitterMaster, SkinDefs.Default, spitterBody);
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

                SpitterFactory.SpawnCards.cscSpitterLakes = spitterFactory.CreateCard("cscSpitterLakes", spitterMaster, SkinDefs.Lakes, spitterBody);
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

                SpitterFactory.SpawnCards.cscSpitterSulfur = spitterFactory.CreateCard("cscSpitterSulfur", spitterMaster, SkinDefs.Sulfur, spitterBody);
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

                SpitterFactory.SpawnCards.cscSpitterDepths = spitterFactory.CreateCard("cscSpitterDepths", spitterMaster, SkinDefs.Depths, spitterBody);
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
                //var colossusFactory = new ColossusFactory();
                //var colossusBody = assets.First(body => body.name == "ColossusBody");
                //ColossusFactory.colossusBody = colossusFactory.CreateColossusBody(colossusBody, null, null, null);
                //bodyList.Add(ColossusFactory.colossusBody);

                //var colossusMaster = assets.First(master => master.name == "ColossusMaster");
                //colossusMaster = colossusFactory.CreateColossusMaster(colossusMaster, colossusBody);
                //masterList.Add(colossusMaster);

                //stateList.Add(typeof(ModdedEntityStates.Colossus.SpawnState));
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
            }));
                
            yield break;
        }

        private IEnumerator LoadAssetBundle(string assetBundleFullPath, IProgress<float> progress, Action<AssetBundle> onAssetBundleLoaded)
        {
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundleFullPath);
            while (!assetBundleCreateRequest.isDone)
            {
                progress.Report(assetBundleCreateRequest.progress);
                yield return null;
            }

            onAssetBundleLoaded(assetBundleCreateRequest.assetBundle);

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
