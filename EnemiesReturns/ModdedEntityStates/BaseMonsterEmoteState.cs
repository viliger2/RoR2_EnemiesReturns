using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates
{
    public abstract class BaseMonsterEmoteState : BaseState
    {
        public abstract float duration { get; }

        public abstract string soundEventPlayName { get; }

        public abstract string soundEventStopName { get; }

        public abstract string layerName { get; }

        public abstract string animationName { get; }

        public abstract bool stopOnDamage { get; }

        public abstract float healthFraction { get; }

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation(layerName, animationName);
            Util.PlaySound(soundEventPlayName, base.gameObject);
            if(stopOnDamage)
            {
                GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Util.PlaySound(soundEventStopName, base.gameObject);
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
            if (stopOnDamage)
            {
                GlobalEventManager.onServerDamageDealt -= GlobalEventManager_onServerDamageDealt;
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            if (report.victimBody == characterBody)
            {
                if ((healthComponent.combinedHealth / healthComponent.fullCombinedHealth) <= healthFraction)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(duration > 0)
            {
                if(fixedAge >= duration && isAuthority)
                {
                    outer.SetNextStateToMain();
                }
            }
        }
    }
}
