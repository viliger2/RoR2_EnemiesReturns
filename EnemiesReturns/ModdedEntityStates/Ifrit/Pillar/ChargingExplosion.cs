using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar
{
    public class ChargingExplosion : BaseState
    {
        public static float duration => EnemiesReturnsConfiguration.Ifrit.PillarExplosionChargeDuration.Value;

        public static Vector3 fireballFinishScale = new Vector3(4f, 4f, 4f);
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
                fireballStartScale = fireball.localScale;
            }

            pillar = childLocator.FindChild("GlowPillar");
            if(pillar)
            {
                pillar.gameObject.SetActive(true);
            }
            //if(pillar)
            //{
            //    pillarStartPosition = pillar.localPosition;
            //}
        }

        public override void Update()
        {
            base.Update();
            if(fireball)
            {
                fireball.localScale = Vector3.Lerp(fireballStartScale, fireballFinishScale, age / duration);
            }
            if(pillar)
            {
                pillar.localScale = new Vector3(pillar.localScale.x, Mathf.Lerp(pillarStartYScale, pillarFinishYScale, age / duration), pillar.localScale.z);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextState(new FireExplosion());
            }
        }
    }
}
