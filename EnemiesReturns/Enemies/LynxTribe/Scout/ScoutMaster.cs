using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Scout
{
    public class ScoutMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("ChaseAndDoubleSlashOffNodegraphWhileSlowingDown")
                {
                    skillSlot = SkillSlot.Primary,
                    minDistance = 0f,
                    maxDistance = 3f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    moveInputScale = 0.4f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    ignoreNodeGraph = true,
                    driverUpdateTimerOverride = 0.5f
                },
                new IAISkillDriver.AISkillDriverParams("ChaseAndDoubleSlashOffNodegraph")
                {
                    skillSlot = SkillSlot.Primary,
                    minDistance = 0f,
                    maxDistance = 6f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    ignoreNodeGraph = true,
                    driverUpdateTimerOverride = 0.5f
                },
                new IAISkillDriver.AISkillDriverParams("ChaseOffNodegraph")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = 7f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    ignoreNodeGraph = true
                },
                new IAISkillDriver.AISkillDriverParams("PathFromAfar")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = float.PositiveInfinity,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                }
            };
        }
    }
}
