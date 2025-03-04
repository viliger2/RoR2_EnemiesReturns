using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Colossus.RockClap
{
    public class RockClapEnd : BaseState
    {
        public static float projectileDamageCoefficient => EnemiesReturns.Configuration.Colossus.RockClapProjectileDamage.Value;

        public static float projectileForce => EnemiesReturns.Configuration.Colossus.RockClapProjectileForce.Value;

        public static float projectileSpeed => EnemiesReturns.Configuration.Colossus.RockClapProjectileSpeed.Value;

        public static float projectileSpeedDelta => EnemiesReturns.Configuration.Colossus.RockClapProjectileSpeedDelta.Value;

        public static float projectileDistanceFraction => EnemiesReturns.Configuration.Colossus.RockClapProjectileDistanceFraction.Value;

        public static float projectileDistanceFractionDelta => EnemiesReturns.Configuration.Colossus.RockClapProjectileDistanceFractionDelta.Value;

        public static float clapDamageCoefficient => EnemiesReturns.Configuration.Colossus.RockClapDamage.Value;

        public static float clapForce => EnemiesReturns.Configuration.Colossus.RockClapForce.Value;

        public static float clapRadius => EnemiesReturns.Configuration.Colossus.RockClapRadius.Value;

        public static SpawnCard golemSpawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Golem/cscGolem.asset").WaitForCompletion();

        public static SpawnCard jellyfishSpawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Jellyfish/cscJellyfish.asset").WaitForCompletion();

        public static SpawnCard wispSpawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Wisp/cscLesserWisp.asset").WaitForCompletion();

        public static GameObject projectilePrefab;

        public static float baseDuration = 2.2f;

        public static GameObject clapEffect;

        private Animator modelAnimator;

        private float duration;

        private bool hasFired;

        private FloatingRocksController rockController;

        private Transform clapTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            rockController = GetModelTransform().gameObject.GetComponent<FloatingRocksController>();
            clapTransform = FindModelChild("ClapPoint");
            PlayCrossfade("Gesture, Override", "ClapEnd", "Clap.playbackrate", duration, 0.1f);
            Util.PlayAttackSpeedSound("ER_Colossus_Clap_Play", gameObject, attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && modelAnimator.GetFloat("Clap.activate") >= 0.8f && !hasFired)
            {
                if (rockController)
                {
                    if (isAuthority)
                    {
                        foreach (GameObject rock in rockController.floatingRocks)
                        {
                            var position = (rock.transform.position - modelLocator.modelTransform.position) * UnityEngine.Random.Range(projectileDistanceFraction - projectileDistanceFractionDelta, projectileDistanceFraction + projectileDistanceFractionDelta);
                            position = new Vector3(modelLocator.modelTransform.position.x + position.x, modelLocator.modelTransform.position.y, modelLocator.modelTransform.position.z + position.z);

                            //var position = rock.transform.position - new Vector3(0f, 5f, 0f);
                            var rotation = Quaternion.LookRotation(rock.transform.position - position, Vector3.up);
                            ProjectileManager.instance.FireProjectile(projectilePrefab, rock.transform.position, rotation, gameObject, damageStat * projectileDamageCoefficient, projectileForce, RollCrit(), RoR2.DamageColorIndex.Default, null, UnityEngine.Random.Range(projectileSpeed - projectileSpeedDelta, projectileSpeed + projectileSpeedDelta), DamageSource.Secondary);
                        }
                        var attack = new BlastAttack();
                        attack.attacker = gameObject;
                        attack.inflictor = gameObject;
                        attack.teamIndex = teamComponent.teamIndex;
                        attack.baseDamage = clapDamageCoefficient * damageStat;
                        attack.baseForce = clapForce;
                        attack.position = clapTransform.position;
                        attack.radius = clapRadius;
                        attack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                        attack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                        attack.damageType = DamageSource.Secondary;
                        attack.Fire();

                    }
                    if (NetworkServer.active && EnemiesReturns.Configuration.Colossus.RockClapPostLoopSpawns.Value && Run.instance.loopClearCount > 0)
                    {
                        SummonHelp();
                    }
                    UnityEngine.Object.Instantiate(clapEffect, clapTransform.position, clapTransform.rotation);
                    rockController.enabled = false;
                }
                hasFired = true;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void SummonHelp()
        {
            int monsterSpawnCount = 1;
            SpawnCard monsterSpawnCard;
            if (characterBody.isElite)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.AffixRed))
                {
                    monsterSpawnCount = 6;
                    monsterSpawnCard = wispSpawnCard;
                }
                else if (characterBody.HasBuff(RoR2Content.Buffs.AffixBlue))
                {
                    monsterSpawnCount = 6;
                    monsterSpawnCard = jellyfishSpawnCard;
                }
                else
                {
                    monsterSpawnCount = 2;
                    monsterSpawnCard = golemSpawnCard;
                }
            }
            else
            {
                monsterSpawnCard = golemSpawnCard;
                monsterSpawnCount = 1;
            }

            for (int i = 0; i < monsterSpawnCount; i++)
            {
                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(monsterSpawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    minDistance = 3f,
                    maxDistance = 20f,
                    spawnOnTarget = transform
                }, RoR2Application.rng);
                directorSpawnRequest.summonerBodyObject = base.gameObject;
                directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, (Action<SpawnCard.SpawnResult>)delegate (SpawnCard.SpawnResult spawnResult)
                {
                    if (spawnResult.success && (bool)spawnResult.spawnedInstance && (bool)base.characterBody)
                    {
                        Inventory component = spawnResult.spawnedInstance.GetComponent<Inventory>();
                        if ((bool)component)
                        {
                            component.CopyEquipmentFrom(base.characterBody.inventory);
                        }
                    }
                });
                DirectorCore.instance?.TrySpawnObject(directorSpawnRequest);
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
