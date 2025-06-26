using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
    public class SpearThrow : BaseWeaponThrow
    {
        public static GameObject staticProjectilePrefab;

        public override GameObject projectilePrefab => staticProjectilePrefab;

        public override float baseDuration => 4f;

        public override float damageCoefficient => Configuration.Judgement.ArraignP2.SpearThrowDamage.Value;

        public override float force => Configuration.Judgement.ArraignP2.SpearThrowForce.Value;

        public override string layerName => "Gesture, Override";

        public override string animName => "ThrowSword";

        public override string playbackRateParamName => "WeaponThrow.playbackRate";

        public override string childOrigin => "HandR";

        public override string throwSound => "";

        public override string chargeSound => "ER_Arraign_SpearThrow_Charge_Play";
    }
}
