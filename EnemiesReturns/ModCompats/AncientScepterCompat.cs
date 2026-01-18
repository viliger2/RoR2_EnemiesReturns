using RoR2;
using RoR2.Skills;
using System;
using System.Runtime.CompilerServices;

namespace EnemiesReturns.ModCompats
{
    internal static class AncientScepterCompat
    {
        public const string PLUGIN_GUID = "com.DestroyedClone.AncientScepter";

        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID);
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
