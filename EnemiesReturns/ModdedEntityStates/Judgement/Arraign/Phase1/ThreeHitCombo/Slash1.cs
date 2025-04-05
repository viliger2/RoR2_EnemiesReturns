using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo
{
    public class Slash1 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash1;

        public static float searchRadius = 20f;

        private Vector3 desiredDirection;

        public override void OnEnter()
        {
            this.baseDuration = 0.64f;
            base.damageCoefficient = 2f;
            base.hitBoxGroupName = "Sword";
            //base.hitEffectPrefab = 
            base.procCoefficient = 1f;
            base.pushAwayForce = 600f;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            //base.swingEffectMuzzleString = "";
            base.mecanimHitboxActiveParameter = "Slash1.attack";
            base.shorthopVelocityFromHit = 0f;
            //base.impactSound = "";
            base.forceForwardVelocity = true;
            base.forwardVelocityCurve = acdSlash1;
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;

            base.duration = base.baseDuration / attackSpeedStat;

            base.OnEnter();

            desiredDirection = inputBank.aimDirection;

            //if (characterDirection)
            //{
            //    characterDirection.forward = inputBank.aimDirection;
            //}
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 targetMoveVelocity = Vector3.zero;
            characterDirection.forward = Vector3.SmoothDamp(characterDirection.forward, desiredDirection, ref targetMoveVelocity, 0.01f, 45f);
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash1", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override void AuthorityOnFinish()
        {
            outer.SetNextState(new Slash2());
            //var hitboxes = GetSphereSearchResult(new SphereSearch(), base.transform.position);
            //if (hitboxes.Count > 0)
            //{
            //    outer.SetNextState(new Slash2());
            //}
            //else
            //{
            //    outer.SetNextState(new FireHomingProjectiles()); // TODO: restore
            //}
        }

        private List<HurtBox> GetSphereSearchResult(SphereSearch sphereSearch, Vector3 origin)
        {
            List<HurtBox> result = new List<HurtBox>();
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.origin = origin;
            sphereSearch.radius = searchRadius;
            sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            sphereSearch.FilterByPlayers();
            sphereSearch.GetHurtBoxes(result);
            sphereSearch.ClearCandidates();

            return result;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
