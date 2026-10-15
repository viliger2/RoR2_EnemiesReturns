using RoR2;
using RoR2.ContentManagement;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace EnemiesReturns.EditorHelpers
{
    [CreateAssetMenu(menuName = "EnemiesReturns/ModdedSkinDefParams")]
    public class ModdedSkinDefParams : ScriptableObject
    {
        [Serializable]
        public struct RenderInfo
        {
            public string rendererName;

            public Material material;

            public ShadowCastingMode defaultShadowCastingMode;

            public bool ignoreOverlays;

            public bool hideOnDeath;

            public bool ignoresMaterialOverrides;
        }

        [Serializable]
        public struct MeshReplacement
        {
            public string rendererName;

            public Mesh mesh;
        }

        [Serializable]
        public struct GameObjectActivation
        {
            public bool spawnPrefabOnModel;

            [Tooltip("This is path to the game object that will be a parent to instantiated prefab. It should be a path from the modelTransform.")]
            public string parentGameObject;

            public bool shouldActivate;

            public AssetReferenceGameObject prefabAddress;

            public GameObject prefab;

            public Vector3 localPosition;

            public Vector3 localRotation;

            public Vector3 localScale;
        }

        [Tooltip("true - uses catalog and catalog names to find bodies instead of loading assets via Addressables")]
        public bool useCatalog = false;

        public AssetReferenceT<SkinDef> baseSkin;

        public AssetReferenceT<SurvivorDef> survivorDef;

        public string bodyPrefabNameCatalog;

        public string baseSkinCatalog;

        public string nameToken;

        public Sprite icon;

        public UnlockableDef unlockable;

        public RenderInfo[] renderInfos = Array.Empty<RenderInfo>();

        public MeshReplacement[] meshReplacements = Array.Empty<MeshReplacement>();

        public GameObjectActivation[] gameObjectActivations = Array.Empty<GameObjectActivation>();

        public SkinDef CreateSkinDef()
        {
            if (useCatalog)
            {
                Log.Warning($"Can't build a skin via CreateSkinDef when useCatalog is true. Skipping {this}...");
                return null;
            }

            if (!this.survivorDef.RuntimeKeyIsValid())
            {
                Log.Warning($"SurvivorDef for {this} is not valid!");
                return null;
            }

            if (!baseSkin.RuntimeKeyIsValid())
            {
                Log.Warning($"BaseSkin for {this} is not valid!");
                return null;
            }

            var survivorDef = AssetAsyncReferenceManager<SurvivorDef>.LoadAsset(this.survivorDef).WaitForCompletion();
            var skinDef = AssetAsyncReferenceManager<SkinDef>.LoadAsset(baseSkin).WaitForCompletion();

            var bodyObject = survivorDef.bodyPrefab;

            var modelLocator = bodyObject.GetComponent<ModelLocator>();
            if (!modelLocator)
            {
                Log.Warning($"Game object {bodyObject} doesn't have ModelLocator!");
                return null;
            }

            var modelGameObject = modelLocator.modelTransform.gameObject;
            if (!modelGameObject)
            {
                Log.Warning($"Game object {bodyObject}'s ModelLocator doesn't have modelTransform!");
                return null;
            }

            var modelSkinController = modelGameObject.GetComponent<ModelSkinController>();
            if (!modelSkinController)
            {
                Log.Warning($"Game object {bodyObject} doesn't have ModelSkinController!");
                return null;
            }

            return CreateSkinDef(modelGameObject, skinDef);
        }

        public SkinDef CreateSkinDef(GameObject bodyObject)
        {
            if (!useCatalog)
            {
                Log.Warning($"Why are you using method for catalog bodies for def without catalog flag. Skipping {this}...");
                return null;
            }

            var modelLocator = bodyObject.GetComponent<ModelLocator>();
            if (!modelLocator)
            {
                Log.Warning($"Game object {bodyObject} doesn't have ModelLocator!");
                return null;
            }

            var modelGameObject = modelLocator.modelTransform.gameObject;
            if (!modelGameObject)
            {
                Log.Warning($"Game object {bodyObject}'s ModelLocator doesn't have modelTransform!");
                return null;
            }

            var modelSkinController = modelGameObject.GetComponent<ModelSkinController>();
            if (!modelSkinController)
            {
                Log.Warning($"Game object {bodyObject} doesn't have ModelSkinController!");
                return null;
            }

            var baseSkin = modelSkinController.skins.FirstOrDefault(skin => skin.name == baseSkinCatalog);
            if (baseSkin == null)
            {
                Log.Warning($"Game object {bodyObject} doesn't have skin with name {baseSkinCatalog}!");
                return null;
            }

            return CreateSkinDef(modelGameObject, baseSkin);
        }

        public SkinDef CreateSkinDef(GameObject modelGameObject, SkinDef baseSkinDef)
        {
            var modelRenderers = modelGameObject.GetComponentsInChildren(typeof(Renderer), true);

            var skin = ScriptableObject.CreateInstance<SkinDef>();
            (skin as ScriptableObject).name = this.name;
            skin.icon = this.icon;
            skin.nameToken = this.nameToken;
            skin.rootObject = modelGameObject;
            skin.skinDefParamsAddress = new AssetReferenceT<SkinDefParams>("");
            skin.baseSkins = new SkinDef[] { baseSkinDef };
            skin.unlockableDef = unlockable;

            var skinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
            (skinDefParams as ScriptableObject).name = this.name + "Params";
            skinDefParams.rendererInfos = Array.ConvertAll(this.renderInfos, (item) =>
            {
                return new CharacterModel.RendererInfo()
                {
                    renderer = (Renderer)modelRenderers.FirstOrDefault(renderer => renderer.name == item.rendererName),
                    defaultMaterialAddress = new AssetReferenceT<Material>(""),
                    defaultMaterial = item.material,
                    defaultShadowCastingMode = item.defaultShadowCastingMode,
                    ignoreOverlays = item.ignoreOverlays,
                    hideOnDeath = item.hideOnDeath,
                    ignoresMaterialOverrides = item.ignoresMaterialOverrides,
                };
            });
            skinDefParams.meshReplacements = Array.ConvertAll(this.meshReplacements, (item) =>
            {
                return new SkinDefParams.MeshReplacement()
                {
                    renderer = (Renderer)modelRenderers.FirstOrDefault(renderer => renderer.name == item.rendererName),
                    mesh = item.mesh,
                    meshAddress = new AssetReferenceT<Mesh>("")
                };
            });
            skinDefParams.gameObjectActivations = Array.ConvertAll(this.gameObjectActivations, (item) =>
            {
                return new SkinDefParams.GameObjectActivation()
                {
                    spawnPrefabOnModelObject = item.spawnPrefabOnModel,
                    shouldActivate = item.shouldActivate,
                    prefabAddress = item.prefabAddress,
                    prefab = item.prefab,
                    localPosition = item.localPosition,
                    localRotation = item.localRotation,
                    localScale = item.localScale,

                    gameObject = modelGameObject.transform.Find(item.parentGameObject).gameObject
                };
            });

            skin.skinDefParams = skinDefParams;

            return skin;
        }
    }
}
