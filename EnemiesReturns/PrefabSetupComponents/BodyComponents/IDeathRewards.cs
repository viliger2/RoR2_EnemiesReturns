using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IDeathRewards
    {
        protected bool NeedToAddDeathRewards();

        protected DeathRewards AddDeathRewards(GameObject bodyPrefab, UnlockableDef log, ExplicitPickupDropTable dropTable)
        {
            DeathRewards deathRewards = null;
            if (NeedToAddDeathRewards())
            {
                deathRewards = bodyPrefab.GetOrAddComponent<DeathRewards>();
                deathRewards.logUnlockableDef = log;
                deathRewards.bossDropTable = dropTable;
            }

            return deathRewards;
        }
    }
}
