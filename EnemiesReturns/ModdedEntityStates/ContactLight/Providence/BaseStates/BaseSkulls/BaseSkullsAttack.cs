using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls
{
    public abstract class BaseSkullsAttack : GenericCharacterMain
    {
        public abstract GameObject projectilePrefab { get; }

        public abstract GameObject effectPrefab { get; }

        public abstract float damageCoefficient { get; }

        public abstract float baseFireFrequency { get; }

        public abstract int projectilesToSpawn { get; }

        public abstract int additionalProjectilesPerPlayer { get; }

        public abstract float projectileSpeed { get; }

        public abstract float maxDistance { get; }

        private int totalProjectiles;

        private List<CharacterBody> activePlayers;

        private float timer;

        private int projectilesSpawned;

        public override void OnEnter()
        {
            base.OnEnter();

            activePlayers = Utils.GetActiveAndAlivePlayerBodies();
            totalProjectiles = projectilesToSpawn + additionalProjectilesPerPlayer * Mathf.Max(0, activePlayers.Count - 1);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority)
            {
                return;
            }

            if (projectilesSpawned >= totalProjectiles)
            {
                outer.SetNextStateToMain();
            }

            if (timer <= 0f)
            {
                var targetBody = activePlayers[projectilesSpawned % activePlayers.Count];
                if (!targetBody || !targetBody.healthComponent || !targetBody.healthComponent.alive)
                {
                    projectilesSpawned++;
                    return;
                }

                var xOffset = GetRandomOffset();
                var zOffset = GetRandomOffset();
                var yOffset = 0f;

                if (!targetBody.characterMotor.isGrounded)
                {
                    yOffset = GetRandomOffset();
                    var distanceToFloor = maxDistance;
                    var distanceToCeiling = maxDistance;
                    if (Physics.Raycast(targetBody.transform.position, Vector3.down, out var hitInfo, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        distanceToFloor = Vector3.Distance(targetBody.transform.position, hitInfo.point);
                    }
                    if (Physics.Raycast(targetBody.transform.position, Vector3.up, out var hitInfo2, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        distanceToCeiling = Vector3.Distance(targetBody.transform.position, hitInfo2.point);
                    }
                    yOffset = Mathf.Clamp(yOffset, -distanceToFloor, distanceToCeiling);
                }

                var position = targetBody.transform.position + xOffset * Vector3.right + yOffset * Vector3.up + zOffset * Vector3.forward;
                var distance = Vector3.Distance(gameObject.transform.position, position);

                var effectData = new EffectData()
                {
                    genericFloat = distance / projectileSpeed, // use EffectRetimer component on effect to rescale effect duration
                    origin = position,
                    rotation = Quaternion.identity,
                };

                EffectManager.SpawnEffect(effectPrefab, effectData, true);

                var projectileInfo = new FireProjectileInfo()
                {
                    crit = RollCrit(),
                    damage = damageStat * damageCoefficient,
                    fuseOverride = distance / projectileSpeed,
                    maxDistance = distance,
                    owner = gameObject,
                    position = gameObject.transform.position,
                    rotation = Util.QuaternionSafeLookRotation((position - gameObject.transform.position).normalized),
                    useFuseOverride = true,
                    useSpeedOverride = true,
                    speedOverride = projectileSpeed,
                    projectilePrefab = projectilePrefab
                };

                ProjectileManager.instance.FireProjectile(projectileInfo);

                projectilesSpawned++;
                timer += baseFireFrequency;
            }
            timer -= GetDeltaTime();
            float GetRandomOffset()
            {
                return UnityEngine.Random.Range(-maxDistance, maxDistance);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
