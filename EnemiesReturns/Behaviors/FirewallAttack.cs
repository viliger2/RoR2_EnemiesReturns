using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class FirewallAttack : NetworkBehaviour
    {
        public ProjectileController projectileController;

        public ProjectileDamage projectileDamage;

        private List<HealthComponent> hitTargets = new List<HealthComponent>();

        private void OnTriggerEnter(Collider collider)
        {
            CharacterBody body = collider.GetComponent<CharacterBody>();
            if(body && body.hasEffectiveAuthority && body.characterMotor && body.characterMotor.isGrounded)
            {
                PerformDamage(body);
            } 
        }

        private void OnTriggetExit(Collider collider)
        {
            CharacterBody body = collider.GetComponent<CharacterBody>();
            if (body && body.hasEffectiveAuthority && body.characterMotor && body.characterMotor.isGrounded)
            {
                PerformDamage(body);
            }
        }

        private void PerformDamage(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                PerformDamageServer(body);
            }
            else
            {
                CmdPerformDamage(body.netId.Value);
            }
        }

        [Command]
        public void CmdPerformDamage(uint netId)
        {
            var gameObject = Util.FindNetworkObject(new NetworkInstanceId(netId));
            if (gameObject && gameObject.TryGetComponent<CharacterBody>(out var characterBody))
            {
                PerformDamageServer(characterBody);
            }
        }

        private void PerformDamageServer(CharacterBody body)
        {
            if (body.healthComponent && !hitTargets.Contains(body.healthComponent))
            {
                if(FriendlyFireManager.ShouldDirectHitProceed(body.healthComponent, projectileController.teamFilter.teamIndex))
                {
                    var damageInfo = new DamageInfo
                    {
                        damage = projectileDamage.damage,
                        crit = projectileDamage.crit,
                        inflictor = gameObject,
                        attacker = projectileController.owner ? projectileController.owner.gameObject : null,
                        position = body.footPosition,
                        canRejectForce = true,
                        damageColorIndex = projectileDamage.damageColorIndex,
                        damageType = projectileDamage.damageType,
                        force = Vector3.zero,
                        procChainMask = projectileController.procChainMask,
                        procCoefficient = projectileController.procCoefficient,
                    };

                    body.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, body.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, body.gameObject);

                    hitTargets.Add(body.healthComponent);
                }
            }
        }
    }
}
