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
        public static GameObject wardrobe;

        public static GameObject swordHilt;

        public static Dictionary<string, PositionAndRotation> SwordShardSpawnPositions = new Dictionary<string, PositionAndRotation>()
        {
            {"golemplains", new PositionAndRotation(Vector3.zero, Vector3.zero) }
        };

        public static Dictionary<string, PositionAndRotation> SwordHiltSpawnPositions = new Dictionary<string, PositionAndRotation>()
        {
            {"artifactworld", new PositionAndRotation(new Vector3(71.0199966f, 3.69985294f, 90.5f), new Vector3(0f ,69.2384033f ,0)) },
            {"artifactworld01", new PositionAndRotation(new Vector3(-53.1899986f,24.6599979f,95.8899994f), new Vector3(0,329.253754f,0)) },
            {"artifactworld02", new PositionAndRotation(new Vector3(-16.2054558f,14.1897697f,-43.7611961f), new Vector3(0,239.659424f,0)) },
            {"artifactworld03", new PositionAndRotation(new Vector3(-18.3999996f,-4.36375141f,3.52999997f), new Vector3(0,242.920288f,0)) }
        };

        public static InteractableSpawnCard iscSwordShard;

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
                args.attackSpeedMultAdd += 0.5f; // TODO: config
                args.primarySkill.cooldownReductionMultAdd += 1f; // TODO: config
            }
        }

        private static void AddWardrobe(Stage stage)
        {
            if (stage.sceneDef.cachedName != "bazaar")
            {
                return;
            }

            // TODO: add unlockable check

            var newObject = UnityEngine.Object.Instantiate(wardrobe, new Vector3(8.97897816f, -5.73999977f, 7.62354374f), Quaternion.identity);
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
                    textMesh.token = "ENEMIESRETURNS_CONTACTLIGHT_WARDROBE_INTERACTION_HEADER";
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
    }
}
