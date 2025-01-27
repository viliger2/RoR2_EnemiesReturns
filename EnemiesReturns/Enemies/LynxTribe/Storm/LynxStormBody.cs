using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using EntityStates.Fauna;
using HG;
using KinematicCharacterController;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Networking;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Storm
{
    public class LynxStormBody : BodyBase
    {
        public static GameObject BodyPrefab;

        public static CharacterSpawnCard cscLynxStorm;

        protected override bool AddAimAssistScale => false;
        protected override bool AddModelSkinController => false;
        protected override bool AddInteractor => false;
        protected override bool AddInteractionDriver => false;
        protected override bool AddFootstepHandler => false;
        protected override bool AddSkills => false;
        protected override bool AddDeathRewards => false;
        protected override bool AddSetStateOnHurt => false;
        protected override bool AddAimAnimator => false;
        protected override bool AddHitBoxes => false;
        protected override bool AddHurtBoxes => false;
        protected override bool AddAnimationEvents => false;

        public CharacterSpawnCard CreateCard(string name, GameObject master)
        {
            return CreateCard(new SpawnCardParams(name, master, 0)
            {
                hullSize = HullClassification.Human,
                occupyPosition = true
            });
        }

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var body = base.AddBodyComponents(bodyPrefab, acdLookup);

            #region TeamIndicator

            var teamComponent = body.GetComponent<TeamComponent>();
            var childLocator = body.GetComponentInChildren<ChildLocator>();

            var scaledTransform = body.transform.Find("ModelBase/mdlStorm/ScaleOnInit");
            var osc = scaledTransform.gameObject.AddComponent<ObjectScaleCurve>();
            osc.useOverallCurveOnly = false;
            osc.resetOnAwake = true;
            osc.useUnscaledTime = false;
            osc.timeMax = Mathf.Max(4f, 1f);
            osc.curveX = acdLookup["acdLinearCurve"].curve;
            osc.curveY = acdLookup["acdLinearCurve"].curve;
            osc.curveZ = acdLookup["acdLinearCurve"].curve;
            osc.overallCurve = acdLookup["acdTeamIndicatorOverallCurve"].curve;

            var indicatorObject = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, GroundOnly.prefab").WaitForCompletion());
            indicatorObject.GetComponent<TeamAreaIndicator>().teamComponent = teamComponent;
            indicatorObject.transform.parent = scaledTransform;
            var teamIndicatorScale = 18.75f * (Configuration.LynxTribe.LynxTotem.SummonStormRadius.Value / 20f);
            indicatorObject.transform.localScale = new Vector3(teamIndicatorScale, teamIndicatorScale, teamIndicatorScale); // 18.75 is equal to 30 explosion radius after rescaling the model
            indicatorObject.transform.localPosition = Vector3.zero;
            indicatorObject.transform.localRotation = Quaternion.identity;

            var sphereIndicator = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, FullSphere.prefab").WaitForCompletion());
            sphereIndicator.GetComponent<TeamAreaIndicator>().teamComponent = teamComponent;
            sphereIndicator.transform.parent = scaledTransform;
            teamIndicatorScale = 3f * (Configuration.LynxTribe.LynxTotem.SummonStormGrabRange.Value / 3f);
            sphereIndicator.transform.localScale = new Vector3(teamIndicatorScale, teamIndicatorScale, teamIndicatorScale);
            sphereIndicator.transform.localPosition = Vector3.zero;
            sphereIndicator.transform.localRotation = Quaternion.identity;

            ArrayUtils.ArrayAppend(ref childLocator.transformPairs, new ChildLocator.NameTransformPair { name = "TeamAreaIndicator", transform = indicatorObject.transform });
            #endregion

            var particlesTransform = body.transform.Find("ModelBase/mdlStorm/StormParticles");
            var stormOutside = particlesTransform.Find("StormOutside");
            stormOutside.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeaf.mat").WaitForCompletion();
            bodyPrefab.AddComponent<DetachParticleOnDestroyAndEndEmission>().particleSystem = stormOutside.GetComponent<ParticleSystem>();

            var stormMiddle = particlesTransform.Find("StormMiddle");
            stormMiddle.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeafAlt.mat").WaitForCompletion();
            bodyPrefab.AddComponent<DetachParticleOnDestroyAndEndEmission>().particleSystem = stormMiddle.GetComponent<ParticleSystem>();

            var stormImside = particlesTransform.Find("StormInside");
            stormImside.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matTreebotTreeLeaf2", CreateTreebotTreeLeaf2Material);
            bodyPrefab.AddComponent<DetachParticleOnDestroyAndEndEmission>().particleSystem = stormImside.GetComponent<ParticleSystem>();

            var tornadoLow = particlesTransform.Find("TornadoMeshLow");
            tornadoLow.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxStormTornadoGreen", CreateStormGreenMaterial);
            bodyPrefab.AddComponent<DetachParticleOnDestroyAndEndEmission>().particleSystem = tornadoLow.GetComponent<ParticleSystem>();

            var tornadoMiddle = particlesTransform.Find("TornadoMeshMiddle");
            tornadoMiddle.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxStormTornadoGreen", CreateStormGreenMaterial);
            bodyPrefab.AddComponent<DetachParticleOnDestroyAndEndEmission>().particleSystem = tornadoMiddle.GetComponent<ParticleSystem>();

            var tornadoUp = particlesTransform.Find("TornadoMeshUp");
            tornadoUp.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxStormTornadoGreen", CreateStormGreenMaterial);
            bodyPrefab.AddComponent<DetachParticleOnDestroyAndEndEmission>().particleSystem = tornadoUp.GetComponent<ParticleSystem>();

            particlesTransform.Find("SummonParticles").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/PassiveHealing/matWoodSpriteFlare.mat").WaitForCompletion();
            particlesTransform.Find("SummonParticles").GetComponent<ParticleSystemRenderer>().trailMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoDiseaseTrail.mat").WaitForCompletion();

            return body;
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams();
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_LYNX_STORM_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                baseMoveSpeed = Configuration.LynxTribe.LynxTotem.SummonStormStormMoveSpeed.Value,
                baseJumpCount = 0,
                baseJumpPower = 0f,
                baseDamage = EnemiesReturns.Configuration.LynxTribe.LynxTotem.BaseDamage.Value,
                levelDamage = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LevelDamage.Value,
                bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage | CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.IgnoreKnockback | CharacterBody.BodyFlags.ImmuneToLava
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            return Array.Empty<SkinDef>();
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Storm.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Storm.MainState))
                }
            };
        }

        protected override ICharacterMotor.CharacterMotorParams CharacterMotorParams()
        {
            return new ICharacterMotor.CharacterMotorParams()
            {
                doNotTriggerJumpVolumes = true
            };
        }

        protected override IFootStepHandler.FootstepHandlerParams FootstepHandlerParams()
        {
            return new IFootStepHandler.FootstepHandlerParams();
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return Array.Empty<IGenericSkill.GenericSkillParams>();
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsLynxStorm";
            return idrs;
        }

        protected override string ModelName() => "mdlStorm";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams();
        }

        protected override SurfaceDef SurfaceDef() => null;

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Storm.DeathState)));
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                aliveLoopStart = "ER_Lynx_Storm_Alive_Loop_Play",
                aliveLoopStop = "ER_Lynx_Storm_Alive_Loop_Stop",
            };
        }

        public static Material CreateTreebotTreeLeaf2Material()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeaf.mat").WaitForCompletion());
            material.name = "matTreebotTreeLeaf2";
            material.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Treebot/texTreebotLeafDiffuse.png").WaitForCompletion());

            return material;
        }

        public static Material CreateStormGreenMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC2/Halcyonite/matWhirlWindHalcyoniteGhost1.mat").WaitForCompletion());
            material.name = "matLynxStormTornadoGreen";
            material.SetColor("_TintColor", new Color(43f / 255f, 99f / 255f, 38f / 255f));

            return material;
        }
        protected override IModelLocator.ModelLocatorParams ModelLocatorParams()
        {
            return new IModelLocator.ModelLocatorParams()
            {
                dontReleaseModelOnDeath = true
            };
        }
    }
}
