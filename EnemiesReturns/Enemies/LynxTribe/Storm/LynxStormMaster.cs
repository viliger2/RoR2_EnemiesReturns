using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Storm
{
    public class LynxStormMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override bool AddAIOwnership => true;

        protected override IBaseAI.BaseAIParams BaseAIParams()
        {
            var aiparams = base.BaseAIParams();
            aiparams.fullVision = true;
            return aiparams;
        }

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
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
