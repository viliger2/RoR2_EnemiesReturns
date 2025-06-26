using EnemiesReturns.Behaviors;
using R2API;
using RoR2;
using RoR2.Hologram;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe
{
    public class LynxTribeStuff
    {
        public static GameObject CustomHologramContent;

        public FamilyDirectorCardCategorySelection CreateLynxTribeFamily()
        {
            var dccsFamily = ScriptableObject.CreateInstance<FamilyDirectorCardCategorySelection>();
            (dccsFamily as ScriptableObject).name = "dccsLynxTribeFamily";

            dccsFamily.selectionChatString = "ENEMIES_RETURNS_FAMILY_LYNX_TRIBE";
            dccsFamily.minimumStageCompletion = 1;
            dccsFamily.maximumStageCompletion = int.MaxValue;

            var champions = new FamilyDirectorCardCategorySelection.Category
            {
                name = ContentProvider.MonsterCategories.Champions,
                selectionWeight = 2f,
                cards = new DirectorCard[]
                {
                    new DirectorCard
                    {
                        spawnCard = Enemies.LynxTribe.Totem.TotemBody.SpawnCards.cscLynxTotemDefault,
                        selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxTotem.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = false
                    }
                }
            };

            var basicMonsters = new FamilyDirectorCardCategorySelection.Category
            {
                name = ContentProvider.MonsterCategories.BasicMonsters,
                selectionWeight = 4f,
                cards = new DirectorCard[]
                {
                    new DirectorCard
                    {
                        spawnCard = Enemies.LynxTribe.Scout.ScoutBody.SpawnCards.cscLynxScoutDefault,
                        selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxScout.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = false
                    },
                    new DirectorCard
                    {
                        spawnCard = Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault,
                        selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxHunter.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                        preventOverhead = false
                    },
                    new DirectorCard
                    {
                        spawnCard = Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault,
                        selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxArcher.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Far,
                        preventOverhead = false
                    }
                }
            };

            // adding shaman separately since he can spawn on his own and has his own enabled flag
            if (EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                HG.ArrayUtils.ArrayAppend(ref basicMonsters.cards, new DirectorCard
                {
                    spawnCard = Enemies.LynxTribe.Shaman.ShamanBody.SpawnCards.cscLynxShamanDefault,
                    selectionWeight = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SelectionWeight.Value,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                    preventOverhead = false
                }
                );
            }

            dccsFamily.categories = new DirectorCardCategorySelection.Category[]
            {
                champions,
                basicMonsters
            };

            return dccsFamily;
        }

        public GameObject CreateRetreatEffect(GameObject prefab)
        {
            var effect = prefab.AddComponent<EffectComponent>();
            effect.soundName = "ER_Lynx_Escape_Effect_Play";

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            prefab.AddComponent<DestroyOnParticleEnd>();

            prefab.transform.Find("ParticleLoop/Sparks").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat").WaitForCompletion();
            prefab.transform.Find("ParticleLoop/Debris, 3D").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/Props/matDetailRock.mat").WaitForCompletion();
            prefab.transform.Find("ParticleLoop/Magma, Billboard").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLarge.mat").WaitForCompletion();
            prefab.transform.Find("ParticleLoop/Dust, Billboard").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLarge.mat").WaitForCompletion();

            return prefab;
        }

        public GameObject CustomCostHologramContentPrefab()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/CostHologramContent.prefab").WaitForCompletion().InstantiateClone("CustomCostHologramContentPrefab", false);

            UnityEngine.Object.DestroyImmediate(prefab.GetComponent<CostHologramContent>());
            var hologramContent = prefab.AddComponent<CustomCostHologramContent>();
            hologramContent.targetTextMesh = prefab.transform.Find("Text").GetComponent<TextMeshPro>();
            hologramContent.targetTextMesh.fontSize = 10f;

            return prefab;
        }

        public GameObject CreateTrapPrefab(GameObject trapPrefab, GameObject leavesPrefab, Material material)
        {
            trapPrefab.AddComponent<NetworkIdentity>();

            var disabler = trapPrefab.AddComponent<DestroyOnTimer>();
            disabler.duration = 15f;
            disabler.enabled = false;

            var spawner = trapPrefab.AddComponent<LynxTribeSpawner>();
            spawner.eliteBias = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapEliteBias.Value;
            spawner.spawnCards = new SpawnCard[]
            {
                Enemies.LynxTribe.Scout.ScoutBody.SpawnCards.cscLynxScoutDefault,
                Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault,
                Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault,
            };
            if (EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
            {
                HG.ArrayUtils.ArrayAppend(ref spawner.spawnCards, Enemies.LynxTribe.Shaman.ShamanBody.SpawnCards.cscLynxShamanDefault);
            }
            spawner.minSpawnCount = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapMinSpawnCount.Value;
            spawner.maxSpawnCount = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapMaxSpawnCount.Value;
            spawner.assignRewards = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapAssignRewards.Value;
            spawner.spawnDistance = 5f;
            spawner.retrySpawnCount = 3;
            spawner.teamIndex = TeamIndex.Monster;

            var trap = trapPrefab.AddComponent<LynxTribeTrap>();
            trap.checkInterval = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapCheckInterval.Value;
            trap.spawnAfterTriggerInterval = 0.5f;
            trap.spawner = spawner;
            trap.destroyOnTimer = disabler;
            trap.hitBox = trapPrefab.transform.Find("Hitbox");
            var teamMask = new TeamMask();
            teamMask.AddTeam(TeamIndex.Player);
            trap.teamFilter = teamMask;
            trap.initialTriggerSound = "ER_LynxTrap_SnapTwig_Play";

            var leavesTransform = trapPrefab.transform.Find("Leaves").transform;
            foreach (Transform child in leavesTransform)
            {
                var leavesClone = UnityEngine.Object.Instantiate(leavesPrefab);
                leavesClone.transform.parent = child;
                leavesClone.transform.localPosition = Vector3.zero;
                leavesClone.transform.localRotation = Quaternion.identity;
                leavesClone.transform.Find("LTFallenLeaf_LOD0").GetComponent<MeshRenderer>().material = material;
                leavesClone.transform.Find("LTFallenLeaf_LOD1").GetComponent<MeshRenderer>().material = material;
            }
            trap.leaves = leavesTransform;

            var branchMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/foggyswamp/matFSRootBundle.mat").WaitForCompletion();
            var branchTransform = trapPrefab.transform.Find("Branches");
            foreach (var renderer in branchTransform.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = branchMaterial;
            }
            trap.branches = branchTransform;

            trapPrefab.RegisterNetworkPrefab();

            return trapPrefab;
        }

        public InteractableSpawnCard CreateLynxTrapSpawnCard(GameObject lynxTrap, string stageName)
        {
            var spawnCardTrap = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            (spawnCardTrap as ScriptableObject).name = "iscLynxTrap" + stageName;
            spawnCardTrap.prefab = lynxTrap;
            spawnCardTrap.sendOverNetwork = true;
            spawnCardTrap.hullSize = HullClassification.Human;
            spawnCardTrap.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCardTrap.requiredFlags = RoR2.Navigation.NodeFlags.None;
            spawnCardTrap.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn | RoR2.Navigation.NodeFlags.NoShrineSpawn;
            spawnCardTrap.directorCreditCost = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapDirectorCost.Value;
            spawnCardTrap.occupyPosition = true;
            spawnCardTrap.eliteRules = SpawnCard.EliteRules.Default;
            spawnCardTrap.orientToFloor = true;
            spawnCardTrap.maxSpawnsPerStage = EnemiesReturns.Configuration.LynxTribe.LynxStuff.LynxTrapMaxSpawnPerStage.Value;
            return spawnCardTrap;
        }

        public GameObject CreateShrinePrefab(GameObject shrinePrefab, GameObject shrineEffect, NetworkSoundEventDef nsedFailure, NetworkSoundEventDef nsedSuccess)
        {
            shrinePrefab.AddComponent<NetworkIdentity>();

            var hightLightMesh = shrinePrefab.AddComponent<Highlight>();
            hightLightMesh.strength = 1f;
            hightLightMesh.targetRenderer = shrinePrefab.transform.Find("Base/LynxTotemPole/Pole").GetComponent<Renderer>();
            hightLightMesh.highlightColor = Highlight.HighlightColor.interactive;

            var modelLocator = shrinePrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = shrinePrefab.transform.Find("Base/LynxTotemPole");
            modelLocator.modelBaseTransform = shrinePrefab.transform.Find("Base");
            modelLocator.autoUpdateModelTransform = true;
            modelLocator.dontDetatchFromParent = false;
            modelLocator.noCorpse = false;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.preserveModel = false;
            modelLocator.normalizeToFloor = false;

            var inspectDef = ScriptableObject.CreateInstance<InspectDef>();
            (inspectDef as ScriptableObject).name = "idLynxShrine";
            inspectDef.Info = new RoR2.UI.InspectInfo
            {
                Visual = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texShrineIconOutlined.png").WaitForCompletion(),
                TitleToken = "ENEMIES_RETURNS_LYNX_SHRINE_NAME",
                DescriptionToken = "ENEMIES_RETURNS_LYNX_SHRINE_DESCRIPTION",
                FlavorToken = "ENEMIES_RETURNS_LYNX_SHRINE_LORE",
                TitleColor = Color.white,
                isConsumedItem = false
            };
            shrinePrefab.AddComponent<GenericInspectInfoProvider>().InspectInfo = inspectDef;

            var genericNameInfoProvider = shrinePrefab.AddComponent<GenericDisplayNameProvider>();
            genericNameInfoProvider.displayToken = "ENEMIES_RETURNS_LYNX_SHRINE_NAME";

            var pingInfoProvider = shrinePrefab.AddComponent<PingInfoProvider>();
            pingInfoProvider.pingIconOverride = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texShrineIconOutlined.png").WaitForCompletion();

            var combatSquad = shrinePrefab.AddComponent<CombatSquad>();
            combatSquad.grantBonusHealthInMultiplayer = Configuration.LynxTribe.LynxStuff.LynxShrineMultiplayerScaling.Value;

            var spawner = shrinePrefab.AddComponent<LynxTribeSpawner>();
            spawner.spawnCards = new SpawnCard[]
            {
                Enemies.LynxTribe.Scout.ScoutBody.SpawnCards.cscLynxScoutDefault,
                Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault,
                Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault,
            };

            spawner.assignRewards = false;
            spawner.spawnDistance = 5f;
            spawner.retrySpawnCount = 3;
            spawner.teamIndex = TeamIndex.Monster;
            spawner.combatSquad = combatSquad;
            spawner.isBoss = true;

            var shrine = shrinePrefab.AddComponent<LynxTribeShrine>();
            shrine.dropTable = CreateLynxShrineDropTable();
            shrine.localEjectionVelocity = new Vector3(0f, 15f, 8f);
            shrine.spawner = spawner;
            shrine.escapeDuration = Configuration.LynxTribe.LynxStuff.LynxShrineEscapeTimer.Value;
            shrine.shrineUseEffect = shrineEffect;
            shrine.failureSound = nsedFailure;
            shrine.successSound = nsedSuccess;

            var cubeObject = shrinePrefab.transform.Find("Base/LynxTotemPole").gameObject;
            cubeObject.AddComponent<EntityLocator>().entity = shrinePrefab;

            var baseObject = shrinePrefab.transform.Find("Base").gameObject;
            baseObject.AddComponent<EntityLocator>().entity = shrinePrefab;

            var pickupDisplayObject = shrinePrefab.transform.Find("Base/PickupDisplay").gameObject;

            var highlightPickup = pickupDisplayObject.AddComponent<Highlight>();
            highlightPickup.strength = 1f;
            highlightPickup.highlightColor = Highlight.HighlightColor.pickup;

            var pickupDisplay = pickupDisplayObject.AddComponent<PickupDisplay>();
            pickupDisplay.verticalWave = new Wave()
            {
                amplitude = 0.2f,
                frequency = 0.25f,
                cycleOffset = 0f
            };
            pickupDisplay.dontInstantiatePickupModel = false;
            pickupDisplay.spinSpeed = 75f;
            pickupDisplay.highlight = highlightPickup;

            shrine.pickupDisplay = pickupDisplay;

            var hologramProjector = shrinePrefab.AddComponent<HologramProjector>();
            hologramProjector.displayDistance = Configuration.LynxTribe.LynxStuff.LynxShrineTimerDisplayDistance.Value;
            hologramProjector.hologramPivot = shrinePrefab.transform.Find("Base/Hologram");

            var dither = shrinePrefab.AddComponent<DitherModel>();
            dither.bounds = shrinePrefab.transform.Find("Base/LynxTotemPole").GetComponent<Collider>();
            dither.renderers = shrinePrefab.GetComponentsInChildren<Renderer>();

            shrinePrefab.RegisterNetworkPrefab();

            return shrinePrefab;
        }

        public GameObject CreateShrineUseEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/ShrineUseEffect.prefab").WaitForCompletion().InstantiateClone("LynxShrineUseEffect", false);
            prefab.GetComponent<EffectComponent>().soundName = "ER_Lynx_Shrine_Use_Play";

            return prefab;
        }

        private BasicPickupDropTable CreateLynxShrineDropTable()
        {
            var dropTable = ScriptableObject.CreateInstance<BasicPickupDropTable>();
            dropTable.tier1Weight = Configuration.LynxTribe.LynxStuff.LynxShrineTier1Weight.Value;
            dropTable.tier2Weight = Configuration.LynxTribe.LynxStuff.LynxShrineTier2Weight.Value;
            dropTable.tier3Weight = Configuration.LynxTribe.LynxStuff.LynxShrineTier3Weight.Value;
            dropTable.bossWeight = Configuration.LynxTribe.LynxStuff.LynxShrineTierBossWeight.Value;
            return dropTable;
        }

        public Material CreateShatteredAbodesLeavesMaterialLOD0(Material originalMaterial)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            material.name = "ShatteredAbodesLeaves_LOD0";

            material.SetColor("_Color", new Color(0.9098f, 0.81568f, 0.03921f));
            material.SetInt("_WindQuality", 0);

            return material;
        }

        public Material CreateTitanicPlanesLeavesMaterialLOD0(Material originalMaterial)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            material.name = "TitanicPlainsLeaves_LOD0";

            material.SetColor("_Color", new Color(0.77254f, 0.69411f, 0.1647f));
            material.SetInt("_WindQuality", 0);

            return material;
        }

        public Material CreateWetlandsLeavesMaterialLOD0(Material originalMaterial)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            material.name = "WetlandsLeaves_LOD0";

            material.SetColor("_Color", new Color(0.9098f, 0.0745f, 0.1058f));
            material.SetInt("_WindQuality", 0);

            return material;
        }

        public Material CreateGoldemDiebackLeavesMaterialLOD0(Material originalMaterial)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            material.name = "GoldenDiebackLeaves_LOD0";

            material.SetColor("_Color", new Color(1f, 0f, 0.6509f));
            material.SetInt("_WindQuality", 0);

            return material;
        }

        public Material CreateSkyMeadowLeavesMaterialLOD0(Material originalMaterial)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            material.name = "SkyMeadowLeaves_LOD0";

            material.SetColor("_Color", new Color(0.7411f, 0.2745f, 1f));
            material.SetInt("_WindQuality", 0);

            return material;
        }

        public Material CreateBlackBeachLeavesMaterialLOD0(Material originalMaterial)
        {
            var material = UnityEngine.Object.Instantiate(originalMaterial);

            material.name = "BlackBeachLeaves_LOD0";

            material.SetColor("_Color", new Color(0.2588f, 0.3882f, 0.2235f));
            material.SetInt("_WindQuality", 0);

            return material;
        }

    }
}
