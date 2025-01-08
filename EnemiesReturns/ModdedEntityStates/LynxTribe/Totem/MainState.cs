using EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    public class MainState : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                //if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxShaman.NopeEmoteKey.Value))
                //{
                //    this.outer.SetInterruptState(new NopeEmotePlayer(), InterruptPriority.Any);
                //}
                //else if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxShaman.SingEmoteKey.Value))
                //{
                //    this.outer.SetInterruptState(new SingEmotePlayer(), InterruptPriority.Any);
                //}
            }
        }
    }
}
