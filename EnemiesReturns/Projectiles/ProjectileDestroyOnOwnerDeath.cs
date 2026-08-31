using EnemiesReturns.Components;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileDestroyOnOwnerDeath : MonoBehaviour
    {
        private ProjectileController controller;

        private void Awake()
        {
            controller = GetComponent<ProjectileController>();
        }

        private void Start()
        {
            if (NetworkServer.active)
            {
                if (controller)
                {
                    var body = controller.owner.GetComponent<CharacterBody>();
                    if (body && body.master)
                    {
                        body.master.onBodyDeath.AddListener(OnBodyDeath);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (NetworkServer.active)
            {
                if (controller)
                {
                    var body = controller.owner.GetComponent<CharacterBody>();
                    if (body && body.master)
                    {
                        body.master.onBodyDeath.RemoveListener(OnBodyDeath);
                    }
                }
            }
        }

        private void OnBodyDeath()
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
