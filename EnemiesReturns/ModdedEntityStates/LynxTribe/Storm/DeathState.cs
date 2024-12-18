﻿using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Storm
{
    public class DeathState : GenericCharacterDeath
    {
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
