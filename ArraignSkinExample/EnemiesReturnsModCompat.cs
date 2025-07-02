using EnemiesReturns.Components;
using EnemiesReturns.Configuration.Judgement;
using EnemiesReturns.Enemies.Judgement;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArraignSkinExample
{
    public static class EnemiesReturnsModCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(EnemiesReturns.EnemiesReturnsPlugin.GUID);
                }
                return (bool)_enabled;
            }
        }

        public static SkinDef CreateHiddenSkinDef(string bodyName, SkinDef skin, bool addEliteRamp)
        {
            return EnemiesReturns.Enemies.Judgement.AnointedSkins.CreateAnointedSkin(bodyName, skin, addEliteRamp);
        }
    }
}
