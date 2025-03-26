using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.zJunk.ModdedEntityStates.Judgement.Arraign.Phase2
{
    public class SlashDashPhase2 : BaseSlashDash
    {
        public override float baseDuration => 1f;

        public override float damageCoefficient => 3f;

        public override float procCoefficient => 1f;

        public override float turnSpeed => 150f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SlashDash";

        public override string playbackParamName => "SlashDash.playbackRate";

        public override string hitBoxGroupName => "Sword";

        public static int projectileCount = 5;

        public static GameObject projectilePrefab;

        private float projectileSpawnTime;

        private float projectileTimer;

        public override void OnEnter()
        {
            base.OnEnter();
            projectileSpawnTime = duration / projectileCount;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            projectileTimer += GetDeltaTime();
            if (projectileTimer > projectileSpawnTime && isAuthority)
            {
                var projectileInfo = new FireProjectileInfo()
                {
                    crit = RollCrit(),
                    owner = gameObject,
                    position = characterBody.footPosition,
                    projectilePrefab = projectilePrefab,
                    rotation = Quaternion.identity,
                    damage = damageStat * damageCoefficient
                };

                ProjectileManager.instance.FireProjectile(projectileInfo);
                projectileTimer -= projectileSpawnTime;
            }
        }
    }
}
