using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class SpitterMain : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(EnemiesReturnsConfiguration.Spitter.EmoteKey.Value))
                {
                    this.outer.SetInterruptState(new DeathDancePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
