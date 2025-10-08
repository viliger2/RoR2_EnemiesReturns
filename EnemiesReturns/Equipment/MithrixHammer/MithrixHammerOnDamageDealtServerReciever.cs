using EnemiesReturns.Behaviors;
using EnemiesReturns.Configuration.Judgement;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.CharacterBody;

namespace EnemiesReturns.Equipment.MithrixHammer
{
    public class MithrixHammerOnDamageDealtServerReciever : BossDamageEquipmentOnDamageDealtServerReciever
    {
        public override float damageCoefficient => Judgement.MithrixHammerAeonianBonusDamage.Value;

        public override DamageColorIndex damageColor => DamageColorIndex.Fragile;

        public override EquipmentIndex equipmentIndex => Content.Equipment.MithrixHammer.equipmentIndex;
    }
}
