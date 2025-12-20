using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.Enemies.Colossus
{
    public class ColossusAwooga : MonoBehaviour, IOnKilledServerReceiver
    {
        public bool boing = false;

        // league and anime survivors can go fuck themselves
        public static HashSet<string> dames = new HashSet<string>()
        {
            //"commandobody", // what a hot dame
            "hereticbody",
            "dancerbody",
            "huntressbody",
            "loaderbody",
            "magebody",
            "ss2uchirrbody",
            "seamstressbody",
            "submarinerbody",
            "asteriabody",
            "railgunnerbody",
            "seekerbody",
            "drifterbody",
            "rangerbody"
        };

        public void OnKilledServer(DamageReport damageReport)
        {
            if (damageReport != null && damageReport.attacker)
            {
                string bodyName = damageReport.attacker.name ?? "";
                //bodyName = bodyName.Replace("(Clone)", "");
                if (dames.Contains(bodyName.Trim().ToLower()))
                {
                    boing = true;
                }
            }
        }
    }
}
