using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.Ifrit.Pillar
{
    public abstract class PillarBaseBody : BodyBase
    {
        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        protected override bool AddFootstepHandler => false;
        protected override bool AddCharacterDirection => false;
        protected override bool AddCharacterMotor => false;
        protected override bool AddCameraTargetParams => false;
        protected override bool AddInteractor => false;
        protected override bool AddInteractionDriver => false;
        protected override bool AddDeathRewards => false;
        protected override bool AddSfxLocator => false;
        protected override bool AddSetStateOnHurt => false;
        protected override bool AddAimAnimator => false;
        protected override string ModelName() => "IfritPillar";

        protected abstract float explosionRadius { get; }

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite);

            var lanternFire = body.transform.Find("ModelBase/IfritPillar/IfritPillarArmture/MainPillar/Chain1.1/Lantern/Fire");
            lanternFire.gameObject.GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matIfritLanternFire", CreateLanternFireMaterial); ;

            #region TeamIndicator

            var teamComponent = body.GetComponent<TeamComponent>();
            var childLocator = body.GetComponentInChildren<ChildLocator>();

            var scaledTransform = body.transform.Find("ModelBase/IfritPillar/ScaledOnInit");
            var osc = scaledTransform.gameObject.AddComponent<ObjectScaleCurve>();
            osc.useOverallCurveOnly = false;
            osc.resetOnAwake = true;
            osc.useUnscaledTime = false;
            osc.timeMax = 3f;
            osc.curveX = acdLookup["acdLinearCurve"].curve;
            osc.curveY = acdLookup["acdLinearCurve"].curve;
            osc.curveZ = acdLookup["acdLinearCurve"].curve;
            osc.overallCurve = acdLookup["acdTeamIndicatorOverallCurve"].curve;

            var indicatorObject = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, FullSphere.prefab").WaitForCompletion());
            indicatorObject.GetComponent<TeamAreaIndicator>().teamComponent = teamComponent;
            indicatorObject.transform.parent = scaledTransform;
            var teamIndicatorScale = 18.75f * (explosionRadius / 30f);
            indicatorObject.transform.localScale = new Vector3(teamIndicatorScale, teamIndicatorScale, teamIndicatorScale); // 18.75 is equal to 30 explosion radius after rescaling the model
            indicatorObject.transform.localPosition = Vector3.zero;
            indicatorObject.transform.localRotation = Quaternion.identity;

            ArrayUtils.ArrayAppend(ref childLocator.transformPairs, new ChildLocator.NameTransformPair { name = "TeamAreaIndicator", transform = indicatorObject.transform });
            #endregion

            #region Fireball
            Transform fireball = body.transform.Find("ModelBase/IfritPillar/Fireball");

            var fire = fireball.Find("Fire");
            var fireParticleSystem = fire.gameObject.GetComponent<ParticleSystem>();
            var fireRenderer = fireParticleSystem.GetComponent<Renderer>();
            fireRenderer.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniExplosion1.mat").WaitForCompletion();

            var fireLightTransform = fireball.Find("Light");
            var fireFlickerLight = fireLightTransform.gameObject.AddComponent<FlickerLight>();
            fireFlickerLight.light = fireLightTransform.gameObject.GetComponent<Light>();
            fireFlickerLight.sinWaves = new Wave[]
            {
                new Wave
                {
                    amplitude = 0.1f,
                    frequency = 4,
                    cycleOffset = 1.23f
                },
                new Wave
                {
                    amplitude = 0.2f,
                    frequency = 3,
                    cycleOffset = 1.34f
                },
                new Wave
                {
                    amplitude = 0.2f,
                    frequency = 5,
                    cycleOffset = 0f
                },
            };

            var fireLightIntencityCurve = fireLightTransform.gameObject.AddComponent<LightIntensityCurve>();
            fireLightIntencityCurve.curve = acdLookup["adcIfritPylonLightIntencityCurve"].curve;
            fireLightIntencityCurve.timeMax = EnemiesReturns.Configuration.Ifrit.PillarExplosionRadius.Value;
            #endregion

            return body;
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master)
        {
            return CreateCard(new SpawnCardParams(name, master, 0));
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/IfritPillar/IfritPillarArmture/TopTransform",
                pathToPoint1 = "ModelBase/IfritPillar/IfritPillarArmture/BaseTransform"
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_IFRIT_PYLON_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                bodyFlags = CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.HasBackstabImmunity,
                baseMaxHealth = EnemiesReturns.Configuration.Ifrit.PillarBodyBaseMaxHealth.Value,
                baseMoveSpeed = 0f,
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.Ifrit.PillarBodyLevelMaxHealth.Value,
                levelMoveSpeed = 0f,
                hullClassification = HullClassification.Golem
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var skinnedRenderers = modelPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            var baseRendererInfos = Array.ConvertAll(skinnedRenderers, (item) =>
            {
                return new CharacterModel.RendererInfo
                {
                    renderer = item,
                    defaultMaterial = item.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                };
            });
            var renderers = modelPrefab.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = Array.ConvertAll(renderers, (item) =>
            {
                return new CharacterModel.RendererInfo
                {
                    renderer = item,
                    defaultMaterial = item.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                };
            });
            baseRendererInfos = ArrayUtils.Join(baseRendererInfos, rendererInfos);
            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = baseRendererInfos,
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var skinnedRenderers = modelPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            var baseRendererInfos = Array.ConvertAll(skinnedRenderers, (item) =>
            {
                return new CharacterModel.RendererInfo
                {
                    renderer = item,
                    defaultMaterial = item.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                };
            });
            var renderers = modelPrefab.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = Array.ConvertAll(renderers, (item) =>
            {
                return new CharacterModel.RendererInfo
                {
                    renderer = item,
                    defaultMaterial = item.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                };
            });
            baseRendererInfos = ArrayUtils.Join(baseRendererInfos, rendererInfos);
            SkinDefs.Default = Utils.CreateSkinDef("skinIfritPillarDefault", modelPrefab, baseRendererInfos);
            return new SkinDef[] { SkinDefs.Default };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return Array.Empty<IEntityStateMachine.EntityStateMachineParams>();
        }

        protected override IFootStepHandler.FootstepHandlerParams FootstepHandlerParams()
        {
            return null;
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return Array.Empty<IGenericSkill.GenericSkillParams>();
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsIfritPillar";

            #region PartyHat
            if (Items.PartyHat.PartyHatFactory.ShouldThrowParty())
            {
                var displayRuleGroupPartyHat = new DisplayRuleGroup();
                displayRuleGroupPartyHat.AddDisplayRule(new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Items.PartyHat.PartyHatFactory.PartyHatDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "LineOriginPoint",
                    localPos = new Vector3(0.79973F, 2.67964F, 1.74035F),
                    localAngles = new Vector3(31.03067F, 0F, 0F),
                    localScale = new Vector3(0.42499F, 0.42499F, 0.42499F),
                    limbMask = LimbFlags.None
                });
                ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupPartyHat,
                    keyAsset = Content.Items.PartyHat
                });
            }
            #endregion

            return idrs;
        }

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 15f,
                maxDistance = 50f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Golem/sdGolem.asset").WaitForCompletion();

        public Material CreateLanternFireMaterial()
        {
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC2/helminthroost/Assets/matHRFireStaticRedLArge.mat").WaitForCompletion());
            material.name = "matIfritLanternFire";
            material.SetFloat("_DepthOffset", -10f);

            return material;
        }
    }
}
