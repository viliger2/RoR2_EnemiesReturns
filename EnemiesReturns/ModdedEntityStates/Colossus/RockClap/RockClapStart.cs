using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.RockClap
{
    public class RockClapStart : BaseState
    {
        public static float baseDuration = 1.5f;

        private float duration;

        private FloatingRocksController floatingRocksController;

        private Transform rockTarget;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;

            floatingRocksController = modelLocator.modelTransform.gameObject.GetComponent<FloatingRocksController>();
            floatingRocksController.enabled = true;
            rockTarget = FindModelChild("RocksEnd");

            PlayCrossfade("Gesture, Override", "ClapStart", "Clap.playbackrate", duration, 0.1f);
        }

        public override void Update()
        {
            base.Update();
            floatingRocksController.SetRockThingPosition(Vector3.Lerp(floatingRocksController.initialPosition.position, rockTarget.position, age / duration));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new RockClapEnd());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
