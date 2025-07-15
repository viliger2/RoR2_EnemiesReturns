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

            var sandCrabLog = Utils.CreateUnlockableDef("Logs.SandCrabBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SANDCRAB");
            unlockablesList.Add(sandCrabLog);

            var sandCrabBody = new SandCrabBody();

            sfList.Add(SandCrabBody.SkillFamilies.Primary);

            SandCrabBody.BodyPrefab = sandCrabBody.AddBodyComponents(assets.First(body => body.name == " SandCrabBody"), iconLookup["texSpitterIcon"], sandCrabLog);
            bodyList.Add(SandCrabBody.BodyPrefab);

            SandCrabMaster.MasterPrefab = new SandCrabMaster().AddMasterComponents(assets.First(master => master.name == "SandCrabMaster"), SandCrabBody.BodyPrefab);
            masterList.Add(SandCrabMaster.MasterPrefab);
        }
    }
}
