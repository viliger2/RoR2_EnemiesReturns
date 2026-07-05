using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates
{
    public abstract class BasePlayerEmoteState : BaseState
    {
        public abstract float duration { get; }

        public abstract string soundEventPlayName { get; }

        public abstract string soundEventStopName { get; }

        public override void OnEnter()
        {
            base.OnEnter();
            if (!string.IsNullOrEmpty(soundEventPlayName))
            {
                Util.PlaySound(soundEventPlayName, base.gameObject);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!string.IsNullOrEmpty(soundEventStopName))
            {
                Util.PlaySound(soundEventStopName, base.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            bool endEmote = false;
            if (characterMotor && !characterMotor.isGrounded)
            {
                endEmote = true;
            }

            if (inputBank)
            {
                if (inputBank.skill1.down) endEmote = true;
                if (inputBank.skill2.down) endEmote = true;
                if (inputBank.skill3.down) endEmote = true;
                if (inputBank.skill4.down) endEmote = true;

                if (inputBank.moveVector != Vector3.zero) endEmote = true;
            }

            if (duration > 0 && fixedAge >= duration)
            {
                endEmote = true;
            }

            if (endEmote)
            {
                outer.SetNextStateToMain();
            }
        }

    }
}
