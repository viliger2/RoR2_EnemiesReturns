using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Storm
{
    public class LynxStormOrb : Orb
    {
        public static GameObject orbEffect;

        public GameObject targetObject;

        public float scale = 1f;

        public override void Begin()
        {
            base.Begin();
            if (orbEffect)
            {
                EffectData effectData = new EffectData
                {
                    scale = scale,
                    origin = origin,
                    genericFloat = base.duration
                };
                effectData.SetNetworkedObjectReference(targetObject);
                EffectManager.SpawnEffect(orbEffect, effectData, transmit: true);
            }
        }

        public override void OnArrival()
        {
            base.OnArrival();
        }
    }
}
