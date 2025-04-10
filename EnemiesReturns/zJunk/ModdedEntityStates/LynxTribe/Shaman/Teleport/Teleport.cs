using EnemiesReturns.Behaviors;
using EnemiesReturns.Reflection;
using EntityStates;
using KinematicCharacterController;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport
{
    [RegisterEntityState]
    public class Teleport : BaseState
    {
        public static float minSearchRange => 60f;
        public static float maxSearchrange => float.MaxValue;
        public static int maxSearchIterations => 3;
        public static float searchRangeDeviation => 5f;
        public static float baseDuration => 1f;
        public static float maskFlyDuration => 1.5f;

        public static GameObject ghostMaskPrefab;

        private CharacterModel characterModel;
        private float duration;
        private Vector3 originalPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            var modelTransform = GetModelTransform();
            if (modelLocator)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
            }
            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }

            originalPosition = transform.position;
            var ghostMask = Object.Instantiate(ghostMaskPrefab, transform.position, transform.rotation);
            TeleportAway();

            var moveTowardsMask = ghostMask.GetComponent<MoveTowardsTargetAndDestroyItself>();
            moveTowardsMask.target = transform;
            moveTowardsMask.duration = maskFlyDuration;

            Log.Info($"originalPosition: {originalPosition}, new position: {transform}");
        }

        private void TeleportAway()
        {
            if (!characterBody || !characterBody.hasEffectiveAuthority)
            {
                return;
            }

            var position = PickRandomReachablePosition(minSearchRange, maxSearchrange);
            int searchIteration = 0;
            while (!position.HasValue && searchIteration < maxSearchIterations)
            {
                position = PickRandomReachablePosition(minSearchRange + Random.Range(-searchRangeDeviation, searchRangeDeviation), maxSearchrange + Random.Range(-searchRangeDeviation, searchRangeDeviation));
                searchIteration++;
            }
            // if we still somehow haven't found a position, then I guess do nothing
            if (!position.HasValue)
            {
                return;
            }

            if (characterBody.gameObject.TryGetComponent<KinematicCharacterMotor>(out var motor))
            {
                motor.SetPosition(position.Value, true);
            }
        }

        private Vector3? PickRandomReachablePosition(float minSearchRange, float maxSearchRange)
        {
            var nodeGraph = SceneInfo.instance.GetNodeGraph(characterBody.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
            var nodeList = nodeGraph.FindNodesInRange(characterBody.transform.position, minSearchRange, maxSearchRange, (HullMask)(1 << (int)characterBody.hullClassification));

            NodeGraph.NodeIndex node = nodeList[Random.Range(0, nodeList.Count)];
            if (nodeGraph.GetNodePosition(node, out var position))
            {
                // give it a bit on Y axis since shaman sometimes falls through the ground
                return new Vector3(position.x, position.y + 1f, position.z);
            }

            return null;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > maskFlyDuration)
            {
                if (characterModel)
                {
                    characterModel.invisibilityCount--;
                }
            }
            if (fixedAge > maskFlyDuration + duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
            //if(fixedAge > duration && isAuthority)
            //{
            //    outer.SetNextStateToMain();
            //}
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Stun;
        }
    }
}
