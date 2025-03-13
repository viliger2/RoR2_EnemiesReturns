using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    public class EnterSkyLeap : BaseState
    {
        public static float baseDuration = 4.6f;

        public static string soundString;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(soundString, base.gameObject);
            PlayAnimation("Gesture, Override", "EnterSkyLeap", "SkyLeap.playbackRate", duration);
            if (NetworkServer.active) 
            {
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, baseDuration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(isAuthority && fixedAge > duration)
            {
                outer.SetNextState(new HoldSkyLeap());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Stun;
        }
    }
}
