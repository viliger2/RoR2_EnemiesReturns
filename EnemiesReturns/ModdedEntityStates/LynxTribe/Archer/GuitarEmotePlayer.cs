using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Archer
{
    [RegisterEntityState]
    public class GuitarEmotePlayer : BasePlayerEmoteState
    {
        public override float duration => -1;

        public override string soundEventPlayName => "";

        public override string soundEventStopName => "";

        private Transform arrowTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "SickGuitarSolo", 0.5f);
            arrowTransform = FindModelChild("Arrow");
            if (arrowTransform)
            {
                arrowTransform.gameObject.SetActive(false);
            }
        }

        public override void OnExit()
        {
            if (arrowTransform)
            {
                arrowTransform.gameObject.SetActive(true);
            }
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }
    }
}
