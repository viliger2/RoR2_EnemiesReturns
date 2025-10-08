using EnemiesReturns.Behaviors;
using EnemiesReturns.Configuration.Judgement;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Equipment.VoidlingWeapon
{
    internal class VoidlingWeaponOnDamageDealtServerReciever : BossDamageEquipmentOnDamageDealtServerReciever
    {
        public override float damageCoefficient => Judgement.MithrixHammerAeonianBonusDamage.Value;

        public override DamageColorIndex damageColor => DamageColorIndex.Void;

        public override EquipmentIndex equipmentIndex => Content.Equipment.VoidlingWeapon.equipmentIndex;
    }
}
