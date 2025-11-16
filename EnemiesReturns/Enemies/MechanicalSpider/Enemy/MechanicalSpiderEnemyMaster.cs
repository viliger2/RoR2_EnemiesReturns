using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using UnityEngine;

namespace EnemiesReturns.Enemies.MechanicalSpider.Enemy
{
    public class MechanicalSpiderEnemyMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("StrafeAndShoot")
                {
                    skillSlot = RoR2.SkillSlot.Primary,
                    minDistance = 0f,
                    maxDistance = 60f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    activationRequiresAimConfirmation = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                },
                new IAISkillDriver.AISkillDriverParams("ChaseOffNodegraph")
                {
                    skillSlot = RoR2.SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = 7f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    ignoreNodeGraph = true
                },
                new IAISkillDriver.AISkillDriverParams("DashToTraverse")
                {
                    skillSlot = RoR2.SkillSlot.Utility,
                    requireSkillReady = true,
                    minDistance = 50f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
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
