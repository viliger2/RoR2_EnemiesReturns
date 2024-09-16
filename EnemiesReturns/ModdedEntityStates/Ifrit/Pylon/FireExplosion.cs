using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pylon
{
    public class FireExplosion : BaseState
    {
        public static float duration = 1f; // just to be safe

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.radius = 100f; // TODO
                blastAttack.procCoefficient = 0f;
                blastAttack.position = transform.position;
                blastAttack.crit = false;
                blastAttack.baseDamage = 0.5f * damageStat;
                blastAttack.canRejectForce = false;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 3000f;
                blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                blastAttack.damageType = DamageType.Generic;
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && base.isAuthority)
            {
                base.healthComponent.Suicide();
            }
        }
    }
}
