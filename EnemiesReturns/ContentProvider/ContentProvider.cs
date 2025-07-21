using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Reflection;
using Rewired.Utils.Classes.Utility;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        public string identifier => EnemiesReturnsPlugin.GUID + "." + nameof(ContentProvider);

        private readonly ContentPack _contentPack = new ContentPack();

        public const string AssetBundleName = "enemiesreturns";
        public const string AssetBundleStagesName = "enemiesreturnsstagesscenes";
        public const string AssetBundleStagesAssetsName = "enemiesreturnsstagesassets";
        public const string AssetBundleFolder = "AssetBundles";

        public const string SoundbankFolder = "Soundbanks";
        public const string SoundsSoundBankFileName = "EnemiesReturnsSounds";
        public const string MusicSoundBankFileName = "EnemiesReturnsMusic";
        public const string InitSoundBankFileName = "EnemiesReturnsInit";

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
                SwapMaterials(assets);
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

            yield return CreateJudgementAsync(args, iconLookup, rampLookups, acdLookup, assetBundleFolderPath);

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                CreateSpitter(assets, iconLookup);

                CreateColossus(assets, iconLookup, acdLookup);

                CreateIfrit(assets, iconLookup, texLavaCrackRound, acdLookup);

                CreateMechanicalSpider(assets, iconLookup, acdLookup);

                CreateLynxTribe(assets, iconLookup, acdLookup, rampLookups);

                CreateArcherBug(assets, iconLookup);

                CreateSandCrab(assets, iconLookup);

                stopwatch.Stop();
                Log.Info("Characters created in " + stopwatch.elapsedSeconds);
            }));


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

        }

        void SwapMaterials(Material[] assets)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var materials = assets;

            if (materials != null)
            {
                foreach (Material material in materials)
                {
                    if (!ShaderLookup.TryGetValue(material.shader.name.ToLower(), out var matName))
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
            if (!MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc();
                MaterialCache.Add(materialName, material);
            }
            return material;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Texture2D, Material> materialCreateFunc, Texture2D texture)
        {
            if (!MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc(texture);
                MaterialCache.Add(materialName, material);
            }
            return material;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Material, Material> materialCreateFunc, Material materialOrig)
        {
            if (!MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc(materialOrig);
                MaterialCache.Add(materialName, material);
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

            akResult = AkSoundEngine.LoadBank(InitSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {InitSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {InitSoundBankFileName} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(MusicSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {MusicSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {MusicSoundBankFileName} " +
                    $"Error code : {akResult}");
            }

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
