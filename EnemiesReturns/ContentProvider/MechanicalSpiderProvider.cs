using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.MechanicalSpider;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        private void CreateMechanicalSpider(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (Configuration.MechanicalSpider.Enabled.Value)
            {
                var spiderLog = Utils.CreateUnlockableDef("Logs.MechanicalSpiderBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_MECHANICAL_SPIDER");
                unlockablesList.Add(spiderLog);

                var spiderStuff = new MechanicalSpiderStuff();

                var doubleShotEffect = spiderStuff.CreateDoubleShotImpactEffect();
                effectsList.Add(new EffectDef(doubleShotEffect));

                ModdedEntityStates.MechanicalSpider.DoubleShot.Fire.projectilePrefab = spiderStuff.CreateDoubleShotProjectilePrefab(doubleShotEffect);
                projectilesList.Add(ModdedEntityStates.MechanicalSpider.DoubleShot.Fire.projectilePrefab);

                ModdedEntityStates.MechanicalSpider.DoubleShot.ChargeFire.effectPrefab = spiderStuff.CreateDoubleShotChargeEffect();

                var spiderEnemyBody = new MechanicalSpiderEnemyBody();
                MechanicalSpiderBodyBase.Skills.DoubleShot = spiderEnemyBody.CreateDoubleShotSkill();
                MechanicalSpiderBodyBase.Skills.Dash = spiderEnemyBody.CreateDashSkill();

                ModdedEntityStates.MechanicalSpider.Dash.Dash.forwardSpeedCoefficientCurve = acdLookup["acdSpiderDash"].curve;

                sdList.Add(MechanicalSpiderBodyBase.Skills.DoubleShot);
                sdList.Add(MechanicalSpiderBodyBase.Skills.Dash);

                MechanicalSpiderBodyBase.SkillFamilies.Primary = Utils.CreateSkillFamily("MechanicalSpiderPrimaryFamily", MechanicalSpiderBodyBase.Skills.DoubleShot);
                MechanicalSpiderBodyBase.SkillFamilies.Utility = Utils.CreateSkillFamily("MechanicalSpiderUtilityFamily", MechanicalSpiderBodyBase.Skills.Dash);

                sfList.Add(MechanicalSpiderBodyBase.SkillFamilies.Primary);
                sfList.Add(MechanicalSpiderBodyBase.SkillFamilies.Utility);

                CreateMechanicalSpiderEnemy(assets, iconLookup, spiderLog, spiderEnemyBody);

                CreateMechanichalSpiderDrone(assets, iconLookup, spiderStuff);
            }
        }

        private void CreateMechanichalSpiderDrone(GameObject[] assets, Dictionary<string, Sprite> iconLookup, MechanicalSpiderStuff spiderStuff)
        {
            var spiderAllyBody = new MechanicalSpiderDroneBody();
            MechanicalSpiderDroneBody.BodyPrefab = spiderAllyBody.AddBodyComponents(assets.First(body => body.name == "MechanicalSpiderDroneBody"), iconLookup["texMechanicalSpiderAllyIcon"]);
            bodyList.Add(MechanicalSpiderDroneBody.BodyPrefab);

            var spiderAllyMaster = new MechanicalSpiderDroneMaster();
            MechanicalSpiderDroneMaster.MasterPrefab = spiderAllyMaster.AddMasterComponents(assets.First(master => master.name == "MechanicalSpiderDroneMaster"), MechanicalSpiderDroneBody.BodyPrefab);
            masterList.Add(MechanicalSpiderDroneMaster.MasterPrefab);

            MechanicalSpiderStuff.InteractablePrefab = spiderStuff.CreateInteractable(assets.First(interactable => interactable.name == "MechanicalSpiderBroken"), MechanicalSpiderDroneMaster.MasterPrefab);
            nopList.Add(MechanicalSpiderStuff.InteractablePrefab);

            MechanicalSpiderStuff.SpawnCards.iscMechanicalSpiderBroken = spiderStuff.CreateInteractableSpawnCard("iscMechanicalSpiderBroken", MechanicalSpiderStuff.InteractablePrefab);
        }

        private void CreateMechanicalSpiderEnemy(GameObject[] assets, Dictionary<string, Sprite> iconLookup, UnlockableDef spiderLog, MechanicalSpiderEnemyBody spiderEnemyBody)
        {
            MechanicalSpiderEnemyBody.BodyPrefab = spiderEnemyBody.AddBodyComponents(assets.First(body => body.name == "MechanicalSpiderBody"), iconLookup["texMechanicalSpiderEnemyIcon"], spiderLog);
            bodyList.Add(MechanicalSpiderEnemyBody.BodyPrefab);

            var spiderEnemyMaster = new MechanicalSpiderEnemyMaster();
            MechanicalSpiderEnemyMaster.MasterPrefab = spiderEnemyMaster.AddMasterComponents(assets.First(master => master.name == "MechanicalSpiderMaster"), MechanicalSpiderEnemyBody.BodyPrefab);
            masterList.Add(MechanicalSpiderEnemyMaster.MasterPrefab);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault = spiderEnemyBody.CreateCard("cscMechanicalSpiderDefault", MechanicalSpiderEnemyMaster.MasterPrefab);
            var dcMechanicalSpiderDefault = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault,
                selectionWeight = Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(Configuration.MechanicalSpider.DefaultStageList.Value, dchMechanicalSpiderDefault);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy = spiderEnemyBody.CreateCard("cscMechanicalSpiderGrassy", MechanicalSpiderEnemyMaster.MasterPrefab, MechanicalSpiderEnemyBody.SkinDefs.Grassy, MechanicalSpiderEnemyBody.BodyPrefab);
            var dcMechanicalSpiderGrassy = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy,
                selectionWeight = Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderGrassy = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderGrassy,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(Configuration.MechanicalSpider.GrassyStageList.Value, dchMechanicalSpiderGrassy);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy = spiderEnemyBody.CreateCard("cscMechanicalSpiderSnowy", MechanicalSpiderEnemyMaster.MasterPrefab, MechanicalSpiderEnemyBody.SkinDefs.Snowy, MechanicalSpiderEnemyBody.BodyPrefab);
            var dcMechanicalSpiderSnowy = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy,
                selectionWeight = Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderSnowy = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderSnowy,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(Configuration.MechanicalSpider.SnowyStageList.Value, dchMechanicalSpiderSnowy);
        }

    }
}
