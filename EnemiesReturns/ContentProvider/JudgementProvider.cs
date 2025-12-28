using EnemiesReturns.Components;
using EnemiesReturns.Configuration.Judgement;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Judgement;
using EnemiesReturns.Enemies.Judgement.Arraign;
using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam;
using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
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
        public const string AssetBundleJudgementStagesAssetsName = "enemiesreturns_judgement_stages_assets";

        public const string AssetBundleJudgementStagesName = "enemiesreturns_judgement_stages_scenes";

        public IEnumerator CreateJudgementAsync(LoadStaticContentAsyncArgs args, Dictionary<string, Sprite> iconLookup, Dictionary<string, Texture2D> rampLookups, Dictionary<string, AnimationCurveDef> acdLookup, string assetBundleFolderPath)
        {
            if (Judgement.Enabled.Value)
            {
                AssetBundle assetBundleStagesAssets = null;
                yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleJudgementStagesAssetsName), args.progressReceiver, (resultAssetBundle) => assetBundleStagesAssets = resultAssetBundle);

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Sprite[]>)((assets) =>
                {
                    foreach (var asset in assets)
                    {
                        if (asset.name == "texAnointedSkinIcon")
                        {
                            AnointedSkins.AnointedSkinIcon = asset;
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

                    AnointedSkins.aeonianEliteRamp = rampLookups["texRampAeonianElite"];
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<AnimationCurveDef[]>)((assets) =>
                {
                    zJunk.ModdedEntityStates.Judgement.Arraign.BaseSlashDash.speedCoefficientCurve = assets.First(acd => acd.name == "acdMoveSpeed").curve;

                    var acdOverlay = assets.First(acd => acd.name == "acdPreSlashMaterialAlpha").curve;
                    ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.PreSlash.acdOverlayAlpha = acdOverlay;
                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.PreSlash.acdOverlayAlpha = acdOverlay;
                    ModdedEntityStates.Judgement.Arraign.BaseSkyLeap.BaseExitSkyLeap.acdOverlayAlpha = acdOverlay;
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
                    EliteRamp.AddRamp(Content.Elites.Aeonian, rampLookups["texRampAeonianElite"]);

                    if (ModCompats.EliteReworksCompat.enabled)
                    {
                        ModCompats.EliteReworksCompat.ModifyAeonianElites(Content.Elites.Aeonian);
                    }
                    else
                    {
                        Content.Elites.Aeonian.healthBoostCoefficient = Judgement.AeonianEliteHealthMultiplier.Value;
                        Content.Elites.Aeonian.damageBoostCoefficient = Judgement.AeonianEliteDamageMultiplier.Value;
                    }

                    _contentPack.eliteDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SceneDef[]>)((assets) =>
                {
                    Content.Stages.OutOfTime = assets.First(sd => sd.cachedName == "enemiesreturns_outoftime");
                    Content.Stages.JudgementOutro = assets.First(sd => sd.cachedName == "enemiesreturns_judgementoutro");

                    _contentPack.sceneDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<SkillDef[]>)((assets) =>
                {
                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.ThreeHitCombo = assets.First(asset => (asset as ScriptableObject).name == "sdArraign3HitCombo");
                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.ThreeHitCombo.baseRechargeInterval = Configuration.Judgement.ArraignP1.ThreeHitComboCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.LeftRightSwing = assets.First(asset => (asset as ScriptableObject).name == "sdArraignRightLeftSwing");
                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.LeftRightSwing.baseRechargeInterval = Configuration.Judgement.ArraignP1.LeftRightSwingCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.SwordBeam = assets.First(asset => (asset as ScriptableObject).name == "sdArraignSwordBeam");
                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.SwordBeam.baseRechargeInterval = Configuration.Judgement.ArraignP1.SwordBeamCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.SkyDrop = assets.First(asset => (asset as ScriptableObject).name == "sdArraignSkyDrop");
                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.SkyDrop.baseRechargeInterval = Configuration.Judgement.ArraignP1.SkyDropCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.SwordThrow = assets.First(asset => (asset as ScriptableObject).name == "sdArraignSwordThrow");
                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.SwordThrow.baseRechargeInterval = Configuration.Judgement.ArraignP1.SwordBeamCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.LightningStrikes = assets.First(asset => (asset as ScriptableObject).name == "sdArraignLightningStrikes");
                    Enemies.Judgement.Arraign.ArraignBody.P1Skills.LightningStrikes.baseRechargeInterval = Configuration.Judgement.ArraignP1.LightningStrikesCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.ThreeHitCombo = assets.First(asset => (asset as ScriptableObject).name == "sdArraign3HitComboP2");
                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.ThreeHitCombo.baseRechargeInterval = Configuration.Judgement.ArraignP2.ThreeHitComboCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.LeftRightSwing = assets.First(asset => (asset as ScriptableObject).name == "sdArraignRightLeftSwingP2");
                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.LeftRightSwing.baseRechargeInterval = Configuration.Judgement.ArraignP2.LeftRightSwingCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.DashLeap = assets.First(asset => (asset as ScriptableObject).name == "sdArraignDashAttackP2");
                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.DashLeap.baseRechargeInterval = Configuration.Judgement.ArraignP2.DashLeapCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.SpearThrow = assets.First(asset => (asset as ScriptableObject).name == "sdArraignSpearThrow");
                    Enemies.Judgement.Arraign.ArraignBody.P2Skills.SpearThrow.baseRechargeInterval = Configuration.Judgement.ArraignP2.SpearThrowCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.HauntSkills.ClockAttack = assets.First(asset => (asset as ScriptableObject).name == "sdArraignaHauntClockAttack");
                    Enemies.Judgement.Arraign.ArraignBody.HauntSkills.ClockAttack.baseRechargeInterval = Configuration.Judgement.ArraignP2.ClockAttackCooldown.Value;

                    Enemies.Judgement.Arraign.ArraignBody.HauntSkills.SummonSkyLaser = assets.First(asset => (asset as ScriptableObject).name == "sdArraignHauntSkyLaser");
                    Enemies.Judgement.Arraign.ArraignBody.HauntSkills.SummonSkyLaser.baseRechargeInterval = Configuration.Judgement.ArraignP2.SkyLaserCooldown.Value;

                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<BuffDef[]>)((assets) =>
                {
                    Content.Buffs.AffixAeoninan = assets.First(buff => buff.name == "bdAeonian");
                    Content.Buffs.ImmuneToHammer = assets.First(buff => buff.name == "bdImmuneToHammer");
                    Content.Buffs.ImmuneToAllDamageExceptHammer = assets.First(buff => buff.name == "bdImmuneToAllDamageExceptHammer");
                    Content.Buffs.ImmuneToAllDamageExceptHammer.iconSprite = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_LunarGolem.texBuffLunarShellIcon_tif).WaitForCompletion();

                    _contentPack.buffDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Material[]>)((assets) =>
                {
                    SwapMaterials(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<GameObject[]>)((assets) =>
                {
                    _contentPack.bodyPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterBody>(out _)).ToArray());
                    _contentPack.masterPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterMaster>(out _)).ToArray());
                    _contentPack.projectilePrefabs.Add(assets.Where(asset => asset.TryGetComponent<ProjectileController>(out _)).ToArray());
                    _contentPack.effectDefs.Add(Array.ConvertAll(assets.Where(asset => asset.TryGetComponent<EffectComponent>(out _)).ToArray(), item => new EffectDef(item)));

                    var p1Body = assets.First(asset => asset.name == "ArraignP1Body");
                    Enemies.Judgement.Arraign.ArraignBody.ArraignP1Body = ArraignBody.SetupP1Body(p1Body);

                    var p2Body = assets.First(asset => asset.name == "ArraignP2Body");
                    Enemies.Judgement.Arraign.ArraignBody.ArraignP2Body = ArraignBody.SetupP2Body(p2Body);

                    var arraignStuff = new ArraignStuff();

                    SetupJudgementPath.JudgementInteractable = assets.First(asset => asset.name == "JudgementInteractable");
                    Behaviors.Judgement.WaveInteractable.JudgementSelectionController.modifiedPickerPanel = SetupJudgementPath.CloneOptionPickerPanel();
                    nopList.Add(SetupJudgementPath.JudgementInteractable);

                    SetupJudgementPath.PileOfDirt = assets.First(asset => asset.name == "PileOfDirtInteractable");
                    SetupJudgementPath.PileOfDirt = SetupJudgementPath.SetupLunarKey(SetupJudgementPath.PileOfDirt);
                    nopList.Add(SetupJudgementPath.PileOfDirt);

                    SetupJudgementPath.BrokenTeleporter = assets.First(asset => asset.name == "BrokenTeleporterInteractable");
                    SetupJudgementPath.BrokenTeleporter = SetupJudgementPath.SetupBrokenTeleporter(SetupJudgementPath.BrokenTeleporter);
                    nopList.Add(SetupJudgementPath.BrokenTeleporter);

                    SetupJudgementPath.VoidBrokenTeleporter = SetupJudgementPath.SetupVoidMegaTeleporter(assets.First(asset => asset.name == "VoidMegaTeleporter"));
                    nopList.Add(SetupJudgementPath.VoidBrokenTeleporter);

                    Equipment.VoidlingWeapon.VoidlingWeapon.VoidlingWeaponController = assets.First(asset => asset.name == "VoidlingWeaponController");
                    nopList.Add(Equipment.VoidlingWeapon.VoidlingWeapon.VoidlingWeaponController);

                    SetupJudgementPath.VoidlingWeaponIndicator = SetupJudgementPath.CreateVoidlingWeaponIndicator();

                    ModdedEntityStates.Judgement.VoidlingWeapon.SpawnAndFire.voidlingWeaponVisualsPrefab = SetupJudgementPath.SetupVoidlingWeapon(assets.First(asset => asset.name == "VoidWeaponPrefab"));

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
                    ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap.ExitSkyLeap.secondAttackEffectStatic = explosionEffect;
                    //ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap.ExitSkyLeap.secondAttackEffectStatic = explosionEffect;

                    var waveProjectile = assets.First(asset => asset.name == "ArraignSlash3Wave");
                    waveProjectile = arraignStuff.SetupWaveProjectile(waveProjectile);
                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash3.waveProjectile = waveProjectile;
                    //ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap.ExitSkyLeap.waveProjectileStatic = waveProjectile;
                    ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap.ExitSkyLeap.waveProjectileStatic = waveProjectile;

                    //var lightningProjectile = Enemies.Judgement.Arraign.ArraignStuff.SetupLightningStrikePrefab(assets.First(asset => asset.name == "ArraignPreLightningProjectile"));
                    var lightningProjectile = assets.First(asset => asset.name == "ArraignPreLightningProjectile");
                    ModdedEntityStates.Judgement.Arraign.Phase1.LightningStrikes.projectilePrefab = lightningProjectile;
                    ModdedEntityStates.Judgement.Arraign.Phase2.LeapingDash.LeapDash.projectilePrefab = lightningProjectile;

                    ModdedEntityStates.Judgement.Arraign.Phase2.ClockAttack.projectilePrefab = assets.First(asset => asset.name == "ArraignPreClockAttackProjectile");

                    ModdedEntityStates.Judgement.Arraign.Phase1.WeaponThrow.staticProjectilePrefab = arraignStuff.SetupSwordProjectile(assets.First(asset => asset.name == "ArraignSwordProjectile"));
                    ModdedEntityStates.Judgement.Arraign.Phase2.SpearThrow.staticProjectilePrefab = arraignStuff.SetupSpearProjectile(assets.First(asset => asset.name == "ArraignSpearProjectile"));

                    ModdedEntityStates.Judgement.Arraign.Phase2.ClockAttack.effectPrefab = assets.First(asset => asset.name == "ClockZoneEffect");

                    ModdedEntityStates.Judgement.Arraign.BaseSkyLeap.BaseHoldSkyLeap.dropEffectPrefab = arraignStuff.CreateSkyLeapDropPositionEffect(assets.First(asset => asset.name == "DropPositionEffect"));

                    ModdedEntityStates.Judgement.Arraign.Beam.BeamStart.preBeamIndicatorEffect = arraignStuff.SetupPreBeamIndicatorEffect(assets.First(asset => asset.name == "ArraignPreBeamGroundIndicator"));

                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash1.swingEffect = assets.First(asset => asset.name == "LanceStapEffect");

                    ArraignDamageController.hitEffectPrefab = assets.First(asset => asset.name == "ArraignArmorBreakEffect");

                    ModdedEntityStates.Judgement.SkyLaser.SpawnState.spawnEffect = assets.First(asset => asset.name == "SkyLaserBodySpawnEffect");

                    BeamStart.postProccessBeam = assets.First(asset => asset.name == "BeamPostProccess");

                    BeamLoop.pushBackEffectStatic = assets.First(asset => asset.name == "ArraignBeamPushbackEffect");
                    BeamStart.pushBackEffectStatic = assets.First(asset => asset.name == "ArraignBeamPushbackEffectNoMuzzleParticles");

                    Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay = assets.First(asset => asset.name == "DisplayAeonian");

                    ModifyCredits(assets.First(asset => asset.name == "EnemiesReturnsCreditsAdditions"));

                    Items.LunarFlower.LunarFowerExtraLifeBehaviour.startEffect = SetupJudgementPath.CreateVoidFlowerRespawnStartEffect();
                    effectsList.Add(new EffectDef(Items.LunarFlower.LunarFowerExtraLifeBehaviour.startEffect));

                    Items.LunarFlower.LunarFowerExtraLifeBehaviour.endEffect = SetupJudgementPath.CreateVoidFlowerRespawnEndEffect();
                    effectsList.Add(new EffectDef(Items.LunarFlower.LunarFowerExtraLifeBehaviour.endEffect));
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ItemDef[]>)((assets) =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    Content.Items.TradableRock = assets.First(item => item.name == "TradableRock");
                    Content.Items.TradableRock.pickupModelPrefab = SetupJudgementPath.SetupLunarKey(Content.Items.TradableRock.pickupModelPrefab);

                    Content.Items.LunarFlower = assets.First(item => item.name == "LunarFlower");
                    Content.Items.LunarFlower.pickupModelPrefab = SetupJudgementPath.SetupLunarFlower(Content.Items.LunarFlower.pickupModelPrefab);

                    Content.Items.VoidFlower = assets.First(item => item.name == "VoidFlower");
                    Content.Items.VoidFlower.pickupModelPrefab = SetupJudgementPath.SetupVoidFlower(Content.Items.VoidFlower.pickupModelPrefab);
#pragma warning restore CS0618 // Type or member is obsolete

                    Content.Items.HiddenAnointed = assets.First(item => item.name == "HiddenAnointed");

                    _contentPack.itemDefs.Add(assets);

                    var lunarEliteRecipe = ScriptableObject.CreateInstance<CraftableDef>();
                    (lunarEliteRecipe as ScriptableObject).name = "cdEnemiesReturnsEliteLunar";
                    lunarEliteRecipe.pickup = Addressables.LoadAssetAsync<EquipmentDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_EliteLunar.EliteLunarEquipment_asset).WaitForCompletion();
                    lunarEliteRecipe.recipes = new Recipe[]
                    {
                        new Recipe()
                        {
                            ingredients = new RecipeIngredient[]
                            {
                                new RecipeIngredient()
                                {
                                    pickup = Content.Items.LunarFlower,
                                    type = IngredientTypeIndex.AssetReference
                                },
                                new RecipeIngredient()
                                {
                                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_HeadHunter.HeadHunter_asset).WaitForCompletion(),
                                    type = IngredientTypeIndex.AssetReference
                                }
                            }
                        }
                    };
                    craftableList.Add(lunarEliteRecipe);

                    var voidRecipe = ScriptableObject.CreateInstance<CraftableDef>();
                    (voidRecipe as ScriptableObject).name = "cdEnemiesReturnsEliteVoid";
                    voidRecipe.pickup = Addressables.LoadAssetAsync<EquipmentDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_EliteVoid.EliteVoidEquipment_asset).WaitForCompletion();
                    voidRecipe.recipes = new Recipe[]
                    {
                        new Recipe()
                        {
                            ingredients = new RecipeIngredient[]
                            {
                                new RecipeIngredient()
                                {
                                    pickup = Content.Items.VoidFlower,
                                    type = IngredientTypeIndex.AssetReference
                                },
                                new RecipeIngredient()
                                {
                                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_HeadHunter.HeadHunter_asset).WaitForCompletion(),
                                    type = IngredientTypeIndex.AssetReference
                                }
                            }
                        }
                    };
                    craftableList.Add(voidRecipe);

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
#pragma warning disable CS0618 // Type or member is obsolete
                    Content.Equipment.MithrixHammer = assets.First(equipment => equipment.name == "MithrixHammer");
                    Equipment.MithrixHammer.MithrixHammer.SetupEquipmentConfigValues(Content.Equipment.MithrixHammer);
                    Content.Equipment.MithrixHammer.pickupModelPrefab = Equipment.MithrixHammer.MithrixHammer.SetupPickupDisplay(Content.Equipment.MithrixHammer.pickupModelPrefab);

                    Content.Equipment.EliteAeonian = assets.First(equipment => equipment.name == "EliteAeonianEquipment");

                    Content.Equipment.VoidlingWeapon = assets.First(equipment => equipment.name == "VoidlingWeapon");
                    Equipment.VoidlingWeapon.VoidlingWeapon.SetupEquipmentConfigValues(Content.Equipment.VoidlingWeapon);
                    Content.Equipment.VoidlingWeapon.pickupModelPrefab = Equipment.VoidlingWeapon.VoidlingWeapon.SetupPickupDisplay(Content.Equipment.VoidlingWeapon.pickupModelPrefab);
#pragma warning restore CS0618 // Type or member is obsolete

                    _contentPack.equipmentDefs.Add(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<CharacterSpawnCard[]>)((assets) =>
                {
                    Enemies.Judgement.Arraign.ArraignBody.cscArraignP1 = assets.First(asset => asset.name == "cscArraignP1");
                    Enemies.Judgement.Arraign.ArraignBody.cscArraignP2 = assets.First(asset => asset.name == "cscArraignP2");

                    ModdedEntityStates.Judgement.Arraign.Phase2.SummonSkyLasers.cscSkyLaser = assets.First(asset => asset.name == "cscSkyLaser");
                    ModdedEntityStates.Judgement.Mission.Phase3.cscArraignHaunt = assets.First(asset => asset.name == "cscArraignHaunt");
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<ModdedSkinDefParams[]>)((assets) =>
                {
                    var moddedSkinDefList = assets.Where(item => item.name.Contains("Judgement")).ToArray();
                    foreach(var moddedSkinDef in moddedSkinDefList)
                    {
                        var newSkin = moddedSkinDef.CreateSkinDef();
                        if (newSkin)
                        {
                            var bodyObject = AssetAsyncReferenceManager<GameObject>.LoadAsset(moddedSkinDef.bodyPrefab).WaitForCompletion();
                            var anointedSkin = EnemiesReturns.Enemies.Judgement.AnointedSkins.CreateAnointedSkin(bodyObject.name, moddedSkinDef.CreateSkinDef(), false);
                            var modelSkinController = bodyObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<ModelSkinController>();
                            HG.ArrayUtils.ArrayAppend(ref modelSkinController.skins, in anointedSkin);
                        }
                    }
                }));

                AssetBundle assetBundleStages = null;
                yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleJudgementStagesName), args.progressReceiver, (resultAssetBundle) => assetBundleStages = resultAssetBundle);

                CreateJudgement();
            }

            yield break;
        }

        private void ModifyCredits(GameObject ourCreditPanel)
        {
            var creditPanel = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_UI.CreditsPanel_prefab).WaitForCompletion();
            creditPanel.GetComponent<CreditsPanelController>().scrollDuration += 10f;
            var creditsContent = creditPanel.transform.Find("MainArea/Viewport/CreditsContent");
            var community = creditsContent.Find("CompanyCredits - Community");

            //creditsContent.Find("FinalMessageSpacer").GetComponent<UnityEngine.UI.LayoutElement>().minHeight = 32f;
            //creditsContent.Find("FinalMessage").GetComponent<UnityEngine.UI.LayoutElement>().minHeight = 128f;
            ourCreditPanel.transform.SetParent(creditsContent, false);
            ourCreditPanel.transform.SetSiblingIndex(community.GetSiblingIndex() + 1);
        }

        private void CreateJudgement()
        {
            if (Judgement.Enabled.Value)
            {
                nseList.Add(Utils.CreateNetworkSoundDef("Play_moonBrother_spawn"));

                SetupJudgementPath.AddInteractabilityToNewt();
                SetupJudgementPath.AddWeaponDropToMithrix();
                SetupJudgementPath.AddWeaponDropToVoidling();

                var arraignStuff = new ArraignStuff();
                ModdedEntityStates.Judgement.Arraign.BasePrimaryWeaponSwing.swingEffect = arraignStuff.CreateArraignSwingEffect();
                var swingComboEffect = arraignStuff.CreateArraignSwingComboEffect();
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash1.swingEffect = swingComboEffect;
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash2.swingEffect = swingComboEffect;
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.swingEffect = swingComboEffect;

                ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash2.swingEffect = swingComboEffect;
                ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash3.swingEffect = swingComboEffect;

                //ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.PreSlash.overlayMaterial = GetOrCreateMaterial("matArraignPreSlashOverlay", arraignStuff.CreatePreSlashWeaponOverlayMaterial);
                var slash3Explosion = arraignStuff.CreateSlash3ExplosionEffect();
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.explosionEffect = slash3Explosion;
                ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash3.explosionEffect = slash3Explosion;
                Spawn.slamEffect = slash3Explosion;
                ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap.ExitSkyLeap.firstAttackEffectStatic = slash3Explosion;
                ModdedEntityStates.Judgement.Arraign.Phase2.LeapingDash.LeapDash.blastAttackEffect = slash3Explosion;

                effectsList.Add(new EffectDef(slash3Explosion));

                //ArraignDamageController.hitEffectPrefab = arraignStuff.CreateArmorBreakEffect();
                //effectsList.Add(new EffectDef(ArraignDamageController.hitEffectPrefab));

                ModdedEntityStates.Judgement.Arraign.BaseSkyLeap.BaseHoldSkyLeap.markEffect = arraignStuff.CreateSkyLeapMarktempVisualEffect();
                BeamLoop.beamPrefab = arraignStuff.CreateBeamEffect();

                Enemies.Judgement.Arraign.ArraignDamageController.immuneToAllDamageExceptHammerMaterial = ContentProvider.GetOrCreateMaterial("matImmuneToAllExceptHammer", CreateImmuneToAllExceptHammerMaterial);

                CreateJudgementMusic(Content.Stages.OutOfTime);
            }
        }

        private void CreateJudgementMusic(SceneDef scene)
        {
            var mainCustomTrack = ScriptableObject.CreateInstance<SoundAPI.Music.CustomMusicTrackDef>();
            mainCustomTrack.cachedName = "EnemiesReturns_OutOfTime_Unknown";
            mainCustomTrack.CustomStates = new List<SoundAPI.Music.CustomMusicTrackDef.CustomState>();
            mainCustomTrack.comment = "Unknown from Starstorm 1";

            mainCustomTrack.CustomStates.Add(new SoundAPI.Music.CustomMusicTrackDef.CustomState
            {
                GroupId = 3811162539U, // gathered from the MOD's Init bank txt file, state group id for gameplaySondChoice
                StateId = 3892723418U // Unknown
            });
            mainCustomTrack.CustomStates.Add(new SoundAPI.Music.CustomMusicTrackDef.CustomState
            {
                GroupId = 792781730U, // gathered from the GAME's Init bank txt file
                StateId = 89505537U // gathered from the GAME's Init bank txt file
            });

            Content.MusicTracks.Unknown = mainCustomTrack;
            scene.mainTrack = Content.MusicTracks.Unknown;

            var bossCustomtrack = ScriptableObject.CreateInstance<SoundAPI.Music.CustomMusicTrackDef>();
            bossCustomtrack.cachedName = "EnemiesReturns_OutOfTime_UnknownBoss";
            bossCustomtrack.CustomStates = new List<SoundAPI.Music.CustomMusicTrackDef.CustomState>();
            bossCustomtrack.comment = "UnknownBoss from Starstorm 1";

            bossCustomtrack.CustomStates.Add(new SoundAPI.Music.CustomMusicTrackDef.CustomState
            {
                GroupId = 3811162539U, // gathered from the MOD's Init bank txt file, state group id for gameplaySondChoice
                StateId = 3446177699U // UnknownBoss
            });
            bossCustomtrack.CustomStates.Add(new SoundAPI.Music.CustomMusicTrackDef.CustomState
            {
                GroupId = 792781730U, // gathered from the GAME's Init bank txt file
                StateId = 89505537U // gathered from the GAME's Init bank txt file
            });

            Content.MusicTracks.UnknownBoss = bossCustomtrack;

            var bossCustomtrackP2 = ScriptableObject.CreateInstance<SoundAPI.Music.CustomMusicTrackDef>();
            bossCustomtrackP2.cachedName = "EnemiesReturns_OutOfTime_TheOrigin";
            bossCustomtrackP2.CustomStates = new List<SoundAPI.Music.CustomMusicTrackDef.CustomState>();
            bossCustomtrackP2.comment = "The Origin by Tristan Clark\\nFrom Eviternity 2\\nhttps://tristanclark.bandcamp.com/track/the-origin";

            bossCustomtrackP2.CustomStates.Add(new SoundAPI.Music.CustomMusicTrackDef.CustomState
            {
                GroupId = 3811162539U, // gathered from the MOD's Init bank txt file, state group id for gameplaySondChoice
                StateId = 3446177696U // The Origin
            });
            bossCustomtrackP2.CustomStates.Add(new SoundAPI.Music.CustomMusicTrackDef.CustomState
            {
                GroupId = 792781730U, // gathered from the GAME's Init bank txt file
                StateId = 89505537U // gathered from the GAME's Init bank txt file
            });

            Content.MusicTracks.TheOrigin = bossCustomtrackP2;
        }

        public static Material CreateImmuneToAllExceptHammerMaterial()
        {
            // cloudremap
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common.matEnergyShield_mat).WaitForCompletion());
            newMaterial.name = "matImmuneToAllExceptHammer";
            newMaterial.SetFloat("_OffsetAmount", 0.13f);
            newMaterial.SetColor("_TintColor", new Color(23 / 255f, 202 / 255f, 1f, 1f));
            newMaterial.SetFloat("_Boost", 2.781755f);
            newMaterial.SetFloat("_AlphaBoost", 0.8013554f);

            return newMaterial;
        }
    }
}
