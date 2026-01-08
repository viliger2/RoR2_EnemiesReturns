using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class Disappear : BaseState
    {
        public static GameObject predictedPositionEffect;

        //public static float baseDuration => Configuration.General.ProvidenceP1UtilityInvisibleDuration.Value;
        public static float baseDuration => 2f;

        public Vector3 predictedPosition;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private int originalLayer;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Transform modelTransform = GetModelTransform();
            if (NetworkServer.active)
            {
                CleanseSystem.CleanseBodyServer(base.characterBody, true, false, false, true, false, false);
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            if(SceneInfo.instance && SceneInfo.instance.groundNodes)
            {
                var closestNode = SceneInfo.instance.groundNodes.FindClosestNode(predictedPosition, HullClassification.Golem);
                if(closestNode != RoR2.Navigation.NodeGraph.NodeIndex.invalid)
                {
                    if(SceneInfo.instance.groundNodes.GetNodePosition(closestNode, out var nodePosition))
                    {
                        predictedPosition = nodePosition;
                    }
                }
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
            if(fixedAge > duration && isAuthority)
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
