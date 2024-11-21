using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Enemies.Spitter
{
    public class SpitterMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        public override GameObject AddMasterComponents(GameObject masterPrefab, GameObject bodyPrefab)
        {
            var master = (this as IMaster).CreateMaster(masterPrefab, bodyPrefab);

            return master;
        }

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("ChaseAndBiteOffNodegraphWhileSlowingDown")
                {
                    skillSlot = SkillSlot.Secondary,
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
                new IAISkillDriver.AISkillDriverParams("ChaseAndBiteOffNodegraph")
                {
                    skillSlot = SkillSlot.Secondary,
                    minDistance = 0f,
                    maxDistance = 6f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    ignoreNodeGraph = true,
                    driverUpdateTimerOverride = 0.5f
                },
                new IAISkillDriver.AISkillDriverParams("StrafeAndShootChargedSpit")
                {
                    skillSlot = SkillSlot.Special,
                    minDistance = 15f,
                    maxDistance = 60f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresAimConfirmation = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    moveInputScale = 0.7f,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
                },
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
