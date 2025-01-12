using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace EnemiesReturns.Enemies.LynxTribe.Storm
{
    public class LynxStormOrb : Orb
    {
        public static GameObject orbEffect;

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
                effectData.SetHurtBoxReference(target);
                EffectManager.SpawnEffect(orbEffect, effectData, transmit: true);
            }
        }

        public override void OnArrival()
        {
            base.OnArrival();
        }
    }
}
