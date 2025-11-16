using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using UnityEngine;

namespace EnemiesReturns.Enemies.MechanicalSpider.Drone
{
    public class MechanicalSpiderDroneMaster : MasterBase
    {
        public static GameObject MasterPrefab;

        protected override bool AddAIOwnership => true;

        protected override bool AddSetDontDestoyOnLoad => true;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("HardLeashToLeaderWhileShooting")
                {
                    skillSlot = RoR2.SkillSlot.Primary,
                    minDistance = 60f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                    selectionRequiresAimTarget = true,
                    selectionRequiresTargetLoS = true,
                    activationRequiresAimConfirmation = true,
                    driverUpdateTimerOverride = 3f,
                    resetCurrentEnemyOnNextDriverSelection = true
                },
                new IAISkillDriver.AISkillDriverParams("HardLeashToLeader")
                {
                    minDistance = 60f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                    driverUpdateTimerOverride = 3f,
                    resetCurrentEnemyOnNextDriverSelection = true
                },
                new IAISkillDriver.AISkillDriverParams("SoftLeashToLeaderWhileShooting")
                {
                    skillSlot = RoR2.SkillSlot.Primary,
                    minDistance = 20f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                    selectionRequiresAimTarget = true,
                    selectionRequiresTargetLoS = true,
                    activationRequiresAimConfirmation = true,
                    driverUpdateTimerOverride = 0.05f,
                    resetCurrentEnemyOnNextDriverSelection = true
                },
                new IAISkillDriver.AISkillDriverParams("SoftLeashToLeader")
                {
                    minDistance = 20f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                    driverUpdateTimerOverride = 0.05f,
                    resetCurrentEnemyOnNextDriverSelection = true
                },
                new IAISkillDriver.AISkillDriverParams("StrafeAndShoot")
                {
                    skillSlot = RoR2.SkillSlot.Primary,
                    requireSkillReady = true,
                    minDistance= 0f,
                    maxDistance = 60f,
                    selectionRequiresTargetLoS = true,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    activationRequiresTargetLoS = true,
                    activationRequiresAimConfirmation = true,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy
                },
                new IAISkillDriver.AISkillDriverParams("IdleNearLeaderWhenNoEnemies")
                {
                    minDistance = 0f,
                    maxDistance = 20f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                },
                new IAISkillDriver.AISkillDriverParams("ReturnToLeaderWhenNoEnemies")
                {
                    minDistance = 0f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
                    driverUpdateTimerOverride = 0.05f,
                    resetCurrentEnemyOnNextDriverSelection = true
                }
            };
        }
    }
}
