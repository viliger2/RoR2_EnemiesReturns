using EnemiesReturns.Components;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace EnemiesReturns.Enemies.ArcherBug
{
    public class ArcherBugMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IBaseAI.BaseAIParams BaseAIParams()
        {
            var aiParams = base.BaseAIParams();
            aiParams.graphType = RoR2.Navigation.MapNodeGroup.GraphType.Air;
            aiParams.aimVectorDampTime = 0.1f;
            return base.BaseAIParams();
        }

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            var stopState = new IAISkillDriver.AISkillDriverParams("StopStep")
            {
                moveTargetType = AISkillDriver.TargetType.CurrentEnemy,
                activationRequiresTargetLoS = true,
                movementType = AISkillDriver.MovementType.Stop,
                aimType = AISkillDriver.AimType.AtCurrentEnemy,
                driverUpdateTimerOverride = 1f,
                noRepeat = true
            };
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("PathFromAfar")
                {
                    minDistance = 50f,
                    moveTargetType = AISkillDriver.TargetType.CurrentEnemy,
                    movementType = AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = AISkillDriver.AimType.AtMoveTarget,
                    moveInputScale = 0.5f,
                },
                new IAISkillDriver.AISkillDriverParams("ShootStep")
                {
                    skillSlot = SkillSlot.Primary,
                    requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 30f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    movementType = AISkillDriver.MovementType.Stop,
                    aimType = AISkillDriver.AimType.AtCurrentEnemy,
                    driverUpdateTimerOverride = 0.5f,
                    noRepeat = true,
                    nextHighPriorityOverride = stopState
                },
                new IAISkillDriver.AISkillDriverParams("FollowStep")
                {
                    minDistance = 30f,
                    maxDistance = 50f,
                    moveTargetType = AISkillDriver.TargetType.CurrentEnemy,
                    movementType = AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = AISkillDriver.AimType.AtCurrentEnemy,
                    driverUpdateTimerOverride = 0.5f,
                    noRepeat = true,
                    nextHighPriorityOverride = stopState
                },
                new IAISkillDriver.AISkillDriverParams("StrafeStep")
                {
                    minDistance = 0f,
                    maxDistance = 30f,
                    moveTargetType = AISkillDriver.TargetType.CurrentEnemy,
                    movementType = AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = AISkillDriver.AimType.AtCurrentEnemy,
                    driverUpdateTimerOverride = 0.5f,
                    noRepeat = true,
                    nextHighPriorityOverride = stopState
                },
                stopState
            };
        }
    }
}
