using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.PrefabAPICompat;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components
{
    public interface IBody : INetworkIdentity, ICharacterDirection, IMotor, IInputBankTest, ICharacterBody, ICameraTargetParams, IModelLocator, ITeamComponent, IHealthComponent, IInteractor, IInteractionDriver, IBodyStateMachines, ISkills, ICharacterNetworkTransform, IDeathRewards, IEquipmentSlot, IModel, ISfxLocator
    {
        public string ModelName();

        public GameObject CreateBody(GameObject body, Sprite sprite, UnlockableDef log, ExplicitPickupDropTable droptable)
        {
            var modelBase = GetModelBase(body);
            var modelTransform = GetModelTransform(modelBase);

            AddNetworkIdentity(body, GetNetworkIdentityParams());
            var direction = AddCharacterDirection(body, modelBase, GetAnimator(body), GetCharacterDirectionTurnSpeed());
            AddMotor(body, direction);
            var inputNank = AddInputBankTest(body);
            var characterBody = AddCharacterBody(body, GetCharacterBodyParams(GetAimOrigin(body), sprite.texture));
            AddCameraTargetParams(body, GetCharacterCameraParams());
            AddModelLocator(body, modelBase, modelTransform, GetModelLocatorParams());
            AddSkills(body);
            AddTeamComponent(body, GetTeamIndex());
            var healthComponent = AddHealthComponent(body, GetHealthComponentParams());
            AddInteractor(body, GetInteractionDistance());
            AddInteractionDriver(body);
            AddCharacterNetworkTransform(body, GetCharacterNetworkTransformParams());
            AddBodyStateMachines(body);
            AddDeathRewards(body, log, droptable);
            AddEquipmentSlot(body);
            AddSfxLocator(body, GetSfxLocatorParams());

            SetupModel(modelTransform.gameObject, direction, inputNank, healthComponent, characterBody);

            if (NeedToAddNetworkIdentity())
            {
                body.RegisterNetworkPrefab();
            }

            return body;
        }

        private Transform GetModelTransform(Transform modelBase)
        {
            if (!modelBase)
            {
#if DEBUG || NOWEAVER
                Log.Warning($"Transform ModelBase doesn't exist");
#endif
                return null;
            }

            var transform = modelBase.Find(ModelName());
#if DEBUG || NOWEAVER
            if (!transform)
            {
                Log.Warning($"Transform {modelBase} doesn't have {ModelName()}!");
            }
#endif
            return transform;
        }

        private Transform GetModelBase(GameObject body)
        {
            var transform = body.transform.Find("ModelBase");
#if DEBUG || NOWEAVER
            if (!transform)
            {
                Log.Warning($"Body {body} doesn't have ModelBase!");
            }
#endif
            return transform;
        }

        private Transform GetAimOrigin(GameObject body)
        {
            var transform = body.transform.Find("AimOrigin");
#if DEBUG || NOWEAVER
            if (!transform)
            {
                Log.Warning($"Body {body} doesn't have AimOrigin!");
            }
#endif
            return transform;

        }

        private Animator GetAnimator(GameObject body)
        {
            var animator = body.GetComponentInChildren<Animator>();
#if DEBUG || NOWEAVER
            if (!animator)
            {
                Log.Warning($"Body {body} doesn't have animator!");
            }
#endif
            return animator;
        }

    }
}
