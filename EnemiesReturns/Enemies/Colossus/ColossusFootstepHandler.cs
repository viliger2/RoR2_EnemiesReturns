using EnemiesReturns.Configuration;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Enemies.Colossus
{
    public class ColossusFootstepHandler : FootstepHandler
    {
        private static float maxDistance => EnemiesReturns.Configuration.Colossus.FootstepShockwaveDistance.Value;

        private static float force => EnemiesReturns.Configuration.Colossus.FootstepShockwaveForce.Value;

        public new void Footstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                Footstep(animationEvent.stringParameter, (GameObject)animationEvent.objectReferenceParameter);
            }
        }

        public new void Footstep(string childName, GameObject footstepEffect)
        {
            base.Footstep(childName, footstepEffect);
            Transform transform = childLocator.FindChild(childName);

            if (body && transform)
            {
                BullseyeSearch bullseyeSearch = new BullseyeSearch
                {
                    teamMaskFilter = TeamMask.GetEnemyTeams(body.teamComponent.teamIndex),
                    filterByLoS = false,
                    searchOrigin = transform.position,
                    searchDirection = UnityEngine.Random.onUnitSphere,
                    sortMode = BullseyeSearch.SortMode.Distance,
                    maxDistanceFilter = maxDistance,
                    maxAngleFilter = 360f
                };
                bullseyeSearch.RefreshCandidates();
                bullseyeSearch.FilterOutGameObject(base.gameObject);

                var result = bullseyeSearch.GetResults().ToArray<HurtBox>();
                List<HealthComponent> targets = new List<HealthComponent>();
                foreach (var hurtbox in result)
                {
                    if (targets.Contains(hurtbox.healthComponent))
                    {
                        continue;
                    }
                    targets.Add(hurtbox.healthComponent);

                    if (!Util.HasEffectiveAuthority(hurtbox.healthComponent.gameObject))
                    {
                        continue;
                    }

                    var forceScaled = Vector3.up * force * (1 - (Vector3.Distance(transform.position, hurtbox.transform.position) / maxDistance));
                    if (hurtbox.healthComponent.TryGetComponent<CharacterMotor>(out var motor))
                    {
                        motor.ApplyForce(forceScaled, true, false);
                    }
                    if (hurtbox.healthComponent.TryGetComponent<Rigidbody>(out var rigidBody))
                    {
                        rigidBody.AddForce(forceScaled, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
