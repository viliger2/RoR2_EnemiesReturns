using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.UIElements.ListViewDragger;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap
{
    public abstract class BaseHoldSkyLeap : BaseState
    {
        public abstract float baseDuration { get; }

        public abstract float baseTargetMarked { get; }

        public abstract float baseTargetDropped { get; }

        public static GameObject dropEffectPrefab;

        public static GameObject markEffect;

        private float duration;

        private float targetMarked;

        private float targetDropped;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private int originalLayer;

        private bool isTargetMarked;

        private bool isTargetDropped;

        private GameObject target;

        private TemporaryVisualEffect tempEffect;

        private Vector3 dropPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                Util.CleanseBody(base.characterBody, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: false, removeDots: true, removeStun: false, removeNearbyProjectiles: false);
            }
            duration = baseDuration / attackSpeedStat;
            targetMarked = baseTargetMarked/ attackSpeedStat;
            targetDropped = baseTargetDropped / attackSpeedStat;
            Transform modelTransform = GetModelTransform();
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
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            originalLayer = base.gameObject.layer;
            base.gameObject.layer = LayerIndex.GetAppropriateFakeLayerForTeam(base.teamComponent.teamIndex).intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            if (isAuthority)
            {
                target = Utils.GetRandomAlivePlayer();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(base.fixedAge > targetMarked && !isTargetMarked)
            {
                if (target)
                {
                    var targetBody = target.GetComponent<CharacterBody>();

                    var effectGameObject = UnityEngine.Object.Instantiate(markEffect, targetBody.corePosition, Quaternion.identity);
                    tempEffect = effectGameObject.GetComponent<TemporaryVisualEffect>();
                    tempEffect.parentTransform = targetBody.coreTransform;
                    tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
                    tempEffect.healthComponent = targetBody.healthComponent;
                    tempEffect.radius = targetBody.radius;
                }
                isTargetMarked = true;
            }

            if (base.fixedAge > targetDropped && !isTargetDropped)
            {
                if (tempEffect)
                {
                    tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
                }

                Vector3 originalPosition;
                if (target)
                {
                    originalPosition = target.transform.position;
                }
                else
                {
                    originalPosition = base.transform.position;
                }
                if (Physics.Raycast(originalPosition + Vector3.up * 2f, Vector3.down, out var raycastInfo, 10000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    dropPosition = raycastInfo.point;
                }
                else
                {
                    dropPosition = originalPosition;
                }

                base.characterMotor.Motor.SetPositionAndRotation(dropPosition + Vector3.up * 0.25f, Quaternion.identity);

                EffectManager.SimpleEffect(dropEffectPrefab, dropPosition, Quaternion.identity, false);

                isTargetDropped = true;
            }

            if (isAuthority && base.fixedAge > duration)
            {
                SetNextStateAuthority(dropPosition);
            }
        }

        public abstract void SetNextStateAuthority(Vector3 dropPosition);

        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter--;
            }
            if (characterModel)
            {
                characterModel.invisibilityCount--;
            }
            base.gameObject.layer = originalLayer;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Vehicle;
        }
    }
}
