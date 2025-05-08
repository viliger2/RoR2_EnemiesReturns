using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
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
            return base.BaseAIParams();
            
        }

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("PathFromAfar")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = float.PositiveInfinity,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                },
                new IAISkillDriver.AISkillDriverParams("StrafeAndShootCausticSpit")
                {
                    skillSlot = SkillSlot.Primary,
                    minDistance = 15f,
                    maxDistance = 60f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresAimConfirmation = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    moveInputScale = 0.7f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                },
            };
        }


    }
}
