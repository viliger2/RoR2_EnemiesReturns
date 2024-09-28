using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using RoR2;
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
            //PlayCrossfade("Body", "ClapStart", "Clap.playbackrate", duration, 0.1f);
            Util.PlayAttackSpeedSound("ER_Colossus_ArmsSpread_Play", gameObject, attackSpeedStat);
        }

        public override void Update()
        {
            base.Update();
            floatingRocksController.SetRockThingPosition(Vector3.Lerp(floatingRocksController.initialPosition.position, rockTarget.position, age / duration));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new RockClapEnd());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
