﻿using EnemiesReturns.Reflection;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam
{
    [RegisterEntityState]
    public class BeamStart : BaseBeam
    {
        public override GameObject pushBackEffect => pushBackEffectStatic;

        public static float baseDuration = 4f;

        public static GameObject postProccessBeam;

        public static GameObject pushBackEffectStatic;

        public static GameObject preBeamIndicatorEffect;

        private float duration;

        private Transform preBeam1;

        private Transform preBeam2;

        private GameObject ppBeamInstance;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration;
            PlayCrossfade("Gesture, Override", "SwordLaserBegin", "SwordBeam.playbackRate", duration, 0.1f);
            if (postProccessBeam)
            {
                ppBeamInstance = UnityEngine.Object.Instantiate(postProccessBeam);
            }
            Util.PlaySound("ER_Arraign_BeamStart_Play", gameObject);
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

            var childLocator = GetModelChildLocator();
            if (childLocator)
            {
                EffectManager.SpawnEffect(preBeamIndicatorEffect, new EffectData { rootObject = base.gameObject, modelChildIndex = (short)childLocator.FindChildIndex("SwordBeamEffectForward") }, false);
                EffectManager.SpawnEffect(preBeamIndicatorEffect, new EffectData { rootObject = base.gameObject, modelChildIndex = (short)childLocator.FindChildIndex("SwordBeamEffectBackward") }, false);
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
