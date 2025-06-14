using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    [RegisterEntityState]
    public class EnterSkyLeap : BaseEnterSkyLeap
    {
        public override float baseDuration => 1.5f;

        public override string soundString => "";

        public override string layerName => "Gesture, Override";

        public override string stateName => "EnterSkyLeap";

        public override string playbackRateParam => "SkyLeap.playbackRate";

        public override bool addBuff => true;

        public static GameObject dashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherDashEffect.prefab").WaitForCompletion();

        public static GameObject footstepEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherBigFootstep.prefab").WaitForCompletion();

        private bool effectsSpawned = false;

        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
        }

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new HoldSkyLeap());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(animator && animator.GetFloat("SkyLeap.spawnLeapEffect") > 0.95f && !effectsSpawned)
            {
                var dashEffectData = new EffectData()
                {
                    origin = FindModelChild("MuzzleFloor").position,
                    rotation = Quaternion.Euler(new Vector3(270f, 0, 0))
                };
                EffectManager.SpawnEffect(dashEffect, dashEffectData, false);

                var footLEffectData = new EffectData()
                {
                    origin = FindModelChild("FootL").position
                };
                EffectManager.SpawnEffect(footstepEffect, footLEffectData, false);

                var footREffectData = new EffectData()
                {
                    origin = FindModelChild("FootR").position
                };

                EffectManager.SpawnEffect(footstepEffect, footREffectData, false); 
                
                Util.PlaySound("Play_moonBrother_phaseJump_jumpAway", base.gameObject);
                effectsSpawned = true;
            }
        }
    }
}
