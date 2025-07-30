using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift.Dive
{
    [RegisterEntityState]
    public class DivePrep : BaseState
    {
        public static float baseDuration = 1.8f;

        public static GameObject effectPrefab;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            base.characterDirection.moveVector = base.inputBank.aimDirection;
            PlayCrossfade("Gesture, Override", "DivePrep", "dive.playbackRate", duration, 0.1f);
            Util.PlaySound("ER_Swift_PrepAttack_Play", gameObject);
            var modelChildLocator = GetModelChildLocator();
            if (modelChildLocator)
            {
                var beakTranform = modelChildLocator.FindChild("Beak");
                if (beakTranform)
                {
                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        rootObject = base.gameObject,
                        modelChildIndex = (short)modelChildLocator.FindChildIndex(beakTranform),
                        origin = beakTranform.position,
                        rotation = Quaternion.identity,
                    }, false);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterDirection.moveVector = base.inputBank.aimDirection;
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new Dive());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
