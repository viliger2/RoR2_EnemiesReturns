using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Console
{
    [RegisterEntityState]
    public class WaitForGameEnd : BaseState
    {
        private GenericInteraction genericInteraction;

        public override void OnEnter()
        {
            base.OnEnter();
            genericInteraction = gameObject.GetComponent<GenericInteraction>();
            if (genericInteraction)
            {
                genericInteraction.onActivation = new GenericInteraction.InteractorUnityEvent(); // removing existing listeners
                genericInteraction.onActivation.AddListener(EndGame);

                genericInteraction.SetInteractabilityAvailable();
            }
            if (NetworkServer.active)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "ENEMIES_RETURNS_CONTACTLIGHT_CONSOLE_GAME_END_ACTIVE"
                });
            }
        }

        private void EndGame(Interactor activator)
        {
            genericInteraction.SetInteractabilityDisabled();
            outer.SetNextState(new EndGame());
        }

        public override void OnExit()
        {
            base.OnExit();
            if (genericInteraction)
            {
                genericInteraction.onActivation.RemoveListener(EndGame);
            }
        }


    }
}
