using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Special
{
    [RegisterEntityState]
    public class TeleportToCenter : BaseState
    {
        public static float baseDuration => 3.33f;

        public static float modelHide = 2.5f;

        public static GameObject teleportEffect;

        private Vector3 position;

        private bool modelHidden;

        private Transform modelTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "BeamInitTeleport", 0.1f);
            position = transform.position;

            modelTransform = GetModelTransform();

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
            if(fixedAge > modelHide && !modelHidden)
            {
                HideModel();
                SpawnTeleportEffect();
                modelHidden = true;
            }
            if (fixedAge > baseDuration && isAuthority)
            {
                base.characterMotor.Motor.SetPositionAndRotation(position, Quaternion.identity);
                outer.SetNextState(new RotatingLaserBegin());
            }
        }

        private void SpawnTeleportEffect()
        {
            var data = new EffectData()
            {
                origin = FindModelChild("TeleportEffectOrigin").position,
                rotation = Quaternion.identity,
            };
            EffectManager.SpawnEffect(teleportEffect, data, false);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            ShowModel();
            SpawnTeleportEffect();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void HideModel()
        {
            if (!modelTransform)
            {
                return;
            }
            var characterModel = modelTransform.GetComponent<CharacterModel>();
            var hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();

            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter++;
            }
        }

        private void ShowModel()
        {
            if (!modelTransform)
            {
                return;
            }
            var characterModel = modelTransform.GetComponent<CharacterModel>();
            var hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();

            if (characterModel)
            {
                characterModel.invisibilityCount--;
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter--;
            }
        }
    }
}
