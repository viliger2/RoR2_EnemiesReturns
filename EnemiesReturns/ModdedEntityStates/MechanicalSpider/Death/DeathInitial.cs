using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    internal class DeathInitial : BaseState
    {
        public static float spawnChance => EnemiesReturns.Configuration.MechanicalSpider.DroneSpawnChance.Value;

        public override void OnEnter()
        {
            base.OnEnter();
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
            if (isAuthority)
            {
#if DEBUG || NOWEAVER
                outer.SetNextState(new DeathDrone());
#else
                var chance = RoR2.Run.instance.spawnRng.RangeFloat(0f, 100f);
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
    }
}
