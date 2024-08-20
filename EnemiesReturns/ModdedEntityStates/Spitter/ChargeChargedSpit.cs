using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Rewired.InputMapper;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class ChargeChargedSpit : GenericCharacterMain
    {
        public static float baseDuration = 1.1f;

        public static GameObject chargeVfxPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoFistEffect.prefab").WaitForCompletion();

        public static string attackString = ""; // TODO: come up with something

        private float duration;

        private GameObject chargeVfxInstance;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            //GetModelAnimator();
            var modelTransform = GetModelTransform();
            Util.PlayAttackSpeedSound(attackString, gameObject, attackSpeedStat);
            if (modelTransform)
            {
                var childLocator = modelTransform.GetComponent<ChildLocator>();
                if(childLocator)
                {
                    var muzzleMouth = childLocator.FindChild("MuzzleMouth");
                    if(muzzleMouth)
                    {
                        chargeVfxInstance = UnityEngine.Object.Instantiate(chargeVfxPrefab, muzzleMouth.position, muzzleMouth.rotation);
                        chargeVfxInstance.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                        chargeVfxInstance.transform.parent = muzzleMouth;
                    }
                }
            }

            PlayAnimation("Gesture", "ChargeSpit", "ChargedSpit.playbackRate", duration);
            if (characterBody)
            {
                characterBody.SetAimTimer(2f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
            if (chargeVfxInstance)
            {
                EntityState.Destroy(chargeVfxInstance);
            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && isAuthority)
            {
                //outer.SetNextStateToMain();
                //EntityStateMachine.FindByCustomName(characterBody.gameObject, "Body")?.SetState(RoR2.EntityStateCatalog.InstantiateState(typeof(ModdedEntityStates.Spitter.FireChargedSpit)));
                outer.SetNextState(new FireChargedSpit());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
