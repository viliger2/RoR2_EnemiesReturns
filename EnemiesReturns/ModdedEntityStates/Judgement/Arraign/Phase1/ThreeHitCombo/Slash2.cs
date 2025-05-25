using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo
{
    // TODO: add some effects to feet to better indicate sliding in water
    [RegisterEntityState]
    public class Slash2 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash2;

        public static GameObject swingEffect;

        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/OmniImpactVFXHuntress.prefab").WaitForCompletion();

        public static float searchRadius = 20f;

        private Vector3 desiredDirection;

        public override void OnEnter()
        {
            this.baseDuration = 0.48f;
            base.damageCoefficient = 3f;
            base.hitBoxGroupName = "Sword";
            base.hitEffectPrefab = hitEffect;
            base.procCoefficient = 1f;
            base.pushAwayForce = 6000f;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            base.swingEffectPrefab = swingEffect;
            base.swingEffectMuzzleString = "SwingCombo2EffectMuzzle";
            base.mecanimHitboxActiveParameter = "Slash2.attack";
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "Play_merc_sword_swing"; // TODO: something heavier, got NGB sound archive, grab from Debilarough or whatever its called
            //base.impactSound = "";
            base.forceForwardVelocity = true;
            base.forwardVelocityCurve = acdSlash2;
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;

            base.OnEnter();

            desiredDirection = inputBank.aimDirection;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 targetMoveVelocity = Vector3.zero;
            characterDirection.forward = Vector3.SmoothDamp(characterDirection.forward, desiredDirection, ref targetMoveVelocity, 0.01f, 45f);
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash2", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType.damageSource = DamageSource.Secondary;
        }

        public override void AuthorityOnFinish()
        {
            outer.SetNextState(new Slash3());
            //var hitboxes = GetSphereSearchResult(new SphereSearch(), base.transform.position);
            //if (hitboxes.Count > 0)
            //{
            //    outer.SetNextState(new Slash3());
            //}
            //else
            //{
            //    outer.SetNextState(new FireHomingProjectiles()); //TODO: restore
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
