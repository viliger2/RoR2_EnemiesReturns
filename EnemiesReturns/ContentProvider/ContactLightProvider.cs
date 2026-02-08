using EnemiesReturns.EditorHelpers;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        public const string AssetBundleContactLightStagesAssetsName = "enemiesreturns_contactlight_stages_assets";

        public const string AssetBundleContactLightStagesName = "enemiesreturns_contactlight_stages_scenes";

        public static CharacterSpawnCard cscTempleGuardian;

        public static GameObject TempleGuard2Body;

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
                var bodyObject = AssetAsyncReferenceManager<GameObject>.LoadAsset(templeGuardians.bodyPrefab).WaitForCompletion();
                var modelSkinController = bodyObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<ModelSkinController>();
                if (newSkin)
                {
                    HG.ArrayUtils.ArrayAppend(ref modelSkinController.skins, in newSkin);
                }

                var templeGuardians2 = assets.Where(item => item.name == "skinLunarGolemTempleGuardian2").First();
                var newSkin2 = templeGuardians2.CreateSkinDef();
                if (newSkin2)
                {
                    HG.ArrayUtils.ArrayAppend(ref modelSkinController.skins, in newSkin2);
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

                //TempleGuard2Body = assets.First(prefab => prefab.name == "TempleGuard2Body");
                //TempleGuard2Body.GetComponentInChildren<ModelSkinController>()._avatarAddress = new AssetReferenceT<Avatar>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarGolem.mdlLunarGolem_fbx);

                ModdedEntityStates.ContactLight.Providence.P1.Orbs.FireSingleOrb.projectilePrefab = assets.First(prefab => prefab.name == "OrbProjectile");
                ModdedEntityStates.ContactLight.Providence.P1.Utility.Disappear.staticPredictedPositionEffect = assets.First(prefab => prefab.name == "LandingEffect");

                ModdedEntityStates.ContactLight.Providence.P2.Primary.ProjectileSwingsWithClones.cloneEffect = assets.First(prefab => prefab.name == "ProvidenceP2PrimaryShadowClone");
                ModdedEntityStates.ContactLight.Providence.P2.Secondary.DashAttack.projectileClone = assets.First(prefab => prefab.name == "ProvidenceSecondaryCloneProjectile");
                ModdedEntityStates.ContactLight.Providence.P2.Special.FireRingsWithClones.cloneEffectPrefab = assets.First(prefab => prefab.name == "ProvidenceP2SpecialShadowClone");
                ModdedEntityStates.ContactLight.Providence.P2.Utility.FireClones.projectilePrefab = assets.First(prefab => prefab.name == "ProvidenceCloneUtilityPreProjectile");
                ModdedEntityStates.ContactLight.Providence.P2.Utility.FireClones.predictedPositionEffect = assets.First(prefab => prefab.name == "LandingEffect");

                //ModdedEntityStates.ContactLight.Providence.P2.Utility.Disappear.staticPredictedPositionEffect = assets.First(prefab => prefab.name == "LandingEffect");

                var orbProjectilePrefab = assets.First(prefab => prefab.name == "ProviTwoSwingsProjectile");
                orbProjectilePrefab.GetComponent<ProjectileController>().ghostPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarWisp.LunarWispTrackingBombGhost_prefab).WaitForCompletion();
                ModdedEntityStates.ContactLight.Providence.P1.Primary.TwoSwingsIntoProjectile.FireProjectiles.staticProjecilePrefab = orbProjectilePrefab;
                ModdedEntityStates.ContactLight.Providence.P2.Primary.TwoSwingsIntoProjectile.FireProjectiles.staticProjecilePrefab = orbProjectilePrefab;

                ModdedEntityStates.ContactLight.Providence.P2.Primary.TwoSwingsIntoProjectile.LeftRightSwing.cloneProjectile = assets.First(prefab => prefab.name == "ProviShadowPrimary");
                ModdedEntityStates.ContactLight.Providence.P1.Primary.TwoSwingsIntoProjectile.LeftRightSwing.cloneProjectile = assets.First(prefab => prefab.name == "ProviShadowPrimary");

                ModdedEntityStates.ContactLight.Providence.P1.SkullsAttack.SkullsAttack.staticEffectPrefab = assets.First(prefab => prefab.name == "LandingEffect");
                ModdedEntityStates.ContactLight.Providence.P1.SkullsAttack.SkullsAttack.staticProjectilePrefab = assets.First(prefab => prefab.name == "ProviShadowPrimary");

                ModdedEntityStates.ContactLight.Providence.P2.SkullsAttack.SkullsAttack.staticEffectPrefab = assets.First(prefab => prefab.name == "LandingEffect");
                ModdedEntityStates.ContactLight.Providence.P2.SkullsAttack.SkullsAttack.staticProjectilePrefab = assets.First(prefab => prefab.name == "ProviShadowPrimary");

                ModdedEntityStates.ContactLight.Providence.P3.Special.SpawnRotatingLaser.projectilePrefab = assets.First(prefab => prefab.name == "LaserProjectile");
                ModdedEntityStates.ContactLight.Providence.P3.Utility.FireClones.projectilePrefab = assets.First(prefab => prefab.name == "ProvidenceCloneUtilityPreProjectile");
                ModdedEntityStates.ContactLight.Providence.P3.Utility.FireClones.predictedPositionEffect = assets.First(prefab => prefab.name == "LandingEffect");
                ModdedEntityStates.ContactLight.Providence.P3.Secondary.SkullsAttack.staticEffectPrefab = assets.First(prefab => prefab.name == "LandingEffect");
                ModdedEntityStates.ContactLight.Providence.P3.Secondary.SkullsAttack.staticProjectilePrefab = assets.First(prefab => prefab.name == "ProviShadowPrimary");
                ModdedEntityStates.ContactLight.Providence.P3.SwignWithFanClones.ProjectileSwingsWithClones.projectilePrefab = assets.First(prefab => prefab.name == "ProviShadowPrimary");

                ModdedEntityStates.ContactLight.Providence.P1.SkullsAttack.SkullsAttack.staticEffectRedPrefab = assets.First(prefab => prefab.name == "LandingEffectRed");
                ModdedEntityStates.ContactLight.Providence.P2.SkullsAttack.SkullsAttack.staticEffectRedPrefab = assets.First(prefab => prefab.name == "LandingEffectRed");
                ModdedEntityStates.ContactLight.Providence.P3.Secondary.SkullsAttack.staticEffectRedPrefab = assets.First(prefab => prefab.name == "LandingEffectRed");

            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ItemDef[]>)((assets) =>
            {
                _contentPack.itemDefs.Add(assets);
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<BuffDef[]>)((assets) =>
            {
                _contentPack.buffDefs.Add(assets);
                Content.Buffs.ProvidenceImmuneToDamage = assets.First(buff => buff.name == "ProvidenceImmuneToDamage");
            }));

            AssetBundle assetBundleStages = null;
            yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleContactLightStagesName), args.progressReceiver, (resultAssetBundle) => assetBundleStages = resultAssetBundle);

            yield break;
        }
    }
}
