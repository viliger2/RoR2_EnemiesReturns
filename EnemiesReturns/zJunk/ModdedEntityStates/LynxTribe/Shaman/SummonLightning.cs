using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman
{
    public class SummonLightning : BaseState
    {
        public static int strikesCount = 3;

        public static float baseDuration = 2.4f;

        public static float baseSpawnDelay = 0.6f;

        public static float maxDistance = 50f;

        public static float baseParticleSpawnTime = 1.6f;

        public static GameObject projectilePrefab;

        public static float damageCoef = 1f;

        public static float force = 0f;

        private float spawnDelay;

        private float timer;

        private float duration;

        private float particleSpawnTime;

        private GameObject[] spawnZones;

        private Transform targetTransform;

        private int spawnedZonesCount;

        private bool projectilesSpawned;

        public override void OnEnter()
        {
            base.OnEnter();
            spawnDelay = baseSpawnDelay / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            particleSpawnTime = baseParticleSpawnTime / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "CastTeleportFull", "CastTeleport.playbackRate", duration, 0.1f);
            spawnZones = new GameObject[strikesCount];
            if (isAuthority)
            {
                targetTransform = FindTarget();
            }
            spawnZones[0] = SpawnZone();
            spawnedZonesCount++;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += GetDeltaTime();
            if (timer > spawnDelay && spawnedZonesCount < strikesCount)
            {
                spawnZones[spawnedZonesCount] = SpawnZone();
                spawnedZonesCount++;
                timer -= spawnDelay;
            }

            if (fixedAge > particleSpawnTime && isAuthority && !projectilesSpawned)
            {
                for (int i = spawnZones.Length - 1; i >= 0; i--)
                {
                    var fireProjectileInfo = default(FireProjectileInfo);
                    fireProjectileInfo.projectilePrefab = projectilePrefab;
                    fireProjectileInfo.position = spawnZones[i].transform.position;
                    fireProjectileInfo.rotation = Quaternion.identity;
                    fireProjectileInfo.owner = gameObject;
                    fireProjectileInfo.damage = damageStat * damageCoef;
                    fireProjectileInfo.force = force;
                    fireProjectileInfo.damageTypeOverride = new DamageTypeCombo(DamageTypeCombo.Generic, DamageTypeExtended.Generic, DamageSource.Utility);
                    fireProjectileInfo.crit = RollCrit();
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                    UnityEngine.Object.Destroy(spawnZones[i]);
                }
                projectilesSpawned = true;
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            for (int i = spawnZones.Length - 1; i >= 0; i--)
            {
                if (spawnZones[i])
                {
                    UnityEngine.Object.Destroy(spawnZones[i]);
                }
            }
            spawnZones = null;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private Transform FindTarget()
        {
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
            if ((bool)teamComponent)
            {
                bullseyeSearch.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            }
            bullseyeSearch.maxDistanceFilter = maxDistance;
            bullseyeSearch.maxAngleFilter = 90f;
            Ray aimRay = GetAimRay();
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
            bullseyeSearch.RefreshCandidates();
            HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault();
            if (hurtBox)
            {
                return hurtBox.transform;
            }

            return null;
        }

        private GameObject SpawnZone()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = targetTransform.position;
            cube.layer = LayerIndex.noCollision.intVal;
            return cube;
        }

    }
}
