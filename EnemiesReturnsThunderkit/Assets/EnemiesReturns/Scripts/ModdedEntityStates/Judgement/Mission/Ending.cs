using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
//using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    public class Ending : BaseState
    {
        //public static GameObject destroyEffectPrefab => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BrittleDeath.prefab").WaitForCompletion();

        public float delay = 0.5f;
        private bool ended = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    GameObject gameObject = teamMembers[i].gameObject;
                    CharacterBody component = gameObject.GetComponent<CharacterBody>();
                    if ((bool)component)
                    {
                        // EffectManager.SpawnEffect(destroyEffectPrefab, new EffectData
                        // {
                        //     origin = component.corePosition,
                        //     scale = component.radius
                        // }, transmit: true);
                        // EntityState.Destroy(gameObject.gameObject);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > delay && !ended && NetworkServer.active)
            {
                ended = true;
                Run.instance.BeginGameOver(RoR2Content.GameEndings.LimboEnding);
            }
        }
    }
}
