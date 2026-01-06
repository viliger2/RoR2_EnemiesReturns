using EntityStates;
using RoR2.Skills;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence
{
    public abstract class BasePrePrimaryWeaponSwing : BaseState, SteppedSkillDef.IStepSetter
    {
        public static float baseDuration => Configuration.General.ProvidenceP1PrimaryPreSwingDuration.Value;

        private float duration;

        private int swingCount;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            var swingNameState = (swingCount % 2) == 0 ? "Slash1Init" : "Slash2Init";
            PlayCrossfade("UpperBodyOnly", swingNameState, "combo.playbackRate", duration, 0.25f);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("UpperBodyOnly", "BufferEmpty", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                var nextState = GetNextEntityState();
                nextState.swingCount = swingCount;
                outer.SetNextState(nextState);
            }
        }

        public abstract BasePrimaryWeaponSwing GetNextEntityState();

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)swingCount);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingCount = reader.ReadByte();
        }

        public void SetStep(int i)
        {
            swingCount = i;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
