using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.Colossus
{
    public class ColossusFootstepHandler : FootstepHandler
    {
        public new void Footstep(AnimationEvent animationEvent)
        {
            // if(animationEvent.animatorClipInfo.weight > 0.5f)
            // {
            //     Footstep(animationEvent.stringParameter, (GameObject)animationEvent.objectReferenceParameter);
            // }
        }

        public new void Footstep(string childName, GameObject footstepEffect)
        {
            // base.Footstep(childName, footstepEffect);
            // Transform transform = childLocator.FindChild(childName);

            // if (transform)
            // {
            //     BlastAttack blastAttack = new BlastAttack();
            //     blastAttack.radius = 10f; // TODO
            //     blastAttack.procCoefficient = 0f;
            //     blastAttack.position = transform.position;
            //     blastAttack.attacker = body.gameObject;
            //     blastAttack.crit = false;
            //     blastAttack.baseDamage = 0f;
            //     blastAttack.canRejectForce = false;
            //     blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
            //     blastAttack.baseForce = 2000f;
            //     blastAttack.teamIndex = body.teamComponent.teamIndex;
            //     blastAttack.damageType = DamageType.NonLethal;
            //     blastAttack.attackerFiltering = AttackerFiltering.Default;
            //     blastAttack.Fire();
            // }

        }

    }
}
