using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.MithrixHammer
{
    public class Fire : BaseMithrixHammerState
    {
        public static float duration = 1f;

        public static float initialFrames = 0.05f;

        public static float endFrames = 0.4f;

        public static GameObject swingEffect;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/OmniImpactVFXHuntress.prefab").WaitForCompletion();

        private OverlapAttack attack;

        private CharacterBody body;

        private GameObject hitBoxObject;

        private InputBankTest bodyInputBank;

        private GameObject hammerEffect;

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
            return new OverlapAttack()
            {
                attacker = bodyGameObject,
                damage = 2000000f,
                damageColorIndex = DamageColorIndex.Fragile,
                hitBoxGroup = hitbox,
                isCrit = false,
                inflictor = bodyGameObject,
                procCoefficient = 0f,
                teamIndex = body.teamComponent.teamIndex,
                pushAwayForce = 10000f,
                hitEffectPrefab = hitEffectPrefab
                //damageType = // TODO
            };
        }

        public override void OnExit()
        {
            base.OnExit();
            if (hitBoxObject)
            {
                UnityEngine.Object.Destroy(hitBoxObject);
            }
            if(NetworkServer.active)
            {
                UnityEngine.Object.Destroy(this.gameObject);
            }
        }

    }
}
