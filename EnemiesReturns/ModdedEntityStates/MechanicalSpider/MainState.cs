using EnemiesReturns.ModdedEntityStates.Spitter;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    public class MainState : GenericCharacterMain
    {
        private GameObject leftSpark;

        private GameObject rightSpark;

        private GameObject smoke;

        public override void OnEnter()
        {
            base.OnEnter();
            var rightSparkTransform = FindModelChild("SparkRightFrontLeg");
            if (rightSparkTransform)
            {
                rightSpark = rightSparkTransform.gameObject;
            }
            var leftSparkTransform = FindModelChild("SparkLeftBackLeg");
            if (leftSparkTransform)
            {
                leftSpark = leftSparkTransform.gameObject;
            }
            var smokeTransform = FindModelChild("Smoke");
            if (smokeTransform)
            {
                smoke = smokeTransform.gameObject;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (healthComponent)
            {
                CheckGameObject(leftSpark, 0.5f);
                CheckGameObject(rightSpark, 0.4f);
                CheckGameObject(smoke, 0.2f);
            }
        }

        private void CheckGameObject(GameObject gameObject, float healthFraction)
        {
            if(gameObject && !gameObject.activeSelf && (healthComponent.health / healthComponent.fullHealth) < healthFraction)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
