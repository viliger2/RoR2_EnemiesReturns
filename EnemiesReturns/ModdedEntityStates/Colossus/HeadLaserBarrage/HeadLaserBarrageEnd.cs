using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.HeadLaserBarrage
{
    public class HeadLaserBarrageEnd : BaseState
    {
        public static float baseDuration = 3.5f;

        public static float initialEmission = ColossusFactory.MAX_BARRAGE_EMISSION;

        public static float finalLightRange = ColossusFactory.normalEyeLightRange;

        public static float initialSpotlightRange = ColossusFactory.MAX_SPOT_LIGHT_RANGE;

        public static float finalSpotlightRange = 0f;

        public static float finalEmission = 0f;

        private float duration;

        private float startYaw;

        private float startPitch;

        private Animator modelAnimator;

        private Renderer eyeRenderer;

        private MaterialPropertyBlock eyePropertyBlock;

        private float _finalEmission;

        private float initialLightRange;

        private Light headLight;

        private Light spotlight;

        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;

            childLocator = GetModelChildLocator();

            eyeRenderer = childLocator.FindChildComponent<Renderer>("EyeModel");
            eyePropertyBlock = new MaterialPropertyBlock();
            _finalEmission = finalEmission;
            if (_finalEmission == 0f)
            {
                _finalEmission = eyeRenderer.material.GetFloat("_EmPower");
            }
            eyePropertyBlock.SetFloat("_EmPower", initialEmission);
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);

            headLight = childLocator.FindChildComponent<Light>("HeadLight");
            initialLightRange = headLight.range;

            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                startYaw = modelAnimator.GetFloat(MissingAnimationParameters.aimYawCycle);
                startPitch = modelAnimator.GetFloat(MissingAnimationParameters.aimPitchCycle);
            }

            spotlight = childLocator.FindChildComponent<Light>("LaserChargeSpotlight");
            PlayCrossfade("Body", "LaserBeamEnd", "Laser.playbackrate", duration, 0.1f);
        }

        public override void Update()
        {
            base.Update();
            if (modelAnimator)
            {
                modelAnimator.SetFloat(MissingAnimationParameters.aimYawCycle, Mathf.Clamp(Mathf.Lerp(startYaw, 0.5f, age / duration), 0f, 0.99f));
                modelAnimator.SetFloat(MissingAnimationParameters.aimPitchCycle, Mathf.Clamp(Mathf.Lerp(startPitch, 0.5f, age / duration), 0f, 0.99f));
            }
            if (headLight) 
            {
                headLight.range = Mathf.Lerp(initialLightRange, finalLightRange, age / duration);
            }
            if (spotlight) 
            {
                spotlight.range = Mathf.Lerp(initialSpotlightRange, finalSpotlightRange, age / duration);
            }
            eyePropertyBlock.SetFloat("_EmPower", Mathf.Lerp(initialEmission, _finalEmission, age / duration));
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            headLight.range = finalLightRange;
            eyePropertyBlock.SetFloat("_EmPower", _finalEmission);
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);
            var childLocator = GetModelChildLocator();
            var spotlight = childLocator.FindChild("LaserChargeSpotlight");
            if(spotlight)
            {
                spotlight.gameObject.SetActive(false);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
