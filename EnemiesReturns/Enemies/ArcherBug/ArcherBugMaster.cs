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
         //   var stopState = new IAISkillDriver.AISkillDriverParams("Stop")
         //   {               
         //       minDistance = 30f,  
         //      maxDistance = 50f,
         //       activationRequiresTargetLoS = true,
         //       movementType = AISkillDriver.MovementType.Stop,
         //       aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,
         //       driverUpdateTimerOverride = 1f,
         //       noRepeat = true,      
         //   };
            return new IAISkillDriver.AISkillDriverParams[]
            {
                new IAISkillDriver.AISkillDriverParams("PathFromAfar")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 90f,
                    maxDistance = float.PositiveInfinity,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                },
                new IAISkillDriver.AISkillDriverParams("Shoot")
                {
                    skillSlot = SkillSlot.Primary,
                    requireSkillReady = true,
                    selectionRequiresTargetLoS = true,
                    movementType = AISkillDriver.MovementType.Stop,
                    minDistance = 0f,
                    maxDistance = 30f,
                    driverUpdateTimerOverride = 2f,                   
                   // nextHighPriorityOverride = stopState                    
                },
                new IAISkillDriver.AISkillDriverParams("Follow")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 30f,
                    maxDistance = 90f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,                   
                  //  nextHighPriorityOverride = stopState
                },
              new IAISkillDriver.AISkillDriverParams("Strafe")
                {
                    skillSlot = SkillSlot.None,
                    minDistance = 0f,
                    maxDistance = 30f,
                    moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                    movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                    aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy,              
                  //  nextHighPriorityOverride = stopState
                },
              // stopState


    };
        }


    }
}
