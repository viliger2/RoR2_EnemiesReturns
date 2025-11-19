using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    [RegisterEntityState]
    public class SummonTotemDeath : GenericCharacterDeath
    {
        public static float deathDuration = 2f;

        private bool startedGrounded = true;

        private bool totemSpawned = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isVoidDeath)
            {
                return;
            }

            if (characterMotor)
            {
                if (characterMotor.isGrounded)
                {
                    if (NetworkServer.active)
                    {
                        SpawnTotem(base.characterBody.footPosition);
                        DestroyBody();
                    }
                    DestroyModel();
                }
                else
                {
                    startedGrounded = false;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!startedGrounded && characterMotor && characterMotor.isGrounded)
            {
                if (NetworkServer.active && !totemSpawned)
                {
                    SpawnTotem(base.characterBody.footPosition);
                    totemSpawned = true;
                    DestroyBody();
                }
                DestroyModel();
            }
            if (fixedAge >= deathDuration && NetworkServer.active)
            {
                DestroyBody();
            }
        }

        private void DestroyBody()
        {
            if (base.gameObject)
            {
                EntityState.Destroy(base.gameObject);
            }
        }

        private void SpawnTotem(Vector3 spawnPosition)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var placementRule = new DirectorPlacementRule()
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                position = spawnPosition
            };

            var directorSpawnRequest = new DirectorSpawnRequest(EnemiesReturns.Enemies.LynxTribe.Totem.TotemBody.SpawnCards.cscLynxTotemDefault, placementRule, RoR2Application.rng)
            {
                ignoreTeamMemberLimit = true,
                teamIndexOverride = teamComponent.teamIndex
            };
            var result = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
            if (result)
            {
                var inventory = result.GetComponent<Inventory>();
                if (inventory && characterBody.inventory)
                {
                    inventory.CopyEquipmentFrom(characterBody.inventory, false);
                    inventory.CopyItemsFrom(characterBody.inventory);
                }

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

        public static void OnBodyStart(CharacterBody body)
        {
            if (body && NetworkServer.active)
            {
                EntityStateMachine.FindByCustomName(body.gameObject, "Body")?.SetState(new ModdedEntityStates.LynxTribe.Totem.SpawnStateFromShaman());
            }            
        }
    }
}
