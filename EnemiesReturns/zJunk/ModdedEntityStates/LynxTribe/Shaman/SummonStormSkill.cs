using EnemiesReturns.Behaviors;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman
{
    [RegisterEntityState]
    public class SummonStormSkill : BaseState
    {
        public static float baseDuration = 4f;
        public static float minDistance => Configuration.LynxTribe.LynxTotem.SummonStormMinRange.Value;
        public static float maxDistance => Configuration.LynxTribe.LynxTotem.SummonStormMaxRange.Value;
        public static float rechargeOnFailure = 0.75f;
        public static CharacterSpawnCard cscStorm;
        public static int stormCount = 1;
        public static float baseSkillRechargeTime => Configuration.LynxTribe.LynxTotem.SummonStormCooldown.Value;
        public static float effectSpawn => 0.2f;
        public static GameObject summonEffectPrefab;
        public static float playableMaxDistance = 1000f;

        private float duration;
        private float effectTimer;

        private GameObject[] storms = Array.Empty<GameObject>();
        private Transform effectSpawnPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "SummonStorm", "SummonStorm.playbackRate", duration, 0.1f);
            SummonStorms();
            effectSpawnPoint = FindModelChild("StaffUpperPoint");
            if (!effectSpawnPoint)
            {
                effectSpawnPoint = transform;
            }
        }

        private void SummonStorms()
        {
            if (isAuthority)
            {
                if (characterBody.isPlayerControlled)
                {
                    if (Physics.Raycast(GetAimRay(), out var hitInfo, playableMaxDistance, LayerIndex.world.mask))
                    {
                        SummonStormPlayer(hitInfo.point);
                    }
                }
                else
                {
                    if (NetworkServer.active)
                    {
                        storms = new GameObject[stormCount];
                        for (int i = 0; i < stormCount; i++)
                        {
                            foreach (var ai in characterBody.master.aiComponents)
                            {
                                if (!ai.currentEnemy.characterBody)
                                {
                                    continue;
                                }

                                storms[i] = SummonStormAI(ai.currentEnemy.characterBody.transform);
                            }
                        }
                    }
                }
            }
        }

        private void SummonStormPlayer(Vector3 position)
        {
            if (characterBody)
            {
                characterBody.SendConstructTurret(characterBody, position, Quaternion.identity, MasterCatalog.FindMasterIndex(cscStorm.prefab));
            }
        }

        private GameObject SummonStormAI(Transform transform)
        {
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(cscStorm, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = minDistance,
                maxDistance = maxDistance,
                spawnOnTarget = transform
            }, RoR2Application.rng);

            directorSpawnRequest.summonerBodyObject = gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = (spawnResult) =>
            {
                if (!spawnResult.success)
                {
                    SummonStormAI(transform); // surely this won't break anything
                    return;
                }
                if (spawnResult.spawnedInstance && characterBody)
                {
                    var aiownership = spawnResult.spawnedInstance.GetComponent<AIOwnership>();
                    if (aiownership)
                    {
                        aiownership.ownerMaster = characterBody.master;
                    }
                    var inventory = spawnResult.spawnedInstance.GetComponent<Inventory>();
                    if (inventory)
                    {
                        inventory.CopyEquipmentFrom(characterBody.inventory);
                        inventory.CopyItemsFrom(characterBody.inventory);
                    }
                }
            };
            return DirectorCore.instance?.TrySpawnObject(directorSpawnRequest);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void Update()
        {
            base.Update();
            effectTimer += Time.deltaTime;
            if (effectTimer > effectSpawn && summonEffectPrefab)
            {
                for (int i = 0; i < storms.Length; i++)
                {
                    var stormChildLocator = GetChildLocator(storms[i]);
                    if (stormChildLocator)
                    {
                        var neweffect = UnityEngine.Object.Instantiate(summonEffectPrefab, effectSpawnPoint.position, effectSpawnPoint.rotation);
                        neweffect.GetComponent<MoveTowardsTargetAndDestroyItself>().target = stormChildLocator.FindChild("EffectPoint" + UnityEngine.Random.Range(1, 9));
                    }
                }
                effectTimer -= effectSpawn;
            }
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);
            if (!(nextState is GenericCharacterMain))
            {
                if (NetworkServer.active)
                {
                    foreach (var gameObject in storms)
                    {
                        if (gameObject && gameObject.TryGetComponent<CharacterMaster>(out var master))
                        {
                            master.TrueKill();
                        }
                    }
                }
                skillLocator.special.RunRecharge(baseSkillRechargeTime * rechargeOnFailure);
            }
        }

        private ChildLocator GetChildLocator(GameObject masterGameObject)
        {
            if (masterGameObject && masterGameObject.TryGetComponent<CharacterMaster>(out var master))
            {
                var body = master.GetBody();
                if (body && body.modelLocator && body.modelLocator.modelTransform)
                {
                    return body.modelLocator.modelTransform.GetComponent<ChildLocator>();
                }
            }
            return null;
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            storms = null;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
