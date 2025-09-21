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
                    bubbleImpactEffect);

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

                SandCrabBody.BodyPrefab = sandCrabBody.AddBodyComponents(assets.First(body => body.name == "SandCrabBody"), iconLookup["texSpitterIcon"], sandCrabLog);
                bodyList.Add(SandCrabBody.BodyPrefab);

                SandCrabMaster.MasterPrefab = new SandCrabMaster().AddMasterComponents(assets.First(master => master.name == "SandCrabMaster"), SandCrabBody.BodyPrefab);
                masterList.Add(SandCrabMaster.MasterPrefab);
            }
        }
    }
}
