using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class Disappear : BaseState
    {
        public static GameObject predictedPositionEffect;

        public static float baseDuration = 0.5f;

        public Vector3 predictedPosition;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private int originalLayer;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            if (NetworkServer.active)
            {
                CleanseSystem.CleanseBodyServer(base.characterBody, true, false, false, true, false, false);
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            base.characterMotor.Motor.SetPositionAndRotation(predictedPosition + Vector3.up * 0.25f, Quaternion.identity);

            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }

            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter++;
            }
            originalLayer = base.gameObject.layer;
            base.gameObject.layer = LayerIndex.GetAppropriateFakeLayerForTeam(base.teamComponent.teamIndex).intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            EffectManager.SimpleEffect(predictedPositionEffect, predictedPosition, Quaternion.identity, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new Attack());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            if (characterModel)
            {
                characterModel.invisibilityCount--;
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter--;
            }
            base.gameObject.layer = originalLayer;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
