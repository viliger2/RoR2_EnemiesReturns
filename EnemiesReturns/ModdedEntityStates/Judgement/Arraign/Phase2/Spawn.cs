using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
    public class Spawn : GenericCharacterSpawnState
    {
        public static float speechTimer = 7f;

        private bool spoke = false;

        public override void OnEnter()
        {
            duration = 9f;
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 10f);
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P2_1",
                    sender = base.gameObject,
                });
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && !spoke && fixedAge > speechTimer)
            {
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P2_2",
                    sender = base.gameObject,
                });
                spoke = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P2_3",
                    sender = base.gameObject,
                });
            }
        }
    }
}
