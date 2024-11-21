using RoR2.Networking;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ICharacterNetworkTransform
    {
        protected class CharacterNetworkTransformParams
        {
            public float positionTransmitInterval = 0.1f;
            public float interpolationFactor = 2f;
        }

        protected bool NeedToAddCharacterNetworkTransform();

        protected CharacterNetworkTransformParams GetCharacterNetworkTransformParams();

        protected CharacterNetworkTransform AddCharacterNetworkTransform(GameObject bodyPrefab, CharacterNetworkTransformParams characterNetworkTransformParams)
        {
            CharacterNetworkTransform characterNetworkTransform = null;
            if (NeedToAddCharacterNetworkTransform())
            {
                characterNetworkTransform = bodyPrefab.GetOrAddComponent<CharacterNetworkTransform>();
                characterNetworkTransform.positionTransmitInterval = characterNetworkTransformParams.positionTransmitInterval;
                characterNetworkTransform.interpolationFactor = characterNetworkTransformParams.interpolationFactor;
            }

            return characterNetworkTransform;
        }
    }
}
