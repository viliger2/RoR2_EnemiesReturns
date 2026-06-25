using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.ContactLight;
using EnemiesReturns.Enemies.ContactLight.TempleGuard;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

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

            AssetBundle assetBundleStages = null;
            yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleContactLightStagesName), args.progressReceiver, (resultAssetBundle) => assetBundleStages = resultAssetBundle);

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Material[]>)((assets) =>
            {
                SwapMaterials(assets);

                ModdedEntityStates.ContactLight.CargoHoldDoors.Opening.matTerminalGreen = assets.First(material => material.name == "matTerminalGreen");
                ModdedEntityStates.ContactLight.BonusRoomDoors.Opening.openedMaterial = assets.First(material => material.name == "matKeypadOpened");

                ModdedEntityStates.ContactLight.TempleGuard.UtilityOverclock.Overclock.overclockMaterial = assets.First(material => material.name == "matTempleGuardOverclockOverlay");
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SceneDef[]>)((assets) =>
            {
                Content.Stages.ContactLight = assets.First(sd => sd.cachedName == "enemiesreturns_contactlight");

                _contentPack.sceneDefs.Add(assets);
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                _contentPack.bodyPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterBody>(out _)).ToArray());
                _contentPack.masterPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterMaster>(out _)).ToArray());
                _contentPack.projectilePrefabs.Add(assets.Where(asset => asset.TryGetComponent<ProjectileController>(out _)).ToArray());
                _contentPack.effectDefs.Add(Array.ConvertAll(assets.Where(asset => asset.TryGetComponent<EffectComponent>(out _)).ToArray(), item => new EffectDef(item)));

                _contentPack.networkedObjectPrefabs.Add(assets.Where(asset => 
                    asset.TryGetComponent<NetworkIdentity>(out _) 
                        && !(asset.TryGetComponent<CharacterBody>(out _) 
                            || asset.TryGetComponent<CharacterMaster>(out _) 
                            || asset.TryGetComponent<ProjectileController>(out _))).ToArray());

                Content.BodyPrefabs.TempleGuardBody = new TempleGuardBody().SetupBody(assets.First(asset => asset.name == "TempleGuardBody"));

                Content.MasterPrefabs.TempleGuardMaster = assets.First(asset => asset.name == "TempleGuardMaster");

                ModdedEntityStates.ContactLight.TempleGuard.Primary.FirePrimary.projectilePrefab = assets.First(asset => asset.name == "TempleGuardianPrimaryProjectile");
                ModdedEntityStates.ContactLight.TempleGuard.Primary.FirePrimary.primaryEffect = assets.First(asset => asset.name == "TempleGuardianPrimaryFiring");
                ModdedEntityStates.ContactLight.TempleGuard.Primary.ChargePrimary.effectPrefab = assets.First(asset => asset.name == "TempleGuardianPrimaryCharge");
                ModdedEntityStates.ContactLight.TempleGuard.UtilityOverclock.Overclock.preShieldEffect = assets.First(asset => asset.name == "TempleGuardOverclockCharge");

                ModdedEntityStates.ContactLight.SwordHilt.SpawnPortal.portalContactLight = assets.First(asset => asset.name == "PortalContactLight");
                Enemies.ContactLight.SetupContactLight.swordHilt = assets.First(asset => asset.name == "SwordHiltPortal");

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

                var wardrobe = assets.First(prefab => prefab.name == "WardrobeInteractable");
                var pickerController = wardrobe.GetComponent<Behaviors.SkinDefPicker.SkinDefPickerController>();
                pickerController.panelPrefab = SetupContactLight.CreateSkinDefPickerPanel();
                PrefabAPI.RegisterNetworkPrefab(wardrobe);

                Enemies.ContactLight.SetupContactLight.wardrobe = wardrobe;
                //nopList.Add(SetupContactLight.wardrobe);

                //nopList.Add(assets.First(prefab => prefab.name == "SurgicalBed"));
                //nopList.Add(assets.First(prefab => prefab.name == "EquipmentChest"));
                //nopList.Add(assets.First(prefab => prefab.name == "NanoChest"));
                //nopList.Add(assets.First(prefab => prefab.name == "GaussCannon"));
                //nopList.Add(assets.First(asset => asset.name == "SwordShardInteractable"));
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ItemDef[]>)((assets) =>
            {
                _contentPack.itemDefs.Add(assets);
                Content.Items.AccessCard = assets.First(item => item.name == "AccessCard");
                Content.Items.AdrenalineCore = assets.First(item => item.name == "AdrenalineCore");
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<BuffDef[]>)((assets) =>
            {
                _contentPack.buffDefs.Add(assets);
                Content.Buffs.ProvidenceImmuneToDamage = assets.First(buff => buff.name == "ProvidenceImmuneToDamage");
                Content.Buffs.AdrenalineCoreProtection = assets.First(buff => buff.name == "AdrenalineCoreProtection");
                Content.Buffs.TempleGuardOverclock = assets.First(buff => buff.name == "bdTempleGuardOverclock");
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<UnlockableDef[]>)((assets) =>
            {
                _contentPack.unlockableDefs.Add(assets);
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<EquipmentDef[]>)((assets) =>
            {
                Content.Equipment.EliteSlayer = assets.First(equipment => equipment.name == "EliteSlayer");

                _contentPack.equipmentDefs.Add(assets);
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SpawnCard[]>)((assets) =>
            {
                Enemies.ContactLight.SetupContactLight.iscSwordShard = (InteractableSpawnCard)assets.First(sc => sc.name == "iscSwordShard");
            }));
            
            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Sprite[]>)((assets) =>
            {
                var sprite = assets.First(sprite => sprite.name == "texArcaneCircleProviMask");
                Equipment.EliteSlayer.EliteSlayer.EliteSlayerIndicator = Enemies.ContactLight.SetupContactLight.CreateEliteSlayerIndicator(sprite);

            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SkillFamily[]>)((assets) =>
            {
                _contentPack.skillFamilies.Add(assets);
            }));

            yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SkillDef[]>)((assets) =>
            {
                _contentPack.skillDefs.Add(assets);
            }));

            Content.CostTypes.AccessCard = new CostTypeDef()
            {
                name = "EnemiesReturnsKeycardCost",
                costStringFormatToken = "ENEMIES_RETURNS_CONTACT_LIGHT_COST_KEYCARD_FORMAT",
                isAffordable = delegate (CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
                {
                    if (context.activator)
                    {
                        var characterBody = context.activator.GetComponent<CharacterBody>();
                        if (characterBody)
                        {
                            var inventory = characterBody.inventory;
                            if (inventory && !inventory.inventoryDisabled)
                            {
                                return inventory.GetItemCountEffective(Content.Items.AccessCard) > 0;
                            }
                        }
                    }
                    return false;
                },
                payCost = delegate (CostTypeDef.PayCostContext context, CostTypeDef.PayCostResults result)
                {
                    if (context.activatorBody && context.activatorBody.inventory)
                    {
                        var inventory = context.activatorBody.inventory;
                        Inventory.ItemTransformation itemTransformation = new Inventory.ItemTransformation()
                        {
                            originalItemIndex = Content.Items.AccessCard.itemIndex,
                            newItemIndex = ItemIndex.None,
                            maxToTransform = 1,
                        };
                        if (itemTransformation.TryTransform(inventory, out var result2))
                        {
                            result.AddTakenItemsFromTransformation(in result2);
                        }
                    }
                },
                colorIndex = ColorCatalog.ColorIndex.VoidCoin
            };

            yield break;
        }
    }
}
