using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Spitter
{
    public class SpitterDeathDanceController : MonoBehaviour
    {
        public CharacterBody body;
        public ModelLocator modelLocator;

        private static float triggerDistance = 20f;

        private void OnEnable()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void OnDisable()
        {
            if (!NetworkServer.active) { return; }
            GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.victimBody.isPlayerControlled)
            {
                var distance = Vector3.Distance(damageReport.victimBody.modelLocator.modelTransform.position, modelLocator.modelTransform.position);
                if (distance <= triggerDistance)
                {
                    EntityStateMachine.FindByCustomName(body.gameObject, "Body").SetNextState(new ModdedEntityStates.Spitter.DeathDance());
                }
            }
        }
    }
}
