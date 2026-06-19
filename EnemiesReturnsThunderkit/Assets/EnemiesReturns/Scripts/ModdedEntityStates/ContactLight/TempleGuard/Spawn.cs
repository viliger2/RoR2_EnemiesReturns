using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard
{
    [RegisterEntityState]
    public class Spawn : GenericCharacterSpawnState
    {
        public static float glowDuration = 1f;

        public static float glowInitialValue = 10f;

        public static float glowFinalValue = 0f;

        private Renderer bodyRenderer;

        private Renderer clothRenderer;

        private MaterialPropertyBlock propertyBlock;

        public override void OnEnter()
        {
            duration = 3f;

            var bodyObject = FindModelChild("BodyRenderer");
            if (bodyObject)
            {
                bodyRenderer = bodyObject.GetComponent<Renderer>();
            }

            var clothObject = FindModelChild("ClothRenderer");
            if (clothObject)
            {
                clothRenderer = clothObject.GetComponent<Renderer>();
            }

            propertyBlock = new MaterialPropertyBlock();

            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();
            SetMaterialProperties(bodyRenderer);
            SetMaterialProperties(clothRenderer);
        }

        private void SetMaterialProperties(Renderer renderer)
        {
            if (renderer)
            {
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_EmPower", Mathf.Lerp(glowInitialValue, glowFinalValue, age / glowDuration));
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}
