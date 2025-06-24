using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.ListViewDragger;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam
{
    [RegisterEntityState]
    public class BeamLoop : BaseBeam
    {
        public override GameObject pushBackEffect => pushBackEffectStatic;

        public static float baseDuration => Configuration.Judgement.ArraignP1.SwordBeamDuration.Value;

        public static float degreesPerSecond => Configuration.Judgement.ArraignP1.SwordBeamDegreesPerSecond.Value;

        public static float beamDamage => Configuration.Judgement.ArraignP1.SwordBeamDamage.Value;

        public static string hitBoxGroupName = "SwordBeam";

        public static float procCoefficient => Configuration.Judgement.ArraignP1.SwordBeamProcCoefficient.Value;

        public static GameObject beamPrefab;

        public static GameObject pushBackEffectStatic;

        private OverlapAttack overlapAttack;

        private GameObject forwardBeam;

        private GameObject backwardsBeam;

        public GameObject ppBeamInstance;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "SwordLaserLoop", 0.1f);

            overlapAttack = CreateOverlapAttack(GetModelTransform());

            forwardBeam = UnityEngine.Object.Instantiate(beamPrefab);
            forwardBeam.transform.SetParent(FindModelChild("SwordBeamEffectForward"));
            forwardBeam.transform.localPosition = Vector3.zero;
            forwardBeam.transform.localRotation = Quaternion.identity;

            backwardsBeam = UnityEngine.Object.Instantiate(beamPrefab);
            backwardsBeam.transform.SetParent(FindModelChild("SwordBeamEffectBackward"));
            backwardsBeam.transform.localPosition = Vector3.zero;
            backwardsBeam.transform.localRotation = Quaternion.identity;

            Util.PlaySound("ER_Arraign_BeamLoop_Play", gameObject);
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

            if (fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new BeamEnd());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Util.PlaySound("ER_Arraign_BeamLoop_Stop", gameObject);
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
    }
}
