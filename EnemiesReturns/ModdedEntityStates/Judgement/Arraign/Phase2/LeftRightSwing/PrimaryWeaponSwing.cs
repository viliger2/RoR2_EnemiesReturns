using EnemiesReturns.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.LeftRightSwings
{
    [RegisterEntityState]
    public class PrimaryWeaponSwing : BasePrimaryWeaponSwing
    {
        public override string swingSoundEffect => "ER_Arraign_LeftRightSwingP2_Play";

        public static GameObject hitEffectStatic = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/OmniImpactVFXHuntress.prefab").WaitForCompletion();

        public override GameObject hitEffect => hitEffectStatic;

        public override float swingDamageCoefficient => Configuration.Judgement.ArraignP2.LeftRightSwingDamage.Value;

        public override float swingProcCoefficient => Configuration.Judgement.ArraignP2.LeftRightSwingProcCoefficient.Value;

        public override float swingForce => Configuration.Judgement.ArraignP2.LeftRightSwingForce.Value;
    }
}
