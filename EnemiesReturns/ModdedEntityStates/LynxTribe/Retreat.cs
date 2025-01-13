using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe
{
    public class Retreat : BaseState
    {
        public static float duraion = 2.5f;

        private Transform cachedModelTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            cachedModelTransform = (base.modelLocator ? base.modelLocator.modelTransform : null);
            PlayAnimation("Body", "Retreat");
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duraion)
            {
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyMaster();
                    DestroyBody();
                }
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

        private void DestroyModel()
        {
            if ((bool)cachedModelTransform)
            {
                EntityState.Destroy(cachedModelTransform.gameObject);
                cachedModelTransform = null;
            }
        }
    }
}
