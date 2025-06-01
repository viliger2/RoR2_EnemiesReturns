//using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam
{
    //[RegisterEntityState]
    public class BeamStart : BaseBeam
    {
        public override GameObject pushBackEffect => pushBackEffectStatic;

        public static float baseDuration = 4f;

        public static GameObject postProccessBeam;

        public static GameObject pushBackEffectStatic;

        private float duration;

        private Transform preBeam1;

        private Transform preBeam2;

        private GameObject ppBeamInstance;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "SwordLaserBegin", "SwordBeam.playbackRate", duration, 0.1f);
            if (postProccessBeam)
            {
                ppBeamInstance = UnityEngine.Object.Instantiate(postProccessBeam);
            }
            Util.PlaySound("ER_Colossus_Barrage_Charge_Play", gameObject); // TODO
            preBeam1 = FindModelChild("SwordPreBeamForwardParticles");
            if (preBeam1)
            {
                preBeam1.gameObject.SetActive(true);
            }

            preBeam2 = FindModelChild("SwordPreBeamBackwardsParticles");
            if (preBeam2)
            {
                preBeam2.gameObject.SetActive(true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > duration && isAuthority)
            {
                var nextState = new BeamLoop();
                nextState.ppBeamInstance = ppBeamInstance;
                outer.SetNextState(nextState);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            if (preBeam1)
            {
                preBeam1.gameObject.SetActive(false);
            }
            if (preBeam2)
            {
                preBeam2.gameObject.SetActive(false);
            }
        }
    }
}
