using EnemiesReturns.Enemies.Judgement.Arraign;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1
{
    [RegisterEntityState]
    public class Spawn : BaseState
    {
        private static int Spawn1StateHash = Animator.StringToHash("Spawn1");

        private static int Spawn1ParamHash = Animator.StringToHash("Spawn1.playbackRate");

        private static int LandEffectHash = Animator.StringToHash("spawn.landEffect");

        public static float duration = 6f;

        public static string spawnSoundString;

        public static GameObject slamEffect;

        private Animator animator;

        private bool landEffectSpawned;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(spawnSoundString, base.gameObject);
            animator = GetModelAnimator();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0f);
            }
            PlayAnimation("Body", Spawn1StateHash, Spawn1ParamHash, duration);
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!landEffectSpawned && animator.GetFloat(LandEffectHash) > 0.9f)
            {
                var effectData = new EffectData()
                {
                    origin = FindModelChild("MuzzleFloor").position,
                    scale = 7f
                };
                EffectManager.SpawnEffect(slamEffect, effectData, true);
                EntitySoundManager.EmitSoundServer((AkEventIdArg)"Play_moonBrother_spawn", gameObject);
                landEffectSpawned = true;
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                SetNextState();
            }
        }

        private void SetNextState()
        {
            if (base.characterBody.isPlayerControlled)
            {
                outer.SetNextStateToMain();
                return;
            }

            bool playersCanAttack = false;
            var players = Utils.GetActiveAndAlivePlayerBodies();
            foreach (var player in players)
            {
                if (player.inventory)
                {
                    playersCanAttack = playersCanAttack
                        || player.inventory.GetItemCount(Content.Items.HiddenAnointed) > 0
                        || player.inventory.HasEquipment(Content.Equipment.MithrixHammer)
                        || player.inventory.HasEquipment(Content.Equipment.EliteAeonian);
                }
            }
            if (!playersCanAttack)
            {
                foreach (var characterBody in CharacterBody.readOnlyInstancesList)
                {
                    if (characterBody.teamComponent.teamIndex != TeamIndex.Player)
                    {
                        continue;
                    }

                    if (ArraignDamageController.BodyCanBypassArmor(characterBody.bodyIndex))
                    {
                        playersCanAttack = true;
                        break;
                    }
                }
            }
            if (!playersCanAttack)
            {
                outer.SetNextState(new SomebodyGettingFucked());
            }
            else
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1f);
            }
            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                Chat.SendBroadcastChat(new Chat.NpcChatMessage
                {
                    formatStringToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_DIALOGUE_FORMAT",
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_ARRAIGN_SPAWN_P1_2",
                    sender = base.gameObject,
                });
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
