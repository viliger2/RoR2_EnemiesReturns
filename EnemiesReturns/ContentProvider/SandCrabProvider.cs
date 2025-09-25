using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.ArcherBug;
using EnemiesReturns.Enemies.SandCrab;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.ModdedEntityStates.SandCrab.Snip;
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
        private void CreateSandCrab(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (Configuration.SandCrab.Enabled.Value)
            {
                var sandCrabStuff = new SandCrabStuff();
                FireSnip.snipEffectPrefab = sandCrabStuff.CreateSnipEffect();
                effectsList.Add(new EffectDef(FireSnip.snipEffectPrefab));

                var bubbleImpactEffect = sandCrabStuff.CreateBubbleImpactEffect(assets.First(effect => effect.name == "SandCrabBubbleImpactEffect"));
                effectsList.Add(new EffectDef(bubbleImpactEffect));

                ModdedEntityStates.SandCrab.Bubbles.FireBubbles.projectilePrefab = sandCrabStuff.CreateBubbleProjectile(
                    assets.First(projectile => projectile.name == "SandCrabBubbleProjectile"),
                    sandCrabStuff.CreateBubbleGhost(assets.First(ghost => ghost.name == "SandCrabBubbleGhost"), acdLookup),
                    acdLookup["acdSandCrabBubbleSpeed"],
                    bubbleImpactEffect,
                    sandCrabStuff.CreateBubbleFlightLoop());

                projectilesList.Add(ModdedEntityStates.SandCrab.Bubbles.FireBubbles.projectilePrefab);
                bodyList.Add(ModdedEntityStates.SandCrab.Bubbles.FireBubbles.projectilePrefab);

                var sandCrabBody = new SandCrabBody();

                var sandCrabLog = Utils.CreateUnlockableDef("Logs.SandCrabBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SANDCRAB");
                unlockablesList.Add(sandCrabLog);

                SandCrabBody.Skills.ClawSnip = sandCrabBody.CreateClawSnipSkill();
                SandCrabBody.Skills.Bubbles = sandCrabBody.CreateBubblesSkill();

                sdList.Add(SandCrabBody.Skills.ClawSnip);
                sdList.Add(SandCrabBody.Skills.Bubbles);

                SandCrabBody.SkillFamilies.Primary = Utils.CreateSkillFamily("SandCrabPrimaryFamily", SandCrabBody.Skills.ClawSnip);
                SandCrabBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("SandCrabSecondaryFamily", SandCrabBody.Skills.Bubbles);

                sfList.Add(SandCrabBody.SkillFamilies.Primary);
                sfList.Add(SandCrabBody.SkillFamilies.Secondary);

                SandCrabBody.BodyPrefab = sandCrabBody.AddBodyComponents(assets.First(body => body.name == "SandCrabBody"), iconLookup["texSandCrabIcon"], sandCrabLog);
                bodyList.Add(SandCrabBody.BodyPrefab);

                SandCrabMaster.MasterPrefab = new SandCrabMaster().AddMasterComponents(assets.First(master => master.name == "SandCrabMaster"), SandCrabBody.BodyPrefab);
                masterList.Add(SandCrabMaster.MasterPrefab);

                SandCrabBody.SpawnCards.cscSandCrabDefault = sandCrabBody.CreateCard("cscSandCrabDefault", SandCrabMaster.MasterPrefab, SandCrabBody.SkinDefs.Default, SandCrabBody.BodyPrefab);
                var dcSandCrabDefault = new DirectorCard
                {
                    spawnCard = SandCrabBody.SpawnCards.cscSandCrabDefault,
                    selectionWeight = Configuration.SandCrab.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    minimumStageCompletions = Configuration.SandCrab.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSandCrabDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSandCrabDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
                };
                Utils.AddMonsterToStages(Configuration.SandCrab.DefaultStageList.Value, dchSandCrabDefault);

                SandCrabBody.SpawnCards.cscSandCrabGrassy = sandCrabBody.CreateCard("cscSandCrabGrassy", SandCrabMaster.MasterPrefab, SandCrabBody.SkinDefs.Grassy, SandCrabBody.BodyPrefab);
                var dcSandCrabGrassy = new DirectorCard
                {
                    spawnCard = SandCrabBody.SpawnCards.cscSandCrabGrassy,
                    selectionWeight = Configuration.SandCrab.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    minimumStageCompletions = Configuration.SandCrab.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSandCrabGrassy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSandCrabGrassy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
                };
                Utils.AddMonsterToStages(Configuration.SandCrab.GrassyStageList.Value, dchSandCrabGrassy);

                SandCrabBody.SpawnCards.cscSandCrabSandy = sandCrabBody.CreateCard("cscSandCrabSandy", SandCrabMaster.MasterPrefab, SandCrabBody.SkinDefs.Sandy, SandCrabBody.BodyPrefab);
                var dcSandCrabSandy = new DirectorCard
                {
                    spawnCard = SandCrabBody.SpawnCards.cscSandCrabSandy,
                    selectionWeight = Configuration.SandCrab.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    minimumStageCompletions = Configuration.SandCrab.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSandCrabSandy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSandCrabSandy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
                };
                Utils.AddMonsterToStages(Configuration.SandCrab.SandyStateList.Value, dchSandCrabSandy);

                SandCrabBody.SpawnCards.cscSandCrabSulfur = sandCrabBody.CreateCard("cscSandCrabSulfur", SandCrabMaster.MasterPrefab, SandCrabBody.SkinDefs.Sulfur, SandCrabBody.BodyPrefab);
                var dcSandCrabSulfur = new DirectorCard
                {
                    spawnCard = SandCrabBody.SpawnCards.cscSandCrabSulfur,
                    selectionWeight = Configuration.SandCrab.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    minimumStageCompletions = Configuration.SandCrab.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSandCrabSulfur = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSandCrabSulfur,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses
                };
                Utils.AddMonsterToStages(Configuration.SandCrab.SulfurStageList.Value, dchSandCrabSulfur);
            }
        }
    }
}
