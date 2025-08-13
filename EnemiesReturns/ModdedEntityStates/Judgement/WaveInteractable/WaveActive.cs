using EnemiesReturns.Behaviors.Judgement.WaveInteractable;
using EnemiesReturns.Reflection;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    // TODO: investigate wave end effect not firing on clients
    [RegisterEntityState]
    public class WaveActive : BaseJudgementIntaractable
    {
        public static string soundEntryEvent = "Play_boss_spawn_radius_appear";

        public static float gracePeriod = 10f;

        private Transform waveStartedEffects;

        public override void OnEnter()
        {
            base.OnEnter();
            if (childLocator)
            {
                waveStartedEffects = childLocator.FindChild("WaveStartedEffect");
                if (waveStartedEffects)
                {
                    waveStartedEffects.gameObject.SetActive(true);
                }
            }
            Util.PlaySound(soundEntryEvent, gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge < gracePeriod)
            {
                return;
            }

            if (isAuthority && JudgementMissionController.instance)
            {
                var instance = JudgementMissionController.instance;
                var endRound = true;
                for (int i = 0; i < instance.combatDirectors.Length; i++)
                {
                    var director = instance.combatDirectors[i];
                    endRound = endRound && (director.combatSquad.defeatedServer || director.combatSquad.membersList.Count == 0);
                }
                if (endRound)
                {
                    instance.EndRound();
                    if (instance.maxWaves <= instance.currentRound)
                    {
                        outer.SetNextState(new Inactive());
                    }
                    else
                    {
                        outer.SetNextState(new AwaitingSelection());
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (waveStartedEffects)
            {
                waveStartedEffects.gameObject.SetActive(false);
            }
            if (childLocator)
            {
                var waveFinishedEffect = childLocator.FindChild("WaveFinishedEffect");
                if (waveFinishedEffect)
                {
                    waveFinishedEffect.gameObject.SetActive(true);
                }
            }
        }

    }
}
