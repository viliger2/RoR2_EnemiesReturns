using EnemiesReturns.Behaviors.SkinDefPicker;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using Rewired.ComponentControls.Effects;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static EnemiesReturns.Utils;
using static RoR2.EquipmentSlot;

namespace EnemiesReturns.Enemies.ContactLight
{
    public static class SetupContactLight
    {
        public const string PROVIDENCE_FLAG = "ER_ProviKilled";

        public static GameObject wardrobe;

        public static GameObject swordHilt;

        public static Dictionary<string, PositionAndRotation> SwordShardSpawnPositions = new Dictionary<string, PositionAndRotation>()
        {
            // Vanilla
            {"blackbeach", new PositionAndRotation(new Vector3(25.9078293f,-212.110001f,-41.7553978f), new Vector3(0,24.2757854f,0)) },
            {"blackbeach2", new PositionAndRotation(new Vector3(-125.284706f,45.8801422f,-92.0441971f), new Vector3(0,25.2306004f,0)) },
            {"dampcavesimple", new PositionAndRotation(new Vector3(-131.555969f,-126.443047f,-94.0129852f), new Vector3(0,40.1930809f,0)) },
            {"foggyswamp", new PositionAndRotation(new Vector3(23.097084f,-126.070091f,-39.6605682f), new Vector3(345.538361f,72.3866272f,328.421631f)) },
            {"frozenwall", new PositionAndRotation(new Vector3(-232.835388f,8.98909187f,-212.541748f), new Vector3(13.2651386f,84.6467819f,0.509293139f)) },
            {"golemplains", new PositionAndRotation(new Vector3(250.007111f,-110.020447f,-171.305389f), new Vector3(11.8163185f,339.907776f,359.205048f)) },
            {"golemplains2", new PositionAndRotation(new Vector3(232.748322f,32.4539223f,-127.812988f), new Vector3(7.9908824f,102.586617f,345.833466f)) },
            {"goolake", new PositionAndRotation(new Vector3(330.696564f,-123.855148f,75.0318756f), new Vector3(5.26236439f,307.132477f,353.093506f)) },
            {"rootjungle", new PositionAndRotation(new Vector3(-181.194f,38.8932304f,-79.3384171f), new Vector3(339.7547f,36.0840416f,333.300354f)) },
            {"shipgraveyard", new PositionAndRotation(new Vector3(73.812645f,70.9198532f,164.952393f), new Vector3(2.68316221f,35.6639099f,13.0117798f)) },
            {"skymeadow", new PositionAndRotation(new Vector3(-190.556305f,71.5799103f,259.585266f), new Vector3(9.5210743f,331.407166f,346.61618f)) },
            {"wispgraveyard", new PositionAndRotation(new Vector3(109.391602f,29.8812714f,43.2056808f), new Vector3(343.942871f,297.159882f,343.310791f)) },
            // DLC1
            {"ancientloft", new PositionAndRotation(new Vector3(237.003021f,19.780489f,-93.3829803f), new Vector3(346.007782f,311.807007f,357.564667f)) },
            {"snowyforest", new PositionAndRotation(new Vector3(44.9599991f,63.1362381f,17.1499996f), new Vector3(350.468842f,48.378437f,359.102386f)) },
            {"sulfurpools", new PositionAndRotation(new Vector3(-11.9200001f,19.4960003f,181.169998f), new Vector3(15.5018167f,304.464935f,341.536346f)) },
            // DLC2
            {"habitat", new PositionAndRotation(new Vector3(-18.8846111f,25.0342827f,-10.8930779f), new Vector3(346.155334f,10.8311501f,1.01098871f)) },
            {"habitatfall", new PositionAndRotation(new Vector3(-29.9997349f,8.36633968f,54.0528564f), new Vector3(5.50275373f,288.861908f,338.528015f)) },
            {"helminthroost", new PositionAndRotation(new Vector3(-600.76001f,141.719894f,45.7000008f), new Vector3(31.0964794f,13.3499193f,326.913757f)) },
            {"lakes", new PositionAndRotation(new Vector3(154.275864f,26.6254158f,83.172348f), new Vector3(2.25087667f,353.700806f,344.349213f)) },
            {"lakesnight", new PositionAndRotation(new Vector3(-151.071396f,36.142868f,14.8224525f), new Vector3(2.31677246f,267.877716f,317.093842f)) },
            {"lemuriantemple", new PositionAndRotation(new Vector3(19.6079712f,9.67194176f,29.456768f), new Vector3(16.8745155f,308.250366f,341.187378f)) },
            {"village", new PositionAndRotation(new Vector3(222.573303f,48.5539246f,-188.741714f), new Vector3(338.850189f,294.373688f,357.482727f)) },
            {"villagenight", new PositionAndRotation(new Vector3(-85.3516159f,-0.221232414f,18.6638184f), new Vector3(17.725853f,292.213867f,339.188385f)) },
            // DLC3
            {"conduitcanyon", new PositionAndRotation(new Vector3(6.34806681f,86.4391708f,122.390572f), new Vector3(15.4529705f,0.0872457996f,352.564331f)) },
            {"ironalluvium", new PositionAndRotation(new Vector3(-80.388176f,120.799591f,-118.117729f), new Vector3(345.621124f,2.8644954e-06f,344.843842f)) },
            {"ironalluvium2", new PositionAndRotation(new Vector3(124.226974f,64.2011261f,-14.3238783f), new Vector3(345.001312f,47.5802574f,343.146973f)) },
            {"nest", new PositionAndRotation(new Vector3(14.8768663f,61.5068626f,-19.0734692f), new Vector3(33.5635986f,25.417572f,352.944733f)) },
            {"repurposedcrater", new PositionAndRotation(new Vector3(-252.506226f,2.19022465f,38.5696259f), new Vector3(23.1821651f,275.498352f,334.07605f)) },
            // Mods 
            {"forgottenwreckage_ws", new PositionAndRotation(new Vector3(241.263489f,131.417358f,-216.176849f), new Vector3(338.87558f,337.174835f,358.52417f)) },
            {"sm64_bbf_SM64_BBF", new PositionAndRotation(new Vector3(28.7595501f,58.9383583f,-26.8056107f), new Vector3(21.0945263f,166.54306f,352.08667f)) },
            {"agatevillage", new PositionAndRotation(new Vector3(710.714783f,53.7347412f,-17.3651123f), new Vector3(328.491638f,342.050354f,357.774872f)) },
            {"catacombs_DS1_Catacombs", new PositionAndRotation(new Vector3(12.1837921f,193.53717f,-361.958221f), new Vector3(6.97203779f,318.215088f,10.1320143f)) },
            {"FBLScene", new PositionAndRotation(new Vector3(405.519775f,229.787415f,-45.6870232f), new Vector3(347.212128f,238.204254f,4.20200825f)) },
            {"coast_wormsworms", new PositionAndRotation(new Vector3(-173.039383f,43.0044632f,105.01458f), new Vector3(12.3944674f,41.3582001f,352.10437f)) },
            {"foggyswampdownpour", new PositionAndRotation(new Vector3(-1066.87317f,76.3767014f,-2068.56812f), new Vector3(16.2758408f,321.471191f,345.012604f)) },
            {"tropics_wormsworms", new PositionAndRotation(new Vector3(-85.5800018f,-29.4200001f,81.1100006f), new Vector3(358.987518f,286.002197f,353.71051f)) },
            {"tropicsnight_wormsworms", new PositionAndRotation(new Vector3(-152.939758f,-23.5399399f,-109.110558f), new Vector3(336.112305f,84.2687683f,321.315948f)) },
            {"sunkentombs_wormsworms", new PositionAndRotation(new Vector3(-86.1399994f,84.0999985f,-190.400223f), new Vector3(348.008484f,321.7724f,347.561798f)) },
            {"broadcastperch_wormsworms", new PositionAndRotation(new Vector3(-27.8478451f,279.912231f,-48.4336548f), new Vector3(35.3873405f,42.7137489f,18.4060478f)) },
            {"hollowsummit_wormsworms", new PositionAndRotation(new Vector3(-169.85759f,129.94986f,12.4457617f), new Vector3(8.7619772f,69.3884964f,352.214203f)) },
            {"hollowsummitnight_wormsworms", new PositionAndRotation(new Vector3(-82.6136017f,192.5905f,-1.87729919f), new Vector3(345.780243f,214.367493f,8.68370247f)) },
            {"observatory_wormsworms", new PositionAndRotation(new Vector3(-163.679108f,103.736893f,-44.4260979f), new Vector3(333.505585f,278.427856f,4.65514374f)) },
            {"swampybog_winslow", new PositionAndRotation(new Vector3(-317.516968f,94.306488f,-20.8014488f), new Vector3(25.6750984f,268.223755f,350.365906f)) },
            {"swampybognight_winslow", new PositionAndRotation(new Vector3(107.397781f,110.898125f,133.695282f), new Vector3(351.607666f,47.3283272f,359.936737f)) },
        };

