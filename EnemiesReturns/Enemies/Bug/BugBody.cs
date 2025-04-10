using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.Bug
{
    public class BugBody : BodyBase
    {
        public struct Skills
        {
        }

        public struct SkillFamilies
        {
        }

        public struct SkinDefs
        {
        }

        public struct SpawnCards
        {
        }


        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            throw new NotImplementedException();
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            throw new NotImplementedException();
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            throw new NotImplementedException();
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            throw new NotImplementedException();
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            throw new NotImplementedException();
        }

        protected override IFootStepHandler.FootstepHandlerParams FootstepHandlerParams()
        {
            throw new NotImplementedException();
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            throw new NotImplementedException();
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            throw new NotImplementedException();
        }

        protected override string ModelName()
        {
            throw new NotImplementedException();
        }

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            throw new NotImplementedException();
        }

        protected override SurfaceDef SurfaceDef()
        {
            throw new NotImplementedException();
        }
    }
}
