using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Judgement.SkyLaser
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
            //var particles = modelLocator.modelTransform.GetComponentsInChildren<ParticleSystem>();
            //foreach (var particle in particles)
            //{
            //    particle.Stop();
            //}
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
