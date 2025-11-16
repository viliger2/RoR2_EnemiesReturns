using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Drone;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.MechanicalSpider.Drone
{
    public class MechanicalSpiderDroneBody : MechanicalSpiderBodyBase
    {
        public struct SkinDefs
        {
            public static SkinDef Minion;
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

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite);

            body.AddComponent<MechanicalSpiderVictoryDanceController>().body = body.GetComponent<CharacterBody>();

            return body;
        }

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

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "DoubleShot", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(MechanicalSpiderBodyBase.SkillFamilies.Utility, "Dash", SkillSlot.Utility)
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MechanicalSpider").gameObject.GetComponent<SkinnedMeshRenderer>();
            modelRenderer.material = ContentProvider.MaterialCache["matMechanicalSpiderMinion"];

            var baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = baseRendererInfos
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MechanicalSpider").gameObject.GetComponent<SkinnedMeshRenderer>();
            modelRenderer.material = ContentProvider.MaterialCache["matMechanicalSpiderMinion"];

            var baseRenderInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                }
            };

            SkinDefs.Minion = Utils.CreateSkinDef("skinMechanicalSpiderMinion", modelPrefab, baseRenderInfos);

            return new SkinDef[] { SkinDefs.Minion };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.Death.DeathDrone)));
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            var bodyParams = base.CharacterBodyParams(aimOrigin, icon);
            bodyParams.baseRegen = Configuration.MechanicalSpider.DroneBaseRegen.Value;
            bodyParams.levelRegen = Configuration.MechanicalSpider.DroneLevelRegen.Value;
            bodyParams.lavaCooldown = 1f;
            bodyParams.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            return bodyParams;
        }

    }
}
