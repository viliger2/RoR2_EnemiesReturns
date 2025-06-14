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
        public static float firstSpeechTimer = 4f;

        public static float secondSpeechTimer = 12f;

        public static float thirdSpeechTimer = 18f;

        private bool spokeFirst = false;

        private bool spokeSecond = false; 

        private bool spokeThird = false;

        public override void OnEnter()
        {
            duration = 20f;
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 20f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && !spokeFirst && fixedAge > firstSpeechTimer)
            {
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P2_1",
                    sender = base.gameObject,
                });
                spokeFirst = true;
            }
            if (NetworkServer.active && !spokeSecond && fixedAge > secondSpeechTimer)
            {
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P2_2",
                    sender = base.gameObject,
                });
                spokeSecond = true;
            }
            if (NetworkServer.active && !spokeThird && fixedAge > thirdSpeechTimer)
            {
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P2_3",
                    sender = base.gameObject,
                });
                spokeThird = true;
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
            }
        }
    }
}
