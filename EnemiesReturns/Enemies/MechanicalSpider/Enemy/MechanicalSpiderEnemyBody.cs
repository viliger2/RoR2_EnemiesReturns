using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Enemy;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.MechanicalSpider.Enemy
{
    public class MechanicalSpiderEnemyBody : MechanicalSpiderBodyBase
    {
        public struct SkinDefs
        {
            public static SkinDef Default;
            public static SkinDef Grassy;
            public static SkinDef Snowy;
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

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscMechanicalSpiderDefault;
            public static CharacterSpawnCard cscMechanicalSpiderGrassy;
            public static CharacterSpawnCard cscMechanicalSpiderSnowy;
        }

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite, log);

            return body;
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            var esmParams = base.EntityStateMachineParams();

            HG.ArrayUtils.ArrayAppend(ref esmParams, new IEntityStateMachine.EntityStateMachineParams()
            {
                name = "Body",
                initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.SpawnState)),
                mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.MainState))
            });

            return esmParams;
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "DoubleShot", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(MechanicalSpiderBodyBase.SkillFamilies.Utility, "Dash", SkillSlot.Utility)
            };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                aliveLoopStart = "ER_Spider_Alive_Loop_Play",
                aliveLoopStop = "ER_Spider_Alive_Loop_Stop"
            };
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

            var bsaeRenderInfos = new CharacterModel.RendererInfo[] { new CharacterModel.RendererInfo
            {
                renderer = modelRenderer,
                defaultMaterial = modelRenderer.material,
                ignoreOverlays = false,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                hideOnDeath = false
            } };

            SkinDefs.Default = Utils.CreateSkinDef("skinMechanicalSpiderDefault", modelPrefab, bsaeRenderInfos);

            CharacterModel.RendererInfo[] snowyRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matMechanicalSpiderSnowy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
            };
            SkinDefs.Snowy = Utils.CreateSkinDef("skinMechanicalSpiderSnowy", modelPrefab, snowyRender, SkinDefs.Default);

            CharacterModel.RendererInfo[] grassyRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matMechanicalSpiderGrassy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
            };
            SkinDefs.Grassy = Utils.CreateSkinDef("skinMechanicalSpiderGrassy", modelPrefab, grassyRender, SkinDefs.Default);

            return new SkinDef[] { SkinDefs.Default, SkinDefs.Snowy, SkinDefs.Grassy };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.Death.DeathInitial)));
        }
    }
}
