using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns
{
    public static class Utils
    {
        public class RenderInfo
        {
            public Material material;
            public SkinnedMeshRenderer renderer;
            public bool ignoreOverlays;
        }

        public static SkinDef CreateSkinDef(string name, GameObject model, RenderInfo[] renderInfo, SkinDef baseSkin = null)
        {
            //var skinDef = ScriptableObject.CreateInstance<SkinDef>();
            // fuck me
            var skinDef = ScriptableObject.Instantiate(Addressables.LoadAssetAsync<SkinDef>("RoR2/Base/Beetle/skinBeetleDefault.asset").WaitForCompletion());
            (skinDef as ScriptableObject).name = name;
            skinDef.baseSkins = Array.Empty<SkinDef>();
            skinDef.icon = null;
            skinDef.nameToken = "";
            skinDef.unlockableDef = null;
            skinDef.rootObject = null;
            skinDef.rendererInfos = Array.Empty<CharacterModel.RendererInfo>();
            skinDef.gameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();
            skinDef.meshReplacements = Array.Empty<SkinDef.MeshReplacement>();
            skinDef.projectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>();
            skinDef.minionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>();
            skinDef.runtimeSkin = null;
            skinDef.skinIndex = SkinIndex.None;
            // and fuck the one reading this for good measure

            if (baseSkin)
            {
                skinDef.baseSkins = new SkinDef[] { baseSkin };
            }
            skinDef.rootObject = model;
            skinDef.rendererInfos = Array.ConvertAll(renderInfo, item => new CharacterModel.RendererInfo
            {
                renderer = item.renderer,
                defaultMaterial = item.material,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = item.ignoreOverlays,
                hideOnDeath = false
            });

            skinDef.Bake(); //dunno if we need it

            return skinDef;
        }

        public static SkillFamily CreateSkillFamily(string name, params SkillDef[] skills)
        {
            var skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (skillFamily as ScriptableObject).name = name;
            skillFamily.variants = Array.ConvertAll(skills, item => new SkillFamily.Variant
            {
                skillDef = item,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(item.skillNameToken, false, null)
            });
            skillFamily.defaultVariantIndex = 0;

            return skillFamily;
        }

        public static UnlockableDef CreateUnlockableDef(string name, string nameToken)
        {
            UnlockableDef unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            (unlockableDef as ScriptableObject).name = name;
            unlockableDef.cachedName = name;
            unlockableDef.nameToken = nameToken;

            return unlockableDef;
        }

        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, string categoryName)
        {
            for (int i = 0; i < dcs.categories.Length; i++)
            {
                if (string.CompareOrdinal(dcs.categories[i].name, categoryName) == 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
