using EnemiesReturns.ModdedEntityStates.Spitter;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class ShamanMainState : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxShaman.EmoteKey.Value))
                {
                    this.outer.SetInterruptState(new NopePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
