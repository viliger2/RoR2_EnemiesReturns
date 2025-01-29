using EnemiesReturns.ModdedEntityStates.LynxTribe.Hunter;
using EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Scout
{
    public class MainState : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
                {
                    if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxScout.SingEmoteKey.Value))
                {
                    this.outer.SetInterruptState(new DrumEmote(), InterruptPriority.Any);
                }
            }
        }
    }
}
