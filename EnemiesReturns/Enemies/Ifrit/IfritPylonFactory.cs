using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Helpers;
using EnemiesReturns.PrefabAPICompat;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritPylonFactory
    {
        public static SpawnCard scIfritPylon;

        public static GameObject IfritPylonBody;

        public static GameObject IfritPylonMaster;

        public GameObject CreateBody(GameObject bodyPrefab)
        {
            Transform modelBase = bodyPrefab.transform.Find("ModelBase");
            Transform modelTransform = bodyPrefab.transform.Find("ModelBase/mdlPylon");
            Transform hurtboxTransform = bodyPrefab.transform.Find("ModelBase/Hurtbox");

            #region PylonBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>();
            #endregion

            #region SkillLocator
            bodyPrefab.AddComponent<SkillLocator>();
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if (!bodyPrefab.TryGetComponent<TeamComponent>(out teamComponent))
            {
                teamComponent = bodyPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.Monster;
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if (!bodyPrefab.TryGetComponent<CharacterBody>(out characterBody))
            {
                characterBody = bodyPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ENEMIES_RETURNS_IFRIT_PYLON_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.HasBackstabImmunity;
            characterBody.baseMaxHealth = 300f; // TODO: stats
            characterBody.baseDamage = 20f;
            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = 90f;
            characterBody.levelDamage = 4f;
            characterBody.hullClassification = HullClassification.Golem;
            #endregion

            #region HealthComponent
            var healthComponent = bodyPrefab.AddComponent<HealthComponent>();
            healthComponent.globalDeathEventChanceCoefficient = 1f;
            healthComponent.dontShowHealthbar = true;
            #endregion

            #region ModelLocator
            var modelLocator = bodyPrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBase;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = true;
            modelLocator.normalizeToFloor = false;
            #endregion

            #region EntityStateMachine_Body
            var esmBody = bodyPrefab.AddComponent<EntityStateMachine>();
            esmBody.customName = "Body";
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pylon.ChargingExplosion));
            esmBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pylon.ChargingExplosion));
            #endregion

            #region NetworkStateMachine
            var networkStateMachine = bodyPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new EntityStateMachine[] { esmBody };
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = bodyPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterDeath));
            #endregion

            #region Deployable
            bodyPrefab.AddComponent<Deployable>();
            #endregion

            #endregion

            #region Hurtbox
            var hurtbox = hurtboxTransform.gameObject.AddComponent<HurtBox>();
            hurtbox.healthComponent = healthComponent;
            #endregion

            #region Model
            var mdlPylonGameObject = modelTransform.gameObject;

            #region HurtBoxGroup
            var hurtboxGroup = mdlPylonGameObject.AddComponent<HurtBoxGroup>();
            hurtboxGroup.mainHurtBox = hurtbox;
            hurtboxGroup.hurtBoxes = new HurtBox[] { hurtbox };
            #endregion

            #region ChildLocator
            var childLocator = mdlPylonGameObject.AddComponent<ChildLocator>();
            var ourChildLocator = mdlPylonGameObject.GetComponent<OurChildLocator>();
            childLocator.transformPairs = Array.ConvertAll(ourChildLocator.transformPairs, item =>
            {
                return new ChildLocator.NameTransformPair
                {
                    name = item.name,
                    transform = item.transform,
                };
            });
            UnityEngine.Object.Destroy(ourChildLocator);
            #endregion

            #region CharacterModel
            //modelRenderer.material = skinsLookup["matIfrit"];
            //headRenderer.material = skinsLookup["matColossus"];

            var characterModel = mdlPylonGameObject.AddComponent<CharacterModel>();
            characterModel.body = characterBody;
            characterModel.autoPopulateLightInfos = true;
            // TODO: FIX
            var renderers = bodyPrefab.GetComponentsInChildren<MeshRenderer>();
            characterModel.baseRendererInfos = Array.ConvertAll(renderers, (item) =>
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
            //characterModel.baseLightInfos = new CharacterModel.LightInfo[] // TODO
            //{
            //    new CharacterModel.LightInfo
            //    {
            //        light = eyeLight,
            //        defaultColor = eyeLight.color
            //    }
            //};
            #endregion

            #region LineRendererHelper
            mdlPylonGameObject.AddComponent<DeployableLineRendererToOwner>();
            #endregion

            #endregion

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
        }

        public GameObject CreateMaster(GameObject masterPrefab, GameObject bodyPrefab)
        {
            #region NetworkIdentity
            masterPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterMaster
            var characterMaster = masterPrefab.AddComponent<CharacterMaster>();
            characterMaster.bodyPrefab = bodyPrefab;
            characterMaster.spawnOnStart = false;
            characterMaster.teamIndex = TeamIndex.Monster;
            characterMaster.destroyOnBodyDeath = true;
            characterMaster.isBoss = false;
            characterMaster.preventGameOver = true;
            #endregion

            #region Inventory
            masterPrefab.AddComponent<Inventory>();
            #endregion

            #region MinionOwnership
            if (!masterPrefab.TryGetComponent<MinionOwnership>(out _))
            {
                masterPrefab.AddComponent<MinionOwnership>();
            }
            #endregion

            masterPrefab.RegisterNetworkPrefab();

            return masterPrefab;
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            var card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            (card as ScriptableObject).name = name;
            card.prefab = master;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.Golem;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            card.directorCreditCost = 0;
            card.occupyPosition = true;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.noElites = true;
            card.forbiddenAsBoss = true;
            if (skin && bodyGameObject && bodyGameObject.TryGetComponent<CharacterBody>(out var body))
            {
                card.loadout = new SerializableLoadout
                {
                    bodyLoadouts = new SerializableLoadout.BodyLoadout[]
                    {
                        new SerializableLoadout.BodyLoadout()
                        {
                            body = body,
                            skinChoice = skin,
                            skillChoices = Array.Empty<SerializableLoadout.BodyLoadout.SkillChoice>() // yes, we need it
                        }
                    }
                };
            };

            return card;
        }
    }
}
