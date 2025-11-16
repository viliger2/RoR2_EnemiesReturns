using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot
{
    public abstract class BaseChargeFire : BaseState
    {
        public static GameObject effectPrefab;

        public static float baseDuration => Configuration.MechanicalSpider.DoubleShotChargeDuration.Value;

        public abstract string soundString { get; }

        private float duration;

        private GameObject chargeEffect;

        protected EffectManagerHelper _efh_Charge;

        public override void OnEnter()
        {
            base.OnEnter();
            if (teamComponent.teamIndex == TeamIndex.Player)
            {
                duration = baseDuration / attackSpeedStat;
            }
            else
            {
                duration = baseDuration;
            }
            SpawnEffect(FindModelChild("GunNozzle"));
            PlayAnimation("Gesture, Additive", "ChargeFire", "Fire.playbackRate", duration);
            Util.PlayAttackSpeedSound(soundString, gameObject, attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge > duration)
            {
                outer.SetNextState(GetNextState());
            }
        }

        public abstract EntityState GetNextState();

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            base.OnExit();
            DestroyEffect();
        }

        private void SpawnEffect(Transform transform)
        {
            if (!EffectManager.ShouldUsePooledEffect(effectPrefab))
            {
                chargeEffect = UnityEngine.Object.Instantiate(effectPrefab, transform.position, transform.rotation);
            }
            else
            {
                _efh_Charge = EffectManager.GetAndActivatePooledEffect(effectPrefab, transform.position, transform.rotation);
                chargeEffect = _efh_Charge.gameObject;
            }
            chargeEffect.transform.parent = transform;
            ScaleParticleSystemDuration component2 = chargeEffect.GetComponent<ScaleParticleSystemDuration>();
            if ((bool)component2)
            {
                component2.newDuration = duration;
            }
        }

        private void DestroyEffect()
        {
            if ((bool)chargeEffect)
            {
                if (!EffectManager.UsePools)
                {
                    Destroy(chargeEffect);
                }
                else if (_efh_Charge != null && _efh_Charge.OwningPool != null)
                {
                    if (!_efh_Charge.OwningPool.IsObjectInPool(_efh_Charge))
                    {
                        _efh_Charge.OwningPool.ReturnObject(_efh_Charge);
                    }
                }
                else
                {
                    if (_efh_Charge != null)
                    {
                        Debug.LogFormat("ChargeFire has no owning pool {0} {1}", gameObject.name, gameObject.GetInstanceID());
                    }
                    Destroy(chargeEffect);
                }
            }
        }
    }

}
