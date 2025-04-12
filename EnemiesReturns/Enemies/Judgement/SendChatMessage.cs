using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Judgement
{
    public class SendChatMessage : MonoBehaviour
    {
        public string messageToken = "";

        public bool withDelay;

        public float delay;

        public void Send()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (withDelay)
            {
                Invoke("SendMessage", delay);
            } else
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = messageToken,
            });
        }
    }
}
