using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Dash
{
    [RegisterEntityState]
    public class Dash : BaseState
    {
        public static AnimationCurve forwardSpeedCoefficientCurve;

        public static float heightCheck => Configuration.MechanicalSpider.DashHeightCheck.Value;

        public static float duration => Configuration.MechanicalSpider.DashDuration.Value;

        public static string soundString = "ER_Spider_Dash_Play";

        public static string soundStringMinion = "ER_Spider_Dash_Drone_Play";

        private bool startedStateGrounded;

        private Vector3 forwardDirection;

        private Transform ledgeHandling;

        private bool isMinion = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (inputBank && characterDirection)
            {
                characterDirection.forward = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
            }

            if (characterMotor)
            {
                startedStateGrounded = characterMotor.isGrounded;
            }

            if (!startedStateGrounded)
            {
                Vector3 velocity = characterMotor.velocity;
                velocity.y = characterBody.jumpPower;
                characterMotor.velocity = velocity;
            }
            PlayCrossfade("Body", "Dash", 0.2f);
            isMinion = characterBody.inventory.GetItemCount(RoR2Content.Items.MinionLeash) > 0;
            Util.PlaySound(isMinion ? soundStringMinion : soundString, base.gameObject);
            ledgeHandling = FindModelChild("LedgeHandling");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (inputBank && characterDirection)
                {
                    characterDirection.moveVector = inputBank.moveVector;
                    forwardDirection = characterDirection.forward;
                }
                if (characterMotor)
                {
                    var num2 = !startedStateGrounded ? forwardSpeedCoefficientCurve.Evaluate(fixedAge / duration) : forwardSpeedCoefficientCurve.Evaluate(fixedAge / duration); // maybe separate?
                    characterMotor.rootMotion += num2 * moveSpeedStat * forwardDirection * GetDeltaTime();
#if DEBUG || NOWEAVER
                    if (ledgeHandling)
#else
                    if (ledgeHandling && !characterBody.isPlayerControlled)
#endif
                    {
                        var result = Physics.Raycast(ledgeHandling.position, Vector3.down, out var hitinfo, Mathf.Infinity, LayerIndex.world.mask);
                        if (!result || hitinfo.distance > heightCheck)
                        {
                            outer.SetNextState(new DashStop());
                        }
                    }
                }
                if (fixedAge >= duration)
                {
                    outer.SetNextState(new DashStop());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
