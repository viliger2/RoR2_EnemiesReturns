using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(BossGroup))]
    public class BossGroupHealthColorOverride : MonoBehaviour
    {
        public Color healthBarColorOverride;


        // public static void Hooks()
        // {
        //     On.RoR2.UI.HUDBossHealthBarController.OnClientDamageNotified += HUDBossHealthBarController_OnClientDamageNotified;
        // }

        // private static void HUDBossHealthBarController_OnClientDamageNotified(On.RoR2.UI.HUDBossHealthBarController.orig_OnClientDamageNotified orig, RoR2.UI.HUDBossHealthBarController self, DamageDealtMessage damageDealtMessage)
        // {
        //     orig(self, damageDealtMessage);
        //     if (self.currentBossGroup && self.fillRectImage)
        //     {
        //         var component = self.currentBossGroup.gameObject.GetComponent<BossGroupHealthColorOverride>();
        //         if (component)
        //         {
        //             self.fillRectImage.color = component.healthBarColorOverride;
        //         }
        //     }
        // }
    }
}
