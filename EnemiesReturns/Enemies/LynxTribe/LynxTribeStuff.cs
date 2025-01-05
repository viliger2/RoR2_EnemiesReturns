using EnemiesReturns.Behaviors;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe
{
    public class LynxTribeStuff
    {
        public GameObject CreateSpawnEffect(GameObject prefab)
        {
            var spawnEffect = prefab.InstantiateClone("LynxTribeSpawnEffect", false);

            spawnEffect.transform.Find("PurpleLeaves").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeaf.mat").WaitForCompletion();
            //prefab.transform.Find("YellowLeaves").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeafAlt.mat").WaitForCompletion();
            spawnEffect.transform.Find("YellowLeaves").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matTreebotTreeLeaf2", LynxStormBody.CreateTreebotTreeLeaf2Material);

            spawnEffect.AddComponent<EffectComponent>().positionAtReferencedTransform = true;

            var vfxattributes = spawnEffect.AddComponent<VFXAttributes>();
            vfxattributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxattributes.vfxIntensity = VFXAttributes.VFXIntensity.High;

            spawnEffect.AddComponent<DestroyOnParticleEnd>();

            return spawnEffect;
        }

        public FamilyDirectorCardCategorySelection CreateLynxTribeFamily()
        {
            var dccsFamily = ScriptableObject.CreateInstance<FamilyDirectorCardCategorySelection>();
            (dccsFamily as ScriptableObject).name = "dccsLynxTribeFamily";

            dccsFamily.selectionChatString = "ENEMIES_RETURNS_FAMILY_LYNX_TRIBE";
            dccsFamily.minimumStageCompletion = 1;
            dccsFamily.maximumStageCompletion = int.MaxValue;

            // TODO: add totem
            var champions = new FamilyDirectorCardCategorySelection.Category
            {
                name = ContentProvider.MonsterCategories.Champions,
                selectionWeight = 2f
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

        // TODO: config this shit
        public GameObject CreateTrapPrefab(GameObject trapPrefab)
        {
            trapPrefab.AddComponent<NetworkIdentity>();

            var disabler = trapPrefab.AddComponent<DestroyOnTimerNetwork>();
            disabler.duration = 15f;
            disabler.enabled = false;

            var spawner = trapPrefab.AddComponent<LynxTribeSpawner>();
            spawner.eliteBias = 1f;
            spawner.spawnCards = new SpawnCard[]
            {
                Enemies.LynxTribe.Scout.ScoutBody.SpawnCards.cscLynxScoutDefault,
                Enemies.LynxTribe.Shaman.ShamanBody.SpawnCards.cscLynxShamanDefault, 
                Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault, 
                Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault,
            };
            spawner.minSpawnCount = 3;
            spawner.maxSpawnCount = 5;
            spawner.assignRewards = true;
            spawner.spawnDistance = 5f;
            spawner.retrySpawnCount = 3;
            spawner.teamIndex = TeamIndex.Monster;

            var trap = trapPrefab.AddComponent<LynxTribeTrap>(); // TODO: add sound effects
            trap.checkInterval = 0.25f;
            trap.spawnAfterTriggerInterval = 0.5f;
            trap.spawner = spawner;
            trap.destroyOnTimer = disabler;
            trap.hitBox = trapPrefab.transform.Find("Hitbox");
            var teamMask = new TeamMask();
            teamMask.AddTeam(TeamIndex.Player);
            trap.teamFilter = teamMask;
            trap.initialTriggerSound = "ER_LynxTrap_SnapTwig_Play"; // TODO

            var leaves = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/lemuriantemple/Assets/LTFallenLeaf.spm").WaitForCompletion();
            var leavesTransform = trapPrefab.transform.Find("Leaves").transform;
            foreach (Transform child in leavesTransform)
            {
                var leavesClone = UnityEngine.Object.Instantiate(leaves);
                leavesClone.transform.parent = child;
                leavesClone.transform.localPosition = Vector3.zero;
                leavesClone.transform.localRotation = Quaternion.identity;
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

        // TODO: lodsofconfig
        public GameObject CreateShrinePrefab(GameObject shrinePrefab)
        {
            shrinePrefab.AddComponent<NetworkIdentity>();

            var hightLightMesh = shrinePrefab.AddComponent<Highlight>();
            hightLightMesh.strength = 1f;
            hightLightMesh.targetRenderer = shrinePrefab.GetComponentInChildren<Renderer>(); // TODO
            hightLightMesh.highlightColor = Highlight.HighlightColor.interactive;

            var modelLocator = shrinePrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = shrinePrefab.transform.Find("Base/Cube"); // TODO
            modelLocator.modelBaseTransform = shrinePrefab.transform.Find("Base");
            modelLocator.autoUpdateModelTransform = false;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = false;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.preserveModel = false;
            modelLocator.normalizeToFloor = false;

            var spawner = shrinePrefab.AddComponent<LynxTribeSpawner>();
            spawner.eliteBias = 1f;
            spawner.spawnCards = new SpawnCard[]
            {
                Enemies.LynxTribe.Scout.ScoutBody.SpawnCards.cscLynxScoutDefault,
                Enemies.LynxTribe.Shaman.ShamanBody.SpawnCards.cscLynxShamanDefault,
                Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault,
                Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault,
            };
            spawner.minSpawnCount = 3;
            spawner.maxSpawnCount = 5;
            spawner.assignRewards = false;
            spawner.spawnDistance = 5f;
            spawner.retrySpawnCount = 3;
            spawner.teamIndex = TeamIndex.Monster;

            var shrine = shrinePrefab.AddComponent<LynxTribeShrine>();
            shrine.dropTable = CreateLynxShrineDropTable();
            shrine.localEjectionVelocity = new Vector3(0f, 15f, 8f);
            shrine.spawner = spawner;

            var cubeObject = shrinePrefab.transform.Find("Base/Cube").gameObject;
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

            shrinePrefab.RegisterNetworkPrefab();

            return shrinePrefab;
        }

        // TODO: config values
        private BasicPickupDropTable CreateLynxShrineDropTable()
        {
            var dropTable = ScriptableObject.CreateInstance<BasicPickupDropTable>();
            (dropTable as ScriptableObject).name = "dtLynxShrine";
            dropTable.tier1Weight = 0.55f;
            dropTable.tier2Weight = 0.3f;
            dropTable.tier3Weight = 0.05f;
            dropTable.bossWeight = 0.1f;

            return dropTable;
        }

    }
}
