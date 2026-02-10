using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Swift;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        private void CreateSwift(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (Configuration.General.EnableSwift.Value)
            {
                var swiftLog = Utils.CreateUnlockableDef("Logs.SwiftBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SWIFT");

                var swiftStuff = new SwiftStuff();
                ModdedEntityStates.Swift.Dive.DivePrep.effectPrefab = swiftStuff.CreateDiveChargeEffect(assets.First(effect => effect.name == "SwiftChargeDiveEffect"));
                effectsList.Add(new RoR2.EffectDef(ModdedEntityStates.Swift.Dive.DivePrep.effectPrefab));

                ModdedEntityStates.Swift.Dive.DiveEnd.effectPrefab = swiftStuff.CreateDiveGroundImpactEffect();
                effectsList.Add(new RoR2.EffectDef(ModdedEntityStates.Swift.Dive.DiveEnd.effectPrefab));

                var swiftBody = new SwiftBody();

                SwiftBody.Skills.Dive = swiftBody.CreateDiveSkill();
                sdList.Add(SwiftBody.Skills.Dive);

                SwiftBody.SkillFamilies.Primary = Utils.CreateSkillFamily("SwiftPrimaryFamily", SwiftBody.Skills.Dive);
                sfList.Add(SwiftBody.SkillFamilies.Primary);

                SwiftBody.BodyPrefab = swiftBody.AddBodyComponents(assets.First(body => body.name == "SwiftBody"), iconLookup["texSwiftIcon"], swiftLog, acdLookup);
                bodyList.Add(SwiftBody.BodyPrefab);

                SwiftMaster.MasterPrefab = new SwiftMaster().AddMasterComponents(assets.First(master => master.name == "SwiftMaster"), SwiftBody.BodyPrefab);
                masterList.Add(SwiftMaster.MasterPrefab);

                SwiftBody.SpawnCards.cscSwiftDefault = swiftBody.CreateCard("cscSwiftDefault", SwiftMaster.MasterPrefab, SwiftBody.SkinDefs.Default, SwiftBody.BodyPrefab);
                var dcSwiftDefault = new DirectorCard
                {
                    spawnCard = SwiftBody.SpawnCards.cscSwiftDefault,
                    selectionWeight = Configuration.Swift.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Swift.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSwiftDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSwiftDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
                };
                Utils.AddMonsterToStages(Configuration.Swift.DefaultStageList.Value, dchSwiftDefault);
                Utils.AddMonsterToCardCategory(dcSwiftDefault, MonsterCategories.BasicMonsters, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion());

                SwiftBody.SpawnCards.cscSwiftRallypoint = swiftBody.CreateCard("cscSwiftRallypoint", SwiftMaster.MasterPrefab, SwiftBody.SkinDefs.RallypointDelta, SwiftBody.BodyPrefab);
                var dcSwiftRallypoint = new DirectorCard
                {
                    spawnCard = SwiftBody.SpawnCards.cscSwiftRallypoint,
                    selectionWeight = Configuration.Swift.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Swift.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSwiftRallypoint = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSwiftRallypoint,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
                };
                Utils.AddMonsterToStages(Configuration.Swift.RallypointStageList.Value, dchSwiftRallypoint);
            }
        }
    }
}
