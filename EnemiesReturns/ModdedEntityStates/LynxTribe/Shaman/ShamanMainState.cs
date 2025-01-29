using EntityStates;
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
                if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxShaman.NopeEmoteKey.Value))
                {
                    this.outer.SetInterruptState(new NopeEmotePlayer(), InterruptPriority.Any);
                }
                else if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxShaman.SingEmoteKey.Value))
                {
                    this.outer.SetInterruptState(new SingEmotePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
