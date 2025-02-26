using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Elites.Aeonian
{
    public class AeonianFactory
    {
        public BuffDef CreateAeoninanBuff(Sprite icon, EliteDef elite)
        {
            var buffDef = ScriptableObject.CreateInstance<BuffDef>();
            (buffDef as ScriptableObject).name = "bdAeoninan";
            buffDef.iconSprite = icon;
            buffDef.buffColor = Color.white;
            buffDef.canStack = false;
            buffDef.eliteDef = elite;

            return buffDef;
        }
    }
}
