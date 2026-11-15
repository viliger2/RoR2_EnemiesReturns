using EnemiesReturns.Quests;
using RoR2;
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
using static RoR2.OverlapAttack;

namespace EnemiesReturns
{
#if DEBUG || NOWEAVER
    public class DebugClass
    {
        public static void DebugHooks()
        {
            //On.RoR2.Quests.QuestRunner.OnDeserialize += QuestRunner_OnDeserialize;
            //On.RoR2.Quests.QuestRunner.OnSerialize += QuestRunner_OnSerialize;
            //On.RoR2.RoR2Application.OnLoad += RoR2Application_OnLoad;
            //FunTimeWithWhatevers();
            //On.EntityStates.StunState.PlayStunAnimation += StunState_PlayStunAnimation;
            //On.RoR2.UI.LogBook.LogBookController.GetMonsterStatus += LogBookController_GetMonsterStatus;
            //On.RoR2.OverlapAttack.HurtBoxPassesFilter += OverlapAttack_HurtBoxPassesFilter;
            //On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;
        }

        private static bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HurtBox> hitResults)
        {
            if (!self.hitBoxGroup)
            {
                return false;
            }
            HitBox[] hitBoxes = self.hitBoxGroup.hitBoxes;
            foreach (HitBox hitBox in hitBoxes)
            {
                if (!hitBox || !hitBox.enabled || !hitBox.gameObject || !hitBox.gameObject.activeInHierarchy || !hitBox.transform)
                {
                    continue;
                }
                Transform transform = hitBox.transform;
                Vector3 position = transform.position;
                Vector3 halfExtents = transform.lossyScale * 0.5f;
                Quaternion rotation = transform.rotation;
                if (float.IsInfinity(halfExtents.x) || float.IsInfinity(halfExtents.y) || float.IsInfinity(halfExtents.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are infinite.");
                    continue;
                }
                if (float.IsNaN(halfExtents.x) || float.IsNaN(halfExtents.y) || float.IsNaN(halfExtents.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are NaN.");
                    continue;
                }
                if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is infinite.");
                    continue;
                }
                if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is NaN.");
                    continue;
                }
                if (float.IsInfinity(rotation.x) || float.IsInfinity(rotation.y) || float.IsInfinity(rotation.z) || float.IsInfinity(rotation.w))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is infinite.");
                    continue;
                }
                if (float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || float.IsNaN(rotation.z) || float.IsNaN(rotation.w))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is NaN.");
                    continue;
                }
                int num = 0;
                Collider[] colliders;
                if (self.isSphereOverlap)
                {
                    Vector3 lossyScale = transform.lossyScale;
                    float radius = Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
                    num = HGPhysics.OverlapSphere(out colliders, position, radius, LayerIndex.entityPrecise.mask);
                }
                else
                {
                    num = HGPhysics.OverlapBox(out colliders, position, halfExtents, rotation, LayerIndex.entityPrecise.mask);
                }
                self.cleanupRetriggerList();
                discoveredHurtboxes.Clear();
                damageProcessorHurtboxes.Clear();
                self.charactersWithAttackHitProcessors.Clear();
                for (int j = 0; j < num; j++)
                {
                    if (!colliders[j])
                    {
                        continue;
                    }
                    HurtBox component = colliders[j].GetComponent<HurtBox>();
                    if (!component || !component.transform || self.HealthComponentAlreadyHit(component))
                    {
                        continue;
                    }
                    discoveredHurtboxes.Add(component);
                    if ((bool)component.healthComponent && component.hasAttackHitRejector)
                    {
                        damageProcessorHurtboxes.Add(component);
                        if (!self.charactersWithAttackHitProcessors.Contains(component.healthComponent))
                        {
                            self.charactersWithAttackHitProcessors.Add(component.healthComponent);
                        }
                    }
                }
                self.hasDamageProcessingHurtboxes = damageProcessorHurtboxes.Count > 0;
                num = discoveredHurtboxes.Count;
                int num2 = 0;
                for (int k = 0; k < num; k++)
                {
                    HurtBox hurtBox = discoveredHurtboxes[k];
                    if (!self.HurtBoxPassesFilter(hurtBox))
                    {
                        Log.Info($"{hurtBox} didnt pass filter");
                        continue;
                    }
                    Vector3 position2 = hurtBox.transform.position;
                    AttackUtils.ResolveHitOriginArgs resolveHitOriginArgs = default(AttackUtils.ResolveHitOriginArgs);
                    resolveHitOriginArgs.attackCenter = position;
                    resolveHitOriginArgs.attackOriginPreference = self.attackOriginPreference;
                    resolveHitOriginArgs.attacker = self.attacker;
                    resolveHitOriginArgs.inflictor = self.inflictor;
                    AttackUtils.ResolveHitOriginArgs args = resolveHitOriginArgs;
                    OverlapInfo overlapInfo = default(OverlapInfo);
                    overlapInfo.hurtBox = hurtBox;
                    overlapInfo.hitPosition = position2;
                    overlapInfo.pushDirection = (position2 - position).normalized;
                    overlapInfo.hitOrigin = AttackUtils.ResolveHitOrigin(in args);
                    OverlapInfo overlapInfo2 = overlapInfo;
                    if (self.hasDamageProcessingHurtboxes)
                    {
                        bool flag = false;
                        for (int l = 0; l < damageProcessorHurtboxes.Count; l++)
                        {
                            if (damageProcessorHurtboxes[l].AttackHitRejectsHurtbox(new HurtBox.HurtBoxAttackHitProcessorArgs(overlapInfo2, self.attacker)))
                            {
                                if (!self.HealthComponentAlreadyHit(overlapInfo2.hurtBox))
                                {
                                    self.addIgnoredHitList(overlapInfo2.hurtBox.healthComponent);
                                }
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            Log.Info("Damage proccessing hitboxes or something");
                            continue;
                        }
                    }
                    self.overlapList.Add(overlapInfo2);
                    self.lastHitObject = hurtBox.healthComponent.gameObject;
                    self.addIgnoredHitList(hurtBox.healthComponent);
                    hitResults?.Add(hurtBox);
                    num2++;
                    if (num2 >= self.maximumOverlapTargets)
                    {
                        break;
                    }
                }
                HGPhysics.ReturnResults(colliders);
            }
            Log.Info($"actually process hits, number of hits: {self.overlapList.Count}");
            self.ProcessHits(self.overlapList);
            bool result = self.overlapList.Count > 0;
            self.overlapList.Clear();
            return result;
        }

        private static bool OverlapAttack_HurtBoxPassesFilter(On.RoR2.OverlapAttack.orig_HurtBoxPassesFilter orig, OverlapAttack self, HurtBox hurtBox)
        {
            var result = orig(self, hurtBox);
            Log.Info($"result for {hurtBox.healthComponent.gameObject} is {result}");
            return result;
        }

        private static RoR2.UI.LogBook.EntryStatus LogBookController_GetMonsterStatus(On.RoR2.UI.LogBook.LogBookController.orig_GetMonsterStatus orig, ref RoR2.UI.LogBook.Entry entry, UserProfile viewerProfile)
        {
            CharacterBody characterBody = (CharacterBody)entry.extraData;
            UnlockableDef unlockableDef = characterBody.GetComponent<DeathRewards>()?.logUnlockableDef;
            if (!unlockableDef)
            {
                return EntryStatus.None;
            }
            if (viewerProfile.HasUnlockable(unlockableDef))
            {
                return EntryStatus.Available;
            }
            if (viewerProfile.statSheet.GetStatValueULong(PerBodyStatDef.killsAgainst, characterBody.gameObject.name) != 0)
            {
                return EntryStatus.Unencountered;
            }
            return EntryStatus.Locked;
        }

        private static void StunState_PlayStunAnimation(On.EntityStates.StunState.orig_PlayStunAnimation orig, EntityStates.StunState self)
        {
            Animator modelAnimator = self.GetModelAnimator();
            if ((bool)modelAnimator)
            {
                int layerIndex = modelAnimator.GetLayerIndex("Body");
                modelAnimator.CrossFadeInFixedTime((UnityEngine.Random.Range(0, 2) == 0) ? "Hurt1" : "Hurt2", 0.1f);
                modelAnimator.Update(0f);
                AnimatorStateInfo nextAnimatorStateInfo = modelAnimator.GetNextAnimatorStateInfo(layerIndex);
                self.duration = Mathf.Max(self.duration, Mathf.Max(self.stunDuration, nextAnimatorStateInfo.length));
                if (self.stunDuration >= 0f)
                {
                    if ((bool)self.stunVfxInstance)
                    {
                        self.ReleaseStunVFX();
                    }
                    if (!EffectManager.ShouldUsePooledEffect(EntityStates.StunState.stunVfxPrefab))
                    {
                        self.stunVfxInstance = UnityEngine.Object.Instantiate(EntityStates.StunState.stunVfxPrefab, self.transform);
                    }
                    else
                    {
                        self._efhStunEffect = EffectManager.GetAndActivatePooledEffect(EntityStates.StunState.stunVfxPrefab, self.transform);
                        self.stunVfxInstance = self._efhStunEffect.gameObject;
                    }
                    ScaleParticleSystemDuration component = self.stunVfxInstance.GetComponent<ScaleParticleSystemDuration>();
                    component.newDuration = self.duration;
                    component.UpdateDuration();
                }
            }
            else
            {
                self.duration = self.stunDuration;
            }
        }

        private static bool QuestRunner_OnSerialize(On.RoR2.Quests.QuestRunner.orig_OnSerialize orig, QuestRunner self, UnityEngine.Networking.NetworkWriter writer, bool initialState)
        {
            //System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            //Log.Info(t.ToString());
            Log.Info("started quest serialization");
            uint num = self.syncVarDirtyBits;
            if (initialState)
            {
                num = 7u;
            }
            bool num2 = (num & 1) != 0;
            bool flag = (num & 2) != 0;
            bool flag2 = (num & 4) != 0;
            writer.Write((byte)num);
            if (num2)
            {
                Log.Info("we are at num2, sending quest index");
                writer.Write(self.quest?.questIndex ?? QuestIndex.None);
                Log.Info("quest index " + (int)(self.quest?.questIndex ?? QuestIndex.None));
            }
            if (flag)
            {
                Log.Info("flag, serializing quest");
                self.quest?.OnSerialize(writer);
            }
            if (flag2)
            {
                Log.Info("flag2, next quest info");
                bool flag3 = self.hasNextQuest;
                Log.Info("has next quest " + flag3);
                writer.Write(flag3);
                if (flag3)
                {
                    Log.Info("questCompletedSuccesfully " + self.questCompletedSuccesfully);
                    writer.Write(self.questCompletedSuccesfully);
                    Log.Info("quest index " + (int)(self.nextQuest?.questIndex ?? QuestIndex.None));
                    writer.Write(self.nextQuest?.questIndex ?? QuestIndex.None);
                    Log.Info("serializing quest");
                    self.nextQuest?.OnSerialize(writer);
                }
            }
            Log.Info("ended quest serialization");
            if (!initialState)
            {
                return num != 0;
            }
            return false;
        }

        private static System.Collections.IEnumerator RoR2Application_OnLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2Application self)
        {
            yield return orig(self);
            Log.Info("quest index" + QuestCatalog.FindQuestIndex(typeof(CompulsionQuestSurviveIfritPillars)));
        }

        private static void QuestRunner_OnDeserialize(On.RoR2.Quests.QuestRunner.orig_OnDeserialize orig, RoR2.Quests.QuestRunner self, UnityEngine.Networking.NetworkReader reader, bool initialState)
        {
            Log.Info("started deserializing quest");
            byte num = reader.ReadByte();
            bool flag = (num & 1) != 0; // 1 is quest
            bool flag2 = (num & 2) != 0; // 2 is quest data
            bool num2 = (num & 4) != 0; // 4 is next quest
            if (flag)
            {
                Log.Info("flag, has quest, does quest.onexit and new quest instance");
                self.quest?.OnExit();
                self.quest = QuestCatalog.CreateQuestInstance(reader.ReadQuestIndex());
                if (self.quest != null)
                {
                    Log.Info("quest is not null, setting the runner");
                    self.quest.runner = self;
                }
            }
            if (flag2)
            {
                Log.Info("flag2, has quest data, does deserialization and on enter");
                self.quest?.OnDeserialize(reader);
                if (flag)
                {
                    Log.Info("we also have quest so we enter it");
                    self.quest?.OnEnter();
                }
            }
            if (num2 && reader.ReadBoolean())
            {
                Log.Info("num2, has next quest or something");
                self.hasNextQuest = true;
                self.hasChangedQuest = false;
                self.questCompletedSuccesfully = reader.ReadBoolean();
                self.nextQuest = QuestCatalog.CreateQuestInstance(reader.ReadQuestIndex());
                self.nextQuest?.OnDeserialize(reader);
            }
            Log.Info("ended deserializing quest");

        }

        private static void FunTimeWithWhatevers()
        {
            var locator = Addressables.ResourceLocators.FirstOrDefault(l => l.LocatorId == "AddressablesMainContentCatalog") as ResourceLocationMap;

            if (locator == null)
            {
                Log.Error("Couldn't find game's locator");
                return;
            }

            var guidRegex = new Regex("^[0-9a-f]{32}$", RegexOptions.Compiled);

            foreach (var (key, locations) in locator.Locations)
            {

                if(!(key is string guid))
                {
                    continue;
                }

                if(guid.Length != 32 || !guidRegex.IsMatch(guid))
                {
                    continue;
                }

                foreach (var location in locations)
                {
                    var type = typeof(ChampRushQuadrantEncounter);

                    if(location.ResourceType == type)
                    {
                        Log.Info(guid);
                        var encounter = Addressables.LoadAssetAsync<ChampRushQuadrantEncounter>(guid).WaitForCompletion();
                        PrintEncounterInfo(encounter);
                    }
                }
            }
        }

        private static void PrintEncounterInfo(ChampRushQuadrantEncounter encounter)
        {
            //Log.Info("Begin Encounter Info " + encounter.name);
            Log.Info("Encounter name: " + encounter.nameToken);
            //Log.Info("Spawn Requests: ");
            Log.Info("Card\tHealth\tCount\tisElite\teliteStats\thasItems\tTotalHealthWithCountAndStats");
            var allSpawnsHealth = 0f;
            var allSpawnsHealthWithElite = 0f;
            foreach(var spawnRequest in encounter.spawnRequests)
            {
                var health = spawnRequest.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().baseMaxHealth;
                var totalHealth = health * spawnRequest.total;
                var totalHealthWithElite = totalHealth;
                var eliteStats = 0f;
                if (spawnRequest.startingLoadout.equipment && !spawnRequest.startingLoadout.noEliteStats)
                {
                    eliteStats = spawnRequest.startingLoadout.equipment.passiveBuffDef.eliteDef.healthBoostCoefficient;
                    totalHealthWithElite *= eliteStats;
                }

                Log.Info($"{spawnRequest.spawnCard}\t{health}\t{spawnRequest.total}\t{(bool)spawnRequest.startingLoadout.equipment}\t{eliteStats}\t{spawnRequest.startingLoadout.startingItems.Length > 0}\t{totalHealthWithElite}");

                allSpawnsHealth += totalHealth;
                allSpawnsHealthWithElite += totalHealthWithElite;
            }
            Log.Info($"Total Health: {allSpawnsHealth}, Total Health With Elite Bonuses: {allSpawnsHealthWithElite}");
            //Log.Info("End Encounter Info " + encounter.name);
        }
    }
#endif
}
