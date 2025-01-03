﻿using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    // I love doing this stupid shit because I refuse to use thunderkit
    // and unity doesn't keep events through scenes or whatever
    // TODO: shards fly away on clients
    public class DumbProjectileStickHelper : NetworkBehaviour
    {
        public GameObject shardEffect;

        public ProjectileStickOnImpact stickOnImpact;

        public ProjectileOverlapAttack overlapAttack;

        public ProjectileController controller;

        public string childToRotateTo;

        private void OnEnable()
        {
            stickOnImpact = GetComponent<ProjectileStickOnImpact>();
            overlapAttack = GetComponent<ProjectileOverlapAttack>();
            if (stickOnImpact && overlapAttack)
            {
                if (stickOnImpact.stickEvent == null)
                {
                    stickOnImpact.stickEvent = new UnityEngine.Events.UnityEvent();
                }
                stickOnImpact.stickEvent.AddListener(OnStick);
            }
        }

        private void OnStick()
        {
            if (NetworkServer.active)
            {
                overlapAttack.enabled = false;
            }
            EnableShard();
            RpcEnableShard();
        }

        private void EnableShard()
        {
            if (shardEffect)
            {
                shardEffect.SetActive(true);
                if (controller && controller.owner)
                {
                    var modelLocator = controller.owner.GetComponent<ModelLocator>();
                    if (!modelLocator)
                    {
                        return;
                    }
                    var modelTransform = modelLocator.modelTransform;
                    if (!modelTransform)
                    {
                        return;
                    }
                    var childLocator = modelTransform.gameObject.GetComponent<ChildLocator>();
                    if (!childLocator)
                    {
                        return;
                    }
                    var childTransform = childLocator.FindChild(childToRotateTo);
                    if (!childTransform)
                    {
                        return;
                    }

                    shardEffect.transform.LookAt(childTransform.position);
                }
            }
        }

        [ClientRpc]
        private void RpcEnableShard()
        {
            EnableShard();
        }
    }
}
