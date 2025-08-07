using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    [RegisterEntityState]
    public class SwiftMain : GenericCharacterMain
    {
        private int flyOverideLayer;

        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            if (animator)
            {
                flyOverideLayer = animator.GetLayerIndex("FlyOverride");
            }
        }

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
            if (characterBody && animator)
            {
                animator.SetLayerWeight(flyOverideLayer, base.characterMotor.isGrounded ? 0 : 1);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (animator)
            {
                animator.SetLayerWeight(flyOverideLayer, 0);
            }
        }
    }
}
