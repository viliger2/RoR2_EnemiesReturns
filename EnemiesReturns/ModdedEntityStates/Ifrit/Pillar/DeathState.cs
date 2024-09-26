using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar
{
    public class DeathState : GenericCharacterDeath
    {
        public static GameObject fallEffect;

        public static float fallEffectSpawnTime = 2.1f;

        private bool fallEffectSpawned;

        private Transform fallTransform;

        public override void OnEnter()
        {
            bodyPreservationDuration = 3f;
            var childLocator = GetModelChildLocator();
            if (childLocator)
            {
                var fireball = childLocator.FindChild("Fireball");
                if (fireball)
                {
                    fireball.gameObject.SetActive(false);
                }
                var areaIndicator = childLocator.FindChild("TeamAreaIndicator");
                if (areaIndicator)
                {
                    areaIndicator.gameObject.SetActive(false);
                }
                var lineRenderer = childLocator.FindChild("LineOriginPoint");
                if (lineRenderer)
                {
                    lineRenderer.gameObject.SetActive(false);
                }
                fallTransform = childLocator.FindChild("DeathEffectOrigin");
            }
            fallEffectSpawned = false;
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= fallEffectSpawnTime && !fallEffectSpawned && fallTransform)
            {
                Util.PlaySound("ER_Ifrit_Pillar_Death_Play", gameObject);
                EffectManager.SpawnEffect(fallEffect, new EffectData { origin = fallTransform.position }, true);
                fallEffectSpawned = true;
            }
        }
    }
}
