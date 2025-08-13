using EnemiesReturns.Enemies.ArcherBug;
using EnemiesReturns.Enemies.SandCrab;
using EnemiesReturns.Enemies.Spitter;
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
        private void CreateSandCrab(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            var SandCrabStuff = new SandCrabStuff();
            ModdedEntityStates.SandCrab.FireSnip.snipEffectPrefab = SandCrabStuff.CreateSnipEffect();

            var sandCrabBody = new SandCrabBody();

            var sandCrabLog = Utils.CreateUnlockableDef("Logs.SandCrabBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SANDCRAB");
            unlockablesList.Add(sandCrabLog);          

            
            SandCrabBody.Skills.ClawSnip = sandCrabBody.CreateClawSnipSkill();

            sdList.Add(SandCrabBody.Skills.ClawSnip);

            SandCrabBody.SkillFamilies.Primary = Utils.CreateSkillFamily("SandCrabPrimaryFamily", SandCrabBody.Skills.ClawSnip);

            sfList.Add(SandCrabBody.SkillFamilies.Primary);

            SandCrabBody.BodyPrefab = sandCrabBody.AddBodyComponents(assets.First(body => body.name == "SandCrabBody"), iconLookup["texSpitterIcon"], sandCrabLog);
            bodyList.Add(SandCrabBody.BodyPrefab);

            SandCrabMaster.MasterPrefab = new SandCrabMaster().AddMasterComponents(assets.First(master => master.name == "SandCrabMaster"), SandCrabBody.BodyPrefab);
            masterList.Add(SandCrabMaster.MasterPrefab);
        }
    }
}
