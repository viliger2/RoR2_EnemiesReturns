using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class ImmuneDamageNumbers : MonoBehaviour
    {
        [SerializeField]
        public float[] textData;

        private List<Vector4> customData = new List<Vector4>();

        private ParticleSystem ps;

        public static ImmuneDamageNumbers instance { get; private set; }

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            if(textData.Length == 0)
            {
                //Log.Warning("ImmuneDamageNumbers textData array lengh is zero!");
                this.gameObject.SetActive(false);
            }
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
                startColor = Color.white * Color.white,
                applyShapeToPosition = true
            }, 1);
            ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
            float value = 122345f;
            if(textData.Length == 1)
            {
                value = textData[0];
            } else
            {
                value = textData[UnityEngine.Random.Range(0, textData.Length)];
            }
            customData[customData.Count - 1] = new Vector4(1, 0, value, 0);
            ps.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        }
    }
}
