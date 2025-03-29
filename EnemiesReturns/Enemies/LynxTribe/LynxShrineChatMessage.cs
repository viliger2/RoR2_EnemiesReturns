using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using static RoR2.Chat;

namespace EnemiesReturns.Enemies.LynxTribe
{
    public class LynxShrineChatMessage : PlayerPickupChatMessage 
    {
        public static void Hooks()
        {
            var type = typeof(LynxShrineChatMessage);
            RoR2.ChatMessageBase.chatMessageTypeToIndex.Add(type, (byte)chatMessageIndexToType.Count);
            RoR2.ChatMessageBase.chatMessageIndexToType.Add(type);
        }

        public override string ConstructChatString()
        {
            string subjectName = GetSubjectName();
            string chatString = RoR2.Language.GetString(GetResolvedToken());

            string itemName = RoR2.Language.GetString(pickupToken) ?? "???";
            itemName = Util.GenerateColoredString(itemName, pickupColor);
            try
            {
                return string.Format(chatString, subjectName, itemName);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return "";
        }
    }
}
