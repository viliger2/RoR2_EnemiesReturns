using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class DeathDance : BaseMonsterEmoteState
    {
        public override float duration => 20f;

        public override string soundEventPlayName => "ER_Spitter_Laugh_Play";

        public override string soundEventStopName => "ER_Spitter_Laugh_Stop";

        public override string layerName => "Gesture, Override";

        public override string animationName => "DeathDance";

        public override bool stopOnDamage => true;

        public override float healthFraction => 0.5f;

        public Transform target;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (target)
            {
                StartAimMode(new Ray(target.position, target.forward), 0.16f, false);
            }
        }
    }
}
