using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1
{
    [RegisterEntityState]
    public class WeaponThrow : BaseWeaponThrow
    {
        public static GameObject staticProjectilePrefab;

        public override GameObject projectilePrefab => staticProjectilePrefab;

        public override float baseDuration => 4f;

        public override float damageCoefficient => Configuration.Judgement.ArraignP1.SwordThrowDamage.Value;

        public override float force => Configuration.Judgement.ArraignP1.SwordThrowForce.Value;

        public override string layerName => "Gesture, Override";

        public override string animName => "ThrowSword";

        public override string playbackRateParamName => "WeaponThrow.playbackRate";

        public override string childOrigin => "HandR";

        public override string throwSound => "ER_Arraign_SwordThrow_Throw_Play";

        public override string chargeSound => "ER_Arraign_SwordThrow_Charge_Play";
    }
}
