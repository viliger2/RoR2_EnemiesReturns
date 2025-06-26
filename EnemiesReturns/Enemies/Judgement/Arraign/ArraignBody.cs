using EnemiesReturns.Configuration.Judgement;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class ArraignBody
    {
        public static GameObject ArraignP1Body;

        public static GameObject ArraignP2Body;

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
