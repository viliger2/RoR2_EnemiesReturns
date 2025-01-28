using RoR2;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Enemy
{
    public class FireExplosion : BaseFireExplosion
    {
        public override float damage => EnemiesReturns.Configuration.Ifrit.PillarExplosionDamage.Value;

        public override float radius => EnemiesReturns.Configuration.Ifrit.PillarExplosionRadius.Value;

        public override float force => EnemiesReturns.Configuration.Ifrit.PillarExplosionForce.Value;

        public override bool ignoresLoS => EnemiesReturns.Configuration.Ifrit.PillarExplosionIgnoesLoS.Value;

        public override float damagePerStack => 0f;

        public override DamageTypeCombo damageType => new DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.Generic, DamageSource.Special);

        public override void Suicide()
        {
            var cdb = characterBody.gameObject.GetComponent<CharacterDeathBehavior>();
            if (cdb)
            {
                cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.Enemy.SuicideDeathState));
            }
            base.Suicide();
        }
    }
}
