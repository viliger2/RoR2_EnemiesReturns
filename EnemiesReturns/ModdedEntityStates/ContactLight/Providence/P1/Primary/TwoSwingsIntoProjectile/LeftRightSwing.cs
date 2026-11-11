using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Primary.TwoSwingsIntoProjectile
{
    [RegisterEntityState]
    public class LeftRightSwing : BaseLeftRightSwing
    {
        public override float damageCoefficient => 2f;

        public override float baseDuration => 3f;

        public override string hitBoxGroupName => "SwordHitbox";

        public override float procCoefficient => 1f;

        public override float pushAwayForce => 500f;

        public override Vector3 forceVector => Vector3.zero;

        public override float hitPauseDuration => 0.1f;

        public override string mecanimHitboxActiveParameter => "Slash1.attack";

        public override float shorthopVelocityFromHit => 0f;

        public override string beginStateSoundString => ""; // TODO

        public override string beginSwingSoundString => "ER_Providence_Swing_Play"; // TODO

        public override bool forceForwardVelocity => false;

        public override bool scaleHitPauseDurationAndVelocityWithAttackSpeed => false;

        public override bool ignoreAttackSpeed => false;

        public override DamageTypeCombo damageType => DamageTypeCombo.GenericPrimary;

        public override float earlyExit => 2f;

        public override string swingEffectMuzzleString { get => _swingEffectMuzzleString; set => _swingEffectMuzzleString = value; }

        public static string fireCloneMecanimParam = "FireClone.attack";

        public static GameObject cloneProjectile;

        public static float maxSearchDistance = 250f;

        public static float maxSearchAngle = 75f;

        public static float cloneProjectileSpeed = 50f;

        public static float cloneProjectileDamage = 2f;

        public static float projectileHealthThreshold = 0.85f;

        private string _swingEffectMuzzleString;

        private bool firstSwing;

        public override void OnEnter()
        {
            swingEffectMuzzleString = "FirstSwingMuzzle";
            base.OnEnter();
        }

        public override EntityState GetNextStateIfMissed()
        {
            return new FireProjectiles();
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("UpperBodyOnly", "Slash", "combo.playbackRate", duration, 0.1f);
        }

        public override void AuthorityFixedUpdate()
        {
            base.AuthorityFixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
//            PlayAnimation("UpperBodyOnly", "BufferEmpty");
        }

        public override void BeginMeleeAttackEffect()
        {
            if(meleeAttackStartTime != Run.FixedTimeStamp.positiveInfinity && !firstSwing)
            {
                firstSwing = true;
            }
            if(meleeAttackStartTime == Run.FixedTimeStamp.positiveInfinity && firstSwing)
            {
                swingEffectMuzzleString = "SecondSwingMuzzle";
            }
            // it works fine when I copy paste the method, it doesn't if I just call base, whatever
            //base.BeginMeleeAttackEffect();
            if (meleeAttackStartTime != Run.FixedTimeStamp.positiveInfinity)
            {
                return;
            }
            meleeAttackStartTime = Run.FixedTimeStamp.now;
            Util.PlaySound(beginSwingSoundString, base.gameObject);
            if (!swingEffectPrefab)
            {
                return;
            }
            Transform transform = FindModelChild(swingEffectMuzzleString);
            if ((bool)transform)
            {
                if (!EffectManager.ShouldUsePooledEffect(swingEffectPrefab))
                {
                    swingEffectInstance = Object.Instantiate(swingEffectPrefab, transform);
                }
                else
                {
                    _emh_swingEffectInstance = EffectManager.GetAndActivatePooledEffect(swingEffectPrefab, transform, inResetLocal: true);
                    swingEffectInstance = _emh_swingEffectInstance.gameObject;
                }
                ScaleParticleSystemDuration component = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if ((bool)component)
                {
                    component.newDuration = component.initialDuration;
                }
            }
        }
    }
}
