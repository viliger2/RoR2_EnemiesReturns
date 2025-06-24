using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.ArcherBug
{
    public class ArcherBugBody : BodyBase
    {
        public struct Skills
        {
            public static SkillDef CausticSpit;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscArcherBugDefault;
        }

        public static GameObject BodyPrefab;

        protected override bool AddFootstepHandler => false;

        protected override bool AddCharacterMotor => false;

        protected override bool AddCharacterDirection => false;

        protected override bool AddRigidbodyMotor => true;

        public SkillDef CreateCausticSpitSkill()
        {
            var acridEpidemic = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoDisease.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("ArcherBugBodyCausticSpit", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ArcherBugs.FireCausticSpit)))
            {
                nameToken = "ENEMIES_RETURNS_ARCHERBUG_CAUSTIC_SPIT_NAME",
                descriptionToken = "ENEMIES_RETURNS_ARCHERBUG_CAUSTIC_SPIT_DESCRIPTION",
                icon = acridEpidemic.icon,
                activationStateMachine = "Weapon",
                baseRechargeInterval = Configuration.ArcherBug.CausticSpitCooldown.Value
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyPrefab = null)
        {
            return CreateCard(new SpawnCardParams(name, master, Configuration.ArcherBug.DirectorCost.Value)
            {
                hullSize = HullClassification.Human,
                forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn,
                occupyPosition = true,
                forbiddenAsBoss = false,
                skinDef = skin,
                bodyPrefab = bodyPrefab
            });
        }

        protected override IRigidBodyDirection.RigidbodyDirectionParams RigidbodyDirectionParams()
        {
            return new IRigidBodyDirection.RigidbodyDirectionParams()
            {
                aimDirection = Vector3.one,
                angularVelocityPID = new QuaternionPIDParams()
                {
                    customName = "Angular Velocity PID",
                    PID = new Vector3(5f, 1f, 0f),
                    inputQuat = Quaternion.identity,
                    targetQuat = Quaternion.identity,
                    gain = 3f
                },
                torquePID = new VectorPIDParams()
                {
                    customName = "torquePID",
                    PID = new Vector3(2f, 1f, 0f),
                    isAngle = true,
                    gain = 3f
                },
            };
        }

        protected override IRigidbodyMotor.RigidbodyMotorParams RigidbodyMotorParams()
        {
            return new IRigidbodyMotor.RigidbodyMotorParams()
            {
                forcePID = new VectorPIDParams()
                {
                    customName = "Force PID",
                    PID = new Vector3(3f, 0f, 0f),
                    isAngle = false,
                    gain = 1f
                },
                canTakeImpactDamage = true
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_ARCHERBUG_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 0,
                baseAcceleration = 100f,
                baseMaxHealth = Configuration.ArcherBug.BaseMaxHealth.Value,
                levelMaxHealth = Configuration.ArcherBug.LevelMaxHealth.Value,
                baseDamage = Configuration.ArcherBug.BaseDamage.Value,
                levelDamage = Configuration.ArcherBug.LevelDamage.Value,
                baseArmor = Configuration.ArcherBug.BaseArmor.Value,
                levelArmor = Configuration.ArcherBug.LevelArmor.Value,
                baseMoveSpeed = Configuration.ArcherBug.BaseMoveSpeed.Value,
                baseJumpCount = 1,
                baseJumpPower = Configuration.ArcherBug.BaseJumpPower.Value,
                isChampion = false,
                autoCalculateStats = true,
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var bugBodyRenderer = modelPrefab.transform.Find("Bug").gameObject.GetComponent<SkinnedMeshRenderer>();
            var bugWingsRenderer = modelPrefab.transform.Find("Wings").gameObject.GetComponent<SkinnedMeshRenderer>();
           
            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = bugBodyRenderer,
                    defaultMaterial = bugBodyRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = bugWingsRenderer,
                    defaultMaterial = bugWingsRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
               
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender
            };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                aliveLoopStart = "Play_item_use_bugWingFlapLoop",
                aliveLoopStop = "Stop_item_use_bugWingFlapLoop"
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var bugBodyRenderer = modelPrefab.transform.Find("Bug").gameObject.GetComponent<SkinnedMeshRenderer>();
            var bugWingsRenderer = modelPrefab.transform.Find("Wings").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = bugBodyRenderer,
                    defaultMaterial = bugBodyRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = bugWingsRenderer,
                    defaultMaterial = bugWingsRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },

            };
            SkinDefs.Default = Utils.CreateSkinDef("skinArcherBugDefault", modelPrefab, defaultRender);

            return new SkinDef[] {SkinDefs.Default};
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ArcherBugs.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.FlyState)),
                },
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Weapon",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle))
                },
            };
        }

        protected override IFootStepHandler.FootstepHandlerParams FootstepHandlerParams()
        {
            return new IFootStepHandler.FootstepHandlerParams();
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/Bug/ArcherBugArmature/ROOT/Base/Body/Head",
                pathToPoint1 = "ModelBase/Bug/ArcherBugArmature/ROOT/Abdomen1/Abdomen2/Abdomen3/Abdomen3_end"
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
             {
                 new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "CausticSpit", SkillSlot.Primary),
             };
        }

        // TODO: elite displays
        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsArcherBug";
            return idrs;
        }

        protected override string ModelName() => "Bug";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                maxDistance = 6f,
                minDistance = 1.5f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ArcherBugs.DeathState)));
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Beetle/sdBeetleGuard.asset").WaitForCompletion();
    }
}
