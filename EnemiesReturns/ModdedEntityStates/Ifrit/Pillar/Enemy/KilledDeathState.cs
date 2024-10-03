using HG;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Enemy
{
    public class KilledDeathState : BaseDeathState
    {
        public static float explosionDelay = 0.5f;

        public static float damage => EnemiesReturnsConfiguration.Ifrit.PillarExplosionDamage.Value;

        public static float radius => EnemiesReturnsConfiguration.Ifrit.PillarExplosionRadius.Value;

        public static float force => EnemiesReturnsConfiguration.Ifrit.PillarExplosionForce.Value;

        public static bool ignoresLoS => EnemiesReturnsConfiguration.Ifrit.PillarExplosionIgnoesLoS.Value;

        public static AnimationCurve fireballYCurve;

        public static GameObject explosionPrefab;

        private bool hasExploded;

        private Transform fireball;

        private float initialFireballY;
        private float initialFireballX;
        private float finalFireballY;
        private Vector3 initialFireballLocation;

        public override void OnEnter()
        {
            var childLocator = GetModelChildLocator();
            if (childLocator)
            {
                fireball = childLocator.FindChild("Fireball");
                if (fireball)
                {
                    fireball.parent = null;
                    initialFireballLocation = fireball.transform.position;
                    initialFireballX = fireball.transform.position.x;
                    initialFireballY = fireball.transform.position.y;
                    finalFireballY = initialFireballY - 25.5f; // magic number from editor
                    if(Physics.Raycast(fireball.transform.position, Vector3.down, out var result, 100f, LayerIndex.world.intVal, QueryTriggerInteraction.Ignore))
                    {
                        finalFireballY = result.point.y;
                    }
                }
            }
            Util.PlaySound("ER_Ifrit_Pillar_Killed_By_Player_Play", gameObject);
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            if(fixedAge >= explosionDelay && !hasExploded)
            {
                if (isAuthority)
                {
                    var blastAttack = new BlastAttack
                    {
                        attacker = null,
                        inflictor = null,
                        radius = radius,
                        procCoefficient = 1f,
                        position = transform.position,
                        crit = false,
                        baseDamage = damageStat * damage,
                        canRejectForce = false,
                        falloffModel = BlastAttack.FalloffModel.None,
                        baseForce = force,
                        losType = ignoresLoS ? BlastAttack.LoSType.None : BlastAttack.LoSType.NearestHit,
                        teamIndex = TeamIndex.Player // hardcoding since we have no idea who killed it, also means if player's ifrit tower gets killed by monsters at least it wont kill his allies
                    };
                    blastAttack.damageType.damageType = DamageType.IgniteOnHit;
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                    blastAttack.Fire();
                }
                if (explosionPrefab)
                {
                    EffectManager.SpawnEffect(explosionPrefab, new EffectData { origin = initialFireballLocation, scale = 5f * (radius / 30f) }, false);
                }
                Util.PlaySound("ER_Ifrit_Pillar_Explosion_Play", gameObject);
                UnityEngine.GameObject.Destroy(fireball.gameObject);
                hasExploded = true;
            }
            base.FixedUpdate();
        }

        public override void Update()
        {
            if(fireball && !hasExploded)
            {
                float y = initialFireballY - (Mathf.Abs(finalFireballY - initialFireballY) * fireballYCurve.Evaluate(age / explosionDelay));
                float x = Mathf.Lerp(initialFireballX, initialFireballX + 10f, age / explosionDelay);
                fireball.position = new Vector3(x, y, fireball.position.z);
            }
            base.Update();
        }
    }
}
