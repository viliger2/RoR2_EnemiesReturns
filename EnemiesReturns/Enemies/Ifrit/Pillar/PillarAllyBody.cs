using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.EditorHelpers;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.Enemies.Ifrit.Pillar
{
    public class PillarAllyBody : PillarBaseBody
    {
        public static GameObject BodyPrefab;

        public static CharacterSpawnCard SpawnCard;

        protected override float explosionRadius => EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillRadius.Value;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite, acdLookup);
            var model = body.transform.Find("ModelBase/IfritPillar").gameObject;

            var linerenderer = model.GetComponentInChildren<LineRenderer>();
            UnityEngine.Object.Destroy(linerenderer);

            body.transform.Find("ModelBase/IfritPillar/Fireball/Light").gameObject.SetActive(false);
            body.transform.Find("ModelBase/IfritPillar/Fireball/PP").gameObject.SetActive(false);

            return body;
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            var bodyParams = base.CharacterBodyParams(aimOrigin, icon);
            bodyParams.baseDamage = EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillBodyBaseDamage.Value;
            bodyParams.levelDamage = EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillBodyLevelDamage.Value;
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
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.Player.ChargingExplosion))
                }
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.SuicideDeathState)));
        }
    }
}
