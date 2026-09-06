using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TeleportButton
{
    [RegisterEntityState]
    public class TeleportBoombox : BaseState
    {
        public override void OnEnter()
        {
            // base.OnEnter();
            // PlayAnimation("Base", "Press");
            // Util.PlaySound("", gameObject);

            // var cscGolem = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Golem.cscGolem_asset).WaitForCompletion();

            // var childLocator = GetComponent<ChildLocator>();
            // if (childLocator)
            // {
            //     var boomBox = childLocator.FindChild("Boombox");
            //     if (boomBox)
            //     {
            //         var effect = Run.instance.GetTeleportEffectPrefab(null);
            //         if (effect)
            //         {
            //             EffectManager.SimpleEffect(effect, boomBox.transform.position, UnityEngine.Quaternion.identity, transmit: false);
            //         }
            //         boomBox.gameObject.SetActive(false);
            //     }

            //     DeactivateObjectAndSpawnMonster(childLocator.FindChild("Golem1"), cscGolem);
            //     DeactivateObjectAndSpawnMonster(childLocator.FindChild("Golem2"), cscGolem);
            //     DeactivateObjectAndSpawnMonster(childLocator.FindChild("Golem3"), cscGolem);
            //     if (Configuration.General.EnableColossus.Value)
            //     {
            //         DeactivateObjectAndSpawnMonster(childLocator.FindChild("Colossus"), Enemies.Colossus.ColossusBody.SpawnCards.cscColossusDefault);
            //     }
            // }
        }

        private void DeactivateObjectAndSpawnMonster(Transform dancingTransform, CharacterSpawnCard spawnCard)
        {
            if (!dancingTransform)
            {
                return;
            }

            var gameObject = dancingTransform.gameObject;
            gameObject.SetActive(false);

            if (NetworkServer.active)
            {
                var placementRule = new DirectorPlacementRule()
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Direct,
                    position = dancingTransform.position
                };

                var directorSpawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng)
                {
                    ignoreTeamMemberLimit = true,
                    teamIndexOverride = TeamIndex.Monster
                };
                var result = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                if (result)
                {
                    var master = result.GetComponent<CharacterMaster>();
                    if (master)
                    {
                        var body = master.GetBodyObject();
                        if (!body)
                        {
                            master.onBodyStart += OnBodyStart;
                        }
                        else
                        {
                            OnBodyStart(body.GetComponent<CharacterBody>());
                        }
                    }
                }
            }
        }

        private void OnBodyStart(CharacterBody body)
        {
            if(body && NetworkServer.active)
            {
                EntityStateMachine.FindByCustomName(body.gameObject, "Body")?.SetNextStateToMain();
                body.master.onBodyStart -= OnBodyStart;
            }
        }
    }
}
