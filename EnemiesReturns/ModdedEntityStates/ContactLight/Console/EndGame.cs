using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Console
{
    [RegisterEntityState]
    public class EndGame : BaseState
    {
        public static event Action onEndingTriggered;

        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active)
            {
                onEndingTriggered?.Invoke();

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
                Run.instance.BeginGameOver(Content.GameEndings.EscapeIntoPast);
            }
        }
    }
}
