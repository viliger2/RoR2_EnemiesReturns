using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class Ending : BaseState
    {
        public static GameObject destroyEffectPrefab => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BrittleDeath.prefab").WaitForCompletion();

        public float delay = 3f;
        private bool ended = false;

        public static event Action onArraignDefeated;

        public override void OnEnter()
        {
            base.OnEnter();

            var childLocator = gameObject.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var judgementMission = childLocator.FindChild("JudgementMission");
                if(judgementMission)
                {
                    judgementMission.gameObject.SetActive(false);
                }
                var ending = childLocator.FindChild("Ending");
                if (ending)
                {
                    ending.gameObject.SetActive(true);
                }
            }

            if (NetworkServer.active)
            {
                onArraignDefeated?.Invoke(); 
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > delay && !ended && NetworkServer.active)
            {
                ended = true;
                Run.instance.BeginGameOver(Content.GameEndings.SurviveJudgement);
            }
        }
    }
}
