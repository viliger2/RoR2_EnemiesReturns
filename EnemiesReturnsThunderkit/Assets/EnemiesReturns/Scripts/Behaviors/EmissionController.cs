using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.PrintController;

namespace EnemiesReturns.Behaviors
{
    public class EmissionController : MonoBehaviour
    {
        public float emissionTime;

        public AnimationCurve emissionCurve;

        public float age;

        public bool disableWhenFinished = true;

        public bool paused;

        public float startingEmissionValue;

        public float endEmissionValue;

        private CharacterModel characterModel;

        private MaterialPropertyBlock propBlock;

        private static int emissionValuePropertyId;

        private bool hasSetup;

        private RendererMaterialPair[] rendererMaterialPairs = Array.Empty<RendererMaterialPair>();

        [InitDuringStartupPhase(GameInitPhase.DuringIntro, 0)]
        private static void Init()
        {
            emissionValuePropertyId = Shader.PropertyToID("_EmPower");
        }

        private void Awake()
        {
            characterModel = GetComponent<CharacterModel>();
            propBlock = new MaterialPropertyBlock();
            if(TryGetComponent<ModelSkinController>(out var component))
            {
                age = 0f;
                component.onSkinApplied += Component_onSkinApplied;
            } else
            {
                SetupEmission();
            }
        }


        private void OnEnable()
        {
            age = 0f;
        }

        private void SetupEmission()
        {
            if (hasSetup)
            {
                return;
            }

            hasSetup = true;
            if (characterModel)
            {
                CharacterModel.RendererInfo[] baseRendererInfos = characterModel.baseRendererInfos;
                int num = 0;
                for (int i = 0; i < baseRendererInfos.Length; i++)
                {
                    if (!(baseRendererInfos[i].defaultMaterial?.shader != printShader) && (!baseRendererInfos[i].renderer.gameObject.GetComponent<PrintController>()))
                    {
                        num++;
                    }
                }
                Array.Resize(ref rendererMaterialPairs, num);
                int j = 0;
                int num2 = 0;
                for (; j < baseRendererInfos.Length; j++)
                {
                    ref CharacterModel.RendererInfo reference = ref baseRendererInfos[j];
                    if (!(reference.defaultMaterial?.shader != printShader) && (!baseRendererInfos[j].renderer.gameObject.GetComponent<PrintController>()))
                    {
                        Material material = (reference.defaultMaterial = UnityEngine.Object.Instantiate(reference.defaultMaterial));
                        rendererMaterialPairs[num2++] = new RendererMaterialPair(reference.renderer, material);
                    }
                }
            } else
            {
                List<Renderer> gameObjectComponentsInChildren = GetComponentsCache<Renderer>.GetGameObjectComponentsInChildren(base.gameObject, includeInactive: true);
                Array.Resize(ref rendererMaterialPairs, gameObjectComponentsInChildren.Count);
                int k = 0;
                for (int count = gameObjectComponentsInChildren.Count; k < count; k++)
                {
                    Renderer renderer = gameObjectComponentsInChildren[k];
                    Material material2 = renderer.material;
                    rendererMaterialPairs[k] = new RendererMaterialPair(renderer, material2);
                }
                GetComponentsCache<Renderer>.ReturnBuffer(gameObjectComponentsInChildren);
            }
            age = 0f;
        }

        private void Component_onSkinApplied(int obj)
        {
            SetupEmission();
            if (enabled)
            {
                SetEmissionThreshold(0f);
            }
            age = 0f;
        }

        public void SetPaused(bool paused)
        {
            this.paused = paused;
        }

        private void Update()
        {
            UpdateEmission(Time.deltaTime);
        }

        private void UpdateEmission(float deltaTime)
        {
            if(hasSetup && emissionCurve != null)
            {
                if (!paused)
                {
                    age += deltaTime;
                }
                float threshold = emissionCurve.Evaluate(age / emissionTime);
                SetEmissionThreshold(threshold);
                if(age >= emissionTime && disableWhenFinished)
                {
                    this.enabled = false;
                    age = 0f;
                }
            }
        }

        private void SetEmissionThreshold(float threshold)
        {
            float num = 1f - threshold;
            float value = threshold * endEmissionValue + num * startingEmissionValue;
            for (int i = 0; i < rendererMaterialPairs.Length; i++)
            {
                ref var reference = ref rendererMaterialPairs[i];
                reference.renderer.GetPropertyBlock(propBlock);
                propBlock.SetFloat(emissionValuePropertyId, value);
                reference.renderer.SetPropertyBlock(propBlock);
            }
        }

    }
}
