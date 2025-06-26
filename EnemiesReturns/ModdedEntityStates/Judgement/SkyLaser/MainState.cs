using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.SkyLaser
{
    [RegisterEntityState]
    public class MainState : GenericCharacterMain
    {
        public static float laserRange = 15f;

        public static float lifetime => Configuration.Judgement.ArraignP2.SkyLaserLifetime.Value;

        public static string hitBoxGroupName = "Laser";

        public static float damageCoefficient => Configuration.Judgement.ArraignP2.SkyLaserDamageCoefficient.Value;

        public static float procCoefficient = 0f;

        public static float movementSpeed => Configuration.Judgement.ArraignP2.SkyLaserMovementSpeed.Value;

        private OverlapAttack overlapAttack;

        public override void OnEnter()
        {
            base.OnEnter();
            overlapAttack = SetupOverlapAttack(GetModelTransform());
            if (characterBody)
            {
                characterBody.baseMoveSpeed = movementSpeed;
                characterBody.statsDirty = true;
            }
            Util.PlaySound("ER_SkyLaser_Alive_Loop_Play", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                overlapAttack.Fire();
            }

            if (fixedAge >= lifetime && NetworkServer.active)
            {
                characterBody.healthComponent.godMode = false;
                characterBody.healthComponent.Suicide();
            }
        }

        private OverlapAttack SetupOverlapAttack(Transform modelTransform)
        {
            var overlapAttack = new OverlapAttack();
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            overlapAttack.damage = damageCoefficient * damageStat;
            //swordAttack.hitEffectPrefab = ;
            overlapAttack.isCrit = RollCrit();
            overlapAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == hitBoxGroupName);
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.damageType = new DamageTypeCombo(DamageType.BypassBlock | DamageType.BypassOneShotProtection | DamageType.BypassArmor, DamageTypeExtended.Generic, DamageSource.NoneSpecified);
            overlapAttack.retriggerTimeout = 0.25f;

            return overlapAttack;
        }

        public override void OnExit()
        {
            base.OnExit();
            Util.PlaySound("ER_SkyLaser_Alive_Loop_Stop", base.gameObject);
        }
    }
}
