using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject burrowPrefab;

        public static GameObject eyeEffectPrefab;

        private Animator animator;

        private bool effectSpawned;

        private GameObject eyeModel;

        private GameObject headLight;

        public override void OnEnter()
        {
            duration = 5f;
            spawnSoundString = "ER_Colossus_Spawn_Play";

            base.OnEnter();

            var eyeModelTransform = FindModelChild("EyeModel");
            if (eyeModelTransform)
            {
                eyeModel = eyeModelTransform.gameObject;
                eyeModel.SetActive(false);
            }

            var headLightTransform = FindModelChild("HeadLight");
            if(headLightTransform)
            {
                headLight = headLightTransform.gameObject;
                headLight.SetActive(false);
            }

            if (burrowPrefab)
            {
                EffectManager.SimpleMuzzleFlash(burrowPrefab, base.gameObject, "BurrowCenter", false);
            }
            animator = GetModelAnimator();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > 4.75f && !effectSpawned)
            {
                SpawnEffects();

                effectSpawned = true;
            }
        }

        private void SpawnEffects()
        {
            if (eyeModel)
            {
                eyeModel.SetActive(true);
            }
            if (headLight)
            {
                headLight.SetActive(true);
            }
            var data = new EffectData
            {
                rootObject = gameObject,
                modelChildIndex = (short)GetModelChildLocator().FindChildIndex(headLight.transform)
            };
            EffectManager.SpawnEffect(eyeEffectPrefab, data, false);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1f);
            }
            if (!effectSpawned)
            {
                SpawnEffects();
            }
        }
    }
}
