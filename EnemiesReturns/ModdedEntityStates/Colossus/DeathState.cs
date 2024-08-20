using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using Rewired.HID;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    public class DeathState : GenericCharacterDeath
    {
        public static float duration = 1f;

        private Renderer eyeRenderer;

        private MaterialPropertyBlock eyePropertyBlock;

        private float initialEmmision;

        private Light headLight;

        private float initialRange;

        public override void OnEnter()
        {
            base.OnEnter();
            var rockController = GetModelTransform().gameObject.GetComponent<FloatingRocksController>();

            if(rockController)
            {
                rockController.enabled = false;
            }

            var childLocator = GetModelChildLocator();

            eyeRenderer = childLocator.FindChildComponent<Renderer>("EyeModel");
            eyePropertyBlock = new MaterialPropertyBlock();
            initialEmmision = eyeRenderer.material.GetFloat("_EmPower");
            eyePropertyBlock.SetFloat("_EmPower", initialEmmision);
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);

            headLight = childLocator.FindChildComponent<Light>("HeadLight");
            initialRange = headLight.range;
        }

        public override void Update()
        {
            base.Update();
            if(age <= duration)
            {
                eyePropertyBlock.SetFloat("_EmPower", Mathf.Lerp(initialEmmision, 0f, age / duration));
                eyeRenderer.SetPropertyBlock(eyePropertyBlock);
                headLight.range = Mathf.Lerp(initialRange, 0f, age / duration);
            }
        }

    }
}
