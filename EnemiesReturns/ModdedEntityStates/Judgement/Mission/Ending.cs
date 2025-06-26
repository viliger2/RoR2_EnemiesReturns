using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class Ending : BaseState
    {
        public float delay = 0.5f;
        private bool ended = false;

        public static event Action onArraignDefeated;

        public override void OnEnter()
        {
            base.OnEnter();

            var childLocator = gameObject.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var judgementMission = childLocator.FindChild("JudgementMission");
                if (judgementMission)
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
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
                if (teamMembers.Count > 0)
                {
                    GameObject gameObject = teamMembers[0].gameObject;
                    CharacterBody component = gameObject.GetComponent<CharacterBody>();
                    if ((bool)component)
                    {
                        EntityState.Destroy(gameObject.gameObject);
                    }
                }
                Run.instance.BeginGameOver(Content.GameEndings.SurviveJudgement);
            }
        }
    }
}
