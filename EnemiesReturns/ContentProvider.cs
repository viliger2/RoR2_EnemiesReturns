using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using R2API;
using Rewired.Utils.Classes.Utility;
using Rewired.Utils.Interfaces;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.DirectorAPI;

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

            Texture2D texLavaCrackRound = null;
            yield return LoadAllAssetsAsync(assetbundle, args.progressReceiver, (Action<Texture2D[]>)((textures) =>
            {
                texLavaCrackRound = textures.First(texture => texture.name == "texLavaCrackRound");
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

                stateList.Add(typeof(ModdedEntityStates.BaseEmoteState)); // dunno if I need it, but just in case

                CreateSpitter(assets, iconLookup);

                CreateColossus(assets, iconLookup, acdLookup);

                CreateIfrit(assets, iconLookup, texLavaCrackRound, acdLookup);

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

            totalStopwatch.Stop();
            Log.Info("Total loading time: " + totalStopwatch.elapsedSeconds);

            yield break;
        }

        #region Content
        private void CreateIfrit(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Texture2D texLavaCrackRound, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            CreateIfritPillar(assets, iconLookup, acdLookup);
            ExplicitPickupDropTable dtIfrit = CreateIfritItem(assets, iconLookup);

            if (EnemiesReturnsConfiguration.Ifrit.Enabled.Value)
            {

                var ifritFactory = new IfritFactory();
                var ifritManePrefab = assets.First(mane => mane.name == "IfritManeFireParticle");
                ifritManePrefab.GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matIfritManeFire", ifritFactory.CreateManeFiresMaterial);

                ModdedEntityStates.Ifrit.SummonPylon.screamPrefab = ifritFactory.CreateBreathParticle();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.SummonPylon.screamPrefab));

                var nsedHellzoneRockFire = Utils.CreateNetworkSoundDef("ER_Ifrit_Hellzone_Rock_Play");
                nseList.Add(nsedHellzoneRockFire);

                var ifritHellzonePillarProjectile = assets.First(projectile => projectile.name == "IfritHellzonePillarProjectile");
                var ifritHellzonePillarProjectileGhost = assets.First(projectile => projectile.name == "IfritHellzonePillarProjectileGhost");
                var pillarProjectile = ifritFactory.CreateHellzonePillarProjectile(ifritHellzonePillarProjectile, ifritHellzonePillarProjectileGhost);
                var dotZoneProjectile = ifritFactory.CreateHellfireDotZoneProjectile(pillarProjectile, texLavaCrackRound, nsedHellzoneRockFire);
                var hellzoneProjectile = ifritFactory.CreateHellzoneProjectile(dotZoneProjectile);

                projectilesList.Add(dotZoneProjectile);
                projectilesList.Add(hellzoneProjectile);
                projectilesList.Add(pillarProjectile);

                ModdedEntityStates.Ifrit.FlameCharge.FlameCharge.flamethrowerEffectPrefab = ifritFactory.CreateFlameBreath();
                ModdedEntityStates.Ifrit.Hellzone.FireHellzoneFire.projectilePrefab = hellzoneProjectile;

                IfritFactory.Skills.Hellzone = ifritFactory.CreateHellzoneSkill();
                IfritFactory.SkillFamilies.Secondary = Utils.CreateSkillFamily("IfritSecondaryFamily", IfritFactory.Skills.Hellzone);

                IfritFactory.Skills.SummonPylon = ifritFactory.CreateSummonPylonSkill();
                IfritFactory.SkillFamilies.Special = Utils.CreateSkillFamily("IfritSpecialFamily", IfritFactory.Skills.SummonPylon);

                IfritFactory.Skills.FlameCharge = ifritFactory.CreateFlameChargeSkill();
                IfritFactory.SkillFamilies.Utility = Utils.CreateSkillFamily("IfritUtilityFamily", IfritFactory.Skills.FlameCharge);

                var ifritLog = Utils.CreateUnlockableDef("Logs.IfritBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_IFRIT");
                unlockablesList.Add(ifritLog);

                var ifritBody = assets.First(body => body.name == "IfritBody");
                IfritFactory.IfritBody = ifritFactory.CreateBody(ifritBody, iconLookup["texIconIfritBody"], ifritLog, dtIfrit); // TODO: sprite
                bodyList.Add(IfritFactory.IfritBody);

                var ifritMaster = assets.First(master => master.name == "IfritMaster");
                IfritFactory.IfritMaster = ifritFactory.CreateMaster(ifritMaster, IfritFactory.IfritBody);
                masterList.Add(IfritFactory.IfritMaster);

                IfritFactory.SpawnCards.cscIfritDefault = ifritFactory.CreateCard("cscIfritDefault", ifritMaster, IfritFactory.SkinDefs.Default, ifritBody);
                var dcIfritDefault = new DirectorCard
                {
                    spawnCard = IfritFactory.SpawnCards.cscIfritDefault,
                    selectionWeight = EnemiesReturnsConfiguration.Ifrit.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Ifrit.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchIfritDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcIfritDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.AbyssalDepths);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.AbyssalDepthsSimulacrum);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.SiphonedForest);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.RallypointDelta);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.RallypointDeltaSimulacrum);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.HelminthHatchery);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.Custom, "catacombs_DS1_Catacombs");

                DirectorAPI.Helpers.AddNewMonsterToStage(dchIfritDefault, false, DirectorAPI.Stage.VoidCell);

                stateList.Add(typeof(ModdedEntityStates.Ifrit.SpawnState));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.DeathState));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.SummonPylon));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneStart));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneFire));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneEnd));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameChargeBegin));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameCharge));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameChargeEnd));
            }
        }

        private ExplicitPickupDropTable CreateIfritItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtIfrit = null;
            if (EnemiesReturnsConfiguration.Ifrit.ItemEnabled.Value)
            {
                var pillarItemFactory = new SpawnPillarOnChampionKillFactory();

                SpawnPillarOnChampionKillFactory.itemDef = pillarItemFactory.CreateItem(assets.First(item => item.name == "IfritItem"), iconLookup["texIconIfritItem"]);
                itemList.Add(SpawnPillarOnChampionKillFactory.itemDef);

                dtIfrit = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtIfrit as ScriptableObject).name = "epdtIfrit";
                dtIfrit.canDropBeReplaced = true;
                dtIfrit.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = SpawnPillarOnChampionKillFactory.itemDef
                        }
                };
            }

            return dtIfrit;
        }

        private void CreateIfritPillar(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            if (EnemiesReturnsConfiguration.Ifrit.ItemEnabled.Value || EnemiesReturnsConfiguration.Ifrit.Enabled.Value)
            {
                var ifritPylonFactory = new IfritPillarFactory();

                ModdedEntityStates.Ifrit.Pillar.SpawnState.burrowPrefab = ifritPylonFactory.CreateSpawnEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.SpawnState.burrowPrefab));

                ModdedEntityStates.Ifrit.Pillar.DeathState.fallEffect = ifritPylonFactory.CreateDeathFallEffect();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.DeathState.fallEffect));

                ModdedEntityStates.Ifrit.Pillar.BaseFireExplosion.explosionPrefab = ifritPylonFactory.CreateExlosionEffectAlt();
                effectsList.Add(new EffectDef(ModdedEntityStates.Ifrit.Pillar.BaseFireExplosion.explosionPrefab));

                var pylonBody = assets.First(body => body.name == "IfritPylonBody");
                var pylonMaster = assets.First(master => master.name == "IfritPylonMaster");

                if (EnemiesReturnsConfiguration.Ifrit.Enabled.Value)
                {
                    var pillarEnemyBodyInformation = new IfritPillarFactory.BodyInformation
                    {
                        bodyPrefab = pylonBody.InstantiateClone("IfritPylonEnemyBody", false),
                        sprite = iconLookup["texIconPillarEnemy"],
                        baseDamage = EnemiesReturnsConfiguration.Ifrit.BaseDamage.Value,
                        levelDamage = EnemiesReturnsConfiguration.Ifrit.LevelDamage.Value,
                        mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.ChargingExplosion)),
                        enableLineRenderer = true,
                        explosionRadius = EnemiesReturnsConfiguration.Ifrit.PillarExplosionRadius.Value
                    };

                    IfritPillarFactory.Enemy.IfritPillarBody = ifritPylonFactory.CreateBody(pillarEnemyBodyInformation, acdLookup);
                    bodyList.Add(IfritPillarFactory.Enemy.IfritPillarBody);

                    IfritPillarFactory.Enemy.IfritPillarMaster = ifritPylonFactory.CreateMaster(pylonMaster.InstantiateClone("IfritPylonEnemyMaster", false), IfritPillarFactory.Enemy.IfritPillarBody);
                    masterList.Add(IfritPillarFactory.Enemy.IfritPillarMaster);

                    IfritPillarFactory.Enemy.scIfritPillar = ifritPylonFactory.CreateCard("cscIfritEnemyPillar", IfritPillarFactory.Enemy.IfritPillarMaster);

                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.ChargingExplosion));
                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.FireExplosion));
                }

                if (EnemiesReturnsConfiguration.Ifrit.ItemEnabled.Value)
                {
                    var pillarPlayerBodyInformation = new IfritPillarFactory.BodyInformation
                    {
                        bodyPrefab = pylonBody.InstantiateClone("IfritPylonPlayerBody", false),
                        sprite = iconLookup["texIconPillarAlly"],
                        baseDamage = EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillBodyBaseDamage.Value,
                        levelDamage = EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillBodyLevelDamage.Value,
                        mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.Player.ChargingExplosion)),
                        enableLineRenderer = false,
                        explosionRadius = EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillRadius.Value
                    };

                    IfritPillarFactory.Player.IfritPillarBody = ifritPylonFactory.CreateBody(pillarPlayerBodyInformation, acdLookup);
                    bodyList.Add(IfritPillarFactory.Player.IfritPillarBody);

                    IfritPillarFactory.Player.IfritPillarMaster = ifritPylonFactory.CreateMaster(pylonMaster.InstantiateClone("IfritPylonPlayerMaster", false), IfritPillarFactory.Player.IfritPillarBody);
                    masterList.Add(IfritPillarFactory.Player.IfritPillarMaster);

                    IfritPillarFactory.Player.scIfritPillar = ifritPylonFactory.CreateCard("cscIfritPlayerPillar", IfritPillarFactory.Player.IfritPillarMaster);
                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Player.ChargingExplosion));
                    stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.Player.FireExplosion));
                }

                stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.SpawnState));
                stateList.Add(typeof(ModdedEntityStates.Ifrit.Pillar.DeathState));
            }
        }

        private void CreateColossus(GameObject[] assets, Dictionary<string, Sprite> iconLookup, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            ExplicitPickupDropTable dtColossus = CreateColossusItem(assets, iconLookup);

            if (EnemiesReturnsConfiguration.Colossus.Enabled.Value)
            {
                var colossusFactory = new ColossusFactory();

                var stompEffect = colossusFactory.CreateStompEffect();
                ModdedEntityStates.Colossus.Stomp.StompBase.stompEffectPrefab = stompEffect;
                effectsList.Add(new EffectDef(stompEffect));

                var stompProjectile = colossusFactory.CreateStompProjectile();
                ModdedEntityStates.Colossus.Stomp.StompBase.projectilePrefab = stompProjectile;
                projectilesList.Add(stompProjectile);

                var deathFallEffect = colossusFactory.CreateDeathFallEffect();
                effectsList.Add(new EffectDef(deathFallEffect));
                ModdedEntityStates.Colossus.Death.Death2.fallEffect = deathFallEffect;

                var death2Effect = colossusFactory.CreateDeath2Effect();
                effectsList.Add(new EffectDef(death2Effect));
                ModdedEntityStates.Colossus.Death.Death2.deathEffect = death2Effect;

                var clapEffect = colossusFactory.CreateClapEffect();
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.clapEffect = clapEffect;

                var flyingRockGhost = colossusFactory.CreateFlyingRocksGhost();
                Enemies.Colossus.FloatingRocksController.flyingRockPrefab = flyingRockGhost;
                var flyingRockProjectile = colossusFactory.CreateFlyingRockProjectile(flyingRockGhost);
                ModdedEntityStates.Colossus.RockClap.RockClapEnd.projectilePrefab = flyingRockProjectile;
                projectilesList.Add(flyingRockProjectile);

                var laserBarrageProjectile = colossusFactory.CreateLaserBarrageProjectile();
                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.projectilePrefab = laserBarrageProjectile;
                projectilesList.Add(laserBarrageProjectile);

                var spawnEffect = colossusFactory.CreateSpawnEffect();
                ModdedEntityStates.Colossus.SpawnState.burrowPrefab = spawnEffect;
                effectsList.Add(new EffectDef(spawnEffect));

                ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageAttack.intencityGraph = acdLookup["LaserBarrageLightIntencity"].curve;

                var colossusLog = Utils.CreateUnlockableDef("Logs.ColossusBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_COLOSSUS");
                unlockablesList.Add(colossusLog);

                var laserEffect = colossusFactory.CreateLaserEffect();
                Junk.ModdedEntityStates.Colossus.HeadLaser.HeadLaserAttack.beamPrefab = laserEffect;

                ColossusFactory.Skills.Stomp = colossusFactory.CreateStompSkill();
                ColossusFactory.Skills.StoneClap = colossusFactory.CreateStoneClapSkill();
                ColossusFactory.Skills.LaserBarrage = colossusFactory.CreateLaserBarrageSkill();
                ColossusFactory.Skills.HeadLaser = colossusFactory.CreateHeadLaserSkill();
                sdList.Add(ColossusFactory.Skills.Stomp);
                sdList.Add(ColossusFactory.Skills.StoneClap);
                sdList.Add(ColossusFactory.Skills.LaserBarrage);
                sdList.Add(ColossusFactory.Skills.HeadLaser);

                ColossusFactory.SkillFamilies.Primary = Utils.CreateSkillFamily("ColossusPrimaryFamily", ColossusFactory.Skills.Stomp);
                ColossusFactory.SkillFamilies.Secondary = Utils.CreateSkillFamily("ColossusSecondaryFamily", ColossusFactory.Skills.StoneClap);
                ColossusFactory.SkillFamilies.Utility = Utils.CreateSkillFamily("ColossusUtilityFamily", ColossusFactory.Skills.LaserBarrage);
                ColossusFactory.SkillFamilies.Special = Utils.CreateSkillFamily("ColossusSpecialFamily", ColossusFactory.Skills.HeadLaser);
                sfList.Add(ColossusFactory.SkillFamilies.Primary);
                sfList.Add(ColossusFactory.SkillFamilies.Secondary);
                sfList.Add(ColossusFactory.SkillFamilies.Utility);
                sfList.Add(ColossusFactory.SkillFamilies.Special);

                var colossusBody = assets.First(body => body.name == "ColossusBody");
                ColossusFactory.ColossusBody = colossusFactory.CreateBody(colossusBody, iconLookup["texColossusIcon"], colossusLog, dtColossus);
                bodyList.Add(ColossusFactory.ColossusBody);

                var colossusMaster = assets.First(master => master.name == "ColossusMaster");
                ColossusFactory.ColossusMaster = colossusFactory.CreateMaster(colossusMaster, colossusBody);
                masterList.Add(ColossusFactory.ColossusMaster);

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

                Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusDefault = colossusFactory.CreateCard("cscColossusDefault", colossusMaster, ColossusFactory.SkinDefs.Default, colossusBody);
                DirectorCard dcColossusDefault = new DirectorCard
                {
                    spawnCard = ColossusFactory.SpawnCards.cscColossusDefault,
                    selectionWeight = EnemiesReturnsConfiguration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Colossus.MinimumStageCompletion.Value
                };
                Utils.AddMonsterToCardCategory(dcColossusDefault, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion());

                Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSkyMeadow = colossusFactory.CreateCard("cscColossusSkyMeadow", colossusMaster, ColossusFactory.SkinDefs.SkyMeadow, colossusBody);
                DirectorCard dcColossusSkyMeadow = new DirectorCard
                {
                    spawnCard = ColossusFactory.SpawnCards.cscColossusSkyMeadow,
                    selectionWeight = EnemiesReturnsConfiguration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusSkyMeadow = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSkyMeadow,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusSkyMeadow, false, DirectorAPI.Stage.SkyMeadow);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusSkyMeadow, false, DirectorAPI.Stage.SkyMeadowSimulacrum);

                Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusGrassy = colossusFactory.CreateCard("cscColossusGrassy", colossusMaster, ColossusFactory.SkinDefs.Grassy, colossusBody);
                DirectorCard dcColossusGrassy = new DirectorCard
                {
                    spawnCard = ColossusFactory.SpawnCards.cscColossusGrassy,
                    selectionWeight = EnemiesReturnsConfiguration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusGrassy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusGrassy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                Utils.AddMonsterToCardCategory(dcColossusGrassy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilyNature").WaitForCompletion());
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusGrassy, false, DirectorAPI.Stage.TitanicPlains);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusGrassy, false, DirectorAPI.Stage.TitanicPlainsSimulacrum);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusGrassy, false, DirectorAPI.Stage.VoidCell);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusGrassy, false, DirectorAPI.Stage.AbyssalDepths);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusGrassy, false, DirectorAPI.Stage.AbyssalDepthsSimulacrum);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusGrassy, false, DirectorAPI.Stage.Custom, "FBLScene");

                Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusCastle = colossusFactory.CreateCard("cscColossusCastle", colossusMaster, ColossusFactory.SkinDefs.Castle, colossusBody);
                DirectorCard dcColossusCastle = new DirectorCard
                {
                    spawnCard = ColossusFactory.SpawnCards.cscColossusCastle,
                    selectionWeight = EnemiesReturnsConfiguration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Colossus.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchColossusCastle = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusCastle,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusCastle, false, DirectorAPI.Stage.Custom, "sm64_bbf_SM64_BBF");

                Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSandy = colossusFactory.CreateCard("cscColossusSandy", colossusMaster, ColossusFactory.SkinDefs.Sandy, colossusBody);
                DirectorCard dcColossusSandy = new DirectorCard
                {
                    spawnCard = ColossusFactory.SpawnCards.cscColossusSandy,
                    selectionWeight = EnemiesReturnsConfiguration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Colossus.MinimumStageCompletion.Value
                };
                Utils.AddMonsterToCardCategory(dcColossusSandy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySandy.asset").WaitForCompletion());
                DirectorAPI.DirectorCardHolder dchColossusSandy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSandy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusSandy, false, DirectorAPI.Stage.AbandonedAqueduct);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusSandy, false, DirectorAPI.Stage.AbandonedAqueductSimulacrum);

                Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSnowy = colossusFactory.CreateCard("cscColossusSnowy", colossusMaster, ColossusFactory.SkinDefs.Snowy, colossusBody);
                DirectorCard dcColossusSnowy = new DirectorCard
                {
                    spawnCard = ColossusFactory.SpawnCards.cscColossusSnowy,
                    selectionWeight = EnemiesReturnsConfiguration.Colossus.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Colossus.MinimumStageCompletion.Value
                };
                Utils.AddMonsterToCardCategory(dcColossusSnowy, MonsterCategories.Champions, Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamilySnowy.asset").WaitForCompletion());
                DirectorAPI.DirectorCardHolder dchColossusSnowy = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcColossusSnowy,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusSnowy, false, DirectorAPI.Stage.SiphonedForest);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusSnowy, false, DirectorAPI.Stage.RallypointDelta);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchColossusSnowy, false, DirectorAPI.Stage.RallypointDeltaSimulacrum);
            }
        }

        private ExplicitPickupDropTable CreateColossusItem(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            ExplicitPickupDropTable dtColossus = null;
            if (EnemiesReturnsConfiguration.Colossus.ItemEnabled.Value)
            {
                var knurlFactory = new ColossalKnurlFactory();

                ColossalKnurlFactory.itemDef = knurlFactory.CreateItem(assets.First(item => item.name == "PickupColossalCurl"), iconLookup["texColossalKnurlIcon"]);
                itemList.Add(ColossalKnurlFactory.itemDef);

                dtColossus = ScriptableObject.CreateInstance<ExplicitPickupDropTable>();
                (dtColossus as ScriptableObject).name = "epdtColossus";
                dtColossus.canDropBeReplaced = true;
                dtColossus.pickupEntries = new ExplicitPickupDropTable.PickupDefEntry[]
                {
                        new ExplicitPickupDropTable.PickupDefEntry
                        {
                            pickupWeight = 1,
                            pickupDef = ColossalKnurlFactory.itemDef
                        }
                };

                var knurlProjectileGhost = assets.First(item => item.name == "ColossalKnurlFistProjectileGhost");
                knurlProjectileGhost = knurlFactory.CreateFistGhostPrefab(knurlProjectileGhost);

                var knurlProjectile = assets.First(item => item.name == "ColossalKnurlFistProjectile");
                ColossalKnurlFactory.projectilePrefab = knurlFactory.CreateFistProjectile(knurlProjectile, knurlProjectileGhost);

                projectilesList.Add(ColossalKnurlFactory.projectilePrefab);
            }

            return dtColossus;
        }

        private void CreateSpitter(GameObject[] assets, Dictionary<string, Sprite> iconLookup)
        {
            if (EnemiesReturnsConfiguration.Spitter.Enabled.Value)
            {
                // TODO: bite effect broke
                var spitterFactory = new SpitterFactory();

                var biteEffectPrefab = spitterFactory.CreateBiteEffect();
                ModdedEntityStates.Spitter.Bite.biteEffectPrefab = biteEffectPrefab;
                effectsList.Add(new EffectDef(biteEffectPrefab));

                var chargedSpitSmallDoTZone = spitterFactory.CreatedChargedSpitSmallDoTZone();
                var chargedSpitDoTZone = spitterFactory.CreateChargedSpitDoTZone();
                var chargedSpitChunkProjectile = spitterFactory.CreateChargedSpitSplitProjectile(chargedSpitSmallDoTZone);
                var chargedSpitProjectile = spitterFactory.CreateChargedSpitProjectile(chargedSpitDoTZone, chargedSpitChunkProjectile); ;
                ModdedEntityStates.Spitter.FireChargedSpit.projectilePrefab = chargedSpitProjectile;
                projectilesList.Add(chargedSpitProjectile);
                projectilesList.Add(chargedSpitSmallDoTZone);
                projectilesList.Add(chargedSpitDoTZone);
                projectilesList.Add(chargedSpitChunkProjectile);

                Junk.ModdedEntityStates.Spitter.NormalSpit.normalSpitProjectile = spitterFactory.CreateNormalSpitProjectile();
                projectilesList.Add(Junk.ModdedEntityStates.Spitter.NormalSpit.normalSpitProjectile);

                var spitterLog = Utils.CreateUnlockableDef("Logs.SpitterBody.0", "ENEMIES_RETURNS_UNLOCKABLE_LOG_SPITTER");
                unlockablesList.Add(spitterLog);

                SpitterFactory.Skills.NormalSpit = spitterFactory.CreateNormalSpitSkill();
                SpitterFactory.Skills.Bite = spitterFactory.CreateBiteSkill();
                SpitterFactory.Skills.ChargedSpit = spitterFactory.CreateChargedSpitSkill();

                sdList.Add(SpitterFactory.Skills.NormalSpit);
                sdList.Add(SpitterFactory.Skills.Bite);
                sdList.Add(SpitterFactory.Skills.ChargedSpit);

                SpitterFactory.SkillFamilies.Primary = Utils.CreateSkillFamily("SpitterPrimaryFamily", SpitterFactory.Skills.NormalSpit);
                SpitterFactory.SkillFamilies.Secondary = Utils.CreateSkillFamily("SpitterSecondaryFamily", SpitterFactory.Skills.Bite);
                SpitterFactory.SkillFamilies.Special = Utils.CreateSkillFamily("SpitterSpecialFamily", SpitterFactory.Skills.ChargedSpit);

                sfList.Add(SpitterFactory.SkillFamilies.Primary);
                sfList.Add(SpitterFactory.SkillFamilies.Secondary);
                sfList.Add(SpitterFactory.SkillFamilies.Special);

                var spitterBody = assets.First(body => body.name == "SpitterBody");
                SpitterFactory.SpitterBody = spitterFactory.CreateBody(spitterBody, iconLookup["texSpitterIcon"], spitterLog);
                bodyList.Add(SpitterFactory.SpitterBody);

                var spitterMaster = assets.First(master => master.name == "SpitterMaster");
                SpitterFactory.SpitterMaster = spitterFactory.CreateMaster(spitterMaster, spitterBody);
                masterList.Add(SpitterFactory.SpitterMaster);

                SpitterFactory.SpawnCards.cscSpitterDefault = spitterFactory.CreateCard("cscSpitterDefault", spitterMaster, SpitterFactory.SkinDefs.Default, spitterBody);
                var dcSpitterDefault = new DirectorCard
                {
                    spawnCard = SpitterFactory.SpawnCards.cscSpitterDefault,
                    selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterDefault = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDefault,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.WetlandAspect);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.Custom, "FBLScene");
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.VoidCell);
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterDefault, false, DirectorAPI.Stage.ArtifactReliquary);

                SpitterFactory.SpawnCards.cscSpitterLakes = spitterFactory.CreateCard("cscSpitterLakes", spitterMaster, SpitterFactory.SkinDefs.Lakes, spitterBody);
                var dcSpitterLakes = new DirectorCard
                {
                    spawnCard = SpitterFactory.SpawnCards.cscSpitterLakes,
                    selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dchSpitterLakes = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterLakes,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dchSpitterLakes, false, DirectorAPI.Stage.VerdantFalls);

                SpitterFactory.SpawnCards.cscSpitterSulfur = spitterFactory.CreateCard("cscSpitterSulfur", spitterMaster, SpitterFactory.SkinDefs.Sulfur, spitterBody);
                var dcSpitterSulfur = new DirectorCard
                {
                    spawnCard = SpitterFactory.SpawnCards.cscSpitterSulfur,
                    selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dhcSpitterSulfur = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterSulfur,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dhcSpitterSulfur, false, DirectorAPI.Stage.SulfurPools);

                SpitterFactory.SpawnCards.cscSpitterDepths = spitterFactory.CreateCard("cscSpitterDepths", spitterMaster, SpitterFactory.SkinDefs.Depths, spitterBody);
                var dcSpitterDepth = new DirectorCard
                {
                    spawnCard = SpitterFactory.SpawnCards.cscSpitterDepths,
                    selectionWeight = EnemiesReturnsConfiguration.Spitter.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = true,
                    minimumStageCompletions = EnemiesReturnsConfiguration.Spitter.MinimumStageCompletion.Value
                };
                DirectorAPI.DirectorCardHolder dhcSpitterDepths = new DirectorAPI.DirectorCardHolder
                {
                    Card = dcSpitterDepth,
                    MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                };
                DirectorAPI.Helpers.AddNewMonsterToStage(dhcSpitterDepths, false, DirectorAPI.Stage.AbyssalDepths);
                DirectorAPI.Helpers.AddNewMonsterToStage(dhcSpitterDepths, false, DirectorAPI.Stage.AbyssalDepthsSimulacrum);

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
