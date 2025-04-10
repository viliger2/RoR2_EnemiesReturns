using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Archer
{
    [RegisterEntityState]
    public class MainState : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.LynxTribe.LynxArcher.SingEmoteKey.Value))
                {
                    this.outer.SetInterruptState(new GuitarEmotePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
