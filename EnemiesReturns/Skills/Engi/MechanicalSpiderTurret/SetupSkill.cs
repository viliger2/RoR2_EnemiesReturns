using EnemiesReturns.Components;
using EnemiesReturns.Enemies.Judgement.Arraign;
using EnemiesReturns.ModdedEntityStates.Engi;
using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.MasterCatalog;

namespace EnemiesReturns.Skills.Engi.MechanicalSpiderTurret
{
    public class SetupSkill
    {
        // TODO: config

        public static SkillDef normalSkill;

        public static SkillDef scepterSkill;

        public static MasterIndex masterIndex;

        public SkillDef CreateNormalSkill(Sprite icon)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = "ENEMIES_RETURNS_ENGI_SPECIAL_MECHSPIDER_NAME";
            skillDef.skillNameToken = "ENEMIES_RETURNS_ENGI_SPECIAL_MECHSPIDER_NAME";
            skillDef.skillDescriptionToken = "ENEMIES_RETURNS_ENGI_SPECIAL_MECHSPIDER_DESCRIPTION";
            skillDef.keywordTokens = new string[] { };
            skillDef.icon = icon;
            (skillDef as ScriptableObject).name = skillDef.skillName;
            skillDef.activationState = new SerializableEntityStateType(typeof(PlaceMechSpider));
            skillDef.activationStateMachineName = "Weapon";
            skillDef.baseMaxStock = 2;
            skillDef.baseRechargeInterval = 30f;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.canceledFromSprinting = false;
            skillDef.cancelSprintingOnActivation = true;
            skillDef.fullRestockOnAssign = true;
            skillDef.interruptPriority = InterruptPriority.Skill;
            skillDef.isCombatSkill = false;
            skillDef.mustKeyPress = false;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 0;

            return skillDef;
        }

        public SkillDef CreateScepterSkill(Sprite icon)
        {
            SkillDef skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = "ENEMIES_RETURNS_ENGI_SPECIAL_MECHSPIDER_SCEPTER_NAME";
            skillDef.skillNameToken = "ENEMIES_RETURNS_ENGI_SPECIAL_MECHSPIDER_SCEPTER_NAME";
            skillDef.skillDescriptionToken = "ENEMIES_RETURNS_ENGI_SPECIAL_MECHSPIDER_SCEPTER_DESCRIPTION";
            skillDef.keywordTokens = new string[] { };
            skillDef.icon = icon;
            (skillDef as ScriptableObject).name = skillDef.skillName;
            skillDef.activationState = new SerializableEntityStateType(typeof(PlaceMechSpider));
            skillDef.activationStateMachineName = "Weapon";
            skillDef.baseMaxStock = 2;
            skillDef.baseRechargeInterval = 30f;
            skillDef.beginSkillCooldownOnSkillEnd = false;
            skillDef.canceledFromSprinting = false;
            skillDef.cancelSprintingOnActivation = true;
            skillDef.fullRestockOnAssign = true;
            skillDef.interruptPriority = InterruptPriority.Skill;
            skillDef.isCombatSkill = false;
            skillDef.mustKeyPress = false;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 0;

            return skillDef;
        }

        public UnlockableDef CreateUnlockable(Sprite icon)
        {
            UnlockableDef unlockableDef = null;
            if (!Configuration.MechanicalSpider.EngiSkillForceUnlock.Value)
            {
                unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
                (unlockableDef as ScriptableObject).name = "Skills.Engi.EnemiesReturnsSpiderTurret";
                unlockableDef.cachedName = "Skills.Engi.EnemiesReturnsSpiderTurret";
                unlockableDef.nameToken = "ENEMIES_RETURNS_ENGI_SPECIAL_MECHSPIDER_NAME";
                unlockableDef.hidden = false; // it actually does fucking nothing, it only hides it on game finish
                unlockableDef.achievementIcon = icon;
            }
            return unlockableDef;
        }

        public ItemDef CreateMechanicalSpiderTurretScepterHelperItem()
        {
            var itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.canRemove = false;
            itemDef.name = "MechanicalSpiderTurretScepterHelper";
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.NoTier;
#pragma warning restore CS0618 // Type or member is obsolete
            itemDef.descriptionToken = "Stat modifier for Mech Spiders summoned by Engi with Scepter.";
            itemDef.nameToken = "MechanicalSpiderTurretScepterHelper";
            itemDef.pickupToken = "Stat modifier for Mech Spiders summoned by Engi with Scepter.";
            itemDef.hidden = true;
            itemDef.pickupIconSprite = null;
            itemDef.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotSteal,
                ItemTag.CannotDuplicate,
                ItemTag.AIBlacklist
            };

            return itemDef;
        }

        public GameObject SetupBlueprint(GameObject blueprintObject)
        {
            var blueprintOk = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Engi.matBlueprintsOk_mat).WaitForCompletion();

            var renderer = blueprintObject.transform.Find("mdlMechanicalSpider/MechanicalSpider").GetComponent<Renderer>();
            renderer.material = blueprintOk;

            var blueprintController = blueprintObject.AddComponent<BlueprintController>();
            blueprintController.okMaterial = blueprintOk;
            blueprintController.invalidMaterial = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Engi.matBlueprintsInvalid_mat).WaitForCompletion();

            blueprintController.renderers = new Renderer[]
            {
                renderer
            };

            var existingBlueprint = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Engi.EngiTurretBlueprints_prefab).WaitForCompletion();

            blueprintObject.transform.Find("BlobLightProjector").GetComponent<Projector>().material = existingBlueprint.transform.Find("BlobLightProjector").GetComponent<Projector>().material;

            return blueprintObject;
        }

        [SystemInitializer(new Type[] { typeof(MasterCatalog) })]
        private static void Init()
        {
            if (!EnemiesReturns.EnemiesReturnsPlugin.ModIsLoaded)
            {
                return;
            }

            if (!EnemiesReturns.Configuration.MechanicalSpider.EngiSkillEnabled.Value)
            {
                return;
            }

            masterIndex = MasterCatalog.FindMasterIndex("MechanicalSpiderTurretMaster");

        }

        public static void GiveScepterItem(MasterSummon.MasterSummonReport summonReport)
        {
            var engiMaster = summonReport.leaderMasterInstance;
            if (!engiMaster || !engiMaster.inventory)
            {
                return;
            }
            var spiderMaster = summonReport.summonMasterInstance;
            if(!spiderMaster || !spiderMaster.inventory)
            {
                return;
            }
            if (engiMaster.inventory.GetItemCountPermanent(ModCompats.AncientScepterCompat.AncientScepterItemIndex) > 0 && spiderMaster.masterIndex == masterIndex)
            {
                spiderMaster.inventory.GiveItemPermanent(Content.Items.MechanicalSpiderTurretScepterHelper);
            }
        }

    }
}
