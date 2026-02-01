using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Special
{
    [RegisterEntityState]
    public class TeleportToCenter : BaseState
    {
        public static float baseDuration => 1f;

        private Vector3 position;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "EnterSkyLeap", "SkyLeap.playbackRate", baseDuration);
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
                outer.SetNextState(new FireRingsWithClones());
            }
        }


        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
