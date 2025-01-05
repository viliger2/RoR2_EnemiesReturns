using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Archer
{
    public class ArcherMaster : MasterBase
    {

        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("Shoot")
                {
                    skillSlot = SkillSlot.Primary,
                    requireSkillReady = true,
                    minDistance = 35f,
                    maxDistance = 60f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    activationRequiresAimConfirmation = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                },
                new IAISkillDriver.AISkillDriverParams("Flee")
                {
                    minDistance = 0f,
                    maxDistance = 35f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    shouldSprint = true,
                    driverUpdateTimerOverride = 3f,
                    noRepeat = true
                },
                new IAISkillDriver.AISkillDriverParams("Path")
                {
                    minDistance = 30f,
                    maxDistance = float.PositiveInfinity,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                },
                new IAISkillDriver.AISkillDriverParams("PathStrafe")
                {
                    minDistance = 0f,
                    maxDistance = 30f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                }
            };
        }
    }
}
