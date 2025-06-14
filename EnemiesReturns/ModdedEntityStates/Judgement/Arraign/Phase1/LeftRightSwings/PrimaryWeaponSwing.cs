using EnemiesReturns.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
