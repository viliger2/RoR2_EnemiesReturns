using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1
{
    [RegisterEntityState]
    public class LightningStrikes : BaseState
    {
        public static float baseDuration = 2.5f;

        public static float baseInitialDelay = 0f;

        public static GameObject projectilePrefab;

        public static int projectileCount => Configuration.Judgement.ArraignP1.LightningStrikesCount.Value;

        public static float delayBetweenSpawns => Configuration.Judgement.ArraignP1.LightningStrikesDelayBetweenProjectiles.Value;

        public static float maxSpawnDistance => Configuration.Judgement.ArraignP1.LightningStrikesMaxDistance.Value;

        public static float minSpawnDistance => Configuration.Judgement.ArraignP1.LightningStrikesMinDistance.Value;

        public static float damageCoefficient => Configuration.Judgement.ArraignP1.LightningStrikesDamage.Value;

        private float timer;

        private int projectilesSpawned;

        private float spawnDistance;

        private Transform target;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture", "Thundercall", 0.1f);

            spawnDistance = maxSpawnDistance - minSpawnDistance;
            if (isAuthority)
            {
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
                if (teamComponent)
                {
                    bullseyeSearch.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
                }
                bullseyeSearch.maxDistanceFilter = 1000f;
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
                    target = hurtBox.healthComponent.body.transform;
                }
                else
                {
                    target = this.transform;
                }
                FireProjectileAuthority(target.position);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge < baseInitialDelay)
            {
                return;
            }

            timer += GetDeltaTime();
            if (isAuthority && timer > delayBetweenSpawns && projectilesSpawned < projectileCount)
            {
                var xOffest = GetRandomOffset();
                var zOffest = GetRandomOffset();

                var position = target.position + Vector3.forward * zOffest + Vector3.right * xOffest;


                FireProjectileAuthority(position);

                timer -= delayBetweenSpawns;
                projectilesSpawned++;
            }

            if (projectilesSpawned >= projectileCount && isAuthority && fixedAge > baseDuration)
            {
                outer.SetNextStateToMain();
            }

            float GetRandomOffset()
            {
                var value = UnityEngine.Random.Range(-spawnDistance, spawnDistance);
                value += value > 0f ? minSpawnDistance : -minSpawnDistance;
                return value;
            }
        }

        private void FireProjectileAuthority(Vector3 position)
        {
            if (Physics.Raycast(position, Vector3.down, out var hitInfo, 1000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                position = hitInfo.point + Vector3.up;
            }

            var projectileInfo = new FireProjectileInfo()
            {
                crit = RollCrit(),
                owner = base.gameObject,
                position = position,
                projectilePrefab = projectilePrefab,
                rotation = Quaternion.identity,
                damage = damageStat * damageCoefficient,
                damageTypeOverride = DamageTypeCombo.Generic
            };

            ProjectileManager.instance.FireProjectile(projectileInfo);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
