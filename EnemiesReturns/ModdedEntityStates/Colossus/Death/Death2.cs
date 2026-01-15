using EnemiesReturns.Reflection;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Death
{
    [RegisterEntityState]
    public class Death2 : BaseDeath
    {
        public override float duration => 1.28f;

        public override float fallEffectSpawnTime => 1.166f;

        public override string fallEffectChild => "Death2FallEffect";

        public static GameObject deathEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isVoidDeath)
            {
                return;
            }
            PlayAnimation("Death, Override", "Death2");
            Util.PlaySound("ER_Colossus_Death2_Play", gameObject);
            var chest = FindModelChild("Chest");
            if (chest)
            {
                EffectManager.SpawnEffect(deathEffect, new EffectData { origin = chest.position }, true);
            }
        }
    }
}
