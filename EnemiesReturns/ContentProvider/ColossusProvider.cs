using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Items.ColossalKnurl;
using R2API;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        private void CreateColossus(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            ExplicitPickupDropTable dtColossus = CreateColossusItem(assets, iconLookup);

            if (Configuration.Colossus.Enabled.Value)
            {
                var colossusStuff = new ColossusStuff();

                var stompEffect = colossusStuff.CreateStompEffect();
                ModdedEntityStates.Colossus.Stomp.StompBase.stompEffectPrefab = stompEffect;
                effectsList.Add(new EffectDef(stompEffect));

                var stompProjectile = colossusStuff.CreateStompProjectile();
                ModdedEntityStates.Colossus.Stomp.StompBase.projectilePrefab = stompProjectile;
                projectilesList.Add(stompProjectile);

                var deathFallEffect = colossusStuff.CreateDeathFallEffect();
                effectsList.Add(new EffectDef(deathFallEffect));
                ModdedEntityStates.Colossus.Death.BaseDeath.fallEffect = deathFallEffect;

                var death2Effect = colossusStuff.CreateDeath2Effect();
                effectsList.Add(new EffectDef(death2Effect));
                ModdedEntityStates.Colossus.Death.Death2.deathEffect = death2Effect;

                var clapEffect = colossusStuff.CreateClapEffect();
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.clapEffect = clapEffect;

                var flyingRockGhost = colossusStuff.CreateFlyingRocksGhost();
                FloatingRocksController.flyingRockPrefab = flyingRockGhost;
                var flyingRockProjectile = colossusStuff.CreateFlyingRockProjectile(flyingRockGhost);
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.projectilePrefab = flyingRockProjectile;
                projectilesList.Add(flyingRockProjectile);

                var laserBarrageProjectile = colossusStuff.CreateLaserBarrageProjectile();
                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.projectilePrefab = laserBarrageProjectile;
                projectilesList.Add(laserBarrageProjectile);

                var spawnEffect = colossusStuff.CreateSpawnEffect();
                ModdedEntityStates.Colossus.SpawnState.burrowPrefab = spawnEffect;
                effectsList.Add(new EffectDef(spawnEffect));

                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.intencityGraph = acdLookup["LaserBarrageLightIntencity"].curve;

                var colossusLog = Utils.CreateUnlockableDef("Logs.ColossusBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_COLOSSUS");
                unlockablesList.Add(colossusLog);

                var laserEffect = colossusStuff.CreateLaserEffect();
                Junk.ModdedEntityStates.Colossus.HeadLaser.HeadLaserAttack.beamPrefab = laserEffect;

                var colossusBody = new ColossusBody();
                ColossusBody.Skills.Stomp = colossusBody.CreateStompSkill();
                ColossusBody.Skills.StoneClap = colossusBody.CreateStoneClapSkill();
                ColossusBody.Skills.LaserBarrage = colossusBody.CreateLaserBarrageSkill();
                ColossusBody.Skills.HeadLaser = colossusBody.CreateHeadLaserSkill();
                sdList.Add(ColossusBody.Skills.Stomp);
                sdList.Add(ColossusBody.Skills.StoneClap);
                sdList.Add(ColossusBody.Skills.LaserBarrage);
                sdList.Add(ColossusBody.Skills.HeadLaser);

                ColossusBody.SkillFamilies.Primary = Utils.CreateSkillFamily("ColossusPrimaryFamily", ColossusBody.Skills.Stomp);
                ColossusBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("ColossusSecondaryFamily", ColossusBody.Skills.StoneClap);
                ColossusBody.SkillFamilies.Utility = Utils.CreateSkillFamily("ColossusUtilityFamily", ColossusBody.Skills.LaserBarrage);
                ColossusBody.SkillFamilies.Special = Utils.CreateSkillFamily("ColossusSpecialFamily", ColossusBody.Skills.HeadLaser);
                sfList.Add(ColossusBody.SkillFamilies.Primary);
                sfList.Add(ColossusBody.SkillFamilies.Secondary);
                sfList.Add(ColossusBody.SkillFamilies.Utility);
                sfList.Add(ColossusBody.SkillFamilies.Special);

                ColossusBody.BodyPrefab = colossusBody.AddBodyComponents(assets.First(body => body.name == "ColossusBody"), iconLookup["texColossusIcon"], colossusLog, dtColossus);
                bodyList.Add(ColossusBody.BodyPrefab);

                var colossusMaster = new ColossusMaster();
                ColossusMaster.MasterPrefab = colossusMaster.AddMasterComponents(assets.First(master => master.name == "ColossusMaster"), ColossusBody.BodyPrefab);
                masterList.Add(ColossusMaster.MasterPrefab);

                ColossusBody.SpawnCards.cscColossusDefault = colossusBody.CreateCard("cscColossusDefault", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Default, ColossusBody.BodyPrefab);
                DirectorCard dcColossusDefault = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusDefault,
                    selectionWeight = Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusDefault, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion());
                Utils.AddMonsterToStages(Configuration.Colossus.DefaultStageList.Value, dchColossusDefault);

                ColossusBody.SpawnCards.cscColossusSkyMeadow = colossusBody.CreateCard("cscColossusSkyMeadow", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.SkyMeadow, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSkyMeadow = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSkyMeadow,
                    selectionWeight = Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSkyMeadow = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSkyMeadow,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStages(Configuration.Colossus.SkyMeadowStageList.Value, dchColossusSkyMeadow);

                ColossusBody.SpawnCards.cscColossusGrassy = colossusBody.CreateCard("cscColossusGrassy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Grassy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusGrassy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusGrassy,
                    selectionWeight = Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusGrassy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusGrassy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusGrassy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common.dccsGolemFamilyNature_asset).WaitForCompletion());
                Utils.AddMonsterToStages(Configuration.Colossus.GrassyStageList.Value, dchColossusGrassy);

                ColossusBody.SpawnCards.cscColossusCastle = colossusBody.CreateCard("cscColossusCastle", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Castle, ColossusBody.BodyPrefab);
                DirectorCard dcColossusCastle = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusCastle,
                    selectionWeight = Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusCastle = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusCastle,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStages(Configuration.Colossus.CastleStageList.Value, dchColossusCastle);

                ColossusBody.SpawnCards.cscColossusSandy = colossusBody.CreateCard("cscColossusSandy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Sandy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSandy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSandy,
                    selectionWeight = Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSandy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSandy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusSandy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySandy.asset").WaitForCompletion());
                Utils.AddMonsterToStages(Configuration.Colossus.SandyStageList.Value, dchColossusSandy);

                ColossusBody.SpawnCards.cscColossusSnowy = colossusBody.CreateCard("cscColossusSnowy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Snowy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSnowy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSnowy,
                    selectionWeight = Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSnowy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSnowy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusSnowy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySnowy.asset").WaitForCompletion());
                Utils.AddMonsterToStages(Configuration.Colossus.SnowyStageList.Value, dchColossusSnowy);

                if (Configuration.Colossus.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
                {
                    ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(ColossusBody.SpawnCards.cscColossusGrassy, 3);
                }
            }
        }

        private ExplicitPickupDropTable CreateColossusItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtColossus = null;

            if (Configuration.Colossus.ItemEnabled.Value)
            {
                ColossalKnurlFactory.ColossalFist = ProcTypeAPI.ReserveProcType();

                var knurlFactory = new ColossalKnurlFactory();

                Content.Items.ColossalCurl = knurlFactory.CreateItem(assets.First(item => item.name == "PickupColossalCurl"), iconLookup["texColossalKnurlIcon"]);
                itemList.Add(Content.Items.ColossalCurl);

                dtColossus = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtColossus as ScriptableObject).name = "epdtColossus";
                dtColossus.canDropBeReplaced = true;
                dtColossus.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = Content.Items.ColossalCurl
                        }
                };

                var knurlProjectileGhost = assets.First(item => item.name == "ColossalKnurlFistProjectileGhost");
                knurlProjectileGhost = knurlFactory.CreateFistGhostPrefab(knurlProjectileGhost);

                var knurlProjectile = assets.First(item => item.name == "ColossalKnurlFistProjectile");
                ColossalKnurlFactory.projectilePrefab = knurlFactory.CreateFistProjectile(knurlProjectile, knurlProjectileGhost);

                projectilesList.Add(ColossalKnurlFactory.projectilePrefab);

                HG.ArrayUtils.ArrayAppend(ref Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = Content.Items.ColossalCurl, itemDef2 = VoidMegaCrabItem });
            }

            return dtColossus;
        }

    }
}
