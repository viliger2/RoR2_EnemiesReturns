using EnemiesReturns.Reflection;
using EntityStates.GameOver;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Ending
{
    [RegisterEntityState]
    public class EscapedEnding : BaseGameOverControllerState
    {
        private static readonly float duration = 1f;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }
    }
}
