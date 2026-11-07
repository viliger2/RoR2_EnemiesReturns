using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls
{
    public abstract class BasePrepareAttack : BaseState
    {
        public static GameObject portalEffectPrefab;

        public abstract float effectSpawnDelay { get; }

        public abstract float baseDuration { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        private bool effectSpawned;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade(layerName, animationStateName, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > effectSpawnDelay && !effectSpawned)
            {
                if (portalEffectPrefab)
                {
                    var origin = FindModelChild("PortalClonesEffectOrigin");
                    if (origin)
                    {
                        var effectData = new EffectData()
                        {
                            rootObject = base.gameObject,
                            modelChildIndex = (short)GetModelChildLocator().FindChildIndex(origin)
                        };

                        EffectManager.SpawnEffect(portalEffectPrefab, effectData, false);
                    }
                }
                effectSpawned = true;
            }

            if (fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(GetNextState());
            }
        }

        public abstract EntityState GetNextState();

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
