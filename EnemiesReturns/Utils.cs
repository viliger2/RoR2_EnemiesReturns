using Newtonsoft.Json.Utilities;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.DirectorAPI;

namespace EnemiesReturns
{
    public static class Utils
    {
        public static Dictionary<string, string> StageDccsPoolLookUp = new Dictionary<string, string>()
        {
            {"golemplains","RoR2/Base/golemplains/dpGolemplainsMonsters.asset"},
            {"blackbeach","RoR2/Base/blackbeach/dpBlackBeachMonsters.asset"},
            {"foggyswamp","RoR2/Base/foggyswamp/dpFoggySwampMonsters.asset"},
            {"goolake","RoR2/Base/goolake/dpGooLakeMonsters.asset"},
            {"frozenwall","RoR2/Base/frozenwall/dpFrozenWallMonsters.asset"},
            {"wispgraveyard","RoR2/Base/wispgraveyard/dpWispGraveyardMonsters.asset"},
            {"dampcavesimple","RoR2/Base/dampcave/dpDampCaveMonsters.asset"},
            {"shipgraveyard","RoR2/Base/shipgraveyard/dpShipgraveyardMonsters.asset"},
            {"goldshores","RoR2/Base/goldshores/dpGoldshoresMonsters.asset"},
            {"arena","RoR2/Base/arena/dpArenaMonsters.asset"},
            {"skymeadow","RoR2/Base/skymeadow/dpSkyMeadowMonsters.asset"},
            {"artifactworld","RoR2/Base/artifactworld/dpArtifactWorldMonsters.asset"},
            {"moon2","RoR2/Base/moon/dpMoonMonsters.asset"},
            {"rootjungle","RoR2/Base/rootjungle/dpRootJungleMonsters.asset"},
            {"ancientloft","RoR2/DLC1/ancientloft/dpAncientLoftMonsters.asset"},
            {"itancientloft","RoR2/DLC1/itancientloft/dpITAncientLoftMonsters.asset"},
            {"itdampcave","RoR2/DLC1/itdampcave/dpITDampCaveMonsters.asset"},
            {"itfrozenwall","RoR2/DLC1/itfrozenwall/dpITFrozenWallMonsters.asset"},
            {"itgolemplains","RoR2/DLC1/itgolemplains/dpITGolemplainsMonsters.asset"},
            {"itgoolake","RoR2/DLC1/itgoolake/dpITGooLakeMonsters.asset"},
            {"itmoon","RoR2/DLC1/itmoon/dpITMoonMonsters.asset"},
            {"itskymeadow","RoR2/DLC1/itskymeadow/dpITSkyMeadowMonsters.asset"},
            {"snowyforest","RoR2/DLC1/snowyforest/dpSnowyForestMonsters.asset"},
            {"sulfurpools","RoR2/DLC1/sulfurpools/dpSulfurPoolsMonsters.asset"},
            {"voidstage","RoR2/DLC1/voidstage/dpVoidStageMonsters.asset"},
            {"lakes","RoR2/DLC2/lakes/dpLakesMonsters.asset"},
            {"lakesnight","RoR2/DLC2/lakesnight/dpLakesnightMonsters.asset"},
            {"artifactworld01","RoR2/DLC2/artifactworld01/dpArtifactWorld01Monsters.asset"},
            {"artifactworld02","RoR2/DLC2/artifactworld02/dpArtifactWorld02Monsters.asset"},
            {"artifactworld03","RoR2/DLC2/artifactworld03/dpArtifactWorld03Monsters.asset"},
            {"village","RoR2/DLC2/village/dpVillageMonsters.asset"},
            {"villagenight","RoR2/DLC2/villagenight/dpVillageNightMonsters.asset"},
            {"lemuriantemple","RoR2/DLC2/lemuriantemple/dpLemurianTempleMonsters.asset"},
            {"habitat","RoR2/DLC2/habitat/dpHabitatMonsters.asset"},
            {"habitatfall","RoR2/DLC2/habitatfall/dpHabitatfallMonsters.asset"},
            {"helminthroost","RoR2/DLC2/helminthroost/dpHelminthRoostMonsters.asset"},
            {"meridian","RoR2/DLC2/meridian/dpMeridianMonsters.asset"},
        };

        public static void AddMonsterFamilyToStage(string stageList, FamilyDirectorCardCategorySelection monsterFamily)
        {
            var defaultStages = stageList.Split(",");
            foreach (var stageString in defaultStages)
            {
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                if(StageDccsPoolLookUp.TryGetValue(cleanStageString, out var dpStageAddress))
                {
                    var dpStage = Addressables.LoadAssetAsync<DccsPool>(dpStageAddress).WaitForCompletion();
                    var poolCat = dpStage.poolCategories.FirstOrDefault(poolCat => poolCat.name == "Family");
                    if(!(poolCat == default || poolCat == null))
                    {
                        HG.ArrayUtils.ArrayAppend(ref poolCat.includedIfConditionsMet, new DccsPool.ConditionalPoolEntry()
                        {
                            dccs = monsterFamily,
                            weight = 1f,
                            requiredExpansions = Array.Empty<RoR2.ExpansionManagement.ExpansionDef>()
                        });
                    }
                }
            }
        }

        public static void AddMonsterToStage(string stageList, DirectorCardHolder directorCard)
        {
            var defaultStages = stageList.Split(",");
            foreach (var stageString in defaultStages)
            {
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                var stage = DirectorAPI.ParseInternalStageName(cleanStageString);
                DirectorAPI.Helpers.AddNewMonsterToStage(directorCard, false, stage, stageString);
            }
        }

        public static SkinDef CreateSkinDef(string name, GameObject model, CharacterModel.RendererInfo[] renderInfo, SkinDef baseSkin = null, GameObject[] gameObjectActivations = null)
        {
            //var skinDef = ScriptableObject.CreateInstance<SkinDef>();
            // fuck me
            var skinDef = ScriptableObject.Instantiate(Addressables.LoadAssetAsync<SkinDef>("RoR2/Base/Beetle/skinBeetleDefault.asset").WaitForCompletion());
            (skinDef as ScriptableObject).name = name;
            skinDef.baseSkins = Array.Empty<SkinDef>();
            skinDef.icon = null;
            skinDef.nameToken ="";
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

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent(out T component))
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
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
