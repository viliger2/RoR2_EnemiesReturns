using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    internal class DeathInitial : BaseState
    {
        public static float spawnChance => EnemiesReturns.Configuration.MechanicalSpider.DroneSpawnChance.Value;

        public override void OnEnter()
        {
            base.OnEnter();
            DisableEffects();
            if (isAuthority)
            {
#if DEBUG || NOWEAVER
                outer.SetNextState(new DeathDrone());
#else
                var isVoidDeath = (bool)base.healthComponent && (ulong)(base.healthComponent.killingDamageType & DamageType.VoidDeath) != 0;
                if (isVoidDeath)
                {
                    outer.SetNextState(new DeathNormal());
                    return;
                }
                var chance = RoR2Application.rng.RangeFloat(0f, 100f);
                if (chance < spawnChance)
                {
                    outer.SetNextState(new DeathDrone());
                }
                else
                {
                    outer.SetNextState(new DeathNormal());
                }
#endif
            }
        }

        private void DisableEffects()
        {
            var rightSparkTransform = FindModelChild("SparkRightFrontLeg");
            if (rightSparkTransform)
            {
                rightSparkTransform.gameObject.SetActive(false);
            }
            var leftSparkTransform = FindModelChild("SparkLeftBackLeg");
            if (leftSparkTransform)
            {
                leftSparkTransform.gameObject.SetActive(false);
            }
            var smokeTransform = FindModelChild("Smoke");
            if (smokeTransform)
            {
                smokeTransform.gameObject.SetActive(false);
            }
        }
    }
}
