using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class TotemMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("SummonTribe")
                {
                    skillSlot = SkillSlot.Secondary,
                    requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 120f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    moveInputScale = 0f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                },
                new IAISkillDriver.AISkillDriverParams("Groundpound")
                {
                    skillSlot = SkillSlot.Primary,
                    requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 20f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    moveInputScale = 0f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                },
                new IAISkillDriver.AISkillDriverParams("SummonStorms")
                {
                    skillSlot = SkillSlot.Special,
                    requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 120f,
                    maxUserHealthFraction = 0.65f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    moveInputScale = 0f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                },
                new IAISkillDriver.AISkillDriverParams("Burrow")
                {
                    skillSlot = SkillSlot.Utility,
                    //requireSkillReady = true,
                    minDistance = 0f,
                    maxDistance = 120f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    activationRequiresAimTargetLoS = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.Stop,
                    moveInputScale = 0f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                },
                new IAISkillDriver.AISkillDriverParams("PathFromAfar")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 120f,
                    maxDistance = float.PositiveInfinity,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                }
            };
        }
    }
}
