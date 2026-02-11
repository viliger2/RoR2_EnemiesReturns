using EnemiesReturns.Equipment.MithrixHammer;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using static RoR2.CharacterBody;

namespace EnemiesReturns.Equipment.EliteAenonian
{
    public class EliteAenonianBehaviour : ItemBehavior
    {
        private SerializableEntityStateType originalDeathState;

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.General.EnableJudgement.Value)
            {
                if (NetworkServer.active)
                {
                    if (body && body.inventory)
                    {
                        var result = body.AddItemBehavior<EliteAenonianBehaviour>(body.inventory.HasEquipment(Content.Equipment.EliteAeonian) ? 1 : 0);
                    }
                }
            }
        }

        private void Awake()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            if (body)
            {
                body.UpdateAuthority();
            }
            if(NetworkServer.active && body && body.hasEffectiveAuthority)
            {
                var deathBehaviour = body.GetComponent<CharacterDeathBehavior>();
                if (!deathBehaviour)
                {
                    this.enabled = false;
                    return;
                }

                originalDeathState = deathBehaviour.deathState;
                deathBehaviour.deathState = new SerializableEntityStateType(typeof(ModdedEntityStates.Judgement.Aeonian.DeathState));
            }
        }

        private void OnDisable()
        {
            if(NetworkServer.active && body && body.hasEffectiveAuthority)
            {
                var deathBehaviour = body.GetComponent<CharacterDeathBehavior>();
                if (!deathBehaviour)
                {
                    return;
                }

                if(originalDeathState.stateType != null)
                {
                    deathBehaviour.deathState = originalDeathState;
                }

            }
        }
    }
}
