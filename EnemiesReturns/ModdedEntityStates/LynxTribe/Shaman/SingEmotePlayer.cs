namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    internal class SingEmotePlayer : BasePlayerEmoteState
    {
        public override float duration => -1f;

        public override string soundEventPlayName => "ER_ShamanTookTheDrug_Play";

        public override string soundEventStopName => "ER_ShamanTookTheDrug_Stop";

        public static float slapEmoteDuration = 3f;

        private bool animSwitched;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "CastTeleport", "CastTeleport.playbackRate", slapEmoteDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > slapEmoteDuration && !animSwitched)
            {
                PlayCrossfade("Gesture, Override", "ISingForYou", 0.5f);
                animSwitched = true;
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

    }
}
