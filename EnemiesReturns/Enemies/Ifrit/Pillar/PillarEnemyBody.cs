using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Behaviors;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
