using EnemiesReturns.Reflection;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1
{
    [RegisterEntityState]
    public class SomebodyGettingFucked : BaseMonsterEmoteState
    {
        public override float duration => -1f;

        public override string soundEventPlayName => "";

        public override string soundEventStopName => "";

        public override string layerName => "Gesture, Override";

        public override string animationName => "EmoteEnter";

        public override bool stopOnDamage => false;

        public override float healthFraction => 0f;

        public static float swordSpawnMoment = 2.68f;

        public static float nanomachines = 20f;

        private Transform swordEmote;

        private bool swordSpawned;

        private Transform normalText;

        private Transform funnyText;

        private bool saidTheMeme;

        public override void OnEnter()
        {
            base.OnEnter();
            swordEmote = FindModelChild("EmoteSword");
            ChildLocator component = SceneInfo.instance.GetComponent<ChildLocator>();
            if ((bool)component)
            {
                normalText = component.FindChild("NormalImmuneText");
                if (normalText)
                {
                    normalText.gameObject.SetActive(false);
                }

                funnyText = component.FindChild("FunnyImmuneText");
                if (funnyText)
                {
                    funnyText.gameObject.SetActive(true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > swordSpawnMoment && !swordSpawned)
            {
                if (swordEmote)
                {
                    swordEmote.gameObject.SetActive(true);
                }
                swordSpawned = true;
            }
            if (fixedAge > nanomachines && !saidTheMeme)
            {
                if (NetworkServer.active)
                {
                    Chat.SendBroadcastChat(new Chat.NpcChatMessage
                    {
                        formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                        baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_NANOMACHINES",
                        sender = base.gameObject,
                    });
                }
                saidTheMeme = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (swordEmote)
            {
                swordEmote.gameObject.SetActive(false);
            }

            if (funnyText)
            {
                funnyText.gameObject.SetActive(false);
            }

            if (normalText)
            {
                normalText.gameObject.SetActive(true);
            }
        }
    }
}
