using EnemiesReturns.Enemies.LynxTribe.Storm;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.ListViewDragger;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SwordBeam
{
    [RegisterEntityState]
    public class SwordBeamLoop : BaseState
    {
        public static float baseDuration = 10f;

        public static float degreesPerSecond = 40f;

        public static float beamDamage = 10f;

        public static string hitBoxGroupName = "SwordBeam";

        public static float procCoefficient = 1f;

        public static float pushRadius = 20f;

        public static float pushStrength = 15f;

        public static GameObject beamPrefab;

        private OverlapAttack overlapAttack;

        private GameObject forwardBeam;

        private GameObject backwardsBeam;

        private SphereSearch pushSphereSearch;

        public GameObject ppBeamInstance;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "SwrodLaserLoop", 0.1f);

            pushSphereSearch = new SphereSearch();

            overlapAttack = CreateOverlapAttack(GetModelTransform());

            forwardBeam = UnityEngine.Object.Instantiate(beamPrefab);
            forwardBeam.transform.SetParent(FindModelChild("SwordBeamEffectForward"));
            forwardBeam.transform.localPosition = Vector3.zero;
            forwardBeam.transform.localRotation = Quaternion.identity;

            backwardsBeam = UnityEngine.Object.Instantiate(beamPrefab);
            backwardsBeam.transform.SetParent(FindModelChild("SwordBeamEffectBackward"));
            backwardsBeam.transform.localPosition = Vector3.zero;
            backwardsBeam.transform.localRotation = Quaternion.identity;

            Util.PlaySound("Play_voidRaid_superLaser_start", base.gameObject); // TODO
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                characterDirection.moveVector = Vector3.zero; // if move vector gets stuck as non zero then rotation breaks
                characterDirection.yaw += degreesPerSecond * GetDeltaTime();
                if (overlapAttack != null)
                {
                    overlapAttack.Fire();
                }
            }

            var position = transform.position;
            var hurtBoxes = SearchForTargets();
            foreach (var hurtbox in hurtBoxes)
            {
                if (hurtbox && hurtbox.healthComponent && hurtbox.healthComponent.body)
                {
                    var targetBody = hurtbox.healthComponent.body;
                    if (targetBody.hasEffectiveAuthority)
                    {
                        var component = targetBody.GetComponent<IDisplacementReceiver>();
                        if (component != null)
                        {
                            component.AddDisplacement(-(position - targetBody.transform.position).normalized * pushStrength * GetDeltaTime());
                        }
                    }
                }
            }

            if (fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new SwordBeamEnd());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            if (forwardBeam)
            {
                UnityEngine.Object.Destroy(forwardBeam);
            }
            if (backwardsBeam)
            {
                UnityEngine.Object.Destroy(backwardsBeam);
            }
            if (ppBeamInstance)
            {
                UnityEngine.Object.Destroy(ppBeamInstance);
            }
        }

        private OverlapAttack CreateOverlapAttack(Transform modelTransform)
        {
            var overlapAttack = new OverlapAttack();
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            overlapAttack.damage = beamDamage * damageStat;
            //swordAttack.hitEffectPrefab = ;
            overlapAttack.isCrit = RollCrit();
            overlapAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == hitBoxGroupName);
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.damageType = new DamageTypeCombo(DamageType.BypassBlock | DamageType.BypassOneShotProtection | DamageType.BypassArmor, DamageTypeExtended.Generic, DamageSource.Special);
            overlapAttack.retriggerTimeout = 0.25f;

            return overlapAttack;
        }

        private HurtBox[] SearchForTargets()
        {
            HurtBox[] result;

            pushSphereSearch.mask = LayerIndex.entityPrecise.mask;
            pushSphereSearch.origin = transform.position;
            pushSphereSearch.radius = pushRadius;
            pushSphereSearch.queryTriggerInteraction = UnityEngine.QueryTriggerInteraction.UseGlobal;
            pushSphereSearch.RefreshCandidates();
            pushSphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(characterBody.teamComponent.teamIndex));
            pushSphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            result = pushSphereSearch.GetHurtBoxes();
            pushSphereSearch.ClearCandidates();

            return result;
        }

    }
}
