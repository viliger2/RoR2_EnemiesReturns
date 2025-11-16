using AdvancedPrediction.Prediction;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModCompats
{
    internal static class AncientScepterCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
                }
                return (bool)_enabled;
            }
        }

        public static ItemIndex AncientScepterItemIndex;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void RegisterScepter(SkillDef originalSkill, string bodyName, SkillDef replacementSkill)
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(replacementSkill, bodyName, originalSkill);
        }

        [SystemInitializer(new Type[] { typeof(ItemCatalog) })]
        private static void Init()
        {
            if (!EnemiesReturns.EnemiesReturnsPlugin.ModIsLoaded)
            {
                return;
            }

            if (!enabled)
            {
                return;
            }

            if (!RoR2.ItemCatalog.availability.available)
            {
                Log.Warning("Somehow got here without inialized ItemCatalog.");
            }

            AncientScepterItemIndex = ItemCatalog.FindItemIndex("ITEM_ANCIENT_SCEPTER");
        }
    }
}
