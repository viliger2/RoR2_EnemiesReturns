using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class PrePhase2 : BaseState
    {
        public static float phaseDuration = 10f;

        public static float speechTimer = 5f;

        private bool spoke = false;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(NetworkServer.active && !spoke && fixedAge > speechTimer)
            {
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P1_1",
                    sender = base.gameObject,
                });
                spoke = true;
            }

            if(fixedAge > phaseDuration && isAuthority)
            {
                outer.SetNextState(new Phase2());
            }
        }
    }
}
