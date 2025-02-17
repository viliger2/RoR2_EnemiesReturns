using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot
{
    public class ChargeFire : BaseState
    {
        public static GameObject effectPrefab;

        public static float baseDuration => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotChargeDuration.Value;

        public static string soundString = "ER_Spider_Fire_Charge_Play";

        public static string soundStringMinion = "ER_Spider_Fire_Charge_Drone_Play";

        private float duration;

        private GameObject chargeEffect;

        protected EffectManagerHelper _efh_Charge;

        private bool isMinion = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (teamComponent.teamIndex == TeamIndex.Player)
            {
                duration = baseDuration / attackSpeedStat;
            } else
            {
                duration = baseDuration;
            }
            SpawnEffect(FindModelChild("GunNozzle"));
            PlayAnimation("Gesture, Additive", "ChargeFire", "Fire.playbackRate", duration);
            isMinion = characterBody.inventory.GetItemCount(RoR2Content.Items.MinionLeash) > 0;
            Util.PlayAttackSpeedSound(isMinion ? soundStringMinion : soundString, base.gameObject, attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge > duration)
            {
                outer.SetNextState(new Fire());
            }
        }

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
                    EntityState.Destroy(chargeEffect);
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
                        Debug.LogFormat("ChargeFire has no owning pool {0} {1}", base.gameObject.name, base.gameObject.GetInstanceID());
                    }
                    EntityState.Destroy(chargeEffect);
                }
            }
        }
    }
}
