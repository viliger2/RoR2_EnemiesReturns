using EntityStates;
using RoR2;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman
{
    public class TeleportFriend : BaseState
    {
        public static float range = 200f;

        public static float baseSkillRechargeTime = 15f;

        public static float refundPortion = 0.75f;

        public static float baseDuration = 2.4f;

        public static float baseTeleportDelay = 1.6f;

        public static float baseEffectSpawnDelay = 1.25f;

        public static float spawnRange = 5f;

        public static HashSet<BodyIndex> blacklist;

        public static GameObject teleportEffect;

        private GameObject teleportee;

        private CharacterBody teleporteeCharacterBody;

        private float duration;

        private float teleportDelay;

        private float effectSpawnDelay;

        private bool effectSpawned;

        private bool teleported;

        private Vector3 teleportTarget;

        private GameObject target;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            teleportDelay = baseTeleportDelay / attackSpeedStat;
            effectSpawnDelay = baseEffectSpawnDelay / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "CastTeleportFull", "CastTeleport.playbackRate", duration, 0.1f);
            if (blacklist == null)
            {
                blacklist = new HashSet<BodyIndex>();
            }

            teleportee = FindFriendlyToTeleport();
            target = FindCurentTarget();

            if ((!teleportee || !target) && isAuthority)
            {
                RefundAndExit();
                return;
            }

            teleporteeCharacterBody = teleportee.GetComponent<CharacterBody>();
        }

        private void RefundAndExit()
        {
            skillLocator.secondary.RunRecharge(baseSkillRechargeTime * refundPortion);
            outer.SetNextStateToMain();
            return;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((!teleportee || !target) && isAuthority)
            {
                RefundAndExit();
                return;
            }

            if (fixedAge > effectSpawnDelay && !effectSpawned)
            {
                var teleportTarget = target.transform.position + target.GetComponent<InputBankTest>().aimDirection * spawnRange;
                var nodeGraph = SceneInfo.instance.GetNodeGraph(teleporteeCharacterBody.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
                var node = nodeGraph.FindClosestNode(teleportTarget, teleporteeCharacterBody.hullClassification);
                nodeGraph.GetNodePosition(node, out this.teleportTarget);
                EffectManager.SimpleEffect(teleportEffect, this.teleportTarget, Quaternion.identity, false);
                effectSpawned = true;
            }

            if (fixedAge > teleportDelay && !teleported)
            {
                if (teleporteeCharacterBody.hasEffectiveAuthority)
                {
                    EffectManager.SimpleEffect(teleportEffect, teleportee.transform.position, Quaternion.identity, false);
                    if (teleportee.TryGetComponent<CharacterMotor>(out var motor))
                    {

                        var position = new Vector3(teleportTarget.x, teleportTarget.y + motor.capsuleHeight, teleportTarget.z);
                        var rotation = Vector3.RotateTowards(position, target.transform.position, float.MaxValue, float.MaxValue).normalized;
                        motor.Motor.SetPositionAndRotation(new Vector3(teleportTarget.x, teleportTarget.y + motor.capsuleRadius, teleportTarget.z), Quaternion.Euler(rotation));
                        EffectManager.SimpleEffect(teleportEffect, motor.Motor.transform.position, Quaternion.identity, true);
                    }
                    else if (teleportee.TryGetComponent<Rigidbody>(out var rigidbody))
                    {
                        rigidbody.position = teleportTarget;
                        EffectManager.SimpleEffect(teleportEffect, rigidbody.position, Quaternion.identity, true);
                    }

                }
                teleported = true;
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        private GameObject FindFriendlyToTeleport()
        {
            var sphereSearch = new SphereSearch()
            {
                mask = LayerIndex.entityPrecise.mask,
                origin = transform.position,
                radius = range,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
            };
            var team = new TeamMask();
            team.AddTeam(teamComponent.teamIndex);

            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(team);
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            var hurtboxes = sphereSearch.GetHurtBoxes();
            foreach (var hurtbox in hurtboxes)
            {
                if (hurtbox.healthComponent == healthComponent)
                {
                    continue;
                }
                if (!blacklist.Contains(hurtbox.healthComponent.body.bodyIndex) && !hurtbox.healthComponent.body.isPlayerControlled)
                {
                    return hurtbox.healthComponent.body.gameObject;
                }
            }

            return null;
        }

        private GameObject FindCurentTarget()
        {
            foreach (var ai in characterBody.master.aiComponents)
            {
                if (!ai.currentEnemy.characterBody)
                {
                    continue;
                }

                return ai.currentEnemy.characterBody.gameObject;
            }

            return null;
        }

    }
}
