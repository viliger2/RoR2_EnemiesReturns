using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using Rewired.HID;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Death
{
    public class Death1 : BaseDeath
    {
        public override float duration => 3.6f;

        public override float fallEffectSpawnTime => 3.2f;

        public override string fallEffectChild => "Death1FallEffect";

        private bool hasFiredAttack;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("ER_Colossus_Death1_Play", gameObject);
            PlayAnimation("Death, Overridee", "Death1");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(NetworkServer.active && fixedAge >= fallEffectSpawnTime && !hasFiredAttack)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 15f; // TODO
                blastAttack.procCoefficient = 0f;
                blastAttack.position = fallTransform.position;
                blastAttack.attacker = characterBody.gameObject;
                blastAttack.crit = false;
                blastAttack.baseDamage = 0.5f * damageStat;
                blastAttack.canRejectForce = false;
                blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                blastAttack.baseForce = 3000f;
                blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                blastAttack.damageType = DamageType.NonLethal;
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.Fire();

                hasFiredAttack = true;
            }
        }

    }
}
