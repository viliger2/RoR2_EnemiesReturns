using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem.Burrow
{
    [RegisterEntityState]
    public class Burrowed : GenericCharacterMain
    {
        public static float duration = 1f;

        private CharacterBody.BodyFlags originalFlags;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Body", "Burrowed", 0.1f);
            if (characterBody)
            {
                originalFlags = characterBody.bodyFlags;
                characterBody.bodyFlags |= CharacterBody.BodyFlags.Unmovable | CharacterBody.BodyFlags.IgnoreKnockup;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (inputBank.moveVector.sqrMagnitude > 0.1f)
                //if (inputBank.moveVector.sqrMagnitude > 0.1f || (base.fixedAge >= duration && inputBank.skill3.down))
                {
                    outer.SetNextState(new Unburrow());
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (characterBody)
            {
                characterBody.bodyFlags = originalFlags;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
