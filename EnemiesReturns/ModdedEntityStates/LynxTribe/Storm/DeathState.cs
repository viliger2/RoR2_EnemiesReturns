﻿using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Storm
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (isVoidDeath)
            {
                return;
            }
            var particles = modelLocator.modelTransform.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particles)
            {
                particle.Stop();
            }
            DestroyModel();
            if (NetworkServer.active)
            {
                DestroyMaster();
                DestroyBody();
            }
        }

        private void DestroyBody()
        {
            if (base.gameObject)
            {
                NetworkServer.Destroy(base.gameObject);
            }
        }

        private void DestroyMaster()
        {
            if (base.characterBody && base.characterBody.master)
            {
                NetworkServer.Destroy(base.characterBody.masterObject);
            }
        }
    }
}
