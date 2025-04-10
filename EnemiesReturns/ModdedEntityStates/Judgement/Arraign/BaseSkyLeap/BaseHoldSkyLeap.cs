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

        private float duration;

        private float targetMarked;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private int originalLayer;

        private bool isTargetMarked;

        private GameObject target;

        private TemporaryVisualEffect tempEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            targetMarked = baseTargetMarked/ attackSpeedStat;
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
                    // TODO: merc expose for now
                    var effectGameObject = UnityEngine.Object.Instantiate(RoR2.CharacterBody.AssetReferences.mercExposeEffectPrefab, targetBody.corePosition, Quaternion.identity);
                    tempEffect = effectGameObject.GetComponent<TemporaryVisualEffect>();
                    tempEffect.parentTransform = targetBody.coreTransform;
                    tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
                    tempEffect.healthComponent = targetBody.healthComponent;
                    tempEffect.radius = targetBody.radius;
                }
                isTargetMarked = true;
            }

            if(base.fixedAge > duration)
            {
                if (tempEffect)
                {
                    tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
                }
                if (isAuthority)
                {
                    Vector3 dropPosition;
                    if (target)
                    {
                        dropPosition = target.transform.position;
                    }
                    else
                    {
                        dropPosition = base.transform.position;
                    }
                    base.characterMotor.Motor.SetPositionAndRotation(dropPosition + Vector3.up * 0.25f, Quaternion.identity);
                    SetNextStateAuthority(dropPosition);
                }
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
