using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.MithrixHammer
{
    public class Fire : BaseMithrixHammerState
    {
        public static float duration = 1f;

        public static float initialFrames = 0.2f;

        private HitBoxGroup hitbox;

        private OverlapAttack attack;

        private CharacterBody body;

        public override void OnEnter()
        {
            base.OnEnter();
            if (bodyAttachment)
            {
                body = bodyAttachment.attachedBody;
            }

            hitbox = GetComponent<HitBoxGroup>();

            if (hitbox)
            {
                attack = SetupAttack(hitbox);
            }

            if (NetworkServer.active && master && master.inventory)
            {
                master.inventory.SetEquipmentDisabled(true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge < initialFrames)
            {
                return;
            }

            if (attack != null && isAuthority)
            {
                attack.Fire();
            }

            if(fixedAge >= duration)
            {
                outer.SetNextState(new Idle());
            }
        }

        private OverlapAttack SetupAttack(HitBoxGroup hitbox)
        {
            return new OverlapAttack()
            {
                attacker = bodyGameObject,
                damage = 2000000f,
                damageColorIndex = DamageColorIndex.Fragile,
                hitBoxGroup = hitbox,
                isCrit = RoR2.Util.CheckRoll(body.crit, master),
                inflictor = bodyGameObject,
                procCoefficient = 0f,
                teamIndex = body.teamComponent.teamIndex,
                pushAwayForce = 10000f,
                //damageType = // TODO
            };
        }

        public override void OnExit()
        {
            base.OnExit();
            if(NetworkServer.active && master && master.inventory)
            {
                master.inventory.SetEquipmentDisabled(false);
            }
        }

    }
}
