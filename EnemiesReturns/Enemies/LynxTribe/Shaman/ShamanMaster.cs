using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman
{
    public class ShamanMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("SummonProjectiles")
                {
                    skillSlot = RoR2.SkillSlot.Primary,
                    requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 40f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresAimConfirmation = true,
                    activationRequiresAimTargetLoS = true,
                    activationRequiresTargetLoS = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    selectionRequiresAimTarget = true,
                    selectionRequiresTargetLoS = true,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy
                },
                //new IAISkillDriver.AISkillDriverParams("SummonStorm")
                //{
                //    skillSlot = RoR2.SkillSlot.Special,
                //    requireSkillReady = true,
                //    minDistance = 0f,
                //    maxDistance = 120f,
                //    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                //    activationRequiresTargetLoS = true,
                //    activationRequiresAimConfirmation = true,
                //    activationRequiresAimTargetLoS = true,
                //    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                //    selectionRequiresTargetLoS = true,
                //    selectionRequiresAimTarget = true,
                //    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy
                //},
                //new IAISkillDriver.AISkillDriverParams("WalkAway")
                //{
                //    skillSlot = RoR2.SkillSlot.None,
                //    minDistance = 0f,
                //    maxDistance = 50f,
                //    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                //    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget,
                //    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                //    driverUpdateTimerOverride = 3f
                //},
                new IAISkillDriver.AISkillDriverParams("StrafeAtDistance")
                {
                    skillSlot = RoR2.SkillSlot.None,
                    requireSkillReady = false, 
                    minDistance = 40f,
                    maxDistance = 60f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy
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
