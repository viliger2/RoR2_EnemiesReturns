using EnemiesReturns.Helpers;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class DeathState : GenericCharacterDeath
    {
        public static float effectDuration = 1.25f;

        private Renderer maneRenderer;
        private Renderer bodyRenderer;
        //private (Transform, Vector3)[] flames;

        private float initialEmission;

        private MaterialPropertyBlock manePropertyBlock;
        private MaterialPropertyBlock bodyPropertyBlock;

        public override void OnEnter()
        {
            bodyPreservationDuration = 2f;
            base.OnEnter();

            var modelTransform = GetModelTransform();
            if(modelTransform && modelTransform.gameObject.TryGetComponent<TransformScaler>(out var transformScaler))
            {
                transformScaler.SetScaling(Vector3.zero, effectDuration);
            }

            var childLocator = GetModelChildLocator();

            var maneTransform = childLocator.FindChild("Mane");
            maneRenderer = maneTransform.GetComponent<Renderer>();
            manePropertyBlock = SetupPropertyBlock(maneRenderer, out initialEmission);

            var bodyTransform = childLocator.FindChild("MainBody");
            bodyRenderer = bodyTransform.GetComponent<Renderer>();
            bodyPropertyBlock = SetupPropertyBlock(bodyRenderer, out initialEmission);

            // fuck me, surely this won't complely shit itself
            //flames = new (Transform, Vector3)[]
            //{
            //    (childLocator.FindChild("Fire1"), childLocator.FindChild("Fire1")?.localScale ?? Vector3.zero),
            //    (childLocator.FindChild("Fire2"), childLocator.FindChild("Fire2")?.localScale ?? Vector3.zero),
            //    (childLocator.FindChild("Fire3"), childLocator.FindChild("Fire3")?.localScale ?? Vector3.zero),
            //    (childLocator.FindChild("Fire4"), childLocator.FindChild("Fire4")?.localScale ?? Vector3.zero),
            //    (childLocator.FindChild("Fire5"), childLocator.FindChild("Fire5")?.localScale ?? Vector3.zero)
            //};
        }

        public override void Update()
        {
            base.Update();
            if(age <= effectDuration)
            {
                SetPropertyBlock(maneRenderer, manePropertyBlock, initialEmission, age);
                SetPropertyBlock(bodyRenderer, bodyPropertyBlock, initialEmission, age);
                //foreach((Transform, Vector3) t in flames)
                //{
                //    if(t.Item1)
                //    {
                //        t.Item1.localScale = Vector3.Lerp(t.Item2, Vector3.zero, age / effectDuration);
                //    }
                //}
            }
        }

        private void SetPropertyBlock(Renderer renderer, MaterialPropertyBlock block, float initialEmission, float age)
        {
            block.SetFloat("_EmPower", Mathf.Lerp(initialEmission, 0f, age / effectDuration));
            renderer.SetPropertyBlock(block);
        }

        private MaterialPropertyBlock SetupPropertyBlock(Renderer renderer, out float initialEmission)
        {
            var propertyBlock = new MaterialPropertyBlock();
            initialEmission = renderer.material.GetFloat("_EmPower");
            propertyBlock.SetFloat("_EmPower", initialEmission);
            renderer.SetPropertyBlock(propertyBlock);

            return propertyBlock;
        }
    }
}
