using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.PrefabSetupComponents.ModelComponents;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components
{
    public interface IModel : IAimAnimator, IChildLocator, IHurtboxes, IAnimationEvents, IDestroyOnUnseen, ICharacterModel, IAddHitboxes, IModelPanelParameters, IFootStepHandler, ISkins, IRandomBlinkController, IRemoveJitterBones
    {
        public void SetupModel(GameObject modelPrefab, CharacterDirection direction, InputBankTest inputBank, HealthComponent healthComponent, CharacterBody characterBody)
        {
            AddAimAnimator(modelPrefab, inputBank, direction, GetAimAnimatorParams());
            AddChildLocator(modelPrefab);
            SetupHurtboxes(modelPrefab, healthComponent);
            AddAnimationEvents(modelPrefab);
            AddDestroyOnUnseen(modelPrefab);
            AddCharacterModel(modelPrefab, characterBody, GetItemDisplayRuleSet(), GetCharacterModelParams(modelPrefab));
            SetupHitboxes(modelPrefab);
            AddModelPanelParameters(modelPrefab, GetFocusPoint(modelPrefab), GetCameraPosition(modelPrefab), GetModelPanelParams());
            AddFootstepHandler(modelPrefab, GetFootstepHandlerParams());
            AddSkins(modelPrefab);
            AddRandomBlinkController(modelPrefab, GetAnimator(modelPrefab), GetRandomBlinkParams());
            AddRemoveJitterBones(modelPrefab);
        }

        private Transform GetFocusPoint(GameObject modelPrefab)
        {
            var transform = modelPrefab.transform.Find("LogBookTarget");
#if DEBUG || NOWEAVER
            if (!transform)
            {
                Log.Warning($"Model {modelPrefab} doesn't have LogBookTarget child.");
            }
#endif
            return transform;
        }

        private Transform GetCameraPosition(GameObject modelPrefab)
        {
            var transform = modelPrefab.transform.Find("LogBookTarget/LogBookCamera");
#if DEBUG || NOWEAVER
            if (!transform)
            {
                Log.Warning($"Model {modelPrefab} doesn't have LogBookTarget/LogBookCamera child.");
            }
#endif
            return transform;
        }

        private Animator GetAnimator(GameObject model)
        {
            var animator = model.GetComponent<Animator>();
#if DEBUG || NOWEAVER
            if (!animator)
            {
                Log.Warning($"Body {model} doesn't have animator!");
            }
#endif
            return animator;
        }
    }
}
