using EnemiesReturns.Enemies.LynxTribe;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class SubjectParamsChatMessage : SubjectChatMessage
    {
        public string[] paramsTokens;

        public static void Hooks()
        {
            var type = typeof(SubjectParamsChatMessage);
            chatMessageTypeToIndex.Add(type, (byte)chatMessageIndexToType.Count);
            chatMessageIndexToType.Add(type);
        }

        public override string ConstructChatString()
        {
            HG.ArrayUtils.ArrayInsert(ref paramsTokens, 0, GetSubjectName());
            return string.Format(RoR2.Language.GetString(GetResolvedToken()), paramsTokens);
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            if(paramsTokens == null)
            {
                writer.Write((short)0);
                return;
            }
            writer.Write((short)paramsTokens.Length);
            foreach(var param in paramsTokens)
            {
                writer.Write(param);
            }
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            short num = reader.ReadInt16();
            if(num == 0)
            {
                paramsTokens = new string[0];
                return;
            }

            paramsTokens = new string[num];
            for(short i = 0; i < num; i++)
            {
                paramsTokens[i] = reader.ReadString();
            }
        }
    }
}
