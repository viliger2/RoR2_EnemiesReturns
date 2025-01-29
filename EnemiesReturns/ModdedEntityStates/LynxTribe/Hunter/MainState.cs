using EnemiesReturns.ModdedEntityStates.LynxTribe.Archer;
using EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Hunter
{
    public class MainState : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
                {
                    if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxHunter.SingEmoteKey.Value))
                {
                    this.outer.SetInterruptState(new HarmonicaEmote(), InterruptPriority.Any);
                }
            }
        }
    }
}
