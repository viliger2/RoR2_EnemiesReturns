using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public AssetReferenceT<SkinDef> baseSkin;

        public string nameToken;

        public Sprite icon;

        public AssetReferenceT<GameObject> bodyPrefab;

        public RenderInfo[] renderInfos = Array.Empty<RenderInfo>();

        public MeshReplacement[] meshReplacements = Array.Empty<MeshReplacement>();

        public SkinDef CreateSkinDef()
        {
            //     if (!bodyPrefab.RuntimeKeyIsValid())
            //     {
            //         Log.Warning($"BodyPrefab for {this} is not valid!");
            //         return null;
            //     }

            //     if (!baseSkin.RuntimeKeyIsValid())
            //     {
            //         Log.Warning($"BaseSkin for {this} is not valid!");
            //         return null;
            //     }

            //     var bodyObject = AssetAsyncReferenceManager<GameObject>.LoadAsset(bodyPrefab).WaitForCompletion();

            //     var modelLocator = bodyObject.GetComponent<ModelLocator>();
            //     if (!modelLocator)
            //     {
            //         Log.Warning($"Game object {bodyObject} doesn't have ModelLocator!");
            //         return null;
            //     }

            //     var modelGameObject = modelLocator.modelTransform.gameObject;
            //     if (!modelGameObject)
            //     {
            //         Log.Warning($"Game object {bodyObject}'s ModelLocator doesn't have modelTransform!");
            //         return null;
            //     }

            //     var modelSkinController = modelGameObject.GetComponent<ModelSkinController>();
            //     if (!modelSkinController)
            //     {
            //         Log.Warning($"Game object {bodyObject} doesn't have ModelSkinController!");
            //         return null;
            //     }

            //     var modelRenderers = modelGameObject.GetComponentsInChildren<Renderer>();

            //     var skin = ScriptableObject.CreateInstance<SkinDef>();
            //     (skin as ScriptableObject).name = this.name;
            //     skin.icon = this.icon;
            //     skin.nameToken = this.nameToken;
            //     skin.rootObject = modelGameObject;
            //     skin.skinDefParamsAddress = new AssetReferenceT<SkinDefParams>("");
            //     skin.baseSkins = new SkinDef[] { AssetAsyncReferenceManager<SkinDef>.LoadAsset(baseSkin).WaitForCompletion() };

            //     var skinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
            //     (skinDefParams as ScriptableObject).name = this.name + "Params";
            //     skinDefParams.rendererInfos = Array.ConvertAll(this.renderInfos, (item) =>
            //     {
            //         return new CharacterModel.RendererInfo()
            //         {
            //             renderer = modelRenderers.FirstOrDefault(renderer => renderer.name == item.rendererName),
            //             defaultMaterialAddress = new AssetReferenceT<Material>(""),
            //             defaultMaterial = item.material,
            //             defaultShadowCastingMode = item.defaultShadowCastingMode,
            //             ignoreOverlays = item.ignoreOverlays,
            //             hideOnDeath = item.hideOnDeath,
            //             ignoresMaterialOverrides = item.ignoresMaterialOverrides,
            //         };
            //     });
            //     skinDefParams.meshReplacements = Array.ConvertAll(this.meshReplacements, (item) =>
            //     {
            //         return new SkinDefParams.MeshReplacement()
            //         {
            //             renderer = modelRenderers.FirstOrDefault(renderer => renderer.name == item.rendererName),
            //             mesh = item.mesh,
            //             meshAddress = new AssetReferenceT<Mesh>("")
            //         };
            //     });

            //     skin.skinDefParams = skinDefParams;

            //     HG.ArrayUtils.ArrayAppend(ref modelSkinController.skins, in skin);

            //     return skin;
            // }
            return null;
        }
    }
}
