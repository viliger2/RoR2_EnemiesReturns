using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.SandCrab
{
    [RegisterEntityState]
    public class SandCrabMain : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.SandCrab.EmoteKey.Value))
                {
                    this.outer.SetInterruptState(new DanceEmote(), InterruptPriority.Any);
                }
            }
        }
    }
}
