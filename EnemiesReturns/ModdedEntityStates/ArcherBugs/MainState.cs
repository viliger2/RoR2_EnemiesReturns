using EnemiesReturns.Reflection;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ArcherBugs
{
    [RegisterEntityState]
    public class MainState : EntityStates.FlyState
    {
        public override void Update()
        {
            base.Update();
            if (base.isAuthority && characterBody.isPlayerControlled)
            {
                if (Input.GetKeyDown(EnemiesReturns.Configuration.ArcherBug.BuckBumbleKey.Value))
                {
                    Util.PlaySound("ER_ArcherBug_BuckBumble_Stop", base.gameObject);
                    Util.PlaySound("ER_ArcherBug_BuckBumble_Play", base.gameObject);
                }
            }
        }

    }
}
