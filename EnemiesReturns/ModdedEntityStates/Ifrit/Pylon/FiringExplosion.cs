using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pylon
{
    public class FiringExplosion : BaseState
    {
        public static float duration = 1f; // TODO: config?

        public static Vector3 fireballFinishPosition = new Vector3(-1f, -1f, 0f);
        public static Vector3 pillarFinishPosition = new Vector3(-1f, -3.2f, 0f);

        private Transform fireball;
        private Transform pillar;

        private Vector3 fireballStartPosition;
        private Vector3 pillarStartPosition;

        public override void OnEnter()
        {
            base.OnEnter();

            var childLocator = GetModelChildLocator();
            fireball = childLocator.FindChild("Fireball");
            if (fireball)
            {
                fireballStartPosition = fireball.localPosition;
            }

            pillar = childLocator.FindChild("GlowPillar");
            if (pillar)
            {
                pillarStartPosition = pillar.localPosition;
            }
        }

        public override void Update()
        {
            base.Update();
            if (fireball)
            {
                fireball.localPosition = Vector3.Lerp(fireballStartPosition, fireballFinishPosition, age / duration);
            }
            if (pillar)
            {
                pillar.localPosition = Vector3.Lerp(pillarStartPosition, pillarFinishPosition, age / duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextState(new FireExplosion());
            }
        }
    }
}
