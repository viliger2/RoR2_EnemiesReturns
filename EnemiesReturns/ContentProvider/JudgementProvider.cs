using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Judgement.Arraign;
using EnemiesReturns.Enemies.Judgement;
using R2API;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        public IEnumerator CreateJudgementAsync(LoadStaticContentAsyncArgs args, Dictionary<string, Sprite> iconLookup, Dictionary<string, Texture2D> rampLookups, Dictionary<string, AnimationCurveDef> acdLookup, string assetBundleFolderPath)
        {
            if (Configuration.Judgement.Enabled.Value)
            {
                AssetBundle assetBundleStagesAssets = null;
                yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleStagesAssetsName), args.progressReceiver, (resultAssetBundle) => assetBundleStagesAssets = resultAssetBundle);

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<Sprite[]>)((assets) =>
                {
                    foreach (var asset in assets)
                    {
                        if (asset.name == "texAnointedSkinIcon")
                        {
                            SetupJudgementPath.AnointedSkinIcon = asset;
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

                    SetupJudgementPath.aeonianEliteRamp = rampLookups["texRampAeonianElite"];
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<AnimationCurveDef[]>)((assets) =>
                {
                    zJunk.ModdedEntityStates.Judgement.Arraign.BaseSlashDash.speedCoefficientCurve = assets.First(acd => acd.name == "acdMoveSpeed").curve;

                    var acdOverlay = assets.First(acd => acd.name == "acdPreSlashMaterialAlpha").curve;
                    ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.PreSlash.acdOverlayAlpha = acdOverlay;
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
                    SwapMaterials(assets);
                }));

                yield return LoadAllAssetsAsync(assetBundleStagesAssets, args.progressReceiver, (Action<GameObject[]>)((assets) =>
                {
                    _contentPack.bodyPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterBody>(out _)).ToArray());
                    _contentPack.masterPrefabs.Add(assets.Where(asset => asset.TryGetComponent<CharacterMaster>(out _)).ToArray());
                    _contentPack.projectilePrefabs.Add(assets.Where(asset => asset.TryGetComponent<ProjectileController>(out _)).ToArray());
                    _contentPack.effectDefs.Add(Array.ConvertAll(assets.Where(asset => asset.TryGetComponent<EffectComponent>(out _)).ToArray(), item => new EffectDef(item)));

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
                    ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap.ExitSkyLeap.secondAttackEffectStatic = explosionEffect;

                    var waveProjectile = assets.First(asset => asset.name == "ArraignSlash3Wave");
                    waveProjectile = arraignStuff.SetupWaveProjectile(waveProjectile);
                    ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo.Slash3.waveProjectile = waveProjectile;
                    ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap.ExitSkyLeap.waveProjectileStatic = waveProjectile;
                    ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap.ExitSkyLeap.waveProjectileStatic = waveProjectile;

                    //var lightningProjectile = Enemies.Judgement.Arraign.ArraignStuff.SetupLightningStrikePrefab(assets.First(asset => asset.name == "ArraignPreLightningProjectile"));
                    var lightningProjectile = assets.First(asset => asset.name == "ArraignPreLightningProjectile");
                    ModdedEntityStates.Judgement.Arraign.Phase1.LightningStrikes.projectilePrefab = lightningProjectile;
                    ModdedEntityStates.Judgement.Arraign.Phase2.LeapingDash.LeapDash.projectilePrefab = lightningProjectile;

                    ModdedEntityStates.Judgement.Arraign.Phase2.ClockAttack.projectilePrefab = assets.First(asset => asset.name == "ArraignPreClockAttackProjectile");

                    ModdedEntityStates.Judgement.Arraign.Phase1.WeaponThrow.staticProjectilePrefab = assets.First(asset => asset.name == "ArraignSwordProjectile");
                    ModdedEntityStates.Judgement.Arraign.Phase2.SpearThrow.staticProjectilePrefab = assets.First(asset => asset.name == "ArraignSpearProjectile");

                    ModdedEntityStates.Judgement.Arraign.Phase2.ClockAttack.effectPrefab = assets.First(asset => asset.name == "ClockZoneEffect");

                    ModdedEntityStates.Judgement.Arraign.BaseSkyLeap.BaseHoldSkyLeap.dropEffectPrefab = arraignStuff.CreateSkyLeapDropPositionEffect(assets.First(asset => asset.name == "DropPositionEffect"));

                    BeamStart.postProccessBeam = assets.First(asset => asset.name == "BeamPostProccess");

                    BeamLoop.pushBackEffectStatic = assets.First(asset => asset.name == "ArraignBeamPushbackEffect");
                    BeamStart.pushBackEffectStatic = assets.First(asset => asset.name == "ArraignBeamPushbackEffectNoMuzzleParticles");
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
                    Equipment.MithrixHammer.MithrixHammer.SetupEquipmentConfigValues(Content.Equipment.MithrixHammer);
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

            yield break;
        }

        private void CreateJudgement()
        {
            if (Configuration.Judgement.Enabled.Value)
            {
                nseList.Add(Utils.CreateNetworkSoundDef("Play_moonBrother_spawn")); // TODO: replce with different sounds, something heavy that lands in water, use wow sound like you usually do

                SetupJudgementPath.AddInteractabilityToNewt();
                SetupJudgementPath.AddWeaponDropToMithrix();

                Content.DamageTypes.EndGameBossWeapon = DamageAPI.ReserveDamageType();

                var arraignStuff = new ArraignStuff();
                ModdedEntityStates.Judgement.Arraign.BasePrimaryWeaponSwing.swingEffect = arraignStuff.CreateArraignSwingEffect();
                var swingComboEffect = arraignStuff.CreateArraignSwingComboEffect();
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash1.swingEffect = swingComboEffect;
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash2.swingEffect = swingComboEffect;
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.swingEffect = swingComboEffect;

                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.PreSlash.overlayMaterial = GetOrCreateMaterial("matArraignPreSlashOverlay", arraignStuff.CreatePreSlashWeaponOverlayMaterial);
                var slash3Explosion = arraignStuff.CreateSlash3ExplosionEffect();
                ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo.Slash3.explosionEffect = slash3Explosion;
                ModdedEntityStates.Judgement.Arraign.Spawn.slamEffect = slash3Explosion;
                ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap.ExitSkyLeap.firstAttackEffectStatic = slash3Explosion;
                effectsList.Add(new EffectDef(slash3Explosion));

                ArraignDamageController.hitEffectPrefab = arraignStuff.CreateArmorBreakEffect();
                effectsList.Add(new EffectDef(ArraignDamageController.hitEffectPrefab));

                ModdedEntityStates.Judgement.Arraign.BaseSkyLeap.BaseHoldSkyLeap.markEffect = arraignStuff.CreateSkyLeapMarktempVisualEffect();
                BeamLoop.beamPrefab = arraignStuff.CreateBeamEffect();
            }
        }

    }
}
