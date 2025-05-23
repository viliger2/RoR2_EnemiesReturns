﻿using EnemiesReturns.Components.MasterComponents;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman
{
    public class ShamanMasterAlly : ShamanMaster
    {
        public new static GameObject MasterPrefab;
        protected override bool AddDeployable => true;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            var aiParams = base.AISkillDriverParams();

            HG.ArrayUtils.ArrayAppend(ref aiParams,
                new IAISkillDriver.AISkillDriverParams("IdleNearLeaderWhenNoEnemies")
                {
                    minDistance = 0f,
                    maxDistance = 20f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                }
            );

            HG.ArrayUtils.ArrayAppend(ref aiParams,
                new IAISkillDriver.AISkillDriverParams("ReturnToLeaderWhenNoEnemies")
                {
                    minDistance = 0f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentLeader,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    driverUpdateTimerOverride = 0.05f,
                    resetCurrentEnemyOnNextDriverSelection = true
                }
            );

            return aiParams;
        }
    }
}
