using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Hellzone
{
    public class FireHellzoneStart : BaseState
    {
        public static float baseDuration = 1.8f;

        public static float maxSearchDistance = 60f;

        public static string attackString = "ER_Ifrit_BreathIn_Fireball_Play";

        private Predictor predictor;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            Util.PlayAttackSpeedSound(attackString, gameObject, attackSpeedStat);
            PlayAnimation("Gesture,Override", "FireballStart", "FireFireball.playbackRate", duration);
            if (NetworkServer.active)
            {
                BullseyeSearch search = new BullseyeSearch();
                search.teamMaskFilter = TeamMask.allButNeutral;
                if (teamComponent)
                {
                    search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
                }
                search.maxDistanceFilter = maxSearchDistance;
                search.maxAngleFilter = 90f;
                var aimRay = GetAimRay();
                search.searchOrigin = aimRay.origin;
                search.searchDirection = aimRay.direction;
                search.filterByLoS = false;
                search.sortMode = BullseyeSearch.SortMode.Angle;
                search.RefreshCandidates();
                var hurtBox = search.GetResults().FirstOrDefault();
                if (hurtBox)
                {
                    predictor = new Predictor(base.transform);
                    predictor.SetTargetTransform(hurtBox.transform);
                } else
                {
                    foreach (var ai in characterBody.master.aiComponents)
                    {
                        if (!ai.currentEnemy.characterBody)
                        {
                            continue;
                        }

                        predictor = new Predictor(base.transform);
                        predictor.SetTargetTransform(ai.currentEnemy.characterBody.transform);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                var nextState = new FireHellzoneFire();
                nextState.predictor = predictor;
                outer.SetNextState(nextState);
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
