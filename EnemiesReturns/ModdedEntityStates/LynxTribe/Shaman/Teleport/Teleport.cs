using EntityStates;
using KinematicCharacterController;
using RoR2;
using RoR2.Navigation;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman.Teleport
{
    // TODO: add animation from teleport to idle
    public class Teleport : BaseState
    {
        public static float minSearchRange => EnemiesReturns.Configuration.LynxTribe.LynxShaman.TeleportMinRange.Value;
        public static float maxSearchrange => EnemiesReturns.Configuration.LynxTribe.LynxShaman.TeleportMaxRange.Value;
        public static int maxSearchIterations => 3;
        public static float searchRangeDeviation => 5f;
        public static float baseDuration => 1f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            TeleportAway();
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
            if(!position.HasValue)
            {
                return;
            }

            if(characterBody.gameObject.TryGetComponent<KinematicCharacterMotor>(out var motor))
            {
                motor.SetPosition(position.Value, true);
            }
        }

        private Vector3? PickRandomReachablePosition(float minSearchRange, float maxSearchRange)
        {
            var nodeGraph = SceneInfo.instance.GetNodeGraph(characterBody.isFlying ? RoR2.Navigation.MapNodeGroup.GraphType.Air : RoR2.Navigation.MapNodeGroup.GraphType.Ground);
            var nodeList = nodeGraph.FindNodesInRange(characterBody.transform.position, minSearchRange, maxSearchRange, (HullMask)(1 << (int)characterBody.hullClassification));

            NodeGraph.NodeIndex node = nodeList[UnityEngine.Random.Range(0, nodeList.Count)];
            if (nodeGraph.GetNodePosition(node, out var position))
            {
                return position;
            }

            return null;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
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
