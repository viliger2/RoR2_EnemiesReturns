using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    [RegisterEntityState]
    public class SpitterMain : GenericCharacterMain
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.Spitter.EmoteKey.Value))
                {
                    this.outer.SetInterruptState(new DeathDancePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
