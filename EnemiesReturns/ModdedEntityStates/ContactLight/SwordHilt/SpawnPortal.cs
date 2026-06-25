using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.SwordHilt
{
    [RegisterEntityState]
    public class SpawnPortal : BaseState
    {
        public static GameObject portalContactLight;

        public static string contactLightChatMessageToken = "ENEMIES_RETURNS_JUDGEMENT_PORTAL_OPEN";

        private Animator animator;

        private bool portalOpened;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            PlayCrossfade("Base", "Reforge", 0.6f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(animator && animator.GetFloat("portal.open") > 0.9f && !portalOpened)
            {
                if(NetworkServer.active && portalContactLight)
                {
                    var positionTransform = FindModelChild("PortalSpawnLocation");
                    if (!positionTransform)
                    {
                        positionTransform = transform;
                    }
                    var newObject = UnityEngine.Object.Instantiate(portalContactLight, positionTransform.position, positionTransform.rotation);
                    NetworkServer.Spawn(newObject);

                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = contactLightChatMessageToken
                    });
                }
                portalOpened = true;
            }
        }

    }
}
