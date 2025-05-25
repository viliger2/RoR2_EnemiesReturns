using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static R2API.DamageAPI;

namespace EnemiesReturns.ModdedEntityStates.Judgement.MithrixHammer
{
    [RegisterEntityState]
    public class Fire : BaseMithrixHammerState
    {
        public static float duration = 0.75f;

        public static float initialFrames = 0.03f;

        public static float endFrames = 0.3f;

        public static GameObject swingEffect;

        public static GameObject swingVFX = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSwing1, Kickup.prefab").WaitForCompletion();

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/OmniImpactVFXHuntress.prefab").WaitForCompletion();

        public static Vector3 spawnEffectVector = new Vector3(270f, 180f, 0);

        public static float damageCoefficient = 15f;

        public static ModdedDamageType damageType => Content.DamageTypes.EndGameBossWeapon;

        private OverlapAttack attack;

        private CharacterBody body;

        private GameObject hitBoxObject;

        private InputBankTest bodyInputBank;

        private GameObject hammerEffect;

        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            base.OnEnter();

            body = bodyAttachment.attachedBody;

            bodyInputBank = bodyGameObject.GetComponent<InputBankTest>();

            hitBoxObject = new GameObject("HammerHitBoxObject")
            {
                layer = LayerIndex.defaultLayer.intVal
            };
            hitBoxObject.transform.localScale = new Vector3(6.5999999f, 1.5f, 5f);

            var hitBox = hitBoxObject.AddComponent<HitBox>();
            var hitBoxGroup = hitBoxObject.AddComponent<HitBoxGroup>();
            hitBoxGroup.groupName = "HammerHitBox";
            hitBoxGroup.hitBoxes = new HitBox[] {hitBox };
           
            attack = SetupAttack(hitBoxGroup);

            hammerEffect = UnityEngine.Object.Instantiate(swingEffect);
            hammerEffect.transform.position = bodyInputBank.aimOrigin;
            hammerEffect.transform.forward = bodyInputBank.aimDirection;

            var childLocator = hammerEffect.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var effectOrigin = childLocator.FindChild("SwingVFXOrigin");
                if (effectOrigin) 
                {
                    swingEffectInstance = UnityEngine.Object.Instantiate(swingVFX, effectOrigin.transform);
                    //swingEffectInstance.transform.position = effectOrigin.transform.position;
                    swingEffectInstance.transform.localRotation = Quaternion.Euler(spawnEffectVector);
                    swingEffectInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    //swingEffectInstance.transform.localRotation = Quaternion.Euler(-180f, 0f, 0f);
                }
            }

            Util.PlaySound("Play_moonBrother_swing_horizontal", bodyGameObject);
        }

        public override void Update()
        {
            base.Update();
            if (hammerEffect)
            {
                hammerEffect.transform.position = bodyInputBank.aimOrigin;
                hammerEffect.transform.forward = bodyInputBank.aimDirection;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge < initialFrames)
            {
                return;
            }

            if (fixedAge < endFrames && attack != null && isAuthority)
            {
                hitBoxObject.transform.forward = bodyInputBank.aimDirection;
                hitBoxObject.transform.position = bodyInputBank.aimOrigin + bodyInputBank.aimDirection * 3f;
                attack.Fire();
            }

            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new Idle());
            }
        }

        private OverlapAttack SetupAttack(HitBoxGroup hitbox)
        {
            var overlap =  new OverlapAttack()
            {
                attacker = bodyGameObject,
                damage = body.damage * damageCoefficient,
                damageColorIndex = DamageColorIndex.Fragile,
                hitBoxGroup = hitbox,
                isCrit = false,
                inflictor = bodyGameObject,
                procCoefficient = 0f,
                teamIndex = body.teamComponent.teamIndex,
                pushAwayForce = 10000f,
                hitEffectPrefab = hitEffectPrefab,
                damageType = DamageSource.Equipment,
            };

            overlap.damageType.AddModdedDamageType(damageType);

            return overlap;
        }

        public override void OnExit()
        {
            base.OnExit();
            if (hitBoxObject)
            {
                UnityEngine.Object.Destroy(hitBoxObject);
            }
            if (swingEffectInstance)
            {
                UnityEngine.Object.Destroy(swingEffectInstance);
            }
            if(NetworkServer.active)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }

    }
}
