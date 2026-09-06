using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    [RegisterEntityState]
    public class ColossusMain : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                var print = modelTransform.gameObject.GetComponent<PrintController>();
                if (print && print.enabled)
                {
                    print.ForceDisablePrint();
                    print.age += print.printTime;
                    print.enabled = false;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (base.isAuthority && base.characterMotor.isGrounded && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.Colossus.EmoteKey.Value))
                {
                    this.outer.SetInterruptState(new DancePlayer(), InterruptPriority.Any);
                }
            }
        }
    }
}
