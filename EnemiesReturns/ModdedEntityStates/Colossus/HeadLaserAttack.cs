using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    public class HeadLaserAttack : BaseState
    {
        public static float baseDuration = 5f;

        //public static GameObject beamPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/VoidRaidCrabSpinBeamVFX.prefab").WaitForCompletion();
        //public static GameObject beamPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/LaserMajorConstruct.prefab").WaitForCompletion();
        public static GameObject beamPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamCorrupt.prefab").WaitForCompletion();
        
        private float duration;

        private Animator modelAnimator;

        private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

        private static readonly int aimPitchCycleHas = Animator.StringToHash("aimPitchCycle");

        private GameObject beamInstance;

        private Transform headpoint;

        private AnimationCurve curve;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            headpoint = modelLocator.modelTransform.Find("Armature/root/root_pelvis_control/spine/spine.001/head/headpoint");
            curve = new AnimationCurve(
                new Keyframe()
                {
                    inTangent = -0.3f,
                    inWeight = 0f,
                    outTangent = -0.3f,
                    outWeight = 0.33f,
                    tangentMode = 34,
                    time = 0f,
                    value = 0f,
                    weightedMode = 0f,
                },
                new Keyframe()
                {
                    inTangent = -0.3f,
                    inWeight = 0.33f,
                    outTangent = -0.3f,
                    outWeight = 0f,
                    tangentMode = 34,
                    time = 1f,
                    value = -0.3f,
                    weightedMode = 0f,
                }
            );
            curve.postWrapMode = WrapMode.ClampForever;
            curve.preWrapMode = WrapMode.ClampForever;
            beamInstance = UnityEngine.Object.Instantiate(beamPrefab);
            beamInstance.transform.localScale = new Vector3(15f, 15f, beamInstance.transform.localScale.z);
            beamInstance.transform.SetParent(headpoint, worldPositionStays: true);
            PlayAnimation("Body", "LaserBeamLoop");
            UpdateBeamTransforms();
            RoR2Application.onLateUpdate += UpdateBeamTransformsInLateUpdate;
            //beamInstance = UnityEngine.Object.Instantiate(beamPrefab, headpoint);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(modelAnimator)
            {
                modelAnimator.SetFloat(aimYawCycleHash, Mathf.Clamp(fixedAge / duration, 0f, 1f));
            }

            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new HeadLaserEnd());
            }
        }

        private void UpdateBeamTransformsInLateUpdate()
        {
            try
            {
                UpdateBeamTransforms();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            modelAnimator.SetFloat(aimPitchCycleHas, 0.5f);
        }

        private void UpdateBeamTransforms()
        {
            Ray beamRay = GetBeamRay();
            beamInstance.transform.SetPositionAndRotation(beamRay.origin, Quaternion.LookRotation(beamRay.direction));
        }

        protected Ray GetBeamRay()
        {
            Vector3 forward = headpoint.forward;
            forward.y = curve.Evaluate(base.fixedAge / duration);
            forward.Normalize();
            return new Ray(headpoint.position, forward);
        }

        public override void OnExit()
        {
            base.OnExit();
            RoR2Application.onLateUpdate -= UpdateBeamTransformsInLateUpdate;
            UnityEngine.Object.Destroy(beamInstance);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
