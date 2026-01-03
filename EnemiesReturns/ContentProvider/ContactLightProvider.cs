using EnemiesReturns.EditorHelpers;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using EnemiesReturns.Enemies.Judgement;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        public const string AssetBundleContactLightStagesAssetsName = "enemiesreturns_contactlight_stages_assets";

        public const string AssetBundleContactLightStagesName = "enemiesreturns_contactlight_stages_scenes";

        public static CharacterSpawnCard cscTempleGuardian;

        public IEnumerator CreateContactLightAsync(LoadStaticContentAsyncArgs args, Dictionary<string, Sprite> iconLookup, Dictionary<string, Texture2D> rampLookups, Dictionary<string, AnimationCurveDef> acdLookup, string assetBundleFolderPath)
        {
            AssetBundle assetBundleStagesAssets = null;
            yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleContactLightStagesAssetsName), args.progressReceiver, (resultAssetBundle) => assetBundleStagesAssets = resultAssetBundle);

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Material[]>)((assets) =>
            {
                SwapMaterials(assets);
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SceneDef[]>)((assets) =>
            {
                Content.Stages.ContactLight = assets.First(sd => sd.cachedName == "enemiesreturns_contactlight");

                _contentPack.sceneDefs.Add(assets);
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ModdedSkinDefParams[]>)((assets) =>
            {
                var templeGuardians = assets.Where(item => item.name == "skinLunarGolemTempleGuardian").First();
                var newSkin = templeGuardians.CreateSkinDef();
                if (newSkin)
                {
                    var bodyObject = AssetAsyncReferenceManager<GameObject>.LoadAsset(templeGuardians.bodyPrefab).WaitForCompletion();
                    var modelSkinController = bodyObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<ModelSkinController>();
                    HG.ArrayUtils.ArrayAppend(ref modelSkinController.skins, in newSkin);

                    var cscLunarGolem = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarGolem.cscLunarGolem_asset).WaitForCompletion();
                    cscTempleGuardian = UnityEngine.Object.Instantiate(cscLunarGolem);
                    cscTempleGuardian.name = "cscTempleGuardian";

                    cscTempleGuardian.loadout = new SerializableLoadout
                    {
                        bodyLoadouts = new SerializableLoadout.BodyLoadout[]
                        {
                            new SerializableLoadout.BodyLoadout()
                            {
                                body = bodyObject.GetComponent<CharacterBody>(),
                                skinChoice = newSkin,
                                skillChoices = Array.Empty<SerializableLoadout.BodyLoadout.SkillChoice>() // yes, we need it
                            }
                        }
                    };
                }

                //var moddedSkinDefList = assets.Where(item => item.name.Contains("Judgement")).ToArray();
                //foreach (var moddedSkinDef in moddedSkinDefList)
                //{
                //    var newSkin = moddedSkinDef.CreateSkinDef();
                //    if (newSkin)
                //    {
                //        var bodyObject = AssetAsyncReferenceManager<GameObject>.LoadAsset(moddedSkinDef.bodyPrefab).WaitForCompletion();
                //        var anointedSkin = EnemiesReturns.Enemies.Judgement.AnointedSkins.CreateAnointedSkin(bodyObject.name, moddedSkinDef.CreateSkinDef(), false);
                //        var modelSkinController = bodyObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<ModelSkinController>();
                //        HG.ArrayUtils.ArrayAppend(ref modelSkinController.skins, in anointedSkin);
                //    }
                //}
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                _contentPack.bodyPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterBody>(out _)).ToArray());
                _contentPack.masterPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterMaster>(out _)).ToArray());
                _contentPack.projectilePrefabs.Add(assets.Where(asset => asset.TryGetComponent<ProjectileController>(out _)).ToArray());
                _contentPack.effectDefs.Add(Array.ConvertAll(assets.Where(asset => asset.TryGetComponent<EffectComponent>(out _)).ToArray(), item => new EffectDef(item)));

                ModdedEntityStates.ContactLight.Providence.P1.Orbs.FireSingleOrb.projectilePrefab = assets.First(prefab => prefab.name == "OrbProjectile");
                ModdedEntityStates.ContactLight.Providence.P1.Utility.Disappear.predictedPositionEffect = assets.First(prefab => prefab.name == "LandingEffect");
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ItemDef[]>)((assets) =>
            {
                _contentPack.itemDefs.Add(assets);
            }));

            AssetBundle assetBundleStages = null;
            yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleContactLightStagesName), args.progressReceiver, (resultAssetBundle) => assetBundleStages = resultAssetBundle);

            yield break;
        }
    }
}
