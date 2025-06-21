using BepInEx.Configuration;
using EnemiesReturns.Configuration;
using EnemiesReturns.Enemies.ArcherBug;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        private void CreateArcherBug(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            var ArcherBugStuff = new ArcherBugStuff();
            var ArcherBugCausticSpitProjectile = ArcherBugStuff.CreateCausticSpitProjectile();
            ModdedEntityStates.ArcherBugs.FireCausticSpit.projectilePrefab = ArcherBugCausticSpitProjectile;

            var archerBugLog = Utils.CreateUnlockableDef("Logs.ArcherBugBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_ARCHERBUG");
            unlockablesList.Add(archerBugLog);
            var ArcherBugDeathEffect = ArcherBugStuff.CreateDeathEffect();
            effectsList.Add(new EffectDef(ArcherBugDeathEffect));
            ModdedEntityStates.ArcherBugs.DeathState.deathEffectPrefab = ArcherBugDeathEffect;
            projectilesList.Add(ArcherBugCausticSpitProjectile);
            var archerBugBody = new ArcherBugBody();
            ArcherBugBody.Skills.CausticSpit = archerBugBody.CreateCausticSpitSkill();

            sdList.Add(ArcherBugBody.Skills.CausticSpit);
            ArcherBugBody.SkillFamilies.Primary = Utils.CreateSkillFamily("ArcherBugPrimaryFamily", ArcherBugBody.Skills.CausticSpit);
            sfList.Add(ArcherBugBody.SkillFamilies.Primary);
            ArcherBugBody.BodyPrefab = archerBugBody.AddBodyComponents(assets.First(body => body.name == "ArcherBugBody"), iconLookup["texArcherBugIcon"], archerBugLog);
            bodyList.Add(ArcherBugBody.BodyPrefab);
            ArcherBugMaster.MasterPrefab = new ArcherBugMaster().AddMasterComponents(assets.First(master => master.name == "ArcherBugMaster"), ArcherBugBody.BodyPrefab);
            masterList.Add(ArcherBugMaster.MasterPrefab);
            ArcherBugBody.SpawnCards.cscArcherBugDefault = archerBugBody.CreateCard("cscArcherBugDefault", ArcherBugMaster.MasterPrefab, ArcherBugBody.SkinDefs.Default, ArcherBugBody.BodyPrefab);
            var dcArcherBugDefault = new DirectorCard
            {
                spawnCard = ArcherBugBody.SpawnCards.cscArcherBugDefault,
                selectionWeight = 1,
                preventOverhead = true,
                minimumStageCompletions = 0
            };
            DirectorAPI.DirectorCardHolder dchArcherBugDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dcArcherBugDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(EnemiesReturns.Configuration.ArcherBug.DefaultStageList.Value, dchArcherBugDefault);
        }
    }
}
