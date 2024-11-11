using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.HeadLaserBarrage
{
    public class HeadLaserBarrageStart : BaseState
    {
        public static float baseDuration = 4.5f;

        public static float baseChargeEffectDuration = 4f;

        public static float targetPitch => EnemiesReturns.Configuration.Colossus.LaserBarrageHeadPitch.Value;

        public static float initialEmmision = 0f;

        public static float finalEmmision => ColossusFactory.MAX_BARRAGE_EMISSION;

        public static float initialLightRange = 0f;

        public static float finalLightRange => ColossusFactory.MAX_EYE_LIGHT_RANGE;

        public static float initialSpotlightRange = 0f;

        public static float finalSpotlightRange => ColossusFactory.MAX_SPOT_LIGHT_RANGE;

        private float duration;

        private float chargeEffectDuration;

        private float startYaw;

        private float startPitch;

        private Animator modelAnimator;

        private Renderer eyeRenderer;

        private MaterialPropertyBlock eyePropertyBlock;

        private Light headLight;

        private Light spotlight;

        private Transform laserChargeParticles;

        private float _initialLightRange;

        private float _initialEmission;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                startYaw = modelAnimator.GetFloat(MissingAnimationParameters.aimYawCycle);
                startPitch = modelAnimator.GetFloat(MissingAnimationParameters.aimPitchCycle);
            };

            var childLocator = GetModelChildLocator();

            eyeRenderer = childLocator.FindChildComponent<Renderer>("EyeModel");
            eyePropertyBlock = new MaterialPropertyBlock();
            _initialEmission = initialEmmision;
            if (_initialEmission == 0f)
            {
                _initialEmission = eyeRenderer.material.GetFloat("_EmPower");
            }
            eyePropertyBlock.SetFloat("_EmPower", initialEmmision);
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);

            headLight = childLocator.FindChildComponent<Light>("HeadLight");
            _initialLightRange = initialLightRange;
            if (_initialLightRange == 0f)
            {
                initialLightRange = headLight.range;
            }

            laserChargeParticles = childLocator.FindChild("LaserChargeParticles");
            if (laserChargeParticles)
            {
                laserChargeParticles.gameObject.SetActive(true);
            }

            var spotlight = childLocator.FindChild("LaserChargeSpotlight");
            if (spotlight)
            {
                this.spotlight = spotlight.GetComponent<Light>();
                spotlight.gameObject.SetActive(true);
            }

            duration = baseDuration / attackSpeedStat;
            chargeEffectDuration = baseDuration / attackSpeedStat;
            PlayCrossfade("Body", "LaserBeamStart", "Laser.playbackrate", duration, 0.1f);
            Util.PlayAttackSpeedSound("ER_Colossus_Barrage_Charge_Play", gameObject, attackSpeedStat);
        }

        public override void Update()
        {
            base.Update();
            if (modelAnimator)
            {
                modelAnimator.SetFloat(MissingAnimationParameters.aimYawCycle, Mathf.Clamp(Mathf.Lerp(startYaw, 0f, age / duration), 0f, 0.99f));
                modelAnimator.SetFloat(MissingAnimationParameters.aimPitchCycle, Mathf.Clamp(Mathf.Lerp(startPitch, targetPitch, age / duration), 0f, 0.99f));
            }

            if (age <= chargeEffectDuration)
            {
                eyePropertyBlock.SetFloat("_EmPower", Mathf.Lerp(_initialEmission, finalEmmision, age / chargeEffectDuration));
                eyeRenderer.SetPropertyBlock(eyePropertyBlock);
                if (headLight)
                {
                    headLight.range = Mathf.Lerp(_initialLightRange, finalLightRange, age / chargeEffectDuration);
                }
                if (spotlight)
                {
                    spotlight.range = Mathf.Lerp(initialSpotlightRange, finalSpotlightRange, age / chargeEffectDuration);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (laserChargeParticles)
            {
                laserChargeParticles.gameObject.SetActive(false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new HeadLaserBarrageAttack());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
