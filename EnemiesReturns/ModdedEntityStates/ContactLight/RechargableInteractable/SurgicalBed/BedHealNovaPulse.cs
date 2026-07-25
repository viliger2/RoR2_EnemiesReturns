using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.RechargableInteractable.SurgicalBed
{
    [RegisterEntityState]
    public class BedHealNovaPulse : BaseState
    {
        private class ClearHealPulse
        {
            private readonly List<HealthComponent> healedTargets = new List<HealthComponent>();

            private readonly SphereSearch sphereSearch;

            private float rate;

            private float t;

            private float finalRadius;

            private float healFractionValue;

            private TeamMask teamMask;

            private readonly List<HurtBox> hurtBoxesList = new List<HurtBox>();

            public bool isFinished => t >= 1f;

            public ClearHealPulse(Vector3 origin, float finalRadius, float healFractionValue, float duration, TeamIndex teamIndex)
            {
                sphereSearch = new SphereSearch
                {
                    mask = LayerIndex.entityPrecise.mask,
                    origin = origin,
                    queryTriggerInteraction = QueryTriggerInteraction.Collide,
                    radius = 0f
                };
                this.finalRadius = finalRadius;
                this.healFractionValue = healFractionValue;
                rate = 1f / duration;
                teamMask = default(TeamMask);
                teamMask.AddTeam(teamIndex);
            }

            public void Update(float deltaTime)
            {
                t += rate * deltaTime;
                t = ((t > 1f) ? 1f : t);
                sphereSearch.radius = finalRadius * novaRadiusCurve.Evaluate(t);
                sphereSearch.RefreshCandidates().FilterCandidatesByHurtBoxTeam(teamMask).FilterCandidatesByDistinctHurtBoxEntities()
                    .GetHurtBoxes(hurtBoxesList);
                int i = 0;
                for (int count = hurtBoxesList.Count; i < count; i++)
                {
                    HealthComponent healthComponent = hurtBoxesList[i].healthComponent;
                    if (!healedTargets.Contains(healthComponent))
                    {
                        healedTargets.Add(healthComponent);
                        HealTarget(healthComponent);
                    }
                }
                hurtBoxesList.Clear();
            }

            private void HealTarget(HealthComponent target)
            {
                target.HealFraction(healFractionValue, default);
                CleanseSystem.CleanseBodyServer(target.body, true, false, true, true, true, false);
                Util.PlaySound("Play_item_proc_TPhealingNova_hitPlayer", target.gameObject);
            }
        }

        public static AnimationCurve novaRadiusCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public static float duration = 2f;

        public static float healFractionValue = 1f;

        public static float radius = 100f;

        private Transform effectTransform;

        private ClearHealPulse healPulse;

        public override void OnEnter()
        {
            base.OnEnter();

            TeamFilter component = GetComponent<TeamFilter>();
            var teamIndex = (component ? component.teamIndex : TeamIndex.None);
            if (NetworkServer.active)
            {
                healPulse = new ClearHealPulse(transform.position, radius, healFractionValue, duration, teamIndex);
            }
            effectTransform = transform.Find("PulseEffect");
            if (effectTransform)
            {
                effectTransform.gameObject.SetActive(true);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (effectTransform)
            {
                effectTransform.gameObject.SetActive(false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                healPulse.Update(GetDeltaTime());
                if(fixedAge > duration)
                {
                    EntityState.Destroy(gameObject);
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (effectTransform)
            {
                float scale = radius * novaRadiusCurve.Evaluate(fixedAge / duration);
                effectTransform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
