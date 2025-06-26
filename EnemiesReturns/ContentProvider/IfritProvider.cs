using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Ifrit.Pillar;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
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
        private void CreateIfrit(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Texture2D texLavaCrackRound, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            CreateIfritPillar(assets, iconLookup, acdLookup);
            ExplicitPickupDropTable dtIfrit = CreateIfritItem(assets, iconLookup);

            if (Configuration.Ifrit.Enabled.Value)
            {
                var ifritStuff = new IfritStuff();

                ModdedEntityStates.Ifrit.DeathState.deathEffect = ifritStuff.CreateDeathEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.DeathState.deathEffect));

                ModdedEntityStates.Ifrit.SummonPylon.screamPrefab = ifritStuff.CreateBreathParticle();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.SummonPylon.screamPrefab));

                var nsedHellzoneRockFire = Utils.CreateNetworkSoundDef("ER_Ifrit_Hellzone_Rock_Play");
                nseList.Add(nsedHellzoneRockFire);

                var ifritSpawnEffect = assets.First(effect => effect.name == "IfritSpawnPortal");
                ModdedEntityStates.Ifrit.SpawnState.spawnEffect = ifritStuff.CreateSpawnEffect(ifritSpawnEffect, acdLookup["acdPortalPP"]);
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.SpawnState.spawnEffect));

                var ifritHellzonePillarProjectile = assets.First(projectile => projectile.name == "IfritHellzonePillarProjectile");
                var ifritHellzonePillarProjectileGhost = assets.First(projectile => projectile.name == "IfritHellzonePillarProjectileGhost");
                var IfritHellzoneVolcanoEffect = assets.First(projectile => projectile.name == "IfritHellzoneVolcanoEffect");
                var pillarProjectile = ifritStuff.CreateHellzonePillarProjectile(ifritHellzonePillarProjectile, ifritHellzonePillarProjectileGhost);
                var dotZoneProjectile = ifritStuff.CreateHellfireDotZoneProjectile(pillarProjectile, IfritHellzoneVolcanoEffect, texLavaCrackRound, nsedHellzoneRockFire);
                var hellzoneProjectile = ifritStuff.CreateHellzoneProjectile();
                var preProjectile = ifritStuff.CreateHellzonePredictionProjectile(dotZoneProjectile, texLavaCrackRound);

                projectilesList.Add(dotZoneProjectile);
                projectilesList.Add(hellzoneProjectile);
                projectilesList.Add(pillarProjectile);
                projectilesList.Add(preProjectile);

                ModdedEntityStates.Ifrit.FlameCharge.FlameCharge.flamethrowerEffectPrefab = ifritStuff.CreateFlameBreath();
                ModdedEntityStates.Ifrit.Hellzone.FireHellzoneFire.projectilePrefab = hellzoneProjectile;
                ModdedEntityStates.Ifrit.Hellzone.FireHellzoneFire.dotZoneProjectile = preProjectile;

                var ifritBody = new IfritBody();

                IfritBody.Skills.Hellzone = ifritBody.CreateHellzoneSkill();
                IfritBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("IfritSecondaryFamily", IfritBody.Skills.Hellzone);

                IfritBody.Skills.SummonPylon = ifritBody.CreateSummonPylonSkill();
                IfritBody.SkillFamilies.Special = Utils.CreateSkillFamily("IfritSpecialFamily", IfritBody.Skills.SummonPylon);

                IfritBody.Skills.FlameCharge = ifritBody.CreateFlameChargeSkill();
                IfritBody.SkillFamilies.Utility = Utils.CreateSkillFamily("IfritUtilityFamily", IfritBody.Skills.FlameCharge);

                IfritBody.Skills.Smash = ifritBody.CreateSmashSkill();
                IfritBody.SkillFamilies.Primary = Utils.CreateSkillFamily("IfritPrimaryFamily", IfritBody.Skills.Smash);

                var ifritLog = Utils.CreateUnlockableDef("Logs.IfritBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_IFRIT");
                unlockablesList.Add(ifritLog);

                IfritBody.BodyPrefab = ifritBody.AddBodyComponents(assets.First(body => body.name == "IfritBody"), iconLookup["texIconIfritBody"], ifritLog, dtIfrit);
                bodyList.Add(IfritBody.BodyPrefab);

                var ifritManePrefab = assets.First(mane => mane.name == "IfritManeFireParticle");
                ifritManePrefab.GetComponent<Renderer>().material = GetOrCreateMaterial("matIfritManeFire", ifritBody.CreateManeFiresMaterial);

                var ifritMaster = new IfritMaster();
                IfritMaster.MasterPrefab = ifritMaster.AddMasterComponents(assets.First(master => master.name == "IfritMaster"), IfritBody.BodyPrefab);
                masterList.Add(IfritMaster.MasterPrefab);

                IfritBody.SpawnCards.cscIfritDefault = ifritBody.CreateCard("cscIfritDefault", IfritMaster.MasterPrefab, IfritBody.SkinDefs.Default, IfritBody.BodyPrefab);
                var dcIfritDefault = new DirectorCard
                {
                    spawnCard = IfritBody.SpawnCards.cscIfritDefault,
                    selectionWeight = Configuration.Ifrit.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = Configuration.Ifrit.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchIfritDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcIfritDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStages(Configuration.Ifrit.DefaultStageList.Value, dchIfritDefault);

                if (Configuration.Ifrit.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
                {
                    ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(IfritBody.SpawnCards.cscIfritDefault, 2);
                }
            }
        }

        private ExplicitPickupDropTable CreateIfritItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtIfrit = null;
            if (Configuration.Ifrit.ItemEnabled.Value)
            {
                var pillarItemFactory = new SpawnPillarOnChampionKillFactory();

                Content.Items.SpawnPillarOnChampionKill = pillarItemFactory.CreateItem(assets.First(item => item.name == "IfritItem"), iconLookup["texIconIfritItem"]);
                itemList.Add(Content.Items.SpawnPillarOnChampionKill);

                dtIfrit = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtIfrit as ScriptableObject).name = "epdtIfrit";
                dtIfrit.canDropBeReplaced = true;
                dtIfrit.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = Content.Items.SpawnPillarOnChampionKill
                        }
                };
                HG.ArrayUtils.ArrayAppend(ref Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = Content.Items.SpawnPillarOnChampionKill, itemDef2 = VoidMegaCrabItem });
            }

            return dtIfrit;
        }

        private void CreateIfritPillar(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (Configuration.Ifrit.ItemEnabled.Value || Configuration.Ifrit.Enabled.Value)
            {
                var pillarStuff = new PillarStuff();

                ModdedEntityStates.Ifrit.Pillar.SpawnState.burrowPrefab = pillarStuff.CreateSpawnEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.SpawnState.burrowPrefab));

                ModdedEntityStates.Ifrit.Pillar.BaseDeathState.fallEffect = pillarStuff.CreateDeathFallEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.BaseDeathState.fallEffect));

                var explosionEffect = pillarStuff.CreateExlosionEffectAlt();
                ModdedEntityStates.Ifrit.Pillar.BaseFireExplosion.explosionPrefab = explosionEffect;
                ModdedEntityStates.Ifrit.Pillar.Enemy.KilledDeathState.explosionPrefab = explosionEffect;
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.BaseFireExplosion.explosionPrefab));

                var pylonBody = assets.First(body => body.name == "IfritPylonBody");
                var pylonMaster = assets.First(master => master.name == "IfritPylonMaster");

                ModdedEntityStates.Ifrit.Pillar.Enemy.KilledDeathState.fireballYCurve = acdLookup["acdFireballFallCurve"].curve;

                PillarStuff.PillarExplosion = ProcTypeAPI.ReserveProcType();

                var pillarMaster = new PillarMaster();

                if (Configuration.Ifrit.Enabled.Value)
                {
                    var pillarEnemyBody = new PillarEnemyBody();
                    PillarEnemyBody.BodyPrefab = pillarEnemyBody.AddBodyComponents(pylonBody.InstantiateClone("IfritPylonEnemyBody", false), iconLookup["texIconPillarEnemy"], acdLookup);
                    bodyList.Add(PillarEnemyBody.BodyPrefab);

                    PillarMaster.EnemyMasterPrefab = pillarMaster.AddMasterComponents(pylonMaster.InstantiateClone("IfritPylonEnemyMaster", false), PillarEnemyBody.BodyPrefab);
                    masterList.Add(PillarMaster.EnemyMasterPrefab);

                    PillarEnemyBody.SpawnCard = pillarEnemyBody.CreateCard("cscIfritEnemyPillar", PillarMaster.EnemyMasterPrefab);
                }

                if (Configuration.Ifrit.ItemEnabled.Value)
                {
                    var pillarAllyBody = new PillarAllyBody();

                    PillarAllyBody.BodyPrefab = pillarAllyBody.AddBodyComponents(pylonBody.InstantiateClone("IfritPylonPlayerBody", false), iconLookup["texIconPillarAlly"], acdLookup);
                    bodyList.Add(PillarAllyBody.BodyPrefab);

                    PillarMaster.AllyMasterPrefab = pillarMaster.AddMasterComponents(pylonMaster.InstantiateClone("IfritPylonPlayerMaster", false), PillarAllyBody.BodyPrefab);
                    masterList.Add(PillarMaster.AllyMasterPrefab);

                    PillarAllyBody.SpawnCard = pillarAllyBody.CreateCard("cscIfritPlayerPillar", PillarMaster.AllyMasterPrefab);
                }
            }
        }

    }
}
