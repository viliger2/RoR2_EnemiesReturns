using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using IL.RoR2.Skills;
using Rewired.Utils.Interfaces;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
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
            var stopState = new IAISkillDriver.AISkillDriverParams("Stop")
            {               
                minDistance = 0,                
                activationRequiresTargetLoS = true,
                movementType = AISkillDriver.MovementType.Stop,                              
                driverUpdateTimerOverride = 2,
                noRepeat = true,      
            };
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("PathFromAfar")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = float.PositiveInfinity,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    driverUpdateTimerOverride = 0.5f,
                },
                new IAISkillDriver.AISkillDriverParams("Shoot")
                {

                },
                new IAISkillDriver.AISkillDriverParams("Follow")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 25f,
                    maxDistance = 50f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    driverUpdateTimerOverride = 0.5f,
                },
                new IAISkillDriver.AISkillDriverParams("Strafe")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 10f,
                    maxDistance = 25f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    driverUpdateTimerOverride = 0.5f,
                },
                new IAISkillDriver.AISkillDriverParams("Flee")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = 10f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.FleeMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                    driverUpdateTimerOverride = 0.5f,
                    nextHighPriorityOverride = 

                },
               


    };
        }


    }
}
