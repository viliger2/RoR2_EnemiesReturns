using EntityStates;
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
