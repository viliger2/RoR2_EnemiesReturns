using RoR2;
using System;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns.ModCompats
{
    internal static class RiskyModCompat
    {
        public static ItemIndex RiskyModAllyMarker;

        public static ItemIndex RiskyModAllyScaling;

        public static ItemIndex RiskyModAllyRegen;

        public static ItemIndex RiskyModAllyAllowVoidDeath;

        public static ItemIndex RiskyModAllyAllowOverheatDeath;

        public static bool enabled;

        [SystemInitializer(new Type[] { typeof(ItemCatalog) })]
        private static void Init()
        {
            if (!RoR2.ItemCatalog.availability.available)
            {
                Log.Warning("Somehow got here without inialized ItemCatalog.");
            }

            RiskyModAllyMarker = ItemCatalog.FindItemIndex("RiskyModAllyMarkerItem");
            RiskyModAllyScaling = ItemCatalog.FindItemIndex("RiskyModAllyScalingItem");
            RiskyModAllyRegen = ItemCatalog.FindItemIndex("RiskyModAllyRegenItem");
            RiskyModAllyAllowVoidDeath = ItemCatalog.FindItemIndex("RiskyModAllyAllowVoidDeathItem");
            RiskyModAllyAllowOverheatDeath = ItemCatalog.FindItemIndex("RiskyModAllyAllowOverheatDeathItem");

            if (RiskyModAllyMarker != ItemIndex.None)
            {
                enabled = true;
            }
        }
    }
}
