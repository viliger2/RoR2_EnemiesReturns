using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    [RegisterEntityState]
    public class EnterSkyLeap : BaseEnterSkyLeap
    {
        public override float baseDuration => 1.5f;

        public override string soundString => "";

        public override string layerName => "Gesture, Override";

        public override string stateName => "EnterSkyLeap";

        public override string playbackRateParam => "SkyLeap.playbackRate";

        public override bool addBuff => true;

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new HoldSkyLeap());
        }
    }
}
