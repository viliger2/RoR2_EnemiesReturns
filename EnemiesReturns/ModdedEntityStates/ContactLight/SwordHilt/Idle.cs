using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.SwordHilt
{
    [RegisterEntityState]
    public class Idle : BaseState
    {
        public static float baseDuration = 1f;

        public static int maxShardsCount = 5;

        public int activeShardsCount = 0;

        public override void OnEnter()
        {
            base.OnEnter();

            var modelChildLocator = GetModelChildLocator();

            if (!modelChildLocator)
            {
                return;
            }

            for(int i = 1; i <= maxShardsCount; i++)
            {
                var renderer = modelChildLocator.FindChild($"Shard{i}Renderer");
                if (renderer)
                {
                    renderer.gameObject.SetActive(activeShardsCount >= i);
                }
                var effect = modelChildLocator.FindChild($"Shard{i}SpawnEffect");
                if (effect)
                {
                    effect.gameObject.SetActive(activeShardsCount >= i);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= baseDuration && isAuthority && activeShardsCount >= maxShardsCount)
            {
                outer.SetNextState(new SpawnPortal());
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)activeShardsCount);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            activeShardsCount = reader.ReadByte();
        }
    }
}
