using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns
{
    public static class Utils
    {
        public static SkinDef CreateSkinDef(string name, GameObject model, CharacterModel.RendererInfo[] renderInfo, SkinDef baseSkin = null, GameObject[] gameObjectActivations = null)
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
            skinDef.rendererInfos = renderInfo;
            if (gameObjectActivations != null)
            {
                skinDef.gameObjectActivations = Array.ConvertAll(gameObjectActivations, item => new SkinDef.GameObjectActivation
                {
                    gameObject = item,
                    shouldActivate = true
                });
            }

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

        public static NetworkSoundEventDef CreateNetworkSoundDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.eventName = eventName;

            return networkSoundEventDef;
        }

        public static void AddMonsterToCardCategory(DirectorCard card, string categoryName, DirectorCardCategorySelection stageCard)
        {
            int num = Utils.FindCategoryIndexByName(stageCard, categoryName);
            if (num >= 0)
            {
                stageCard.AddCard(num, card);
            }
        }

        public static int FindCategoryIndexByName(DirectorCardCategorySelection dcs, string categoryName)
        {
            if (dcs == null)
            {
                return -1;
            }
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
