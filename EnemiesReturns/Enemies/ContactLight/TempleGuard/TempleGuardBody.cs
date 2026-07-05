using EnemiesReturns.Enemies.Colossus;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.ContactLight.TempleGuard
{
    public class TempleGuardBody
    {
        public GameObject SetupBody(GameObject prefab)
        {
            var body = prefab.GetComponent<CharacterBody>();
            if (body)
            {
                body._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();

                body.baseMaxHealth = Configuration.ContactLight.TempleGuard.BaseMaxHealth.Value;
                body.baseMoveSpeed = Configuration.ContactLight.TempleGuard.BaseMoveSpeed.Value;
                body.baseJumpPower = Configuration.ContactLight.TempleGuard.BaseJumpPower.Value;
                body.baseDamage = Configuration.ContactLight.TempleGuard.BaseDamage.Value;
                body.baseArmor = Configuration.ContactLight.TempleGuard.BaseArmor.Value;

                body.levelMaxHealth = Configuration.ContactLight.TempleGuard.LevelMaxHealth.Value;
                body.levelDamage = Configuration.ContactLight.TempleGuard.LevelDamage.Value;
                body.levelArmor = Configuration.ContactLight.TempleGuard.LevelArmor.Value;
            }

            var modelLocator = prefab.GetComponent<ModelLocator>();
            if (modelLocator && modelLocator.modelTransform)
            {
                var footsteps = modelLocator.modelTransform.gameObject.GetComponent<FootstepHandler>();
                if(footsteps)
                {
                    footsteps.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericHugeFootstepDust.prefab").WaitForCompletion();
                }
            }

            return prefab;
        }

        public static void SetupDirectiorCard(CharacterSpawnCard[] cards)
        {
            var card = cards.First(asset => asset.name == "cscTempleGuard");
            card.directorCreditCost = Configuration.ContactLight.TempleGuard.DirectorCost.Value;

            DirectorCard dcTempleGuard = new DirectorCard
            {
                spawnCard = card,
                selectionWeight = Configuration.ContactLight.TempleGuard.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.ContactLight.TempleGuard.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchTempleGuard = new DirectorAPI.DirectorCardHolder
            {
                Card = dcTempleGuard,
                MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
            };
            Utils.AddMonsterToStages(Configuration.ContactLight.TempleGuard.DefaultStageList.Value, dchTempleGuard);

        }

        public static void SetupSkills(SkillDef[] skills)
        {
            var primary = skills.First(skill => (skill as ScriptableObject).name == "sdTempleGuardPrimary");
            primary.baseRechargeInterval = Configuration.ContactLight.TempleGuard.BarrageCooldown.Value;
            primary.icon = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Mage.texMageSkillIcons_png_texMageSkillIcons_0_).WaitForCompletion();            

            var overclock = skills.First(skill => (skill as ScriptableObject).name == "sdTempleGuardianOverclock");
            overclock.baseRechargeInterval = Configuration.ContactLight.TempleGuard.OverclockCooldown.Value;
            overclock.icon = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.texDroneTechSkillIcons_png_texDroneTechSkillIcons_7_).WaitForCompletion();

            var shield = skills.First(skill => (skill as ScriptableObject).name == "sdTempleGuardianShell");
            shield.icon = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common.texBuffGenericShield_tif).WaitForCompletion();
        }

    }

}
