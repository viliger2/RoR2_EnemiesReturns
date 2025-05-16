using EnemiesReturns.Reflection;
using EntityStates;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign
{
    [RegisterEntityState]
    public class BasePrimaryWeaponSwing : BasicMeleeAttack, SteppedSkillDef.IStepSetter
    {
        public new static GameObject swingEffectPrefab;

        public new static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion();

        public int swingCount;

        public override void OnEnter()
        {
            this.baseDuration = 0.4f;
            base.damageCoefficient = 2f;
            base.hitBoxGroupName = "Sword";
            base.hitEffectPrefab = hitEffectPrefab;
            base.procCoefficient = 1f;
            base.pushAwayForce = 600f;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.05f;
            base.swingEffectPrefab = swingEffectPrefab;
            base.swingEffectMuzzleString = "Swing1EffectMuzzle";
            base.mecanimHitboxActiveParameter = null;
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "Play_merc_sword_swing"; // TODO: something heavier, got NGB sound archive, grab from Debilarough or whatever its called
            //base.impactSound = "";
            base.forceForwardVelocity = false;
            base.forwardVelocityCurve = null;
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;
            base.duration = base.baseDuration / attackSpeedStat;

            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("UpperBodyOnly", "BufferEmpty", 0.5f);
        }

        public override void PlayAnimation()
        {
            var swingNameState = (swingCount % 2) == 0 ? "Slash1" : "Slash2";
            PlayCrossfade("UpperBodyOnly", swingNameState, "combo.playbackRate", duration, 0.05f);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)swingCount);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingCount = reader.ReadByte();
        }

        public void SetStep(int i)
        {
            swingCount = i;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
