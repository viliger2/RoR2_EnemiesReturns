using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.SandCrab
{
    public class SandCrabMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("ChaseAndSnipOffNodegraphWhileSlowingDown")
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
                new IAISkillDriver.AISkillDriverParams("ChaseAndSnipOffNodegraph")
                {
                    skillSlot = SkillSlot.Primary,
                    minDistance = 0f,
                    maxDistance = 15f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    ignoreNodeGraph = true,
                    driverUpdateTimerOverride = 0.5f
                },
                new IAISkillDriver.AISkillDriverParams("StrafeAndShootBubbles")
                {
                    skillSlot = SkillSlot.Secondary,
                    requireSkillReady = true,
                    minDistance = 15f,
                    maxDistance = 60f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresAimConfirmation = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    moveInputScale = 1f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy
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
