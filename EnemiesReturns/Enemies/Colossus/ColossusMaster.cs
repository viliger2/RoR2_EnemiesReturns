using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using UnityEngine;

namespace EnemiesReturns.Enemies.Colossus
{
    public class ColossusMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("StoneClap")
                {
                    skillSlot = RoR2.SkillSlot.Secondary,
                    requireSkillReady = true,
                    maxUserHealthFraction = 0.95f,
                    minDistance = 0f,
                    maxDistance = 100f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    noRepeat = true
                },
                new IAISkillDriver.AISkillDriverParams("Stomp")
                {
                    skillSlot = RoR2.SkillSlot.Primary,
                    requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 90f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                },
                new IAISkillDriver.AISkillDriverParams("LaserBarrage")
                {
                    skillSlot= RoR2.SkillSlot.Utility,
                    requireSkillReady = true,
                    maxUserHealthFraction = 0.60f,
                    minDistance = 0f,
                    maxDistance = 100f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.MoveDirection
                },
                new IAISkillDriver.AISkillDriverParams("ChaseOffNodegraph")
                {
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
                    minDistance = 0f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                }
            };
        }
    }
}
