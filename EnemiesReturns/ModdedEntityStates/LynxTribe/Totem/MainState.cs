using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    [RegisterEntityState]
    public class MainState : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxTotem.SexYesEmoteKey.Value))
                {
                    this.outer.SetInterruptState(new SexYesEmote(), InterruptPriority.Any);
                }
            }
        }
    }
}
