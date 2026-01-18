using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override float baseDuration => Configuration.General.ProvidenceP1UtilityAttackDuraion.Value;

        public override float earlyExit => Configuration.General.ProvidenceP1UtilityEarlyExit.Value;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackRateParams => "SkyLeap.playbackRate";

        public override string animatorAttackParam => "SkyLeap.firstAttack";
    }
}
