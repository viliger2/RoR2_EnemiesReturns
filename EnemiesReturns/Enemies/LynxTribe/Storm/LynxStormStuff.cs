using EnemiesReturns.ModCompats.PrefabAPICompat;
using EntityStates.VoidRaidCrab.Leg;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Storm
{
    public class LynxStormStuff
    {
        public static BuffDef StormImmunity;

        public BuffDef CreateStormImmunityBuff()
        {
            BuffDef defBuff = ScriptableObject.CreateInstance<BuffDef>();
            (defBuff as ScriptableObject).name = "bdLynxStormImmunity";
            defBuff.isDebuff = false;
            defBuff.canStack = false;
            defBuff.isCooldown = true;
            defBuff.isDOT = false;
            defBuff.isHidden = true;
            defBuff.buffColor = Color.green;

            return defBuff;
        }
    }
}
