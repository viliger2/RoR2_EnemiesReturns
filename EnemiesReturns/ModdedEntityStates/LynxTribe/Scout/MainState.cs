using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Scout
{
    [RegisterEntityState]
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
