using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    public class LaserClapStart: BaseState
    {
        public static float baseDuration = 1.5f;

        private float duration;

        //private FloatingRocksController controller;

        //private Transform rockTarget;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;

            //controller = modelLocator.modelTransform.gameObject.GetComponent<FloatingRocksController>();
            //controller.enabled = true;
            //rockTarget = modelLocator.modelTransform.Find("rockEndPoint");

            PlayCrossfade("Gesture, Override", "ClapStart", "Clap.playbackrate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //controller.SetRockThingPosition(Vector3.Lerp(controller.initialPosition.position, rockTarget.position, fixedAge / duration));
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new LaserClapEnd());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
