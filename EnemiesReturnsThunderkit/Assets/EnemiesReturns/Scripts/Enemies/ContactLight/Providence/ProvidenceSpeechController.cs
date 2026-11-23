using RoR2;
using RoR2.CharacterSpeech;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.ContactLight.Providence
{
    [RequireComponent(typeof(SimpleCombatSpeechDriver))]
    [RequireComponent(typeof(CharacterSpeechController))]
    public class ProvidenceSpeechController : BaseCharacterSpeechDriver
    {
        public CharacterSpeechController.SpeechInfo[] spawnInfos;

        public CharacterSpeechController.SpeechInfo[] seeHereticResponses;

        public CharacterSpeechController.SpeechInfo[] seeFalseSonResponses;

        public CharacterSpeechController.SpeechInfo[] seeVoidFiendResponses;

        public CharacterSpeechController.SpeechInfo[] seeTitanGoldResponses;

        public CharacterSpeechController.SpeechInfo[] killTitanGoldResponses;

        public CharacterSpeechController.SpeechInfo[] killHereticResponses;

        private SimpleCombatSpeechDriver simpleCombatSpeechDriver;

        private new void Awake()
        {
            base.Awake();
            if (NetworkServer.active)
            {
                simpleCombatSpeechDriver = GetComponent<SimpleCombatSpeechDriver>();
                simpleCombatSpeechDriver.onBodyKill.AddListener(OnBodyKill);
            }
        }

        public void DoSpawnResponse()
        {
            CharacterSpeechController.SpeechInfo[] responsePool = Array.Empty<CharacterSpeechController.SpeechInfo>();
            TrySetResponsePool(spawnInfos, ref responsePool);
            SendReponseFromPool(responsePool);
        }

        public void DoInitialSightResponse()
        {
            bool hereticExists = false;
            bool voidFiendExists = false;
            bool falseSonExists = false;
            bool titanGoldExists = false;

            ReadOnlyCollection<CharacterBody> readOnlyInstancesList = CharacterBody.readOnlyInstancesList;
            for(int i = 0; i < readOnlyInstancesList.Count; i++)
            {
                var bodyIndex = readOnlyInstancesList[i].bodyIndex;
                hereticExists |= bodyIndex == RoR2Content.BodyPrefabs.HereticBody.bodyIndex;
                voidFiendExists |= bodyIndex == DLC1Content.BodyPrefabs.VoidSurvivorBody.bodyIndex;
                falseSonExists |= bodyIndex == DLC2Content.BodyPrefabs.FalseSonBody.bodyIndex || bodyIndex == DLC2Content.BodyPrefabs.FalseSonBossBody.bodyIndex;
                titanGoldExists |= bodyIndex == RoR2Content.BodyPrefabs.TitanGoldBody.bodyIndex;
            }

            CharacterSpeechController.SpeechInfo[] responsePool = Array.Empty<CharacterSpeechController.SpeechInfo>();

            if (hereticExists)
            {
                TrySetResponsePool(seeHereticResponses, ref responsePool);
            }
            if (voidFiendExists)
            {
                TrySetResponsePool(seeVoidFiendResponses, ref responsePool);
            }
            if(falseSonExists)
            {
                TrySetResponsePool(seeFalseSonResponses, ref responsePool);
            }
            if(titanGoldExists)
            {
                TrySetResponsePool(seeTitanGoldResponses, ref responsePool);
            }
            SendReponseFromPool(responsePool);
        }

        private void OnBodyKill(DamageReport damageReport)
        {
            CharacterSpeechController.SpeechInfo[] responsePool = Array.Empty<CharacterSpeechController.SpeechInfo>();
            if (damageReport.victimBody)
            {
                if(damageReport.victimBodyIndex == RoR2Content.BodyPrefabs.HereticBody.bodyIndex)
                {
                    TrySetResponsePool(killHereticResponses, ref responsePool);
                }
                if(damageReport.victimBodyIndex == RoR2Content.BodyPrefabs.TitanGoldBody.bodyIndex)
                {
                    TrySetResponsePool(killTitanGoldResponses, ref responsePool);
                }
                SendReponseFromPool(responsePool);
            }
        }

        private void TrySetResponsePool(CharacterSpeechController.SpeechInfo[] newResponsePool, ref CharacterSpeechController.SpeechInfo[] responsePool)
        {
            if (responsePool.Length == 0)
            {
                responsePool = newResponsePool;
            }
        }

        private void SendReponseFromPool(CharacterSpeechController.SpeechInfo[] responsePool)
        {
            if (responsePool.Length != 0)
            {
                base.characterSpeechController.EnqueueSpeech(in responsePool[UnityEngine.Random.Range(0, responsePool.Length - 1)]);
            }
        }
    }
}
