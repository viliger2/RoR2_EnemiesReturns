using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide
{
    public class BaseSlideState : BaseState
    {
        public static float duration = 0.3f;

        public static AnimationCurve speedCoefficientCurve;

        public static string soundString = "Play_moonBrother_dash";

        public static GameObject slideEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherDashEffect.prefab").WaitForCompletion();

        public static string slideEffectMuzzlestring = "MuzzleCenter";

        protected Quaternion slideRotation;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(soundString, base.gameObject);
            if ((bool)slideEffectPrefab && (bool)base.characterBody)
            {
                Vector3 position = base.characterBody.corePosition;
                Quaternion rotation = Quaternion.identity;
                Transform transform = FindModelChild(slideEffectMuzzlestring);
                if ((bool)transform)
                {
                    position = transform.position;
                }
                if ((bool)base.characterDirection)
                {
                    rotation = Util.QuaternionSafeLookRotation(slideRotation * base.characterDirection.forward, Vector3.up);
                }
                EffectManager.SimpleEffect(slideEffectPrefab, position, rotation, transmit: false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                Vector3 vector = Vector3.zero;
                if ((bool)base.inputBank && (bool)base.characterDirection)
                {
                    vector = base.characterDirection.forward;
                }
                if ((bool)base.characterMotor)
                {
                    float num = speedCoefficientCurve.Evaluate(base.fixedAge / duration);
                    base.characterMotor.rootMotion += slideRotation * (num * characterBody.baseMoveSpeed * vector * GetDeltaTime());
                }
                if (base.fixedAge >= duration)
                {
                    SetNextState();
                }
            }
        }

        public virtual void SetNextState()
        {
            outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
