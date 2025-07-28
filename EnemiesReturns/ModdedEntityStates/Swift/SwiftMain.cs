using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    [RegisterEntityState]
    public class SwiftMain : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if(base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.Swift.EmoteKey.Value))
                {
                    this.outer.SetInterruptState(new DuckDancePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
