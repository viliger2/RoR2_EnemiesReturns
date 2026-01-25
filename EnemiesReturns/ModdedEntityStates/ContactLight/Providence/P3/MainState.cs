using EnemiesReturns.Projectiles;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3
{
    [RegisterEntityState]
    public class MainState : GenericCharacterMain
    {
        public static float beamDamage = 2f;

        public static float procCoefficient = 1f;

        public static string hitBoxGroupName = "Phase3Laser";

        public static float duration = 30f;

        public static float degreesPerSecond = 20f;

        private Transform P3Laser;

        private OverlapAttackAuthority overlapAttack;

        public override void OnEnter()
        {
            base.OnEnter();
            P3Laser = FindModelChild("Phase3Laser");
            P3Laser.gameObject.SetActive(true);

            PlayAnimation("Gesture, Override", "SwordLaserLoop");

            overlapAttack = CreateOverlapAttack(GetModelTransform());
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.Immune, 30f);
            }
        }

        public override void FixedUpdate()
        {
            fixedAge += GetDeltaTime();
            if (hasSkillLocator && isAuthority)
            {
                HandleSkill(base.skillLocator.primary, ref base.inputBank.skill1);
                HandleSkill(base.skillLocator.secondary, ref base.inputBank.skill2);
                HandleSkill(base.skillLocator.utility, ref base.inputBank.skill3);
                HandleSkill(base.skillLocator.special, ref base.inputBank.skill4);
            }
            if (overlapAttack != null)
            {
                overlapAttack.Fire();
            }
            if (P3Laser)
            {
                P3Laser.Rotate(new Vector3(0f, degreesPerSecond * GetDeltaTime(), 0f));
            }
            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new Recharge());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            P3Laser.rotation = Quaternion.identity;
            P3Laser.gameObject.SetActive(false);
            PlayAnimation("Gesture, Override", "BufferEmpty");
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.Immune);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Vehicle;
        }

        private OverlapAttackAuthority CreateOverlapAttack(Transform modelTransform)
        {
            var overlapAttack = new OverlapAttackAuthority();
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