        public static Dictionary<string, PositionAndRotation> SwordHiltSpawnPositions = new Dictionary<string, PositionAndRotation>()
        {
            {"artifactworld", new PositionAndRotation(new Vector3(71.0199966f, 3.69985294f, 90.5f), new Vector3(0f ,69.2384033f ,0)) },
            {"artifactworld01", new PositionAndRotation(new Vector3(-53.1899986f,24.6599979f,95.8899994f), new Vector3(0,329.253754f,0)) },
            {"artifactworld02", new PositionAndRotation(new Vector3(-16.2054558f,14.1897697f,-43.7611961f), new Vector3(0,239.659424f,0)) },
            {"artifactworld03", new PositionAndRotation(new Vector3(-18.3999996f,-4.36375141f,3.52999997f), new Vector3(0,242.920288f,0)) }
        };

        public static InteractableSpawnCard iscSwordShard;

        public static UnlockableDef wardrobeUnlockable;

        public static void Hooks()
        {
            if (Configuration.General.EnableContactLight.Value)
            {
                IL.ProximityHighlight.OnPreRenderOutlineHighlight += ProximityHighlight_OnPreRenderOutlineHighlight;
                IL.RoR2.InteractionDriver.OnPreRenderOutlineHighlight += InteractionDriver_OnPreRenderOutlineHighlight;

                CostTypeCatalog.modHelper.getAdditionalEntries += ModHelper_getAdditionalEntries;
                RoR2.Stage.onServerStageBegin += AddWardrobe;
                RoR2.SceneDirector.onPostPopulateSceneServer += SpawnThings;
            }
            if (Configuration.General.EnableAdrenalineCore.Value)
            {
                Items.AdrenalineCore.AdrenalineCoreUI.Hooks();
            }
        }

