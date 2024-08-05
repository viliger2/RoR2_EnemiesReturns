using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class DeathDance : BaseState
    {
        private static float duration = 20f;
        private static float healthFraction = 0.5f;

        private float stopwatch;

        private Transform target;

        public DeathDance(Transform target) => this.target = target;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "DeathDance");
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            if(report.victimBody == characterBody)
            {
                if((healthComponent.combinedHealth / healthComponent.fullCombinedHealth) <= healthFraction)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            GlobalEventManager.onServerDamageDealt -= GlobalEventManager_onServerDamageDealt;
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (target)
            {
                StartAimMode(new Ray(target.position, target.forward), 0.16f, false);
            }
            if((stopwatch >= duration))
            {
                outer.SetNextStateToMain();
            }
        }
    }
}
