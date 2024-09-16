using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pylon
{
    public class ChargingExplosion : BaseState
    {
        public static float duration = 30f; // TODO: config

        public static Vector3 fireballFinishScale = new Vector3(4f, 4f, 4f);
        public static Vector3 pillarFinishPosition = new Vector3(-1f, 2.71f, 0f);

        private Transform fireball;
        private Transform pillar;

        private Vector3 fireballStartScale;
        private Vector3 pillarStartPosition;

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
                pillarStartPosition = pillar.localPosition;
            }
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
                pillar.localPosition = Vector3.Lerp(pillarStartPosition, pillarFinishPosition, age / duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextState(new FiringExplosion());
            }
        }
    }
}
