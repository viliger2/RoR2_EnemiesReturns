using EnemiesReturns.Behaviors;
using EnemiesReturns.Enemies.ArcherBug;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ArcherBugs
{
    [RegisterEntityState]
    internal class DeathState : GenericCharacterDeath
    {
        public static float deathDelay = 0.4f;

        public static GameObject deathEffectPrefab;

        private bool hasDied;

        public override void OnEnter()
        {
            bodyPreservationDuration = 0.4f;
            base.OnEnter();          

            if (isVoidDeath)
            {
                return;
            }
          
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }
            if (base.fixedAge > deathDelay && !hasDied)
            {
                hasDied = true;               
                EffectManager.SimpleImpactEffect(deathEffectPrefab, base.characterBody.corePosition, Vector3.up, false);
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyBodyAsapServer();
                }
            }
        }
        public override void OnExit()
        {
            base.DestroyModel();
            base.OnExit();
        }
    }
}
