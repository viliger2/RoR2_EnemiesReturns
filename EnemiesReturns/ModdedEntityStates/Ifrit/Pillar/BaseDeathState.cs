using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar
{
    public class BaseDeathState : GenericCharacterDeath
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
