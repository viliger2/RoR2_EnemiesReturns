using EnemiesReturns.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.LeftRightSwings
{
    [RegisterEntityState]
    public class PrimaryWeaponSwing : BasePrimaryWeaponSwing
    {
        public override string swingSoundEffect => "ER_Arraign_LeftRightSwingP1_Play";

        public static GameObject hitEffectStatic = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion();

        public override GameObject hitEffect => hitEffectStatic;

        public override float swingDamageCoefficient => Configuration.Judgement.ArraignP1.LeftRightSwingDamage.Value;

        public override float swingProcCoefficient => Configuration.Judgement.ArraignP1.LeftRightSwingForce.Value;

        public override float swingForce => Configuration.Judgement.ArraignP1.LeftRightSwingForce.Value;
    }
}
