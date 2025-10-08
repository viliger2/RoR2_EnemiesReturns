using EnemiesReturns.Components;
using EnemiesReturns.Reflection;
using EntityStates;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.VoidlingWeapon
{
    [RegisterEntityState]
    public class Fire : BaseNetworkedBodyAttachmentState
    {
        public static float duration = 1.4f;

        public static float disableDuration = 1.2f;

        public static float damageCoefficient = 50f;

        public static GameObject tracerEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_DLC1_VoidRaidCrab.TracerVoidRaidCrabTripleBeamSmall_prefab).WaitForCompletion();

        public static float maxDistance = 300f;

        public GameObject weaponInstance;

        public Transform target;

        private CharacterBody body;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!weaponInstance)
            {
                UnityEngine.Object.Destroy(this.gameObject);
                return;
            }

            var animator = weaponInstance.GetComponentInChildren<Animator>();
            if (animator)
            {
                animator.Play("Fire", animator.GetLayerIndex("Base Layer"));
            }

            body = bodyAttachment.attachedBody;

            if (isAuthority)
            {
                FireBulletAuthority();
            }
            Util.PlaySound("Play_voidRaid_snipe_shoot_final", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > disableDuration && weaponInstance)
            {
                weaponInstance.SetActive(false);
            }
            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new EntityState());
                return;
            }
        }

        private void FireBulletAuthority()
        {
            var aimVector = body.inputBank.aimDirection;
            if (equipmentSlot)
            {
                weaponInstance.transform.LookAt(target);
                aimVector = weaponInstance.transform.forward.normalized;
            }

            BulletAttack bulletAttack = new BulletAttack();
            bulletAttack.owner = body.gameObject;
            bulletAttack.weapon = weaponInstance.gameObject;
            bulletAttack.origin = weaponInstance.transform.position;
            bulletAttack.aimVector = aimVector;
            bulletAttack.minSpread = 0f;
            bulletAttack.maxSpread = 0f;
            bulletAttack.damage = body.damage * damageCoefficient;
            bulletAttack.maxDistance = maxDistance;
            bulletAttack.force = 0;
            bulletAttack.tracerEffectPrefab = tracerEffect;
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            //bulletAttack.tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Commando.TracerCommandoDefault_prefab).WaitForCompletion();
            //bulletAttack.hitEffectPrefab = hitEffectPrefab;
            bulletAttack.isCrit = Util.CheckRoll(body.crit, body.master);
            bulletAttack.radius = 1f;
            bulletAttack.smartCollision = true;
            bulletAttack.trajectoryAimAssistMultiplier = EntityStates.Commando.CommandoWeapon.FirePistol2.trajectoryAimAssistMultiplier; // I don't fucking know
            bulletAttack.hitMask = BulletAttack.defaultHitMask;
            bulletAttack.stopperMask = LayerIndex.world.mask;
            bulletAttack.damageType = DamageSource.Equipment;
            bulletAttack.damageColorIndex = DamageColorIndex.Void;
            bulletAttack.damageType.AddModdedDamageType(Content.DamageTypes.EndGameBossWeapon);
            bulletAttack.Fire();
        }

        public override void Update()
        {
            base.Update();
            if (!weaponInstance)
            {
                return;
            }
            var aimRay = body.inputBank.GetAimRay();
            var angleFromForward = Vector3.SignedAngle(Vector3.forward, new Vector3(aimRay.direction.x, 0, aimRay.direction.z), Vector3.up); // we find how far are we from forward ignoring y axis, so it doesn't affect the angle from forward
            var newRight = Quaternion.AngleAxis(angleFromForward, Vector3.up) * Vector3.right; // using the angle we find our new right to our aim direction
            weaponInstance.transform.position = body.corePosition + newRight * body.bestFitActualRadius;
            if (equipmentSlot)
            {
                weaponInstance.transform.LookAt(equipmentSlot.targetIndicator.targetTransform);
            }
            else
            {
                weaponInstance.transform.forward = body.inputBank.aimDirection;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (weaponInstance)
            {
                UnityEngine.Object.Destroy(weaponInstance);
            }
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
