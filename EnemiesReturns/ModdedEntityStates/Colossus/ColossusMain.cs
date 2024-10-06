using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    public class ColossusMain : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturnsConfiguration.Colossus.EmoteKey.Value))
                {
                    this.outer.SetInterruptState(new DancePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