        private static void SpawnThings(SceneDirector sceneDirector)
        {
            if (!RoR2.SceneInfo.instance || !RoR2.DirectorCore.instance)
            {
                return;
            }

            if (!RoR2.Run.instance)
            {
                return;
            }

            if (RoR2.Run.instance.GetEventFlag(PROVIDENCE_FLAG))
            {
                return;
            }

            var sceneDef = RoR2.SceneInfo.instance.sceneDef;
            if (!sceneDef)
            {
                return;
            }

            SpawnSwordHilt(sceneDef);
            SpawnSwordShard(sceneDirector, sceneDef);
        }

        private static void SpawnSwordHilt(SceneDef sceneDef)
        {
            if(SwordHiltSpawnPositions == null)
            {
                return;
            }

            if(SwordHiltSpawnPositions.TryGetValue(sceneDef.cachedName, out var positionAndRotation))
            {
                var newHilt = UnityEngine.Object.Instantiate(swordHilt);
                newHilt.transform.position = positionAndRotation.position;
                newHilt.transform.rotation = Quaternion.Euler(positionAndRotation.rotation);
                NetworkServer.Spawn(newHilt);
            }
        }

        private static void SpawnSwordShard(SceneDirector sceneDirector, SceneDef sceneDef)
        {
            if (!(sceneDef.sceneType == SceneType.Stage && sceneDef.stageOrder >= 1 && sceneDef.stageOrder <= 5))
            {
                return;
            }

            if (SwordShardSpawnPositions == null)
            {
                return;
            }

            DirectorPlacementRule placementRule = new DirectorPlacementRule();

            if (SwordShardSpawnPositions.TryGetValue(sceneDef.cachedName, out var positionAndRotation))
            {
                placementRule.position = positionAndRotation.position;
                placementRule.rotation = Quaternion.Euler(positionAndRotation.rotation);
                placementRule.placementMode = DirectorPlacementRule.PlacementMode.Direct;
            }
            else
            {
                placementRule.placementMode = DirectorPlacementRule.PlacementMode.Random;
            }

            DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(iscSwordShard, placementRule, sceneDirector.rng));
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(Content.Buffs.TempleGuardOverclock))
            {
                args.attackSpeedMultAdd += Configuration.ContactLight.TempleGuard.OverclockAttackSpeedBuff.Value / 100f;
                args.primarySkill.cooldownReductionMultAdd += Configuration.ContactLight.TempleGuard.OverclockPrimaryCooldownReduction.Value;
            }
        }

        private static void AddWardrobe(Stage stage)
        {
            if (stage.sceneDef.cachedName != "bazaar")
            {
                return;
            }

            if(!(RoR2.Run.instance.IsUnlockableUnlocked(wardrobeUnlockable) || Configuration.ContactLight.ContactLight.ForceUnlock.Value))
            {
                return;
            }

            var newObject = UnityEngine.Object.Instantiate(wardrobe, new Vector3(-136.080002f, -21.1499996f, -33.4500008f), new Quaternion(-0.0393915996f, 0.618751168f, 0.0498490371f, 0.783013642f));
            NetworkServer.Spawn(newObject);
        }

        private static void ModHelper_getAdditionalEntries(List<CostTypeDef> list)
        {
            if (Content.CostTypes.AccessCard != null)
            {
                list.Add(Content.CostTypes.AccessCard);
            }
        }

        private static void InteractionDriver_OnPreRenderOutlineHighlight(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var match = c.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<RoR2.OutlineHighlight>("AddHighlight"));

            if (match)
            {
                c.Emit(OpCodes.Ldarg, 0); // outlinehighlight
                c.Emit(OpCodes.Ldloc, 2); // game object with highlights
                c.Emit(OpCodes.Ldloc, 4); // existing highlight
                c.Emit(OpCodes.Ldloc, 5); // precalculcated color
                c.EmitDelegate<Action<OutlineHighlight, GameObject, Highlight, Color>>(UpdateOtherHighlights);
            }
            else
            {
                Log.Warning($"IL Hook Failed - ProximityHighlight.OnPreRenderOutlineHighlight: Contact Light doors will have only one outline.");
            }

            void UpdateOtherHighlights(OutlineHighlight outlineHighlight, GameObject highlightGameObject, RoR2.Highlight highlight, Color color)
            {
                var highlights = highlightGameObject.GetComponents<Highlight>();
                if (highlights.Length < 2)
                {
                    return;
                }

                foreach (var highlight2 in highlights)
                {
                    if (highlight2 == highlight)
                    {
                        continue;
                    }

                    outlineHighlight.AddHighlight(highlight2.targetRenderer, color * highlight2.strength);
                }
            }
        }

        private static void ProximityHighlight_OnPreRenderOutlineHighlight(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var match = c.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<RoR2.OutlineHighlight>("AddHighlight"));

            if (match)
            {
                c.Emit(OpCodes.Ldarg, 0); // self
                c.Emit(OpCodes.Ldarg, 1); // outlinehighlight
                c.Emit(OpCodes.Ldloc, 2); // gameObject
                c.Emit(OpCodes.Ldloc, 4); // already found highlight
                c.EmitDelegate<Action<ProximityHighlight, RoR2.OutlineHighlight, GameObject, RoR2.Highlight>>(UpdateOtherHighlights);
            }
            else
            {
                Log.Warning($"IL Hook Failed - ProximityHighlight.OnPreRenderOutlineHighlight: Contact Light doors will have only one outline.");
            }

            void UpdateOtherHighlights(ProximityHighlight self, RoR2.OutlineHighlight outline, GameObject gameObject, RoR2.Highlight highlight)
            {
                var highlights = gameObject.GetComponents<Highlight>();
                if (highlights.Length < 2)
                {
                    return;
                }
                foreach (var highlight2 in highlights)
                {
                    if (highlight2 == highlight)
                    {
                        continue;
                    }

                    Color h = highlight2.GetColor() * highlight2.strength * self.highlightScale;
                    outline.AddHighlight(highlight2.targetRenderer, h);
                }
            }
        }

        public static GameObject CreateEliteSlayerIndicator()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_BossHunter.BossHunterIndicator_prefab).WaitForCompletion().InstantiateClone("EliteSlayerWeaponIndicator", false);

            var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
            foreach(var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.color = new Color(0.8490566f, 0.7833268f, 0f, 1f);
            }

            prefab.GetComponentInChildren<TextMeshPro>().color = new Color(0.8490566f, 0.7833268f, 0f, 1f);

            return prefab;
        }

        public static GameObject CreateSkinDefPickerPanel()
        {
            var newPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Command.CommandPickerPanel_prefab).WaitForCompletion().InstantiateClone("SkinDefPickerPanel", false);
            var pickerPanelComponent = newPrefab.GetComponent<PickupPickerPanel>();

            var labelTransform = newPrefab.transform.Find("MainPanel/Juice/Label");
            if (labelTransform)
            {
                var textMesh = labelTransform.GetComponent<LanguageTextMeshController>();
                if (textMesh)
                {
                    textMesh.token = "ENEMIES_RETURNS_CONTACTLIGHT_WARDROBE_INTERACTION_HEADER";
                }
            }

            var skinDefPanel = newPrefab.AddComponent<SkinDefPickerPanel>();
            skinDefPanel.gridlayoutGroup = pickerPanelComponent.gridlayoutGroup;
            skinDefPanel.buttonContainer = pickerPanelComponent.buttonContainer;
            skinDefPanel.buttonPrefab = pickerPanelComponent.buttonPrefab;
            skinDefPanel.coloredImages = pickerPanelComponent.coloredImages;
            skinDefPanel.darkColoredImages = pickerPanelComponent.darkColoredImages;
            skinDefPanel.maxColumnCount = pickerPanelComponent.maxColumnCount;
            skinDefPanel.useLockSpriteForUnavailableOptions = pickerPanelComponent.useLockSpriteForUnavailableOptions;
            skinDefPanel.shouldChangeButtonFrameColor = pickerPanelComponent.shouldChangeButtonFrameColor;
            skinDefPanel.shouldLeaveDisabledButtonsInteractable = pickerPanelComponent.shouldLeaveDisabledButtonsInteractable;

            UnityEngine.Object.DestroyImmediate(pickerPanelComponent);
            UnityEngine.Object.DestroyImmediate(newPrefab.GetComponent<PickerPanelSizeAdjuster>());

            return newPrefab;
        }

        public static GameObject CreateAdrenalineLevelUpEffect(GameObject prefab)
        {
            prefab.transform.Find("Ring").GetComponent<Renderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matOmniRing1Generic_mat).WaitForCompletion();
            prefab.transform.Find("Dust Explosion").GetComponent<Renderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat).WaitForCompletion();
            prefab.transform.Find("BrightFlash").GetComponent<Renderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matTracerBright_mat).WaitForCompletion();
            prefab.transform.Find("Spinner").GetComponent<ParticleSystemRenderer>().trailMaterial = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matGenericTrail_mat).WaitForCompletion();
            prefab.transform.Find("BrightFlash, Lines").GetComponent<Renderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Merc.matOmniHitspark4Merc_mat).WaitForCompletion();

            return prefab;
        }

        public static GameObject CreateCleanseNovaEffect()
        {
            var newObject = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_TPHealingNova.TeleporterHealNovaPulse_prefab).WaitForCompletion().InstantiateClone("SurgicalBedNovaPulse", true);

            var esm = newObject.GetComponent<EntityStateMachine>();
            esm.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ContactLight.RechargableInteractable.SurgicalBed.BedHealNovaPulse));
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ContactLight.RechargableInteractable.SurgicalBed.BedHealNovaPulse));

            return newObject;
        }

        public static GameObject SetupCargoDoorIndicator(GameObject prefab)
        {
            var material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_Fonts_Traceroute.tmpTRACER___SDFBOLD_DROPSHADOW_asset_TRACER___SDF_Material_).WaitForCompletion();
            var font = Addressables.LoadAssetAsync<TMP_FontAsset>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_Fonts_Traceroute.tmpTRACER___SDFBOLD_DROPSHADOW_asset).WaitForCompletion();

            var textMeshPro = prefab.transform.Find("BoingyScaler/DoorSprite/TextMeshPro");
            var component = textMeshPro.GetComponent<TextMeshPro>();
            component.font = font;
            component.material = material;

            textMeshPro.GetComponent<MeshRenderer>().material = material;

            var textMeshPro2 = prefab.transform.Find("BoingyScaler/DoorSprite/TextMeshPro");
            var component2 = textMeshPro2.GetComponent<TextMeshPro>();
            component2.font = font;
            component2.material = material;

            textMeshPro2.GetComponent<MeshRenderer>().material = material;

            return prefab;
        }
    }
}
