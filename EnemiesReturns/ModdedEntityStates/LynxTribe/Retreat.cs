using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe
{
    [RegisterEntityState]
    public class Retreat : BaseState
    {
        public static float duraion = 2.5f;

        public static float effectSpawnDuration = 1f;

        public static GameObject retreatEffectPrefab;

        private Transform cachedModelTransform;

        private bool effectSpawned;

        private Transform effectTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            cachedModelTransform = (base.modelLocator ? base.modelLocator.modelTransform : null);
            effectTransform = FindModelChild("RetreatEffectOrigin");
            if (!effectTransform)
            {
                effectTransform = transform;
            }
            PlayAnimation("Body", "Retreat");
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > effectSpawnDuration && !effectSpawned)
            {
                EffectManager.SimpleEffect(retreatEffectPrefab, effectTransform.position, Quaternion.identity, false);
                effectSpawned = true;
            }

            if (fixedAge > duraion)
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
