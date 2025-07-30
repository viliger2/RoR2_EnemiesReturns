using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Swift;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        private void CreateSwift(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (Configuration.Swift.Enabled.Value)
            {
                var swiftLog = Utils.CreateUnlockableDef("Logs.SwiftBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SWIFT");

                var swiftStuff = new SwiftStuff();
                ModdedEntityStates.Swift.Dive.DivePrep.effectPrefab = swiftStuff.CreateDiveChargeEffect(assets.First(effect => effect.name == "SwiftChargeDiveEffect"));
                effectsList.Add(new RoR2.EffectDef(ModdedEntityStates.Swift.Dive.DivePrep.effectPrefab));

                var swiftBody = new SwiftBody();

                SwiftBody.Skills.Dive = swiftBody.CreateDiveSkill();
                sdList.Add(SwiftBody.Skills.Dive);

                SwiftBody.SkillFamilies.Primary = Utils.CreateSkillFamily("SwiftPrimaryFamily", SwiftBody.Skills.Dive);
                sfList.Add(SwiftBody.SkillFamilies.Primary);

                SwiftBody.BodyPrefab = swiftBody.AddBodyComponents(assets.First(body => body.name == "SwiftBody"), iconLookup["texSpitterIcon"], swiftLog, acdLookup);
                bodyList.Add(SwiftBody.BodyPrefab);

                SwiftMaster.MasterPrefab = new SwiftMaster().AddMasterComponents(assets.First(master => master.name == "SwiftMaster"), SwiftBody.BodyPrefab);
                masterList.Add(SwiftMaster.MasterPrefab);
            }
        }
    }
}
