using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard
{
    [RegisterEntityState]
    public class Main : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            // if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            // {
            //     if (Input.GetKeyDown(EnemiesReturns.Configuration.ContactLight.TempleGuard.EmoteKey.Value))
            //     {
            //         this.outer.SetInterruptState(new StretchPlayer(), InterruptPriority.Any);
            //     }
            // }
        }
    }
}
