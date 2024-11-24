using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using UnityEngine;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("SummonPylon")
                {
                    skillSlot = RoR2.SkillSlot.Special,
                    requireSkillReady = true,
                    maxUserHealthFraction = 0.6f,
                    minDistance = 0f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                },
                new IAISkillDriver.AISkillDriverParams("Hellzone")
                {
                    skillSlot = RoR2.SkillSlot.Secondary,
                    requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 50f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    moveInputScale = 0f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    noRepeat = true
                },
                new IAISkillDriver.AISkillDriverParams("FlameCharge")
                {
                    skillSlot = RoR2.SkillSlot.Utility,
                    requireSkillReady = true,
                    minDistance = 7f,
                    maxDistance = 60f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    activationRequiresAimConfirmation = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    ignoreNodeGraph = true,
                    noRepeat = true
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
