using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar
{
    public abstract class BaseChargingExplosion : BaseState
    {
        //public static float duration => EnemiesReturnsConfiguration.Ifrit.PillarExplosionChargeDuration.Value;
        public abstract float duration { get; }

        public static Vector3 fireballFinishScale = new Vector3(2.5f, 2.5f, 2.5f);
        public static float pillarFinishYScale = 10f;

        private Transform fireball;
        private Transform pillar;

        private Vector3 fireballStartScale = Vector3.one;
        private float pillarStartYScale = 0f;

        public override void OnEnter()
        {
            base.OnEnter();
            var childLocator = GetModelChildLocator();
            fireball = childLocator.FindChild("Fireball");
            if (fireball)
            {
                Util.PlaySound("ER_Ifrit_Pillar_Fire_Play", fireball.gameObject);
                fireballStartScale = fireball.localScale;
            }

            pillar = childLocator.FindChild("GlowPillar");
            //if(pillar)
            //{
            //    Util.PlaySound("ER_Ifrit_Pillar_Lava_Play", pillar.gameObject);
            //    pillar.gameObject.SetActive(true);
            //}
        }

        public override void Update()
        {
            base.Update();
            AkSoundEngine.SetRTPCValue("ER_Ifrit_Pillar_Fire_Volume", Mathf.Clamp((age / duration) * 100, 20, 100));
            if (fireball)
            {
                fireball.localScale = Vector3.Lerp(fireballStartScale, fireballFinishScale, age / duration);
            }
            if(pillar)
            {
                pillar.localScale = new Vector3(pillar.localScale.x, Mathf.Lerp(pillarStartYScale, pillarFinishYScale, age / duration), pillar.localScale.z);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (fireball)
            {
                Util.PlaySound("ER_Ifrit_Pillar_Fire_Stop", fireball.gameObject);
            }
            //if (pillar)
            //{
            //    Util.PlaySound("ER_Ifrit_Pillar_Lava_Stop", pillar.gameObject);
            //}
        }
    }
}
