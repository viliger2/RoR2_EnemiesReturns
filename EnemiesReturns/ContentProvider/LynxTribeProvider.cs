using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.LynxTribe;
using EnemiesReturns.Enemies.LynxTribe.Archer;
using EnemiesReturns.Enemies.LynxTribe.Hunter;
using EnemiesReturns.Enemies.LynxTribe.Scout;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using EnemiesReturns.Enemies.LynxTribe.Totem;
using EnemiesReturns.Items.LynxFetish;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        private void CreateLynxTribe(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, Dictionary<string, Texture2D> rampLookups)
        {
            if (Configuration.LynxTribe.LynxTotem.Enabled.Value
                || Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                CreateLynxStorm(assets, acdLookup);
            }
            if (Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                CreateLynxShaman(assets, iconLookup, acdLookup, rampLookups);
            }
            if (Configuration.LynxTribe.LynxTotem.Enabled.Value)
            {
                var dtLynxTotem = CreateLynxTotemItem(assets, iconLookup);
                CreateLynxScout(assets, iconLookup);
                CreateLynxHunter(assets, iconLookup);
                CreateLynxArcher(assets, iconLookup, rampLookups);
                CreateLynxTotem(assets, iconLookup, acdLookup, dtLynxTotem);
                Utils.AddMonsterFamilyToStages(Configuration.LynxTribe.LynxTotem.DefaultStageList.Value, new LynxTribeStuff().CreateLynxTribeFamily());
                if (Configuration.LynxTribe.LynxStuff.LynxShrineEnabled.Value)
                {
                    CreateLynxShrine(assets);
                }
                if (Configuration.LynxTribe.LynxStuff.LynxTrapEnabled.Value)
                {
                    CreateLynxTrap(assets);
                }
            }
        }

        public void CreateLynxTrap(GameObject[] assets)
        {
            var lynxStuff = new LynxTribeStuff();

            var nseLynxTribeTrapTwigSnap = Utils.CreateNetworkSoundDef("ER_LynxTrap_SnapTwig_Play");
            nseList.Add(nseLynxTribeTrapTwigSnap);

            var leavesPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/lemuriantemple/Assets/LTFallenLeaf.spm").WaitForCompletion();
            var material = leavesPrefab.transform.Find("LTFallenLeaf_LOD0").GetComponent<MeshRenderer>().material;
            var lynxTrapPrefab = assets.First(prefab => prefab.name == "LynxTrapPrefab");

            var defaultStages = Configuration.LynxTribe.LynxTotem.DefaultStageList.Value.Split(",");
            foreach (var stageString in defaultStages)
            {
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                GameObject lynxTrap = lynxTrapPrefab.InstantiateClone(lynxTrapPrefab.name + cleanStageString, false);
                switch (cleanStageString)
                {
                    case "blackbeach":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, GetOrCreateMaterial("SkyMeadowLeaves_LOD0", lynxStuff.CreateBlackBeachLeavesMaterialLOD0, material));
                        break;
                    case "skymeadow":
                    case "itskymeadow":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, GetOrCreateMaterial("SkyMeadowLeaves_LOD0", lynxStuff.CreateSkyMeadowLeavesMaterialLOD0, material));
                        break;
                    case "village":
                    case "villagenight":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, GetOrCreateMaterial("ShatteredAbodesLeaves_LOD0", lynxStuff.CreateShatteredAbodesLeavesMaterialLOD0, material));
                        break;
                    case "golemplains":
                    case "itgolemplains":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, GetOrCreateMaterial("TitanicPlainsLeaves_LOD0", lynxStuff.CreateTitanicPlanesLeavesMaterialLOD0, material));
                        break;
                    case "foggyswamp":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, GetOrCreateMaterial("WetlandsLeaves_LOD0", lynxStuff.CreateWetlandsLeavesMaterialLOD0, material));
                        break;
                    case "habitatfall":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, GetOrCreateMaterial("GoldenDiebackLeaves_LOD0", lynxStuff.CreateGoldemDiebackLeavesMaterialLOD0, material));
                        break;
                    default:
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, material);
                        break;
                }
                InteractableSpawnCard spawnCardTrap = lynxStuff.CreateLynxTrapSpawnCard(lynxTrap, cleanStageString);

                var dcLynxTrap = new DirectorCard
                {
                    spawnCard = spawnCardTrap,
                    selectionWeight = Configuration.LynxTribe.LynxStuff.LynxTrapSelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = false
                };

                var holderTrap = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcLynxTrap,
                    InteractableCategory = Configuration.LynxTribe.LynxStuff.LynxTrapSpawnCategory.Value
                };

                DirectorAPI.Helpers.AddNewInteractableToStage(holderTrap, DirectorAPI.ParseInternalStageName(cleanStageString), cleanStageString);

                nopList.Add(lynxTrap);
            }
        }

        public void CreateLynxShrine(GameObject[] assets)
        {
            var lynxStuff = new LynxTribeStuff();

            ModdedEntityStates.LynxTribe.Retreat.retreatEffectPrefab = lynxStuff.CreateRetreatEffect(assets.First(prefab => prefab.name == "LynxTribeRetreatEffect"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Retreat.retreatEffectPrefab));

            LynxTribeStuff.CustomHologramContent = lynxStuff.CustomCostHologramContentPrefab();

            var shrineEffect = lynxStuff.CreateShrineUseEffect();
            effectsList.Add(new EffectDef(shrineEffect));

            var nsedLynxShrineFailure = Utils.CreateNetworkSoundDef("ER_Lynx_Shrine_Failure_Play");
            nseList.Add(nsedLynxShrineFailure);

            var nsedLynxShrineSuccess = Utils.CreateNetworkSoundDef("ER_Lynx_Shrine_Success_Play");
            nseList.Add(nsedLynxShrineSuccess);

            LynxTribeStuff.LynxShrine1 = lynxStuff.CreateShrinePrefab(assets.First(prefab => prefab.name == "LynxShrinePrefab"), shrineEffect, nsedLynxShrineFailure, nsedLynxShrineSuccess);
            LynxTribeStuff.LynxShrine2 = lynxStuff.CreateShrinePrefab(assets.First(prefab => prefab.name == "LynxShrinePrefab2"), shrineEffect, nsedLynxShrineFailure, nsedLynxShrineSuccess);
            LynxTribeStuff.LynxShrine3 = lynxStuff.CreateShrinePrefab(assets.First(prefab => prefab.name == "LynxShrinePrefab3"), shrineEffect, nsedLynxShrineFailure, nsedLynxShrineSuccess);

            DirectorAPI.DirectorCardHolder holderShrine1 = CreateCardHolderLynxShrine(LynxTribeStuff.LynxShrine1, "1");
            DirectorAPI.DirectorCardHolder holderShrine2 = CreateCardHolderLynxShrine(LynxTribeStuff.LynxShrine2, "2");
            DirectorAPI.DirectorCardHolder holderShrine3 = CreateCardHolderLynxShrine(LynxTribeStuff.LynxShrine3, "3");

            var defaultStages = Configuration.LynxTribe.LynxTotem.DefaultStageList.Value.Split(",");
            foreach (var stageString in defaultStages)
            {
                DirectorAPI.DirectorCardHolder shrineToSpawn;
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                switch (cleanStageString)
                {
                    case "blackbeach":
                    case "skymeadow":
                    case "itskymeadow":
                        shrineToSpawn = holderShrine1;
                        break;
                    case "village":
                    case "villagenight":
                    case "golemplains":
                    case "itgolemplains":
                        shrineToSpawn = holderShrine2;
                        break;
                    case "foggyswamp":
                    case "habitatfall":
                    default:
                        shrineToSpawn = holderShrine3;
                        break;
                }

                DirectorAPI.Helpers.AddNewInteractableToStage(shrineToSpawn, DirectorAPI.ParseInternalStageName(cleanStageString), cleanStageString);
            }

            nopList.Add(LynxTribeStuff.LynxShrine1);
            nopList.Add(LynxTribeStuff.LynxShrine2);
            nopList.Add(LynxTribeStuff.LynxShrine3);
        }

        private DirectorAPI.DirectorCardHolder CreateCardHolderLynxShrine(GameObject lynxShrine1, string suffix)
        {
            var spawnCardShrine1 = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            (spawnCardShrine1 as ScriptableObject).name = "iscLynxShrine" + suffix;
            spawnCardShrine1.prefab = lynxShrine1;
            spawnCardShrine1.sendOverNetwork = true;
            spawnCardShrine1.hullSize = HullClassification.Golem;
            spawnCardShrine1.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCardShrine1.requiredFlags = RoR2.Navigation.NodeFlags.None;
            spawnCardShrine1.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn | RoR2.Navigation.NodeFlags.NoShrineSpawn;
            spawnCardShrine1.directorCreditCost = Configuration.LynxTribe.LynxStuff.LynxShrineDirectorCost.Value;
            spawnCardShrine1.occupyPosition = true;
            spawnCardShrine1.eliteRules = SpawnCard.EliteRules.Default;
            spawnCardShrine1.orientToFloor = false;
            spawnCardShrine1.maxSpawnsPerStage = Configuration.LynxTribe.LynxStuff.LynxShrineMaxSpawnPerStage.Value;

            var dcLynxShrine1 = new DirectorCard
            {
                spawnCard = spawnCardShrine1,
                selectionWeight = Configuration.LynxTribe.LynxStuff.LynxShrineSelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false
            };

            var holderShrine1 = new DirectorAPI.DirectorCardHolder();
            holderShrine1.Card = dcLynxShrine1;
            holderShrine1.InteractableCategory = Configuration.LynxTribe.LynxStuff.LynxShrineSpawnCategory.Value;
            return holderShrine1;
        }

        public void CreateLynxArcher(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, Texture2D> rampLookups)
        {
            var archerStuff = new ArcherStuff();
            var material = GetOrCreateMaterial("matLynxArcherArrow", archerStuff.CreateArcherArrowMaterial);

            var nsdArrowHit = Utils.CreateNetworkSoundDef("ER_Archer_ArrowImpact_Play");
            nseList.Add(nsdArrowHit);

            ModdedEntityStates.LynxTribe.Archer.FireArrow.projectilePrefab = archerStuff.CreateArrowProjectile(
                assets.First(prefab => prefab.name == "ArrowProjectile"),
                archerStuff.CreateArrowProjectileGhost(assets.First(prefab => prefab.name == "ArrowProjectileGhost"), material),
                archerStuff.CreateArrowImpalePrefab(assets.First(prefab => prefab.name == "ArrowProjectileGhost"), material),
                archerStuff.CreateArrowLoopSoundDef(),
                nsdArrowHit);
            projectilesList.Add(ModdedEntityStates.LynxTribe.Archer.FireArrow.projectilePrefab);

            var archerBody = new ArcherBody();
            ArcherBody.Skills.Shot = archerBody.CreateShotSkill();
            sdList.Add(ArcherBody.Skills.Shot);

            ArcherBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxArcherPrimarySkillFamily", ArcherBody.Skills.Shot);
            sfList.Add(ArcherBody.SkillFamilies.Primary);

            ArcherBody.BodyPrefab = archerBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxArcherBody"), sprite: iconLookup["texLynxArcherIcon"]);
            bodyList.Add(ArcherBody.BodyPrefab);

            var archerMaster = new ArcherMaster();
            ArcherMaster.MasterPrefab = archerMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxArcherMaster"), ArcherBody.BodyPrefab);
            masterList.Add(ArcherMaster.MasterPrefab);

            ArcherBody.SpawnCards.cscLynxArcherDefault = archerBody.CreateCard("cscLynxArcherDefault", ArcherMaster.MasterPrefab, ArcherBody.SkinDefs.Default, ArcherBody.BodyPrefab);

            var dhLynxArcherDefault = new DirectorCard
            {
                spawnCard = ArcherBody.SpawnCards.cscLynxArcherDefault,
                selectionWeight = Configuration.LynxTribe.LynxArcher.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.LynxTribe.LynxArcher.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxArcherDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxArcherDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(Configuration.LynxTribe.LynxArcher.DefaultStageList.Value, dchLynxArcherDefault);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, ArcherBody.SpawnCards.cscLynxArcherDefault);

            var archerBodyAlly = new ArcherBodyAlly();
            Content.Buffs.LynxArcherDamage = archerBodyAlly.CreateDamageBuffDef(iconLookup["texLynxArcherBuff"]);
            bdList.Add(Content.Buffs.LynxArcherDamage);

            ArcherBodyAlly.BodyPrefab = archerBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxArcherAllyBody"), sprite: iconLookup["texArcherAllyIcon"]);
            bodyList.Add(ArcherBodyAlly.BodyPrefab);

            var archerMasterAlly = new ArcherMasterAlly();
            ArcherMasterAlly.MasterPrefab = archerMasterAlly.AddMasterComponents(assets.First(prefab => prefab.name == "LynxArcherAllyMaster"), ArcherBodyAlly.BodyPrefab);
            masterList.Add(ArcherMasterAlly.MasterPrefab);

            ArcherBodyAlly.SpawnCards.cscLynxArcherAlly = archerBodyAlly.CreateCard("cscLynxArcherAlly", ArcherMasterAlly.MasterPrefab, ArcherBodyAlly.SkinDefs.Ally, ArcherBodyAlly.BodyPrefab);
        }

        public void CreateLynxHunter(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            var hunterStuff = new HunterStuff();
            var wooshEffect = hunterStuff.CreateHunterAttackEffect(assets.First(prefab => prefab.name == "LynxHunterAttackEffect"));
            Junk.ModdedEntityStates.LynxTribe.Hunter.Stab.wooshEffect = wooshEffect;
            ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.wooshEffect = wooshEffect;
            effectsList.Add(new EffectDef(wooshEffect));

            ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.slideEffectPrefab = hunterStuff.CreateLungeSlideEffect();
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.slideEffectPrefab));

            var coneEffect = hunterStuff.CreateHunterAttackSpearTipEffect(assets.First(prefab => prefab.name == "LynxHunterSpearTipEffect"));
            Junk.ModdedEntityStates.LynxTribe.Hunter.Stab.coneEffect = coneEffect;
            ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge.coneEffect = coneEffect;
            effectsList.Add(new EffectDef(coneEffect));

            var hunterBody = new HunterBody();
            HunterBody.Skills.Stab = hunterBody.CreateStabSkill();
            sdList.Add(HunterBody.Skills.Stab);

            HunterBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxHunterPrimarySkillFamily", HunterBody.Skills.Stab);
            sfList.Add(HunterBody.SkillFamilies.Primary);

            HunterBody.BodyPrefab = hunterBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxHunterBody"), sprite: iconLookup["texLynxHunterIcon"]);
            bodyList.Add(HunterBody.BodyPrefab);

            var hunterMaster = new HunterMaster();
            HunterMaster.MasterPrefab = hunterMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxHunterMaster"), HunterBody.BodyPrefab);
            masterList.Add(HunterMaster.MasterPrefab);

            HunterBody.SpawnCards.cscLynxHunterDefault = hunterBody.CreateCard("cscLynxHunterDefault", HunterMaster.MasterPrefab, HunterBody.SkinDefs.Default, HunterBody.BodyPrefab);

            var dhLynxHunterDefault = new DirectorCard
            {
                spawnCard = HunterBody.SpawnCards.cscLynxHunterDefault,
                selectionWeight = Configuration.LynxTribe.LynxHunter.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.LynxTribe.LynxHunter.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxHunterDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxHunterDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(Configuration.LynxTribe.LynxHunter.DefaultStageList.Value, dchLynxHunterDefault);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, HunterBody.SpawnCards.cscLynxHunterDefault);

            var hunterBodyAlly = new HunterBodyAlly();
            Content.Buffs.LynxHunterArmor = hunterBodyAlly.CreateArmorBuffDef(iconLookup["texLynxHunterBuff"]);
            bdList.Add(Content.Buffs.LynxHunterArmor);

            HunterBodyAlly.BodyPrefab = hunterBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxHunterAllyBody"), sprite: iconLookup["texLynxHunterAllyIcon"]);
            bodyList.Add(HunterBodyAlly.BodyPrefab);

            var hunterMasterAlly = new HunterMasterAlly();
            HunterMasterAlly.MasterPrefab = hunterMasterAlly.AddMasterComponents(assets.First(prefab => prefab.name == "LynxHunterAllyMaster"), HunterBodyAlly.BodyPrefab);
            masterList.Add(HunterMasterAlly.MasterPrefab);

            HunterBodyAlly.SpawnCards.cscLynxHunterAlly = hunterBodyAlly.CreateCard("cscLynxHunterAlly", HunterMasterAlly.MasterPrefab, HunterBodyAlly.SkinDefs.Ally, HunterBodyAlly.BodyPrefab);
        }

        public void CreateLynxScout(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            var scoutStuff = new ScoutStuff();

            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectLeft = scoutStuff.CreateDoubleSlashClawEffect(assets.First(prefab => prefab.name == "LynxScoutClawEffectLeft"));
            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectRight = scoutStuff.CreateDoubleSlashClawEffect(assets.First(prefab => prefab.name == "LynxScoutClawEffectRight"));
            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectLeft = scoutStuff.CreateDoubleSlashLeftHandSwingTrail();
            ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectRight = scoutStuff.CreateDoubleSlashRightHandSwingTrail();

            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectLeft));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.clawEffectRight));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectLeft));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Scout.DoubleSlash.slashEffectRight));

            var scoutBody = new ScoutBody();
            ScoutBody.Skills.DoubleSlash = scoutBody.CreateDoubleSlashSkill();

            sdList.Add(ScoutBody.Skills.DoubleSlash);

            ScoutBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxScoutPrimarySkillFamily", ScoutBody.Skills.DoubleSlash);

            sfList.Add(ScoutBody.SkillFamilies.Primary);

            ScoutBody.BodyPrefab = scoutBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxScoutBody"), sprite: iconLookup["texLynxScoutIcon"]);
            bodyList.Add(ScoutBody.BodyPrefab);

            var scoutMaster = new ScoutMaster();
            ScoutMaster.MasterPrefab = scoutMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxScoutMaster"), ScoutBody.BodyPrefab);
            masterList.Add(ScoutMaster.MasterPrefab);

            ScoutBody.SpawnCards.cscLynxScoutDefault = scoutBody.CreateCard("cscLynxScoutDefault", ScoutMaster.MasterPrefab, ScoutBody.SkinDefs.Default, ScoutBody.BodyPrefab);

            var dhLynxScoutDefault = new DirectorCard
            {
                spawnCard = ScoutBody.SpawnCards.cscLynxScoutDefault,
                selectionWeight = Configuration.LynxTribe.LynxScout.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.LynxTribe.LynxScout.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxScoutDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxScoutDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(Configuration.LynxTribe.LynxScout.DefaultStageList.Value, dchLynxScoutDefault);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, ScoutBody.SpawnCards.cscLynxScoutDefault);

            var scoutBodyAlly = new ScoutBodyAlly();
            Content.Buffs.LynxScoutSpeed = scoutBodyAlly.CreateSpeedBuffDef(iconLookup["texLynxScoutBuff"]);
            bdList.Add(Content.Buffs.LynxScoutSpeed);

            ScoutBodyAlly.BodyPrefab = scoutBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxScoutAllyBody"), sprite: iconLookup["texLynxScoutAllyIcon"]);
            bodyList.Add(ScoutBodyAlly.BodyPrefab);

            var scoutMasterAlly = new ScoutMasterAlly();
            ScoutMasterAlly.MasterPrefab = scoutMasterAlly.AddMasterComponents(assets.First(prefab => prefab.name == "LynxScoutAllyMaster"), ScoutBodyAlly.BodyPrefab);
            masterList.Add(ScoutMasterAlly.MasterPrefab);

            ScoutBodyAlly.SpawnCards.cscLynxScoutAlly = scoutBodyAlly.CreateCard("cscLynxScoutAlly", ScoutMasterAlly.MasterPrefab, ScoutBodyAlly.SkinDefs.Ally, ScoutBodyAlly.BodyPrefab);
        }

        private void CreateLynxTotem(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, ExplicitPickupDropTable dtLynxTotem)
        {
            var totemStuff = new TotemStuff();

            totemStuff.RegisterDeployableSlot();

            var shamanTotemSpawnEffect = totemStuff.CreateShamanTotemSpawnEffect(assets.First(prefab => prefab.name == "LynxSpawnParticles"));
            ModdedEntityStates.LynxTribe.Totem.SummonTribe.summonEffect = shamanTotemSpawnEffect;
            ModdedEntityStates.LynxTribe.Totem.SpawnState.leavesSpawnEffect = shamanTotemSpawnEffect;
            effectsList.Add(new EffectDef(shamanTotemSpawnEffect));

            var spawnEffect = totemStuff.CreateTribesmenSpawnEffect(assets.First(prefab => prefab.name == "LynxSpawnParticles"));
            ModdedEntityStates.LynxTribe.Scout.SpawnState.spawnEffect = spawnEffect;
            ModdedEntityStates.LynxTribe.Hunter.SpawnState.spawnEffect = spawnEffect;
            ModdedEntityStates.LynxTribe.Archer.SpawnState.spawnEffect = spawnEffect;
            effectsList.Add(new EffectDef(spawnEffect));

            Junk.ModdedEntityStates.LynxTribe.Totem.SummonFirewall.projectilePrefab = totemStuff.CreateFirewallProjectile(assets.First(prefab => prefab.name == "LynxTotemFireWallProjectile"), acdLookup["acdLynxTotemFirewall"]);
            projectilesList.Add(Junk.ModdedEntityStates.LynxTribe.Totem.SummonFirewall.projectilePrefab);

            var shakeEffect = totemStuff.CreateGroundpoundShakeEffect();
            Junk.ModdedEntityStates.LynxTribe.Totem.Groundpound.shakeEffect = shakeEffect;
            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.shakeEffect = shakeEffect;
            ModdedEntityStates.LynxTribe.Totem.SpawnStateFromShaman.shakeEffect = shakeEffect;
            effectsList.Add(new EffectDef(shakeEffect));

            var poundEffect = totemStuff.CreateGroundpoundPoundEffect();
            Junk.ModdedEntityStates.LynxTribe.Totem.Groundpound.poundEffect = poundEffect;
            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.poundEffect = poundEffect;
            ModdedEntityStates.LynxTribe.Totem.SpawnState.poundEffect = poundEffect;
            effectsList.Add(new EffectDef(poundEffect));

            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.stoneParticlesEffect = totemStuff.CreateStoneParticlesEffect(assets.First(prefab => prefab.name == "TotemShakeParticles"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.stoneParticlesEffect));

            var singleStoneEffect = totemStuff.CreateStoneParticlesEffect(assets.First(prefab => prefab.name == "TotemSingleStoneParticle"));
            ModdedEntityStates.LynxTribe.Totem.SummonStorm.stoneEffectPrefab = singleStoneEffect;
            ModdedEntityStates.LynxTribe.Totem.SummonTribe.stoneEffectPrefab = singleStoneEffect;
            effectsList.Add(new EffectDef(singleStoneEffect));

            ModdedEntityStates.LynxTribe.Totem.SummonTribe.eyeEffect = totemStuff.CreateEyeGlowEffect(assets.First(prefab => prefab.name == "TotemEyeGlowSummonTribe"), acdLookup["acdLynxTotemEyeGlowSummonTribe"], 1f);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.SummonTribe.eyeEffect));

            ModdedEntityStates.LynxTribe.Totem.SummonStorm.eyeEffect = totemStuff.CreateEyeGlowEffect(assets.First(prefab => prefab.name == "TotemEyeGlowSummonStorm"), acdLookup["acdLynxTotemEyeGlowSummonStorm"], 3.3f);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.SummonStorm.eyeEffect));

            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.eyeEffect = totemStuff.CreateEyeGlowEffect(assets.First(prefab => prefab.name == "TotemEyeGlowGroundpound"), acdLookup["acdLynxTotemEyeGlowGroundpound"], 1.8f);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.eyeEffect));

            ModdedEntityStates.LynxTribe.Totem.SummonStorm.staffEffect = totemStuff.CreateSummonStormsStaffParticle(acdLookup["acdLynxTotemSummonStormStaffEffectScale"]);
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Totem.SummonStorm.staffEffect));

            ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.groundpoundProjectilePrefab = totemStuff.CreateGroundpoundProjectile(assets.First(prefab => prefab.name == "LynxTotemGroundpoundProjectile"));
            projectilesList.Add(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile.groundpoundProjectilePrefab);

            LynxStormOrb.orbEffect = totemStuff.CreateStormSummonOrb(assets.First(prefab => prefab.name == "TotemSummonStormOrb"));
            effectsList.Add(new EffectDef(LynxStormOrb.orbEffect));

            var totemLog = Utils.CreateUnlockableDef("Logs.LynxTotemBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_LYNX_TOTEM");
            unlockablesList.Add(totemLog);

            var totemBody = new TotemBody();
            TotemBody.Skills.Burrow = totemBody.CreateBurrowSkill();
            TotemBody.Skills.SummonStorms = totemBody.CreateSummonStormsSkill();
            TotemBody.Skills.SummonTribe = totemBody.CreateSummonTribeSkill();
            TotemBody.Skills.SummonFirewall = totemBody.CreateSummonFirewallSkill();
            TotemBody.Skills.Groundpound = totemBody.CreateGroundpoundSkill();
            sdList.Add(TotemBody.Skills.Burrow);
            sdList.Add(TotemBody.Skills.SummonStorms);
            sdList.Add(TotemBody.Skills.SummonTribe);
            sdList.Add(TotemBody.Skills.SummonFirewall);
            sdList.Add(TotemBody.Skills.Groundpound);

            TotemBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxTotemPrimarySkillFamily", TotemBody.Skills.Groundpound);
            TotemBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("LynxTotemSecondarySkillFamily", TotemBody.Skills.SummonTribe);
            TotemBody.SkillFamilies.Utility = Utils.CreateSkillFamily("LynxTotemUtilitySkillFamily", TotemBody.Skills.Burrow);
            TotemBody.SkillFamilies.Special = Utils.CreateSkillFamily("LynxTotemSpecialSkillFamily", TotemBody.Skills.SummonStorms);
            sfList.Add(TotemBody.SkillFamilies.Primary);
            sfList.Add(TotemBody.SkillFamilies.Secondary);
            sfList.Add(TotemBody.SkillFamilies.Utility);
            sfList.Add(TotemBody.SkillFamilies.Special);

            ModdedEntityStates.LynxTribe.Totem.SummonStorm.cscStorm = LynxStormBody.cscLynxStorm;

            TotemBody.BodyPrefab = totemBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxTotemBody"), sprite: iconLookup["texLynxTotemIcon"], totemLog, dtLynxTotem);
            bodyList.Add(TotemBody.BodyPrefab);

            var totemMaster = new TotemMaster();
            TotemMaster.MasterPrefab = totemMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxTotemMaster"), TotemBody.BodyPrefab);
            masterList.Add(TotemMaster.MasterPrefab);

            TotemBody.SpawnCards.cscLynxTotemDefault = totemBody.CreateCard("cscLynxTotemDefault", TotemMaster.MasterPrefab, TotemBody.SkinDefs.Default, TotemBody.BodyPrefab);

            var dhLynxTotemDefault = new DirectorCard
            {
                spawnCard = TotemBody.SpawnCards.cscLynxTotemDefault,
                selectionWeight = Configuration.LynxTribe.LynxTotem.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.LynxTribe.LynxTotem.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxTotemDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxTotemDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
            };
            Utils.AddMonsterToStages(Configuration.LynxTribe.LynxTotem.DefaultStageList.Value, dchLynxTotemDefault);

            if (Configuration.LynxTribe.LynxTotem.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
            {
                ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(TotemBody.SpawnCards.cscLynxTotemDefault, 1);
            }
        }

        private ExplicitPickupDropTable CreateLynxTotemItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtLynxTotem = null;

            if (Configuration.LynxTribe.LynxTotem.ItemEnabled.Value)
            {
                var fetishFactory = new LynxFetishFactory();

                Content.Items.LynxFetish = fetishFactory.CreateItem(assets.First(item => item.name == "PickupLynxFetish"), iconLookup["texLynxFetishIcon"]);
                itemList.Add(Content.Items.LynxFetish);

                dtLynxTotem = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtLynxTotem as ScriptableObject).name = "epdtLynxTotem";
                dtLynxTotem.canDropBeReplaced = true;
                dtLynxTotem.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                    new ExplicitPickupDropTable.PickupDefEntry
                    {
                        pickupWeight = 1,
                        pickupDef = Content.Items.LynxFetish
                    }
                };

                HG.ArrayUtils.ArrayAppend(ref Content.ItemRelationshipProviders.ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = Content.Items.LynxFetish, itemDef2 = VoidMegaCrabItem });

                var elitePoisonRecipe = ScriptableObject.CreateInstance<CraftableDef>();
                (elitePoisonRecipe as ScriptableObject).name = "cdEnemiesReturnsElitePoison";
                elitePoisonRecipe.pickup = Addressables.LoadAssetAsync<EquipmentDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ElitePoison.ElitePoisonEquipment_asset).WaitForCompletion();
                elitePoisonRecipe.recipes = new Recipe[]
                {
                        new Recipe()
                        {
                            ingredients = new RecipeIngredient[]
                            {
                                new RecipeIngredient()
                                {
                                    pickup = Content.Items.LynxFetish,
                                    type = IngredientTypeIndex.AssetReference
                                },
                                new RecipeIngredient()
                                {
                                    pickup = Addressables.LoadAssetAsync<ItemDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_HeadHunter.HeadHunter_asset).WaitForCompletion(),
                                    type = IngredientTypeIndex.AssetReference
                                }
                            }
                        }
                };

                craftableList.Add(elitePoisonRecipe);
            }

            return dtLynxTotem;
        }

        private void CreateLynxShaman(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, Dictionary<string, Texture2D> rampLookups)
        {
            var shamanStuff = new ShamanStuff();

            var shamanSpawnEffect = shamanStuff.CreateShamanSpawnEffect(assets.First(prefab => prefab.name == "LynxSpawnParticles"));
            ModdedEntityStates.LynxTribe.Shaman.SpawnState.spawnEffect = shamanSpawnEffect;
            effectsList.Add(new EffectDef(shamanSpawnEffect));

            Content.Buffs.ReduceHealing = shamanStuff.CreateReduceHealingBuff(iconLookup["texReducedHealingBuffColored"]);
            bdList.Add(Content.Buffs.ReduceHealing);

            Junk.ModdedEntityStates.LynxTribe.Shaman.SummonStormSkill.summonEffectPrefab = shamanStuff.CreateSummonStormParticles(assets.First(prefab => prefab.name == "ShamanSummonStormParticle"));
            Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport.Teleport.ghostMaskPrefab = shamanStuff.SetupShamanMaskMaterials(assets.First(prefab => prefab.name == "ShamanMask"));

            Junk.ModdedEntityStates.LynxTribe.Shaman.TeleportFriend.teleportEffect = shamanStuff.CreateShamanTeleportOut(assets.First(prefab => prefab.name == "ShamanTeleportEffectOut"));
            effectsList.Add(new EffectDef(Junk.ModdedEntityStates.LynxTribe.Shaman.TeleportFriend.teleportEffect));

            ModdedEntityStates.LynxTribe.Shaman.PushBack.summonPrefab = shamanStuff.CreateShamanPushBackSummonEffect(assets.First(prefab => prefab.name == "LynxShamanPushBackSummon"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Shaman.PushBack.summonPrefab));

            ModdedEntityStates.LynxTribe.Shaman.PushBack.explosionPrefab = shamanStuff.CreateShamanPushBackExplosionEffect(assets.First(prefab => prefab.name == "LynxShamanPushBackExplosion"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Shaman.PushBack.explosionPrefab));

            TempVisualEffectAPI.AddTemporaryVisualEffect(shamanStuff.CreateReduceHealingVisualEffect(), (body) => { return body.HasBuff(Content.Buffs.ReduceHealing); });

            var projectileImpactEffect = shamanStuff.CreateShamanProjectileImpactEffect(assets.First(prefab => prefab.name == "LynxShamanProjectileImpactEffect"), rampLookups["texRampLynxShamanProjectileImpact"]);
            effectsList.Add(new EffectDef(projectileImpactEffect));

            var projectileImpactSound = Utils.CreateNetworkSoundDef("ER_Shaman_Projectile_Impact_Play");
            nseList.Add(projectileImpactSound);

            var skullProjectile = shamanStuff.CreateShamanTrackingProjectile(
                assets.First(prefab => prefab.name == "ShamanTrackingProjectile"),
                shamanStuff.CreateShamanTrackingProjectileGhost(),
                projectileImpactEffect,
                shamanStuff.CreateProjectileFlightSoundLoop(),
                projectileImpactSound
            );
            Junk.ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesRapidFire.trackingProjectilePrefab = skullProjectile;
            ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesShotgun.trackingProjectilePrefab = skullProjectile;
            projectilesList.Add(skullProjectile);

            var projectilesSummonEffect = shamanStuff.CreateShamanTrackingProjectileSummonEffect(acdLookup["acdShamanSummonProjectilesScaling"]);
            ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesShotgun.summonEffect = projectilesSummonEffect;
            effectsList.Add(new EffectDef(projectilesSummonEffect));

            var shamanLog = Utils.CreateUnlockableDef("Logs.LynxShamanBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_LYNX_SHAMAN");
            unlockablesList.Add(shamanLog);

            var shamanBody = new ShamanBody();
            ShamanBody.Skills.Teleport = shamanBody.CreateTeleportSkill();
            ShamanBody.Skills.SummonStorm = shamanBody.CreateSummonStormSkill();
            ShamanBody.Skills.SummonProjectiles = shamanBody.CreateSummonProjectilesSkill();
            ShamanBody.Skills.TeleportFriend = shamanBody.CreateTeleportFriendSkill();
            ShamanBody.Skills.SummonLightning = shamanBody.CreateSummonLightningSkill();
            ShamanBody.Skills.PushBack = shamanBody.CreatePushBackSkill();

            sdList.Add(ShamanBody.Skills.Teleport);
            sdList.Add(ShamanBody.Skills.SummonStorm);
            sdList.Add(ShamanBody.Skills.SummonProjectiles);
            sdList.Add(ShamanBody.Skills.TeleportFriend);
            sdList.Add(ShamanBody.Skills.SummonLightning);
            sdList.Add(ShamanBody.Skills.PushBack);

            Junk.ModdedEntityStates.LynxTribe.Shaman.SummonStormSkill.cscStorm = LynxStormBody.cscLynxStorm;

            ShamanBody.SkillFamilies.Utility = Utils.CreateSkillFamily("LynxShamanUtilitySkillFamily", ShamanBody.Skills.Teleport);
            //ShamanBody.SkillFamilies.Utility = Utils.CreateSkillFamily("LynxShamanUtilitySkillFamily", ShamanBody.Skills.SummonLightning);
            ShamanBody.SkillFamilies.Special = Utils.CreateSkillFamily("LynxShamanSpecialSkillFamily", ShamanBody.Skills.SummonStorm);
            ShamanBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxShamanPrimarySkillFamily", ShamanBody.Skills.SummonProjectiles);
            //ShamanBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("LynxShamanSecondarySkillFamily", ShamanBody.Skills.TeleportFriend);
            ShamanBody.SkillFamilies.Secondary = Utils.CreateSkillFamily("LynxShamanSecondarySkillFamily", ShamanBody.Skills.PushBack);

            sfList.Add(ShamanBody.SkillFamilies.Utility);
            sfList.Add(ShamanBody.SkillFamilies.Special);
            sfList.Add(ShamanBody.SkillFamilies.Primary);
            sfList.Add(ShamanBody.SkillFamilies.Secondary);

            ShamanBody.BodyPrefab = shamanBody.AddBodyComponents(assets.First(body => body.name == "LynxShamanBody"), iconLookup["texLynxShamanIcon"], shamanLog);
            bodyList.Add(ShamanBody.BodyPrefab);
            ShamanMaster.MasterPrefab = new ShamanMaster().AddMasterComponents(assets.First(master => master.name == "LynxShamanMaster"), ShamanBody.BodyPrefab);
            masterList.Add(ShamanMaster.MasterPrefab);

            ShamanBody.SpawnCards.cscLynxShamanDefault = shamanBody.CreateCard("cscLynxShamanDefault", ShamanMaster.MasterPrefab, ShamanBody.SkinDefs.Default, ShamanBody.BodyPrefab);

            var dhLynxShamanDefault = new DirectorCard
            {
                spawnCard = ShamanBody.SpawnCards.cscLynxShamanDefault,
                selectionWeight = Configuration.LynxTribe.LynxShaman.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = Configuration.LynxTribe.LynxShaman.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxShamanDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxShamanDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStages(Configuration.LynxTribe.LynxShaman.DefaultStageList.Value, dchLynxShamanDefault);

            var shamanBodyAlly = new ShamanBodyAlly();
            Content.Buffs.LynxShamanSpecialDamage = shamanBodyAlly.CreateSpecialBuffDef(iconLookup["texLynxShamanBuff"]);
            bdList.Add(Content.Buffs.LynxShamanSpecialDamage);

            ShamanBodyAlly.BodyPrefab = shamanBodyAlly.AddBodyComponents(assets.First(prefab => prefab.name == "LynxShamanAllyBody"), sprite: iconLookup["texLynxShamanAllyIcon"], log: null);
            bodyList.Add(ShamanBodyAlly.BodyPrefab);

            ShamanMasterAlly.MasterPrefab = new ShamanMasterAlly().AddMasterComponents(assets.First(prefab => prefab.name == "LynxShamanAllyMaster"), ShamanBodyAlly.BodyPrefab);
            masterList.Add(ShamanMasterAlly.MasterPrefab);

            ShamanBodyAlly.SpawnCards.cscLynxShamanAlly = shamanBodyAlly.CreateCard("cscLynxShamanAlly", ShamanMasterAlly.MasterPrefab, ShamanBodyAlly.SkinDefs.Ally, ShamanBodyAlly.BodyPrefab);
        }

        private void CreateLynxStorm(GameObject[] assets, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var stormStuff = new LynxStormStuff();
            Content.Buffs.LynxStormImmunity = stormStuff.CreateStormImmunityBuff();
            bdList.Add(Content.Buffs.LynxStormImmunity);

            LynxStormComponent.dotEffect = stormStuff.CreateStormThrowEffect();
            effectsList.Add(new EffectDef(LynxStormComponent.dotEffect));

            var stormBody = new LynxStormBody();
            LynxStormBody.BodyPrefab = stormBody.AddBodyComponents(assets.First(body => body.name == "StormBody"), acdLookup);
            bodyList.Add(LynxStormBody.BodyPrefab);

            LynxStormMaster.MasterPrefab = new LynxStormMaster().AddMasterComponents(assets.First(master => master.name == "StormMaster"), LynxStormBody.BodyPrefab);
            masterList.Add(LynxStormMaster.MasterPrefab);

            LynxStormBody.cscLynxStorm = stormBody.CreateCard("cscLynxStorm", LynxStormMaster.MasterPrefab);
        }


    }
}
