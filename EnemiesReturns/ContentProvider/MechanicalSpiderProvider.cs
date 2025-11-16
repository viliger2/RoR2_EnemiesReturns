using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Enemies.MechanicalSpider.Drone;
using EnemiesReturns.Enemies.MechanicalSpider.Enemy;
using EnemiesReturns.Enemies.MechanicalSpider.Turret;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

                ModdedEntityStates.MechanicalSpider.DoubleShot.BaseFire.projectilePrefab = spiderStuff.CreateDoubleShotProjectilePrefab(doubleShotEffect);
                projectilesList.Add(ModdedEntityStates.MechanicalSpider.DoubleShot.BaseFire.projectilePrefab);

                ModdedEntityStates.MechanicalSpider.DoubleShot.BaseChargeFire.effectPrefab = spiderStuff.CreateDoubleShotChargeEffect();

                var spiderEnemyBody = new MechanicalSpiderEnemyBody();
                MechanicalSpiderBodyBase.Skills.Dash = spiderEnemyBody.CreateDashSkill();

                ModdedEntityStates.MechanicalSpider.Dash.Dash.forwardSpeedCoefficientCurve = acdLookup["acdSpiderDash"].curve;

                sdList.Add(MechanicalSpiderBodyBase.Skills.Dash);

                MechanicalSpiderBodyBase.SkillFamilies.Utility = Utils.CreateSkillFamily("MechanicalSpiderUtilityFamily", MechanicalSpiderBodyBase.Skills.Dash);

                sfList.Add(MechanicalSpiderBodyBase.SkillFamilies.Utility);

                CreateMechanicalSpiderEnemy(assets, iconLookup, spiderLog, spiderEnemyBody);

                CreateMechanichalSpiderDrone(assets, iconLookup, spiderStuff);

                if (Configuration.MechanicalSpider.EngiSkillEnabled.Value)
                {
                    SetupMechanicalSpiderEngiSkill(assets, iconLookup);
                }
            }
        }

        private void CreateMechanichalSpiderDrone(GameObject[] assets, Dictionary<string, Sprite> iconLookup, MechanicalSpiderStuff spiderStuff)
        {
            var spiderAllyBody = new MechanicalSpiderDroneBody();

            MechanicalSpiderDroneBody.Skills.DoubleShot = spiderAllyBody.CreateDoubleShotSkill("MechanicalSpiderDroneWeaponDoubleShot", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.Drone.OpenHatch)));
            sdList.Add(MechanicalSpiderDroneBody.Skills.DoubleShot);

            MechanicalSpiderDroneBody.SkillFamilies.Primary = Utils.CreateSkillFamily("MechanicalSpiderDronePrimaryFamily", MechanicalSpiderDroneBody.Skills.DoubleShot);
            sfList.Add(MechanicalSpiderDroneBody.SkillFamilies.Primary);

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
            MechanicalSpiderEnemyBody.Skills.DoubleShot = spiderEnemyBody.CreateDoubleShotSkill("MechanicalSpiderEnemyWeaponDoubleShot", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.Enemy.OpenHatch)));
            sdList.Add(MechanicalSpiderEnemyBody.Skills.DoubleShot);

            MechanicalSpiderEnemyBody.SkillFamilies.Primary = Utils.CreateSkillFamily("MechanicalSpiderEnemyPrimaryFamily", MechanicalSpiderEnemyBody.Skills.DoubleShot);
            sfList.Add(MechanicalSpiderEnemyBody.SkillFamilies.Primary);

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

        private void SetupMechanicalSpiderEngiSkill(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            var setupSkill = new Skills.Engi.MechanicalSpiderTurret.SetupSkill();

            Skills.Engi.MechanicalSpiderTurret.SetupSkill.normalSkill = setupSkill.CreateNormalSkill(iconLookup["texEngiMechanicalSpiderIcon"]);
            sdList.Add(Skills.Engi.MechanicalSpiderTurret.SetupSkill.normalSkill);

            var unlockable = setupSkill.CreateUnlockable(iconLookup["texEngiMechanicalSpiderIcon"]);
            if (unlockable)
            {
                unlockablesList.Add(unlockable);
            }

            SkillFamily skillFamily = Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Engi/EngiBodySpecialFamily.asset").WaitForCompletion();
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Skills.Engi.MechanicalSpiderTurret.SetupSkill.normalSkill,
                unlockableDef = unlockable, 
                viewableNode = new ViewablesCatalog.Node(Skills.Engi.MechanicalSpiderTurret.SetupSkill.normalSkill.skillNameToken, false, null)
            };

            if (ModCompats.AncientScepterCompat.enabled)
            {
                Skills.Engi.MechanicalSpiderTurret.SetupSkill.scepterSkill = setupSkill.CreateScepterSkill(iconLookup["texEngiMechanicalSpiderScepterIcon"]);
                sdList.Add(Skills.Engi.MechanicalSpiderTurret.SetupSkill.scepterSkill);
                ModCompats.AncientScepterCompat.RegisterScepter(Skills.Engi.MechanicalSpiderTurret.SetupSkill.normalSkill, "EngiBody", Skills.Engi.MechanicalSpiderTurret.SetupSkill.scepterSkill);

                MasterSummon.onServerMasterSummonGlobal += Skills.Engi.MechanicalSpiderTurret.SetupSkill.GiveScepterItem;

                Content.Items.MechanicalSpiderTurretScepterHelper = setupSkill.CreateMechanicalSpiderTurretScepterHelperItem();
                itemList.Add(Content.Items.MechanicalSpiderTurretScepterHelper);
            }

            var spiderTurretBody = new MechanicalSpiderTurretBody();

            MechanicalSpiderTurretBody.Skills.DoubleShot = spiderTurretBody.CreateDoubleShotSkill("MechanicalSpiderTurretWeaponDoubleShot", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.Turret.OpenHatch)));
            sdList.Add(MechanicalSpiderTurretBody.Skills.DoubleShot);

            MechanicalSpiderTurretBody.SkillFamilies.Primary = Utils.CreateSkillFamily("MechanicalSpiderTurretPrimaryFamily", MechanicalSpiderTurretBody.Skills.DoubleShot);
            sfList.Add(MechanicalSpiderTurretBody.SkillFamilies.Primary);

            MechanicalSpiderTurretBody.BodyPrefab = spiderTurretBody.AddBodyComponents(assets.First(body => body.name == "MechanicalSpiderTurretBody"), iconLookup["texMechanicalSpiderAllyIcon"]);
            bodyList.Add(MechanicalSpiderTurretBody.BodyPrefab);

            var mechanicalSpiderTurretMaster = new MechanicalSpiderTurretMaster();
            MechanicalSpiderTurretMaster.MasterPrefab = mechanicalSpiderTurretMaster.AddMasterComponents(assets.First(master => master.name == "MechanicalSpiderTurretMaster"), MechanicalSpiderTurretBody.BodyPrefab);
            masterList.Add(MechanicalSpiderTurretMaster.MasterPrefab);
            ModdedEntityStates.Engi.PlaceMechSpider.spiderTurretMasterPrefab = MechanicalSpiderTurretMaster.MasterPrefab;

            ModdedEntityStates.Engi.PlaceMechSpider.spiderBlueprintPrefab = setupSkill.SetupBlueprint(assets.First(asset => asset.name == "MechanicalSpiderTurretBlueprints"));
        }
    }
}
