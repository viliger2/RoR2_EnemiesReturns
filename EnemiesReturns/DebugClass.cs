using EnemiesReturns.Quests;
using RoR2;
using RoR2.CharacterSpeech;
using RoR2.Quests;
using RoR2.Stats;
using RoR2.UI.LogBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using static RoR2.CharacterSpeech.CharacterSpeechController;
using static RoR2.CharacterSpeech.SimpleCombatSpeechDriver;
using static RoR2.OverlapAttack;

namespace EnemiesReturns
{
#if DEBUG || NOWEAVER
    public class DebugClass
    {
        public static void DebugHooks()
        {
            //On.RoR2.CharacterSpeech.SimpleCombatSpeechDriver.TriggerSpeech += SimpleCombatSpeechDriver_TriggerSpeech;
            //On.RoR2.CharacterSpeech.SimpleCombatSpeechDriver.SubmitSpeechRequestFromCategory += SimpleCombatSpeechDriver_SubmitSpeechRequestFromCategory;
            //On.RoR2.CharacterSpeech.CharacterSpeechController.Process += CharacterSpeechController_Process;
        }

        private static void CharacterSpeechController_Process(On.RoR2.CharacterSpeech.CharacterSpeechController.orig_Process orig, CharacterSpeechController self, float deltaTime)
        {
            self.localTime += deltaTime;
            if (!(self.nextSpeakTime <= self.localTime))
            {
                return;
            }
            int bestNextRequestIndex = self.GetBestNextRequestIndex();
            if (bestNextRequestIndex == -1)
            {
                return;
            }
            SpeechRequest speechRequest = self.speechRequestQueue[bestNextRequestIndex];
            self.speechRequestQueue.RemoveRange(0, bestNextRequestIndex + 1);
            self.SpeakNow(in speechRequest.speechInfo);
        }

        private static void SimpleCombatSpeechDriver_SubmitSpeechRequestFromCategory(On.RoR2.CharacterSpeech.SimpleCombatSpeechDriver.orig_SubmitSpeechRequestFromCategory orig, RoR2.CharacterSpeech.SimpleCombatSpeechDriver self, ref SpeechCategory category)
        {
            if (category.speechSelection == null || category.speechSelection.Count <= 0)
            {
                if (category.speechInfos.Length == 0)
                {
                    return;
                }
                self.InitializeCategory(ref category);
            }
            int num = category.speechSelection.EvaluateToChoiceIndex(UnityEngine.Random.value);
            RoR2.CharacterSpeech.SimpleCombatSpeechDriver.SpeechInfo value = category.speechSelection.GetChoice(num).value;
            if (self.removeRepeats)
            {
                category.speechSelection.RemoveChoice(num);
            }
            self.lastSpeechStopwatch = 0f;
            CharacterSpeechController obj = self.characterSpeechController;
            CharacterSpeechController.SpeechInfo speechInfo = new CharacterSpeechController.SpeechInfo
            {
                token = value.nameToken,
                duration = self.minimumDurationBetweenSpeech,
                maxWait = self.minimumDurationBetweenSpeech,
                mustPlay = false
            };
            obj.EnqueueSpeech(in speechInfo);
        }

        private static void SimpleCombatSpeechDriver_TriggerSpeech(On.RoR2.CharacterSpeech.SimpleCombatSpeechDriver.orig_TriggerSpeech orig, RoR2.CharacterSpeech.SimpleCombatSpeechDriver self, string refName)
        {
            if (self.lastSpeechStopwatch < self.minimumDurationBetweenSpeech)
            {
                return;
            }
            int num = self.FindSpeechCategoryIndexByName(refName);
            if (num == -1)
            {
                Debug.LogWarningFormat("Speech category \"{0}\" could not be found.", refName);
                return;
            }
            ref SpeechCategory reference = ref self.speechCategories[num];
            if (UnityEngine.Random.value < reference.chanceToTrigger)
            {
                self.SubmitSpeechRequestFromCategory(ref reference);
            }
        }
    }
#endif
}
