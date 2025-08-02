using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.Swift
{
    public class SwiftMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override IBaseAI.BaseAIParams BaseAIParams()
        {
            var aiParams = base.BaseAIParams();
            aiParams.graphType = RoR2.Navigation.MapNodeGroup.GraphType.Air;

            return aiParams;
        }

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[] {
                new IAISkillDriver.AISkillDriverParams("Flee")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = 20f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                },
                new IAISkillDriver.AISkillDriverParams("Dive")
                {
                    skillSlot = SkillSlot.Primary,
                    minDistance = 20f,
                    maxDistance = 30f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    activationRequiresAimConfirmation = true,
                    activationRequiresTargetLoS = true,
                    activationRequiresAimTargetLoS = true,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy
                },
                new IAISkillDriver.AISkillDriverParams("PathFromAfar")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 30f,
                    maxDistance = float.PositiveInfinity,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                }
            };
        }
    }
}
