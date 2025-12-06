using EnemiesReturns.Configuration.Judgement;
using HG;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class ArraignBody
    {
        public static GameObject ArraignP1Body;

        public static GameObject ArraignP2Body;

        public static CharacterSpawnCard cscArraignP1;

        public static CharacterSpawnCard cscArraignP2;

        public static class P1Skills
        {
            public static SkillDef ThreeHitCombo;

            public static SkillDef LeftRightSwing;

            public static SkillDef SwordBeam;

            public static SkillDef SkyDrop;

            public static SkillDef SwordThrow;

            public static SkillDef LightningStrikes;
        }

        public static class P2Skills
        {
            public static SkillDef ThreeHitCombo;

            public static SkillDef LeftRightSwing;

            public static SkillDef DashLeap;

            public static SkillDef SpearThrow;
        }

        public static class HauntSkills
        {
            public static SkillDef ClockAttack;

            public static SkillDef SummonSkyLaser;
        }

        public static ItemDisplayRuleSet CreateIDRS()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsArraign";
            #region PartyHat
            if (Items.PartyHat.PartyHatFactory.ShouldThrowParty())
            {
                var displayRuleGroupPartyHat = new DisplayRuleGroup();
                displayRuleGroupPartyHat.AddDisplayRule(new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Items.PartyHat.PartyHatFactory.PartyHatDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0F, 0.19826F, 0.02128F),
                    localAngles = new Vector3(354.6417F, 0F, 0F),
                    localScale = new Vector3(0.09988F, 0.10159F, 0.10159F),
                    limbMask = LimbFlags.None
                });
                ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupPartyHat,
                    keyAsset = Content.Items.PartyHat
                });
            }
            #endregion

            return idrs;
        }

        public static GameObject SetupP1Body(GameObject bodyPrefab)
        {
            var characterBody = bodyPrefab.GetComponent<CharacterBody>();
            characterBody.baseMaxHealth = ArraignP1.BaseMaxHealth.Value;
            characterBody.baseMoveSpeed = ArraignP1.BaseMoveSpeed.Value;
            characterBody.baseDamage = ArraignP1.BaseDamage.Value;
            characterBody.baseArmor = ArraignP1.BaseArmor.Value;

            characterBody.levelMaxHealth = ArraignP1.LevelMaxHealth.Value;
            characterBody.levelDamage = ArraignP1.LevelDamage.Value;
            characterBody.levelArmor = ArraignP1.LevelArmor.Value;

            characterBody.sprintingSpeedMultiplier = ArraignP1.SprintMultiplier.Value;

            var damageController = bodyPrefab.GetComponent<ArraignDamageController>();
            damageController.segments = ArraignP1.HealthSegments.Value;

            return bodyPrefab;
        }

        public static GameObject SetupP2Body(GameObject bodyPrefab)
        {
            var characterBody = bodyPrefab.GetComponent<CharacterBody>();
            characterBody.baseMaxHealth = ArraignP2.P2BaseMaxHealth.Value;
            characterBody.baseMoveSpeed = ArraignP2.P2BaseMoveSpeed.Value;
            characterBody.baseDamage = ArraignP2.P2BaseDamage.Value;
            characterBody.baseArmor = ArraignP2.P2BaseArmor.Value;

            characterBody.levelMaxHealth = ArraignP2.P2LevelMaxHealth.Value;
            characterBody.levelDamage = ArraignP2.P2LevelDamage.Value;
            characterBody.levelArmor = ArraignP2.P2LevelArmor.Value;

            characterBody.sprintingSpeedMultiplier = ArraignP2.P2SprintMultiplier.Value;

            var damageController = bodyPrefab.GetComponent<ArraignDamageController>();
            damageController.segments = ArraignP2.P2HealthSegments.Value;

            return bodyPrefab;
        }
    }
}
