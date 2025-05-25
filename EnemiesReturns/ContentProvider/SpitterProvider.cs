using EnemiesReturns.Enemies.Spitter;
using R2API;
using RoR2;
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
        private void CreateSpitter(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            if (Configuration.Spitter.Enabled.Value)
            {
                var spitterStuff = new SpitterStuff();
                ModdedEntityStates.Spitter.Bite.biteEffectPrefab = spitterStuff.CreateBiteEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Spitter.Bite.biteEffectPrefab));

                var chargedSpitSmallDoTZone = spitterStuff.CreatedChargedSpitSmallDoTZone();
                var chargedSpitDoTZone = spitterStuff.CreateChargedSpitDoTZone();
                var chargedSpitChunkProjectile = spitterStuff.CreateChargedSpitSplitProjectile(chargedSpitSmallDoTZone);
                var chargedSpitProjectile = spitterStuff.CreateChargedSpitProjectile(chargedSpitDoTZone, chargedSpitChunkProjectile); ;
                ModdedEntityStates.Spitter.FireChargedSpit.projectilePrefab = chargedSpitProjectile;
                projectilesList.Add(chargedSpitProjectile);
                projectilesList.Add(chargedSpitSmallDoTZone);
                projectilesList.Add(chargedSpitDoTZone);
                projectilesList.Add(chargedSpitChunkProjectile);

                Junk.ModdedEntityStates.Spitter.NormalSpit.normalSpitProjectile = spitterStuff.CreateNormalSpitProjectile();
                projectilesList.Add(Junk.ModdedEntityStates.Spitter.NormalSpit.normalSpitProjectile);

                var spitterLog = Utils.CreateUnlockableDef("Logs.SpitterBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SPITTER");
                unlockablesList.Add(spitterLog);

                var spitterBody = new SpitterBody();
                SpitterBody.Skills.NormalSpit = spitterBody.CreateNormalSpitSkill();
                SpitterBody.Skills.ChargedSpit = spitterBody.CreateChargedSpitSkill();
                SpitterBody.Skills.Bite = spitterBody.CreateBiteSkill();

                sdList.Add(SpitterBody.Skills.NormalSpit);
                sdList.Add(SpitterBody.Skills.Bite);
                sdList.Add(SpitterBody.Skills.ChargedSpit);

                SpitterBody.SkillFamilies.Primary = Utils.CreateSkillFamily("SpitterPrimaryFamily", SpitterBody.Skills.NormalSpit);
                SpitterBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("SpitterSecondaryFamily", SpitterBody.Skills.Bite);
                SpitterBody.SkillFamilies.Special = Utils.CreateSkillFamily("SpitterSpecialFamily", SpitterBody.Skills.ChargedSpit);

                sfList.Add(SpitterBody.SkillFamilies.Primary);
                sfList.Add(SpitterBody.SkillFamilies.Secondary);
                sfList.Add(SpitterBody.SkillFamilies.Special);

                SpitterBody.BodyPrefab = spitterBody.AddBodyComponents(assets.First(body => body.name == "SpitterBody"), iconLookup["texSpitterIcon"], spitterLog);
                bodyList.Add(SpitterBody.BodyPrefab);

                SpitterMaster.MasterPrefab = new SpitterMaster().AddMasterComponents(assets.First(master => master.name == "SpitterMaster"), SpitterBody.BodyPrefab);
                masterList.Add(SpitterMaster.MasterPrefab);

                SpitterBody.SpawnCards.cscSpitterDefault = spitterBody.CreateCard("cscSpitterDefault", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Default, SpitterBody.BodyPrefab);
                var dcSpitterDefault = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterDefault,
                    selectionWeight = Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(Configuration.Spitter.DefaultStageList.Value, dchSpitterDefault);

                SpitterBody.SpawnCards.cscSpitterLakes = spitterBody.CreateCard("cscSpitterLakes", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Lakes, SpitterBody.BodyPrefab);
                var dcSpitterLakes = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterLakes,
                    selectionWeight = Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterLakes = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterLakes,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(Configuration.Spitter.LakesStageList.Value, dchSpitterLakes);

                SpitterBody.SpawnCards.cscSpitterSulfur = spitterBody.CreateCard("cscSpitterSulfur", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Sulfur, SpitterBody.BodyPrefab);
                var dcSpitterSulfur = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterSulfur,
                    selectionWeight = Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterSulfur = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterSulfur,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(Configuration.Spitter.SulfurStageList.Value, dchSpitterSulfur);

                SpitterBody.SpawnCards.cscSpitterDepths = spitterBody.CreateCard("cscSpitterDepths", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Depths, SpitterBody.BodyPrefab);
                var dcSpitterDepth = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterDepths,
                    selectionWeight = Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterDepths = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDepth,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStages(Configuration.Spitter.DepthStageList.Value, dchSpitterDepths);
                if (Configuration.Spitter.HelminthroostReplaceMushrum.Value)
                {
                    DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.MiniMushrum, DirectorAPI.Stage.HelminthHatchery);
                }
            }
        }

    }
}
