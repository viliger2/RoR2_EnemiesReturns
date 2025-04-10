using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap
{
    public abstract class BaseEnterSkyLeap : BaseState
    {
        public abstract float baseDuration { get; }

        public abstract string soundString { get; }

        public abstract string layerName { get; }

        public abstract string stateName { get; }

        public abstract string playbackRateParam { get; }

        public abstract bool addBuff { get; }

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(soundString, base.gameObject);
            PlayAnimation(layerName, stateName, playbackRateParam, duration);
            if (NetworkServer.active && addBuff) 
            {
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, baseDuration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(isAuthority && fixedAge > duration)
            {
                SetNextStateAuthority();
            }
        }

        public abstract void SetNextStateAuthority();

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
