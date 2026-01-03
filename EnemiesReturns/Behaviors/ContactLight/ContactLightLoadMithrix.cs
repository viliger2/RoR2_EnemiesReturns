using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.ContactLight
{
    public class ContactLightLoadMithrix : MonoBehaviour
    {
        public ScriptedCombatEncounter combatEncounter;

        private void Awake()
        {
            //if (combatEncounter && combatEncounter.spawns.Length > 0)
            //{
            //    combatEncounter.spawns[0].spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Brother.cscBrother_asset).WaitForCompletion();
            //}
        }

        public void KillAllMonsters()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            foreach (TeamComponent item in new List<TeamComponent>(TeamComponent.GetTeamMembers(TeamIndex.Monster)))
            {
                if ((bool)item)
                {
                    HealthComponent component = item.GetComponent<HealthComponent>();
                    if ((bool)component)
                    {
                        component.Suicide();
                    }
                }
            }
        }
    }
}
