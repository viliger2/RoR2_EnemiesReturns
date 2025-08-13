using EnemiesReturns.Behaviors;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.EditorHelpers;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.PostProcessing;

namespace EnemiesReturns.Enemies.Ifrit.Pillar
{
    public class PillarEnemyBody : PillarBaseBody
    {
        public static GameObject BodyPrefab;

        public static CharacterSpawnCard SpawnCard;

        protected override float explosionRadius => EnemiesReturns.Configuration.Ifrit.PillarExplosionRadius.Value;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite, acdLookup);
            var model = body.transform.Find("ModelBase/IfritPillar").gameObject;

            #region LineRenderer
            var linerenderer = model.GetComponentInChildren<LineRenderer>();
            linerenderer.material = ContentProvider.GetOrCreateMaterial("matIfritPylonLine", CreateLineRendererMaterial);
            #endregion

            #region LineRendererHelper
            model.AddComponent<DeployableLineRendererToOwner>().childOriginName = "LineOriginPoint";
            #endregion

            #region Light
            var light = body.transform.Find("ModelBase/IfritPillar/Fireball/Light").gameObject;
            var flickerLight = light.AddComponent<FlickerLight>();
            flickerLight.light = light.GetComponent<Light>();
            flickerLight.sinWaves = new Wave[]
            {
                new Wave()
                {
                    amplitude = 0.1f,
                    frequency = 9f,
                    cycleOffset = 0f,
                },
                new Wave()
                {
                    amplitude = 0.2f,
                    frequency = 0.333333f,
                    cycleOffset = 0f,
                },
                new Wave()
                {
                    amplitude = 0.06f,
                    frequency = 4f,
                    cycleOffset = 0f,
                },
            };

            var ngssLocal = light.AddComponent<NGSS_Local>();
            ngssLocal.NGSS_NO_UPDATE_ON_PLAY = false;
            ngssLocal.NGSS_MULTIPLE_INSTANCES_WARNING = false;

            ngssLocal.NGSS_SAMPLING_TEST = 16;
            ngssLocal.NGSS_SAMPLING_FILTER = 32;
            ngssLocal.NGSS_SAMPLING_DISTANCE = 75;
            ngssLocal.NGSS_NORMAL_BIAS = 0.1f;

            ngssLocal.NGSS_NOISE_TO_DITHERING_SCALE = 0;
            ngssLocal.NGSS_NOISE_TEXTURE = Addressables.LoadAssetAsync<Texture2D>("NGSS/BlueNoise_R8_8.png").WaitForCompletion(); ;

            ngssLocal.NGSS_SHADOWS_OPACITY = 1;
            ngssLocal.NGSS_PCSS_SOFTNESS_NEAR = 0;
            ngssLocal.NGSS_PCSS_SOFTNESS_FAR = 1;

            light.GetComponent<LightRangeScale>().maxDuration = EnemiesReturns.Configuration.Ifrit.PillarExplosionChargeDuration.Value * 0.6f;
            #endregion

            #region PP
            var postProccess = body.transform.Find("ModelBase/IfritPillar/Fireball/PP").gameObject;
            var ppDuration = postProccess.AddComponent<PostProcessDuration>();
            ppDuration.ppVolume = postProccess.GetComponent<PostProcessVolume>();
            ppDuration.ppWeightCurve = acdLookup["acdPillarPP"].curve;
            ppDuration.maxDuration = EnemiesReturns.Configuration.Ifrit.PillarExplosionChargeDuration.Value * 0.6f;
            ppDuration.destroyOnEnd = false;
            ppDuration.useUnscaledTime = false;
            #endregion

            return body;
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            var bodyParams = base.CharacterBodyParams(aimOrigin, icon);
            bodyParams.baseDamage = EnemiesReturns.Configuration.Ifrit.BaseDamage.Value;
            bodyParams.levelDamage = EnemiesReturns.Configuration.Ifrit.LevelDamage.Value;
            return bodyParams;
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.ChargingExplosion))
                }
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.KilledDeathState)));
        }

        public Material CreateLineRendererMaterial()
        {
            var lineMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Captain/matCaptainAirstrikeAltLaser.mat").WaitForCompletion());
            lineMaterial.name = "matIfritPylonLine";
            lineMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampCaptainAirstrike.png").WaitForCompletion());
            lineMaterial.SetColor("_TintColor", new Color(255f / 255f, 53f / 255f, 0f));
            lineMaterial.SetFloat("_Boost", 7.315614f);
            lineMaterial.SetFloat("_AlphaBoost", 5.603551f);
            lineMaterial.SetFloat("_AlphaBias", 0f);
            lineMaterial.SetFloat("_DistortionStrength", 1f);
            lineMaterial.SetVector("_CutoffScroll", new Vector4(5f, 0f, 0f, 0f));

            return lineMaterial;
        }
    }
}
