using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class SendChatMessage : MonoBehaviour
    {
        public string messageToken = "";

        public bool withDelay;

        public float delay;

        public bool onAwake = false;

        private void Awake()
        {
            if (onAwake)
            {
                SendMessage();
            }
        }

        public void Send()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            SendMessage();
        }

        private void SendMessage()
        {
            if (withDelay)
            {
                Invoke("SendMessageInvoke", delay);
            }
            else
            {
                SendMessage();
            }
        }

        private void SendMessageInvoke()
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = messageToken,
            });
        }
    }
}
