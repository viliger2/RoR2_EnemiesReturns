using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Storm
{
    public class DeathState : GenericCharacterDeath
    {
        // TODO: somehow stop it from triggering on hit effects, while still having the effects stop, probably use detach effects component
        public override void OnEnter()
        {
            base.OnEnter();
            var particles = modelLocator.modelTransform.GetComponentsInChildren<ParticleSystem>();
            foreach(var particle in particles)
            {
                particle.Stop();
            }
        }

    }
}
