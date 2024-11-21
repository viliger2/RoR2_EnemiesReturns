using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ITeamComponent
    {
        protected TeamIndex GetTeamIndex();

        protected bool NeedToAddTeamComponent();

        internal TeamComponent AddTeamComponent(GameObject bodyPrefab, TeamIndex teamIndex = TeamIndex.None)
        {
            TeamComponent teamComponent = null;
            if (NeedToAddTeamComponent())
            {
                teamComponent = bodyPrefab.GetOrAddComponent<TeamComponent>();
                teamComponent.teamIndex = teamIndex;
            }

            return teamComponent;
        }
    }
}
