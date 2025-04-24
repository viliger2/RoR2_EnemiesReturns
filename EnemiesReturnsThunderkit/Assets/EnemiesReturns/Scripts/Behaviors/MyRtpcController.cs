using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class MyRtpcController : MonoBehaviour
    {
        public string akSoundString;

        public string rtpcString;

        public float rtpcValue;

        public bool useCurveInstead;

        public AnimationCurve rtpcValueCurve;

        private float fixedAge;

        private void OnEnable()
        {
            if (akSoundString.Length > 0)
            {
                Util.PlaySound(akSoundString, base.gameObject, rtpcString, rtpcValue);
            }
            else
            {
                //AkSoundEngine.SetRTPCValue(rtpcString, rtpcValue, base.gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (useCurveInstead)
            {
                fixedAge += Time.fixedDeltaTime;
                //AkSoundEngine.SetRTPCValue(rtpcString, rtpcValueCurve.Evaluate(fixedAge), base.gameObject);
            }
        }
    }
}
