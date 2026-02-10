using EnemiesReturns.Enemies.ArcherBug;
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
        private void CreateArcherBug(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            if (Configuration.General.EnableArcherBug.Value)
            {
                ArcherBugBody.StadiaJungleMeshPrefab = assets.First(asset => asset.name == "ArcherBug_stadiajungle");

                var archerBugStuff = new ArcherBugStuff();

                var ArcherBugCausticSpitProjectile = archerBugStuff.CreateCausticSpitProjectile();
                ModdedEntityStates.ArcherBugs.FireCausticSpit.projectilePrefab = ArcherBugCausticSpitProjectile;
                projectilesList.Add(ArcherBugCausticSpitProjectile);

                var archerBugLog = Utils.CreateUnlockableDef("Logs.ArcherBugBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_ARCHERBUG");
                unlockablesList.Add(archerBugLog);

                var ArcherBugDeathEffect = archerBugStuff.CreateDeathEffect();
                effectsList.Add(new EffectDef(ArcherBugDeathEffect));
                ModdedEntityStates.ArcherBugs.DeathState.deathEffectPrefab = ArcherBugDeathEffect;

                ModdedEntityStates.ArcherBugs.FireCausticSpit.chargeEffect = archerBugStuff.CreateCausticSpitChargeEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.ArcherBugs.FireCausticSpit.chargeEffect));

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
                    selectionWeight = Configuration.ArcherBug.SelectionWeight.Value,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.ArcherBug.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchArcherBugDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcArcherBugDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.ArcherBug.DefaultStageList.Value, dchArcherBugDefault);

                ArcherBugBody.SpawnCards.cscArcherBugJungle = archerBugBody.CreateCard("cscArhcerBugJungle", ArcherBugMaster.MasterPrefab, ArcherBugBody.SkinDefs.Jungle, ArcherBugBody.BodyPrefab);
                var dcArhcerBugJungle = new DirectorCard
                {
                    spawnCard = ArcherBugBody.SpawnCards.cscArcherBugJungle,
                    selectionWeight = Configuration.ArcherBug.SelectionWeight.Value,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.ArcherBug.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchArcherBugJungle = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcArhcerBugJungle,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
                };
                Utils.AddMonsterToStages(EnemiesReturns.Configuration.ArcherBug.JungleStageList.Value, dchArcherBugJungle);

            }
        }
    }
}
