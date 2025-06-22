using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace EnemiesReturns.Components.MasterComponents
{
    public interface IBaseAI
    {
        protected class BaseAIParams
        {
            public BaseAIParams(RoR2.Navigation.MapNodeGroup.GraphType graphType, EntityStates.SerializableEntityStateType scanState)
            {
                this.graphType = graphType;
                this.scanState = scanState;
            }

            public bool fullVision = false; // stationary and drones have it set to true
            public bool neverRetaliateFriendlies = true;
            public float enemyAttentionDuration = 5f;
            public RoR2.Navigation.MapNodeGroup.GraphType graphType;
            public EntityStates.SerializableEntityStateType scanState;
            public bool prioritizePlayers = false;
            public bool isHealer = false;
            public float enemyAttention = 0f;
            public float aimVectorDampTime = 0.05f;
            public float aimVectorMaxSpeed = 180f;
            public bool showDebugStateChanges = false;
        }

        protected bool NeedToAddBaseAI();

        protected BaseAIParams GetBaseAIParams();

        protected BaseAI AddBaseAI(GameObject masterPrefab, EntityStateMachine esm, BaseAIParams aiParams)
        {
            BaseAI baseAI = null;
            if (NeedToAddBaseAI())
            {
                baseAI = masterPrefab.GetOrAddComponent<BaseAI>();
                baseAI.fullVision = aiParams.fullVision;
                baseAI.neverRetaliateFriendlies = aiParams.neverRetaliateFriendlies;
                baseAI.enemyAttentionDuration = aiParams.enemyAttentionDuration;
                baseAI.desiredSpawnNodeGraphType = aiParams.graphType;
                baseAI.stateMachine = esm;
                baseAI.scanState = aiParams.scanState;
                baseAI.isHealer = aiParams.isHealer;
                baseAI.enemyAttention = aiParams.enemyAttention;
                baseAI.aimVectorDampTime = aiParams.aimVectorDampTime;
                baseAI.aimVectorMaxSpeed = aiParams.aimVectorMaxSpeed;
                baseAI.showDebugStateChanges = aiParams.showDebugStateChanges;
                baseAI.prioritizePlayers = aiParams.prioritizePlayers;
            }
            return baseAI;
        }
    }
}
