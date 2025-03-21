using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class ImmuneDamageNumbers : MonoBehaviour
    {
        private List<Vector4> customData = new List<Vector4>();

        private ParticleSystem ps;

        public static ImmuneDamageNumbers instance { get; private set; }

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            instance = SingletonHelper.Assign(instance, this);
        }

        private void OnDisable()
        {
            instance = SingletonHelper.Unassign(instance, this);
        }

        public void SpawnDamageNumber(Vector3 position)
        {
            ps.Emit(new ParticleSystem.EmitParams
            {
                position = position,
                startColor = Color.gray * Color.blue,
                applyShapeToPosition = true
            }, 1);
            ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
            customData[customData.Count - 1] = new Vector4(1f, 0f, 122345f, 0f);
            ps.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        }
    }
}
