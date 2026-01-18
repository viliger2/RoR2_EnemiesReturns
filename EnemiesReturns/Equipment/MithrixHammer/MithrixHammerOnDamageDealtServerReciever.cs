using EnemiesReturns.Behaviors;
using EnemiesReturns.Configuration.Judgement;
using RoR2;

namespace EnemiesReturns.Equipment.MithrixHammer
{
    public class MithrixHammerOnDamageDealtServerReciever : BossDamageEquipmentOnDamageDealtServerReciever
    {
        public override float damageCoefficient => Judgement.MithrixHammerAeonianBonusDamage.Value;

        public override DamageColorIndex damageColor => DamageColorIndex.Fragile;

        public override EquipmentIndex equipmentIndex => Content.Equipment.MithrixHammer.equipmentIndex;
    }
}
