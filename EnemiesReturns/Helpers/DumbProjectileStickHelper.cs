using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Helpers
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

        public string childToRotateTo; // TODO: its empty, that's why it probably doesn't work

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
            // TODO: Figure out turning
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
                    var modelTransform = modelLocator.transform;
                    if (!modelTransform)
                    {
                        return;
                    }
                    var childLocator = transform.gameObject.GetComponent<ChildLocator>();
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
