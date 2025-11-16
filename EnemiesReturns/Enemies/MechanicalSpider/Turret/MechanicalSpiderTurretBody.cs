using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Turret;
using EnemiesReturns.Components.BodyComponents.Skills;

namespace EnemiesReturns.Enemies.MechanicalSpider.Turret
{
    public class MechanicalSpiderTurretBody : MechanicalSpiderBodyBase
    {
        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public new struct Skills
        {
            public static SkillDef DoubleShot;
        }

        public new struct SkillFamilies
        {
            public static SkillFamily Primary;
        }

        public static GameObject BodyPrefab;

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            var esmParams = base.EntityStateMachineParams();

            HG.ArrayUtils.ArrayAppend(ref esmParams, new IEntityStateMachine.EntityStateMachineParams()
            {
                name = "Body",
                initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.SpawnStateDrone)),
                mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.MainState))
            });

            return esmParams;
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams();
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MechanicalSpider").gameObject.GetComponent<SkinnedMeshRenderer>();

            var baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = baseRendererInfos
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "DoubleShot", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(MechanicalSpiderBodyBase.SkillFamilies.Utility, "Dash", SkillSlot.Utility)
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MechanicalSpider").gameObject.GetComponent<SkinnedMeshRenderer>();

            var baseRenderInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };

            SkinDefs.Default = Utils.CreateSkinDef("skinMechanicalSpiderTurretDefault", modelPrefab, baseRenderInfos);

            return new SkinDef[] { SkinDefs.Default };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.Death.DeathNormal)));
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            var bodyParams = base.CharacterBodyParams(aimOrigin, icon);
            bodyParams.baseRegen = Configuration.MechanicalSpider.TurretBaseRegen.Value;
            bodyParams.levelRegen = Configuration.MechanicalSpider.TurretLevelRegen.Value;
            bodyParams.baseMoveSpeed = Configuration.MechanicalSpider.TurretBaseMoveSpeed.Value;
            bodyParams.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            bodyParams.lavaCooldown = 1f;
            bodyParams.doNotReassignCollision = true;
            return bodyParams;
        }
    }
}
