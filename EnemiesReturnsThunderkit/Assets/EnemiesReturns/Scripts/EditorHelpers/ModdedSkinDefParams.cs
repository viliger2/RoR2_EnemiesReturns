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

        [Tooltip("true - uses catalog and catalog names to find bodies instead of loading assets via Addressables")]
        public bool useCatalog = false;

        public AssetReferenceT<SkinDef> baseSkin;

        public AssetReferenceT<GameObject> bodyPrefab;

        public string bodyPrefabNameCatalog;

        public string baseSkinCatalog;

        public string nameToken;

        public Sprite icon;

        public RenderInfo[] renderInfos = Array.Empty<RenderInfo>();

        public MeshReplacement[] meshReplacements = Array.Empty<MeshReplacement>();

        public SkinDef CreateSkinDef()
        {
            return null;
            // if (useCatalog)
            // {
            //     Log.Warning($"Can't build a skin via CreateSkinDef when useCatalog is true. Skipping {this}...");
            //     return null;
            // }

            // if (!bodyPrefab.RuntimeKeyIsValid())
            // {
            //     Log.Warning($"BodyPrefab for {this} is not valid!");
            //     return null;
            // }

            // if (!baseSkin.RuntimeKeyIsValid())
            // {
            //     Log.Warning($"BaseSkin for {this} is not valid!");
            //     return null;
            // }

            // var bodyObject = AssetAsyncReferenceManager<GameObject>.LoadAsset(bodyPrefab).WaitForCompletion();
            // var skinDef = AssetAsyncReferenceManager<SkinDef>.LoadAsset(baseSkin).WaitForCompletion();

            // return CreateSkinDef(bodyObject, skinDef);
        }

        public SkinDef CreateSkinDef(GameObject bodyObject, SkinDef baseSkinDef)
        {
            return null;
            // var modelLocator = bodyObject.GetComponent<ModelLocator>();
            // if (!modelLocator)
            // {
            //     Log.Warning($"Game object {bodyObject} doesn't have ModelLocator!");
            //     return null;
            // }

            // var modelGameObject = modelLocator.modelTransform.gameObject;
            // if (!modelGameObject)
            // {
            //     Log.Warning($"Game object {bodyObject}'s ModelLocator doesn't have modelTransform!");
            //     return null;
            // }

            // var modelSkinController = modelGameObject.GetComponent<ModelSkinController>();
            // if (!modelSkinController)
            // {
            //     Log.Warning($"Game object {bodyObject} doesn't have ModelSkinController!");
            //     return null;
            // }

            // var modelRenderers = modelGameObject.GetComponentsInChildren<Renderer>();

            // var skin = ScriptableObject.CreateInstance<SkinDef>();
            // (skin as ScriptableObject).name = this.name;
            // skin.icon = this.icon;
            // skin.nameToken = this.nameToken;
            // skin.rootObject = modelGameObject;
            // skin.skinDefParamsAddress = new AssetReferenceT<SkinDefParams>("");
            // skin.baseSkins = new SkinDef[] { baseSkinDef };

            // var skinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
            // (skinDefParams as ScriptableObject).name = this.name + "Params";
            // skinDefParams.rendererInfos = Array.ConvertAll(this.renderInfos, (item) =>
            // {
            //     return new CharacterModel.RendererInfo()
            //     {
            //         renderer = modelRenderers.FirstOrDefault(renderer => renderer.name == item.rendererName),
            //         defaultMaterialAddress = new AssetReferenceT<Material>(""),
            //         defaultMaterial = ContentProvider.MaterialCache[item.material.name],
            //         defaultShadowCastingMode = item.defaultShadowCastingMode,
            //         ignoreOverlays = item.ignoreOverlays,
            //         hideOnDeath = item.hideOnDeath,
            //         ignoresMaterialOverrides = item.ignoresMaterialOverrides,
            //     };
            // });
            // skinDefParams.meshReplacements = Array.ConvertAll(this.meshReplacements, (item) =>
            // {
            //     return new SkinDefParams.MeshReplacement()
            //     {
            //         renderer = modelRenderers.FirstOrDefault(renderer => renderer.name == item.rendererName),
            //         mesh = item.mesh,
            //         meshAddress = new AssetReferenceT<Mesh>("")
            //     };
            // });

            // skin.skinDefParams = skinDefParams;

            // return skin;
        }
    }
}
