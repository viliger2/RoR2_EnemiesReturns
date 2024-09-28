using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Death
{
    public class Death2 : BaseDeath
    {
        public override float duration => 1f;

        public override float fallEffectSpawnTime => 0.88f;

        public override string fallEffectChild => "Death2FallEffect";

        public static GameObject deathEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Death, Override", "Death2");
            Util.PlaySound("ER_Colossus_Death2_Play", gameObject);
            EffectManager.SpawnEffect(deathEffect, new EffectData { origin = FindModelChild("Chest").transform.position }, true);
        }
    }
}
