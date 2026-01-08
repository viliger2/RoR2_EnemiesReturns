using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    [CreateAssetMenu(menuName = "EnemiesReturns/PrefabDef")]
    public class PrefabDef : ScriptableObject
    {
        public GameObject prefab;
    }
}
