using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ITeamComponent
    {
        protected class TeamComponentParams
        {
            public bool hideAllyCardDisplay = false;
            public TeamIndex teamIndex = TeamIndex.None;
        }

        protected TeamComponentParams GetTeamComponentParams();

        protected bool NeedToAddTeamComponent();

        protected TeamComponent AddTeamComponent(GameObject bodyPrefab, TeamComponentParams teamComponentParams)
        {
            TeamComponent teamComponent = null;
            if (NeedToAddTeamComponent())
            {
                teamComponent = bodyPrefab.GetOrAddComponent<TeamComponent>();
                teamComponent.teamIndex = teamComponentParams.teamIndex;
                teamComponent.hideAllyCardDisplay = teamComponentParams.hideAllyCardDisplay;
            }

            return teamComponent;
        }
    }
}
