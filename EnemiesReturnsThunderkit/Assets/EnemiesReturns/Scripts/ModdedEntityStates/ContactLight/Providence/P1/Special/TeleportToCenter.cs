using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Special
{
    [RegisterEntityState]
    public class TeleportToCenter : BaseState
    {
        public static float baseDuration => 3f;

        private Vector3 position;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "RingsInitial", 0.1f);
            position = transform.position;

            var sceneChildLocator = SceneInfo.instance.gameObject.GetComponent<ChildLocator>();
            if (sceneChildLocator)
            {
                var arenaCenter = sceneChildLocator.FindChild("ArenaCenter");
                if (arenaCenter)
                {
                    position = arenaCenter.position;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > baseDuration && isAuthority)
            {
                base.characterMotor.Motor.SetPositionAndRotation(position + Vector3.up * 0.25f, Quaternion.identity);
                outer.SetNextState(new FireRings());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
