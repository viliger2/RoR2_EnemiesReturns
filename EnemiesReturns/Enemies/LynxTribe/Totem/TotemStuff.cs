using EnemiesReturns.Behaviors;
using EnemiesReturns.EditorHelpers;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class TotemStuff
    {
        // TODO: ghost
        public GameObject CreateFirewallProjectile(GameObject prefab, AnimationCurveDef curveDef)
        {
            prefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            prefab.AddComponent<TeamFilter>();

            var projectileController = prefab.AddComponent<ProjectileController>();
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = 1f; // TODO: config
            //projectileController.flightSoundLoop = // TODO:
            projectileController.cannotBeDeleted = true;
            //projectileController.startSound = // TODO

            var projectileDamage = prefab.AddComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.IgniteOnHit; // TODO: should we?

            var objectScaleCurve = prefab.AddComponent<ObjectScaleCurve>();
            objectScaleCurve.useOverallCurveOnly = true;
            objectScaleCurve.overallCurve = curveDef.curve;
            objectScaleCurve.timeMax = 5f; // TODO

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 5f; // TODO

            var firefall = prefab.transform.Find("Firewall").gameObject;
            var firewallAttack = firefall.AddComponent<FirewallAttack>();
            firewallAttack.projectileController = projectileController;
            firewallAttack.projectileDamage = projectileDamage;

            prefab.RegisterNetworkPrefab();

            return prefab;

        }

    }
}
