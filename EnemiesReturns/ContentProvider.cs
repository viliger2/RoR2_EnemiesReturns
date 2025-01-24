using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Ifrit.Pillar;
using EnemiesReturns.Enemies.LynxTribe;
using EnemiesReturns.Enemies.LynxTribe.Archer;
using EnemiesReturns.Enemies.LynxTribe.Hunter;
using EnemiesReturns.Enemies.LynxTribe.Scout;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using EnemiesReturns.Enemies.LynxTribe.Totem;
using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using R2API;
using Rewired.HID.Drivers;
using Rewired.Utils.Classes.Utility;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace EnemiesReturns
{
    public class ContentProvider : IContentPackProvider
    {
        public string identifier => EnemiesReturnsPlugin.GUID + "." + nameof(ContentProvider);

        private readonly ContentPack _contentPack = new ContentPack();

        public const string AssetBundleName = "EnemiesReturns";
        public const string AssetBundleFolder = "AssetBundles";

        public const string SoundbankFolder = "Soundbanks";
        public const string SoundsSoundBankFileName = "EnemiesReturnsSounds";

        private readonly List<GameObject> bodyList = new List<GameObject>();
        private readonly List<GameObject> masterList = new List<GameObject>();
        private readonly List<Type> stateList = new List<Type>();
        private readonly List<SkillFamily> sfList = new List<SkillFamily>();
        private readonly List<SkillDef> sdList = new List<SkillDef>();
        private readonly List<EffectDef> effectsList = new List<EffectDef>();
        private readonly List<GameObject> projectilesList = new List<GameObject>();
        private readonly List<UnlockableDef> unlockablesList = new List<UnlockableDef>();
        private readonly List<ItemDef> itemList = new List<ItemDef>();
        private readonly List<NetworkSoundEventDef> nseList = new List<NetworkSoundEventDef>();
        private readonly List<GameObject> nopList = new List<GameObject>();
        private readonly List<BuffDef> bdList = new List<BuffDef>();

        public static readonly Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbedror2/base/shaders/hgstandard", "RoR2/Base/Shaders/HGStandard.shader"},
            {"stubbedror2/base/shaders/hgsnowtopped", "RoR2/Base/Shaders/HGSnowTopped.shader"},
            {"stubbedror2/base/shaders/hgtriplanarterrainblend", "RoR2/Base/Shaders/HGTriplanarTerrainBlend.shader"},
            {"stubbedror2/base/shaders/hgintersectioncloudremap", "RoR2/Base/Shaders/HGIntersectionCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgcloudremap", "RoR2/Base/Shaders/HGCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgopaquecloudremap", "RoR2/Base/Shaders/HGOpaqueCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgdistortion", "RoR2/Base/Shaders/HGDistortion.shader" },
            {"stubbedcalm water/calmwater - dx11 - doublesided", "Calm Water/CalmWater - DX11 - DoubleSided.shader" },
            {"stubbedcalm water/calmwater - dx11", "Calm Water/CalmWater - DX11.shader" },
            {"stubbednature/speedtree", "RoR2/Base/Shaders/SpeedTreeCustom.shader"},
            {"stubbeddecalicious/decaliciousdeferreddecal", "Decalicious/DecaliciousDeferredDecal.shader" }
        };

        public static Dictionary<string, Material> MaterialCache = new Dictionary<string, Material>(); //apparently you need it because reasons?

        public static ItemRelationshipProvider ModdedContagiousItemProvider;

        public static ItemDef VoidMegaCrabItem = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/VoidMegaCrabItem.asset").WaitForCompletion();

        public struct MonsterCategories
        {
            public const string Champions = "Champions";
            public const string Minibosses = "Minibosses";
            public const string BasicMonsters = "Basic Monsters";
            public const string Special = "Special";
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(_contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            _contentPack.identifier = identifier;

            ModdedContagiousItemProvider = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
            (ModdedContagiousItemProvider as ScriptableObject).name = "EnemiesReturnsContagiousItemProvider";
            ModdedContagiousItemProvider.relationshipType = Addressables.LoadAssetAsync<ItemRelationshipType>("RoR2/DLC1/Common/ContagiousItem.asset").WaitForCompletion();
            ModdedContagiousItemProvider.relationships = Array.Empty<ItemDef.Pair>();

            Stopwatch segmentStopWatch = new Stopwatch();
            segmentStopWatch.Start();
            string soundbanksFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), SoundbankFolder);
            LoadSoundBanks(soundbanksFolderPath);
            segmentStopWatch.Stop();
            //TimeSpan ts = stopwatch.elapsedSeconds;
            Log.Info("Soundbanks loaded in " + segmentStopWatch.elapsedSeconds);

            string assetBundleFolderPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location), AssetBundleFolder);

            AssetBundle assetbundle = null;
            yield return LoadAssetBundle(System.IO.Path.Combine(assetBundleFolderPath, AssetBundleName), args.progressReceiver, (resultAssetBundle) => assetbundle = resultAssetBundle);

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Material[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var materials = assets;

                if (materials != null)
                {
                    foreach (Material material in materials)
                    {
                        var replacementShader = Addressables.LoadAssetAsync<Shader>(ShaderLookup[material.shader.name.ToLower()]).WaitForCompletion();
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                        }
                        else
                        {
                            Log.Info("Couldn't find replacement shader for " + material.shader.name.ToLower());
                        }
                        MaterialCache.Add(material.name, material);
                    }
                }
                stopwatch.Stop();
                Log.Info("Materials swapped in " + stopwatch.elapsedSeconds);
            }));

            Dictionary<string, Sprite> iconLookup = new Dictionary<string, Sprite>();

            Dictionary<string, Texture2D> rampLookups = new Dictionary<string, Texture2D>();
            Texture2D texLavaCrackRound = null;
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Texture2D[]>)((textures) =>
            {
                texLavaCrackRound = textures.First(texture => texture.name == "texLavaCrackRound");
                rampLookups = textures.Where(texture => texture.name.StartsWith("texRamp")).ToDictionary(texture => texture.name, texture => texture);
            }));

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Sprite[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                iconLookup = assets.ToDictionary(item => item.name, item => item);
                stopwatch.Stop();
                Log.Info("Icons loaded in " + stopwatch.elapsedSeconds);
            }));

            Dictionary<string, AnimationCurveDef> acdLookup = new Dictionary<string, AnimationCurveDef>();
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<AnimationCurveDef[]>)((assets) =>
            {
                acdLookup = assets.ToDictionary(item => item.name);
            }));

            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                stateList.Add(typeof(ModdedEntityStates.BasePlayerEmoteState)); // dunno if I need it, but just in case

                CreateSpitter(assets, iconLookup);

                CreateColossus(assets, iconLookup, acdLookup);

                CreateIfrit(assets, iconLookup, texLavaCrackRound, acdLookup);

                CreateMechanicalSpider(assets, iconLookup, acdLookup);

                CreateLynxTribe(assets, iconLookup, acdLookup, rampLookups);

                stopwatch.Stop();
                Log.Info("Characters created in " + stopwatch.elapsedSeconds);
            }));

            _contentPack.bodyPrefabs.Add(bodyList.ToArray());
            _contentPack.masterPrefabs.Add(masterList.ToArray());
            _contentPack.skillDefs.Add(sdList.ToArray());
            _contentPack.skillFamilies.Add(sfList.ToArray());
            _contentPack.entityStateTypes.Add(stateList.ToArray());
            _contentPack.effectDefs.Add(effectsList.ToArray());
            _contentPack.projectilePrefabs.Add(projectilesList.ToArray());
            _contentPack.unlockableDefs.Add(unlockablesList.ToArray());
            _contentPack.itemDefs.Add(itemList.ToArray());
            _contentPack.networkSoundEventDefs.Add(nseList.ToArray());
            _contentPack.networkedObjectPrefabs.Add(nopList.ToArray());
            _contentPack.itemRelationshipProviders.Add(new[] { ModdedContagiousItemProvider });
            _contentPack.buffDefs.Add(bdList.ToArray());
            totalStopwatch.Stop();
            Log.Info("Total loading time: " + totalStopwatch.elapsedSeconds);

            yield break;
        }

        #region Content

        private void CreateIfrit(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Texture2D texLavaCrackRound, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            CreateIfritPillar(assets, iconLookup, acdLookup);
            ExplicitPickupDropTable dtIfrit = CreateIfritItem(assets, iconLookup);

            if (EnemiesReturns.Configuration.Ifrit.Enabled.Value)
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
                ifritManePrefab.GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matIfritManeFire", ifritBody.CreateManeFiresMaterial);

                var ifritMaster = new IfritMaster();
                IfritMaster.MasterPrefab = ifritMaster.AddMasterComponents(assets.First(master => master.name == "IfritMaster"), IfritBody.BodyPrefab);
                masterList.Add(IfritMaster.MasterPrefab);

                IfritBody.SpawnCards.cscIfritDefault = ifritBody.CreateCard("cscIfritDefault", IfritMaster.MasterPrefab, IfritBody.SkinDefs.Default, IfritBody.BodyPrefab);
                var dcIfritDefault = new DirectorCard
                {
                    spawnCard = IfritBody.SpawnCards.cscIfritDefault,
                    selectionWeight = EnemiesReturns.Configuration.Ifrit.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Ifrit.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchIfritDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcIfritDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Ifrit.DefaultStageList.Value, dchIfritDefault);

                stateList.Add(typeof(ModdedEntityStates.Ifrit.SpawnState));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.DeathState));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.SummonPylon));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneStart));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneFire));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneEnd));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameChargeBegin));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameCharge));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameChargeEnd));
                stateList.Add(typeof(Junk.ModdedEntityStates.Ifrit.Smash));

                if (EnemiesReturns.Configuration.Ifrit.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
                {
                    ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(IfritBody.SpawnCards.cscIfritDefault, 2);
                }
            }
        }

        private ExplicitPickupDropTable CreateIfritItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtIfrit = null;
            if (EnemiesReturns.Configuration.Ifrit.ItemEnabled.Value)
            {
                var pillarItemFactory = new SpawnPillarOnChampionKillFactory();

                SpawnPillarOnChampionKillFactory.ItemDef = pillarItemFactory.CreateItem(assets.First(item => item.name == "IfritItem"), iconLookup["texIconIfritItem"]);
                itemList.Add(SpawnPillarOnChampionKillFactory.ItemDef);

                dtIfrit = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtIfrit as ScriptableObject).name = "epdtIfrit";
                dtIfrit.canDropBeReplaced = true;
                dtIfrit.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = SpawnPillarOnChampionKillFactory.ItemDef
                        }
                };
                HG.ArrayUtils.ArrayAppend(ref ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = SpawnPillarOnChampionKillFactory.ItemDef, itemDef2 = VoidMegaCrabItem });
            }

            return dtIfrit;
        }

        private void CreateIfritPillar(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (EnemiesReturns.Configuration.Ifrit.ItemEnabled.Value || EnemiesReturns.Configuration.Ifrit.Enabled.Value)
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

                if (EnemiesReturns.Configuration.Ifrit.Enabled.Value)
                {
                    var pillarEnemyBody = new PillarEnemyBody();
                    PillarEnemyBody.BodyPrefab = pillarEnemyBody.AddBodyComponents(pylonBody.InstantiateClone("IfritPylonEnemyBody", false), iconLookup["texIconPillarEnemy"], acdLookup);
                    bodyList.Add(PillarEnemyBody.BodyPrefab);

                    PillarMaster.EnemyMasterPrefab = pillarMaster.AddMasterComponents(pylonMaster.InstantiateClone("IfritPylonEnemyMaster", false), PillarEnemyBody.BodyPrefab);
                    masterList.Add(PillarMaster.EnemyMasterPrefab);

                    PillarEnemyBody.SpawnCard = pillarEnemyBody.CreateCard("cscIfritEnemyPillar", PillarMaster.EnemyMasterPrefab);

                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.ChargingExplosion));
                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.FireExplosion));
                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.KilledDeathState));
                }

                if (EnemiesReturns.Configuration.Ifrit.ItemEnabled.Value)
                {
                    var pillarAllyBody = new PillarAllyBody();

                    PillarAllyBody.BodyPrefab = pillarAllyBody.AddBodyComponents(pylonBody.InstantiateClone("IfritPylonPlayerBody", false), iconLookup["texIconPillarAlly"], acdLookup);
                    bodyList.Add(PillarAllyBody.BodyPrefab);

                    PillarMaster.AllyMasterPrefab = pillarMaster.AddMasterComponents(pylonMaster.InstantiateClone("IfritPylonPlayerMaster", false), PillarAllyBody.BodyPrefab);
                    masterList.Add(PillarMaster.AllyMasterPrefab);

                    PillarAllyBody.SpawnCard = pillarAllyBody.CreateCard("cscIfritPlayerPillar", PillarMaster.AllyMasterPrefab);

                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Player.ChargingExplosion));
                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Player.FireExplosion));
                }

                stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.SuicideDeathState));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.SpawnState));
            }
        }

        private void CreateColossus(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            ExplicitPickupDropTable dtColossus = CreateColossusItem(assets, iconLookup);

            if (EnemiesReturns.Configuration.Colossus.Enabled.Value)
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
                ModdedEntityStates.Colossus.Death.Death2.fallEffect = deathFallEffect;

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

                stateList.Add(typeof(ModdedEntityStates.Colossus.ColossusMain));
                stateList.Add(typeof(ModdedEntityStates.Colossus.SpawnState));
                stateList.Add(typeof(ModdedEntityStates.Colossus.DancePlayer));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Death.InitialDeathState));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Death.BaseDeath));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Death.DeathFallBase));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Death.Death1));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Death.Death2));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Death.DeathBoner));
                stateList.Add(typeof(ModdedEntityStates.Colossus.RockClap.RockClapEnd));
                stateList.Add(typeof(ModdedEntityStates.Colossus.RockClap.RockClapStart));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompEnter));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompBase));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompL));
                stateList.Add(typeof(ModdedEntityStates.Colossus.Stomp.StompR));
                stateList.Add(typeof(ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageStart));
                stateList.Add(typeof(ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack));
                stateList.Add(typeof(ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageEnd));
                stateList.Add(typeof(Junk.ModdedEntityStates.Colossus.HeadLaser.HeadLaserStart));
                stateList.Add(typeof(Junk.ModdedEntityStates.Colossus.HeadLaser.HeadLaserAttack));
                stateList.Add(typeof(Junk.ModdedEntityStates.Colossus.HeadLaser.HeadLaserEnd));

                ColossusBody.SpawnCards.cscColossusDefault = colossusBody.CreateCard("cscColossusDefault", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Default, ColossusBody.BodyPrefab);
                DirectorCard dcColossusDefault = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusDefault,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusDefault, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion());
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Colossus.DefaultStageList.Value, dchColossusDefault);

                ColossusBody.SpawnCards.cscColossusSkyMeadow = colossusBody.CreateCard("cscColossusSkyMeadow", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.SkyMeadow, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSkyMeadow = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSkyMeadow,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSkyMeadow = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSkyMeadow,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Colossus.SkyMeadowStageList.Value, dchColossusSkyMeadow);

                ColossusBody.SpawnCards.cscColossusGrassy = colossusBody.CreateCard("cscColossusGrassy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Grassy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusGrassy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusGrassy,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusGrassy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusGrassy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusGrassy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilyNature").WaitForCompletion());
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Colossus.GrassyStageList.Value, dchColossusGrassy);

                ColossusBody.SpawnCards.cscColossusCastle = colossusBody.CreateCard("cscColossusCastle", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Castle, ColossusBody.BodyPrefab);
                DirectorCard dcColossusCastle = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusCastle,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusCastle = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusCastle,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Colossus.CastleStageList.Value, dchColossusCastle);

                ColossusBody.SpawnCards.cscColossusSandy = colossusBody.CreateCard("cscColossusSandy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Sandy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSandy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSandy,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSandy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSandy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusSandy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySandy.asset").WaitForCompletion());
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Colossus.SandyStageList.Value, dchColossusSandy);

                ColossusBody.SpawnCards.cscColossusSnowy = colossusBody.CreateCard("cscColossusSnowy", ColossusMaster.MasterPrefab, ColossusBody.SkinDefs.Snowy, ColossusBody.BodyPrefab);
                DirectorCard dcColossusSnowy = new DirectorCard
                {
                    spawnCard = ColossusBody.SpawnCards.cscColossusSnowy,
                    selectionWeight = EnemiesReturns.Configuration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSnowy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSnowy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusSnowy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySnowy.asset").WaitForCompletion());
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Colossus.SnowyStageList.Value, dchColossusSnowy);

                if (EnemiesReturns.Configuration.Colossus.AddToArtifactOfOrigin.Value && ModCompats.RiskyArtifafactsCompat.enabled)
                {
                    ModCompats.RiskyArtifafactsCompat.AddMonsterToArtifactOfOrigin(ColossusBody.SpawnCards.cscColossusGrassy, 3);
                }
            }
        }

        private ExplicitPickupDropTable CreateColossusItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtColossus = null;

            if (EnemiesReturns.Configuration.Colossus.ItemEnabled.Value)
            {
                Items.ColossalKnurl.ColossalKnurlFactory.ColossalFist = ProcTypeAPI.ReserveProcType();

                var knurlFactory = new ColossalKnurlFactory();

                ColossalKnurlFactory.ItemDef = knurlFactory.CreateItem(assets.First(item => item.name == "PickupColossalCurl"), iconLookup["texColossalKnurlIcon"]);
                itemList.Add(ColossalKnurlFactory.ItemDef);

                dtColossus = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtColossus as ScriptableObject).name = "epdtColossus";
                dtColossus.canDropBeReplaced = true;
                dtColossus.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = ColossalKnurlFactory.ItemDef
                        }
                };

                var knurlProjectileGhost = assets.First(item => item.name == "ColossalKnurlFistProjectileGhost");
                knurlProjectileGhost = knurlFactory.CreateFistGhostPrefab(knurlProjectileGhost);

                var knurlProjectile = assets.First(item => item.name == "ColossalKnurlFistProjectile");
                ColossalKnurlFactory.projectilePrefab = knurlFactory.CreateFistProjectile(knurlProjectile, knurlProjectileGhost);

                projectilesList.Add(ColossalKnurlFactory.projectilePrefab);

                HG.ArrayUtils.ArrayAppend(ref ModdedContagiousItemProvider.relationships, new ItemDef.Pair { itemDef1 = ColossalKnurlFactory.ItemDef, itemDef2 = VoidMegaCrabItem });
            }

            return dtColossus;
        }

        private void CreateSpitter(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            if (EnemiesReturns.Configuration.Spitter.Enabled.Value)
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
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Spitter.DefaultStageList.Value, dchSpitterDefault);

                SpitterBody.SpawnCards.cscSpitterLakes = spitterBody.CreateCard("cscSpitterLakes", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Lakes, SpitterBody.BodyPrefab);
                var dcSpitterLakes = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterLakes,
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterLakes = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterLakes,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Spitter.LakesStageList.Value, dchSpitterLakes);

                SpitterBody.SpawnCards.cscSpitterSulfur = spitterBody.CreateCard("cscSpitterSulfur", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Sulfur, SpitterBody.BodyPrefab);
                var dcSpitterSulfur = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterSulfur,
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterSulfur = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterSulfur,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Spitter.SulfurStageList.Value, dchSpitterSulfur);

                SpitterBody.SpawnCards.cscSpitterDepths = spitterBody.CreateCard("cscSpitterDepths", SpitterMaster.MasterPrefab, SpitterBody.SkinDefs.Depths, SpitterBody.BodyPrefab);
                var dcSpitterDepth = new DirectorCard
                {
                    spawnCard = SpitterBody.SpawnCards.cscSpitterDepths,
                    selectionWeight = EnemiesReturns.Configuration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturns.Configuration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterDepths = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDepth,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                Utils.AddMonsterToStage(EnemiesReturns.Configuration.Spitter.DepthStageList.Value, dchSpitterDepths);
                if (EnemiesReturns.Configuration.Spitter.HelminthroostReplaceMushrum.Value)
                {
                    DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.MiniMushrum, DirectorAPI.Stage.HelminthHatchery);
                }

                stateList.Add(typeof(ModdedEntityStates.Spitter.Bite));
                stateList.Add(typeof(ModdedEntityStates.Spitter.SpawnState));
                stateList.Add(typeof(Junk.ModdedEntityStates.Spitter.NormalSpit));
                stateList.Add(typeof(ModdedEntityStates.Spitter.ChargeChargedSpit));
                stateList.Add(typeof(ModdedEntityStates.Spitter.FireChargedSpit));
                stateList.Add(typeof(ModdedEntityStates.Spitter.DeathDance));
                stateList.Add(typeof(ModdedEntityStates.Spitter.SpitterMain));
                stateList.Add(typeof(ModdedEntityStates.Spitter.DeathDancePlayer));
            }
        }

        private void CreateMechanicalSpider(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (EnemiesReturns.Configuration.MechanicalSpider.Enabled.Value)
            {
                var spiderLog = Utils.CreateUnlockableDef("Logs.MechanicalSpiderBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_MECHANICAL_SPIDER");
                unlockablesList.Add(spiderLog);

                var spiderStuff = new MechanicalSpiderStuff();

                var doubleShotEffect = spiderStuff.CreateDoubleShotImpactEffect();
                effectsList.Add(new EffectDef(doubleShotEffect));

                ModdedEntityStates.MechanicalSpider.DoubleShot.Fire.projectilePrefab = spiderStuff.CreateDoubleShotProjectilePrefab(doubleShotEffect);
                projectilesList.Add(ModdedEntityStates.MechanicalSpider.DoubleShot.Fire.projectilePrefab);

                ModdedEntityStates.MechanicalSpider.DoubleShot.ChargeFire.effectPrefab = spiderStuff.CreateDoubleShotChargeEffect();

                var spiderEnemyBody = new MechanicalSpiderEnemyBody();
                MechanicalSpiderEnemyBody.Skills.DoubleShot = spiderEnemyBody.CreateDoubleShotSkill();
                MechanicalSpiderEnemyBody.Skills.Dash = spiderEnemyBody.CreateDashSkill();

                ModdedEntityStates.MechanicalSpider.Dash.Dash.forwardSpeedCoefficientCurve = acdLookup["acdSpiderDash"].curve;

                sdList.Add(MechanicalSpiderEnemyBody.Skills.DoubleShot);
                sdList.Add(MechanicalSpiderEnemyBody.Skills.Dash);

                MechanicalSpiderEnemyBody.SkillFamilies.Primary = Utils.CreateSkillFamily("MechanicalSpiderPrimaryFamily", MechanicalSpiderEnemyBody.Skills.DoubleShot);
                MechanicalSpiderEnemyBody.SkillFamilies.Utility = Utils.CreateSkillFamily("MechanicalSpiderUtilityFamily", MechanicalSpiderEnemyBody.Skills.Dash);

                sfList.Add(MechanicalSpiderEnemyBody.SkillFamilies.Primary);
                sfList.Add(MechanicalSpiderEnemyBody.SkillFamilies.Utility);

                CreateMechanicalSpiderEnemy(assets, iconLookup, spiderLog, spiderEnemyBody);

                CreateMechanichalSpiderDrone(assets, iconLookup, spiderStuff);

                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.MainState));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.SpawnState));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.SpawnStateDrone));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.VictoryDance));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.VictoryDancePlayer));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.OpenHatch));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.ChargeFire));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.Fire));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.CloseHatch));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.Dash.DashStart));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.Dash.Dash));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.Dash.DashStop));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.Death.DeathInitial));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.Death.DeathDrone));
                stateList.Add(typeof(ModdedEntityStates.MechanicalSpider.Death.DeathNormal));
            }
        }

        private void CreateMechanichalSpiderDrone(GameObject[] assets, Dictionary<string, Sprite> iconLookup, MechanicalSpiderStuff spiderStuff)
        {
            var spiderAllyBody = new MechanicalSpiderDroneBody();
            MechanicalSpiderDroneBody.BodyPrefab = spiderAllyBody.AddBodyComponents(assets.First(body => body.name == "MechanicalSpiderDroneBody"), iconLookup["texMechanicalSpiderAllyIcon"]);
            bodyList.Add(MechanicalSpiderDroneBody.BodyPrefab);

            var spiderAllyMaster = new MechanicalSpiderDroneMaster();
            MechanicalSpiderDroneMaster.MasterPrefab = spiderAllyMaster.AddMasterComponents(assets.First(master => master.name == "MechanicalSpiderDroneMaster"), MechanicalSpiderDroneBody.BodyPrefab);
            masterList.Add(MechanicalSpiderDroneMaster.MasterPrefab);

            MechanicalSpiderStuff.InteractablePrefab = spiderStuff.CreateInteractable(assets.First(interactable => interactable.name == "MechanicalSpiderBroken"), MechanicalSpiderDroneMaster.MasterPrefab);
            nopList.Add(MechanicalSpiderStuff.InteractablePrefab);

            MechanicalSpiderStuff.SpawnCards.iscMechanicalSpiderBroken = spiderStuff.CreateInteractableSpawnCard("iscMechanicalSpiderBroken", MechanicalSpiderStuff.InteractablePrefab);
        }

        private void CreateMechanicalSpiderEnemy(GameObject[] assets, Dictionary<string, Sprite> iconLookup, UnlockableDef spiderLog, MechanicalSpiderEnemyBody spiderEnemyBody)
        {
            MechanicalSpiderEnemyBody.BodyPrefab = spiderEnemyBody.AddBodyComponents(assets.First(body => body.name == "MechanicalSpiderBody"), iconLookup["texMechanicalSpiderEnemyIcon"], spiderLog);
            bodyList.Add(MechanicalSpiderEnemyBody.BodyPrefab);

            var spiderEnemyMaster = new MechanicalSpiderEnemyMaster();
            MechanicalSpiderEnemyMaster.MasterPrefab = spiderEnemyMaster.AddMasterComponents(assets.First(master => master.name == "MechanicalSpiderMaster"), MechanicalSpiderEnemyBody.BodyPrefab);
            masterList.Add(MechanicalSpiderEnemyMaster.MasterPrefab);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault = spiderEnemyBody.CreateCard("cscMechanicalSpeiderDefault", MechanicalSpiderEnemyMaster.MasterPrefab);
            var dcMechanicalSpiderDefault = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault,
                selectionWeight = EnemiesReturns.Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStage(EnemiesReturns.Configuration.MechanicalSpider.DefaultStageList.Value, dchMechanicalSpiderDefault);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy = spiderEnemyBody.CreateCard("cscMechanicalSpiderGrassy", MechanicalSpiderEnemyMaster.MasterPrefab, MechanicalSpiderEnemyBody.SkinDefs.Grassy, MechanicalSpiderEnemyMaster.MasterPrefab);
            var dcMechanicalSpiderGrassy = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy,
                selectionWeight = EnemiesReturns.Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderGrassy = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderGrassy,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStage(EnemiesReturns.Configuration.MechanicalSpider.GrassyStageList.Value, dchMechanicalSpiderGrassy);

            MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy = spiderEnemyBody.CreateCard("cscMechanicalSpiderSnowy", MechanicalSpiderEnemyMaster.MasterPrefab, MechanicalSpiderEnemyBody.SkinDefs.Snowy, MechanicalSpiderEnemyMaster.MasterPrefab);
            var dcMechanicalSpiderSnowy = new DirectorCard
            {
                spawnCard = MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy,
                selectionWeight = EnemiesReturns.Configuration.MechanicalSpider.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.MechanicalSpider.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchMechanicalSpiderSnowy = new DirectorAPI.DirectorCardHolder
            {
                Card = dcMechanicalSpiderSnowy,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStage(EnemiesReturns.Configuration.MechanicalSpider.SnowyStageList.Value, dchMechanicalSpiderSnowy);
        }
        
        private void CreateLynxTribe(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, Dictionary<string, Texture2D> rampLookups)
        {
            if(EnemiesReturns.Configuration.LynxTribe.LynxTotem.Enabled.Value
                || EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                CreateLynxStorm(assets, acdLookup);
            }
            if (EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                CreateLynxShaman(assets, iconLookup, acdLookup, rampLookups);
            }
            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.Enabled.Value)
            {
                CreateLynxScout(assets, iconLookup);
                CreateLynxHunter(assets, iconLookup);
                CreateLynxArcher(assets, iconLookup, rampLookups);
                CreateLynxTotem(assets, iconLookup, acdLookup);
                Utils.AddMonsterFamilyToStage(EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value, new LynxTribeStuff().CreateLynxTribeFamily());
                if (EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineEnabled.Value)
                {
                    CreateLynxShrine(assets);
                }
                if (EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapEnabled.Value)
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

            var defaultStages = EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value.Split(",");
            foreach (var stageString in defaultStages)
            {
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                GameObject lynxTrap = lynxTrapPrefab.InstantiateClone(lynxTrapPrefab.name + cleanStageString, false);
                switch (cleanStageString)
                {
                    case "blackbeach":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("SkyMeadowLeaves_LOD0", lynxStuff.CreateBlackBeachLeavesMaterialLOD0, material));
                        break;
                    case "skymeadow":
                    case "itskymeadow":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("SkyMeadowLeaves_LOD0", lynxStuff.CreateSkyMeadowLeavesMaterialLOD0, material));
                        break;
                    case "village":
                    case "villagenight":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("ShatteredAbodesLeaves_LOD0", lynxStuff.CreateShatteredAbodesLeavesMaterialLOD0, material));
                        break;
                    case "golemplains":
                    case "itgolemplains":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("TitanicPlainsLeaves_LOD0", lynxStuff.CreateTitanicPlanesLeavesMaterialLOD0, material));
                        break;
                    case "foggyswamp":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("WetlandsLeaves_LOD0", lynxStuff.CreateWetlandsLeavesMaterialLOD0, material));
                        break;
                    case "habitatfall":
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, ContentProvider.GetOrCreateMaterial("GoldenDiebackLeaves_LOD0", lynxStuff.CreateGoldemDiebackLeavesMaterialLOD0, material));
                        break;
                    default:
                        lynxTrap = lynxStuff.CreateTrapPrefab(lynxTrap, leavesPrefab, material);
                        break;
                }
                InteractableSpawnCard spawnCardTrap = lynxStuff.CreateLynxTrapSpawnCard(lynxTrap, cleanStageString);

                var dcLynxTrap = new DirectorCard
                {
                    spawnCard = spawnCardTrap,
                    selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapSelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = false
                };

                var holderTrap = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcLynxTrap,
                    InteractableCategory = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapSpawnCategory.Value
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

            var lynxShrine = lynxStuff.CreateShrinePrefab(assets.First(prefab => prefab.name == "LynxShrinePrefab"));

            var spawnCardShrine = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            (spawnCardShrine as ScriptableObject).name = "iscLynxShrine";
            spawnCardShrine.prefab = lynxShrine;
            spawnCardShrine.sendOverNetwork = true;
            spawnCardShrine.hullSize = HullClassification.Golem;
            spawnCardShrine.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCardShrine.requiredFlags = RoR2.Navigation.NodeFlags.None;
            spawnCardShrine.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn | RoR2.Navigation.NodeFlags.NoShrineSpawn;
            spawnCardShrine.directorCreditCost = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineDirectorCost.Value;
            spawnCardShrine.occupyPosition = true;
            spawnCardShrine.eliteRules = SpawnCard.EliteRules.Default;
            spawnCardShrine.orientToFloor = false;

            var dcLynxShrine = new DirectorCard
            {
                spawnCard = spawnCardShrine,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineSelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false
            };

            var holderShrine = new DirectorAPI.DirectorCardHolder();
            holderShrine.Card = dcLynxShrine;
            holderShrine.InteractableCategory = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxShrineSpawnCategory.Value;

            var defaultStages = EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value.Split(",");
            foreach (var stageString in defaultStages)
            {
                string cleanStageString = string.Join("", stageString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                DirectorAPI.Helpers.AddNewInteractableToStage(holderShrine, DirectorAPI.ParseInternalStageName(cleanStageString), cleanStageString);
            }
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Retreat));

            nopList.Add(lynxShrine);
        }

        public void CreateLynxArcher(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, Texture2D> rampLookups)
        {
            var archerStuff = new ArcherStuff();
            var material = GetOrCreateMaterial("matLynxArcherArrow", archerStuff.CreateArcherArrowMaterial);

            ModdedEntityStates.LynxTribe.Archer.FireArrow.projectilePrefab = archerStuff.CreateArrowProjectile(
                assets.First(prefab => prefab.name == "ArrowProjectile"), 
                archerStuff.CreateArrowProjectileGhost(assets.First(prefab => prefab.name == "ArrowProjectileGhost"), material),
                archerStuff.CreateArrowImpalePrefab(assets.First(prefab => prefab.name == "ArrowProjectileGhost"), material));
            projectilesList.Add(ModdedEntityStates.LynxTribe.Archer.FireArrow.projectilePrefab);

            var archerBody = new ArcherBody();
            ArcherBody.Skills.Shot = archerBody.CreateShotSkill();
            sdList.Add(ArcherBody.Skills.Shot);

            ArcherBody.SkillFamilies.Primary = Utils.CreateSkillFamily("LynxArcherPrimarySkillFamily", ArcherBody.Skills.Shot);
            sfList.Add(ArcherBody.SkillFamilies.Primary);

            ArcherBody.BodyPrefab = archerBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxArcherBody"), sprite: null); // TODO: icon
            bodyList.Add(ArcherBody.BodyPrefab);

            var archerMaster = new ArcherMaster();
            ArcherMaster.MasterPrefab = archerMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxArcherMaster"), ArcherBody.BodyPrefab);
            masterList.Add(ArcherMaster.MasterPrefab);

            ArcherBody.SpawnCards.cscLynxArcherDefault = archerBody.CreateCard("cscLynxArcherDefault", ArcherMaster.MasterPrefab, ArcherBody.SkinDefs.Default, ArcherBody.BodyPrefab);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, ArcherBody.SpawnCards.cscLynxArcherDefault);

            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Archer.SpawnState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Archer.MainState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Archer.FireArrow));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Archer.DeathState));
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

            HunterBody.BodyPrefab = hunterBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxHunterBody"), sprite: null); // TODO: icon
            bodyList.Add(HunterBody.BodyPrefab);

            var hunterMaster = new HunterMaster();
            HunterMaster.MasterPrefab = hunterMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxHunterMaster"), HunterBody.BodyPrefab);
            masterList.Add(HunterMaster.MasterPrefab);

            HunterBody.SpawnCards.cscLynxHunterDefault = hunterBody.CreateCard("cscLynxHunterDefault", HunterMaster.MasterPrefab, HunterBody.SkinDefs.Default, HunterBody.BodyPrefab);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, HunterBody.SpawnCards.cscLynxHunterDefault);

            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Hunter.SpawnState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Hunter.MainState));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Hunter.Stab));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Hunter.Lunge.ChargeLunge));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Hunter.Lunge.FireLunge));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Hunter.DeathState));
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

            ScoutBody.BodyPrefab = scoutBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxScoutBody"), sprite: null); // TODO: icon
            bodyList.Add(ScoutBody.BodyPrefab);

            var scoutMaster = new ScoutMaster();
            ScoutMaster.MasterPrefab = scoutMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxScoutMaster"), ScoutBody.BodyPrefab);
            masterList.Add(ScoutMaster.MasterPrefab);

            ScoutBody.SpawnCards.cscLynxScoutDefault = scoutBody.CreateCard("cscLynxScoutDefault", ScoutMaster.MasterPrefab, ScoutBody.SkinDefs.Default, ScoutBody.BodyPrefab);

            HG.ArrayUtils.ArrayAppend(ref ModdedEntityStates.LynxTribe.Totem.SummonTribe.spawnCards, ScoutBody.SpawnCards.cscLynxScoutDefault);

            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Scout.DoubleSlash));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Scout.MainState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Scout.SpawnState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Scout.DeathState));
        }

        private void CreateLynxTotem(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var totemStuff = new TotemStuff();

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

            Enemies.LynxTribe.Storm.LynxStormOrb.orbEffect = totemStuff.CreateStormSummonOrb(assets.First(prefab => prefab.name == "TotemSummonStormOrb"));
            effectsList.Add(new EffectDef(Enemies.LynxTribe.Storm.LynxStormOrb.orbEffect));

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

            TotemBody.BodyPrefab = totemBody.AddBodyComponents(assets.First(prefab => prefab.name == "LynxTotemBody"), sprite: null, totemLog, null); // TODO: icon
            bodyList.Add(TotemBody.BodyPrefab);

            var totemMaster = new TotemMaster();
            TotemMaster.MasterPrefab = totemMaster.AddMasterComponents(assets.First(prefab => prefab.name == "LynxTotemMaster"), TotemBody.BodyPrefab);
            masterList.Add(TotemMaster.MasterPrefab);

            TotemBody.SpawnCards.cscLynxTotemDefault = totemBody.CreateCard("cscLynxTotemDefault", TotemMaster.MasterPrefab, TotemBody.SkinDefs.Default, TotemBody.BodyPrefab);

            var dhLynxTotemDefault = new DirectorCard
            {
                spawnCard = TotemBody.SpawnCards.cscLynxTotemDefault,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxTotem.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.LynxTribe.LynxTotem.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxTotemDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxTotemDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
            };
            Utils.AddMonsterToStage(EnemiesReturns.Configuration.LynxTribe.LynxTotem.DefaultStageList.Value, dchLynxTotemDefault);

            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.SpawnState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.MainState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.SummonStorm));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.SummonTribe));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Totem.SummonFirewall));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Totem.Groundpound));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.Burrow.Burrow));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.Burrow.Burrowed));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.Burrow.Unburrow));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.DeathState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Totem.SpawnStateFromShaman));
        }

        private void CreateLynxShaman(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup, Dictionary<string, Texture2D> rampLookups)
        {
            ShamanStuff.ApplyReducedHealing = DamageAPI.ReserveDamageType();
            var shamanStuff = new ShamanStuff();

            var shamanSpawnEffect = shamanStuff.CreateShamanSpawnEffect(assets.First(prefab => prefab.name == "LynxSpawnParticles"));
            ModdedEntityStates.LynxTribe.Shaman.SpawnState.spawnEffect = shamanSpawnEffect;
            effectsList.Add(new EffectDef(shamanSpawnEffect));

            ShamanStuff.ReduceHealing = shamanStuff.CreateReduceHealingBuff(iconLookup["texReducedHealingBuffColored"]);
            bdList.Add(ShamanStuff.ReduceHealing);

            Junk.ModdedEntityStates.LynxTribe.Shaman.SummonStormSkill.summonEffectPrefab = shamanStuff.CreateSummonStormParticles(assets.First(prefab => prefab.name == "ShamanSummonStormParticle"));
            Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport.Teleport.ghostMaskPrefab = shamanStuff.SetupShamanMaskMaterials(assets.First(prefab => prefab.name == "ShamanMask"));

            Junk.ModdedEntityStates.LynxTribe.Shaman.TeleportFriend.teleportEffect = shamanStuff.CreateShamanTeleportOut(assets.First(prefab => prefab.name == "ShamanTeleportEffectOut"));
            effectsList.Add(new EffectDef(Junk.ModdedEntityStates.LynxTribe.Shaman.TeleportFriend.teleportEffect));

            ModdedEntityStates.LynxTribe.Shaman.PushBack.summonPrefab = shamanStuff.CreateShamanPushBackSummonEffect(assets.First(prefab => prefab.name == "LynxShamanPushBackSummon"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Shaman.PushBack.summonPrefab));

            ModdedEntityStates.LynxTribe.Shaman.PushBack.explosionPrefab = shamanStuff.CreateShamanPushBackExplosionEffect(assets.First(prefab => prefab.name == "LynxShamanPushBackExplosion"));
            effectsList.Add(new EffectDef(ModdedEntityStates.LynxTribe.Shaman.PushBack.explosionPrefab));

            TempVisualEffectAPI.AddTemporaryVisualEffect(shamanStuff.CreateReduceHealingVisualEffect(), (body) => { return body.HasBuff(ShamanStuff.ReduceHealing); });

            var projectileImpactEffect = shamanStuff.CreateShamanProjectileImpactEffect(assets.First(prefab => prefab.name == "LynxShamanProjectileImpactEffect"), rampLookups["texRampLynxShamanProjectileImpact"]);
            effectsList.Add(new EffectDef(projectileImpactEffect));

            var skullProjectile = shamanStuff.CreateShamanTrackingProjectile(
                assets.First(prefab => prefab.name == "ShamanTrackingProjectile"), 
                shamanStuff.CreateShamanTrackingProjectileGhost(),
                projectileImpactEffect,
                shamanStuff.CreateProjectileFlightSoundLoop()
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

            ShamanBody.BodyPrefab = shamanBody.AddBodyComponents(assets.First(body => body.name == "LynxShamanBody"), null, shamanLog); // TODO: sprite
            bodyList.Add(ShamanBody.BodyPrefab);
            ShamanMaster.MasterPrefab = new ShamanMaster().AddMasterComponents(assets.First(master => master.name == "LynxShamanMaster"), ShamanBody.BodyPrefab);
            masterList.Add(ShamanMaster.MasterPrefab);

            ShamanBody.SpawnCards.cscLynxShamanDefault = shamanBody.CreateCard("cscLynxShamanDefault", ShamanMaster.MasterPrefab, ShamanBody.SkinDefs.Default, ShamanBody.BodyPrefab);

            var dhLynxShamanDefault = new DirectorCard
            {
                spawnCard = ShamanBody.SpawnCards.cscLynxShamanDefault,
                selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SelectionWeight.Value,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = true,
                minimumStageCompletions = EnemiesReturns.Configuration.LynxTribe.LynxShaman.MinimumStageCompletion.Value
            };
            DirectorAPI.DirectorCardHolder dchLynxShamanDefault = new DirectorAPI.DirectorCardHolder
            {
                Card = dhLynxShamanDefault,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            Utils.AddMonsterToStage(EnemiesReturns.Configuration.LynxTribe.LynxShaman.DefaultStageList.Value, dchLynxShamanDefault);

            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.SpawnState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesShotgun));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.PushBack));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.ShamanMainState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.NopeEmotePlayer));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.DeathState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.InitialDeathState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Shaman.SummonTotemDeath));

            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Shaman.SummonStormSkill));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesRapidFire));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Shaman.TeleportFriend));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Shaman.SummonLightning));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport.Teleport));
            stateList.Add(typeof(Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport.TeleportStart));
        }

        private void CreateLynxStorm(GameObject[] assets, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var stormStuff = new LynxStormStuff();
            LynxStormStuff.StormImmunity = stormStuff.CreateStormImmunityBuff();
            bdList.Add(LynxStormStuff.StormImmunity);

            Enemies.LynxTribe.Storm.LynxStormComponent.dotEffect = stormStuff.CreateStormThrowEffect();
            effectsList.Add(new EffectDef(Enemies.LynxTribe.Storm.LynxStormComponent.dotEffect));

            var stormBody = new LynxStormBody();
            LynxStormBody.BodyPrefab = stormBody.AddBodyComponents(assets.First(body => body.name == "StormBody"), acdLookup);
            bodyList.Add(LynxStormBody.BodyPrefab);

            LynxStormMaster.MasterPrefab = new LynxStormMaster().AddMasterComponents(assets.First(master => master.name == "StormMaster"), LynxStormBody.BodyPrefab);
            masterList.Add(LynxStormMaster.MasterPrefab);

            LynxStormBody.cscLynxStorm = stormBody.CreateCard("cscLynxStorm", LynxStormMaster.MasterPrefab);

            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Storm.SpawnState));
            stateList.Add(typeof(ModdedEntityStates.LynxTribe.Storm.MainState));
        }
        #endregion

        private IEnumerator LoadAssetBundle(string assetBundleFullPath, IProgress<float> progress, Action<AssetBundle> onAssetBundleLoaded)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundleFullPath);
            while (!assetBundleCreateRequest.isDone)
            {
                progress.Report(assetBundleCreateRequest.progress);
                yield return null;
            }

            onAssetBundleLoaded(assetBundleCreateRequest.assetBundle);
            stopwatch.Stop();
            Log.Info("Asset bundle " + assetBundleFullPath + " loaded in " + stopwatch.elapsedSeconds);

            yield break;
        }

        private static IEnumerator LoadAllAssetsAsync<T>(AssetBundle assetBundle, IProgress<float> progress, Action<T[]> onAssetsLoaded) where T : UnityEngine.Object
        {
            var sceneDefsRequest = assetBundle.LoadAllAssetsAsync<T>();
            while (!sceneDefsRequest.isDone)
            {
                progress.Report(sceneDefsRequest.progress);
                yield return null;
            }

            onAssetsLoaded(sceneDefsRequest.allAssets.Cast<T>().ToArray());

            yield break;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Material> materialCreateFunc)
        {
            if (!ContentProvider.MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc();
                ContentProvider.MaterialCache.Add(materialName, material);
            }
            return material;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Texture2D, Material> materialCreateFunc, Texture2D texture)
        {
            if (!ContentProvider.MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc(texture);
                ContentProvider.MaterialCache.Add(materialName, material);
            }
            return material;
        }

        public static Material GetOrCreateMaterial(string materialName, Func<Material, Material> materialCreateFunc, Material materialOrig)
        {
            if (!ContentProvider.MaterialCache.TryGetValue(materialName, out var material))
            {
                material = materialCreateFunc(materialOrig);
                ContentProvider.MaterialCache.Add(materialName, material);
            }

            return material;
        }

        private void LoadSoundBanks(string soundbanksFolderPath)
        {
            var akResult = AkSoundEngine.AddBasePath(soundbanksFolderPath);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank base path : {soundbanksFolderPath}");
            }
            else
            {
                Log.Error(
                    $"Error adding base path : {soundbanksFolderPath} " +
                    $"Error code : {akResult}");
            }

            //akResult = AkSoundEngine.LoadBank(InitSoundBankFileName, out var _);
            //if (akResult == AKRESULT.AK_Success)
            //{
            //    Log.Info($"Added bank : {InitSoundBankFileName}");
            //}
            //else
            //{
            //    Log.Error(
            //        $"Error loading bank : {InitSoundBankFileName} " +
            //        $"Error code : {akResult}");
            //}

            //akResult = AkSoundEngine.LoadBank(MusicSoundBankFileName, out var _);
            //if (akResult == AKRESULT.AK_Success)
            //{
            //    Log.Info($"Added bank : {MusicSoundBankFileName}");
            //}
            //else
            //{
            //    Log.Error(
            //        $"Error loading bank : {MusicSoundBankFileName} " +
            //        $"Error code : {akResult}");
            //}

            akResult = AkSoundEngine.LoadBank(SoundsSoundBankFileName, out var _);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {SoundsSoundBankFileName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {SoundsSoundBankFileName} " +
                    $"Error code : {akResult}");
            }
        }
    }
}
