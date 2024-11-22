using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.MasterComponents
{
    public interface ICharacterMaster
    {
        protected class CharacterMasterParams
        {
            public bool spawnOnStart = false; // basically never
            public TeamIndex teamIndex = TeamIndex.Monster;
            public bool destroyBodyOnDeath = true;
            public bool isBoss = false;
            public bool preventGameOver = true;
        }

        protected bool NeedToAddCharacterMaster();

        protected CharacterMasterParams GetCharacterMasterParams();

        protected CharacterMaster AddCharacterMaster(GameObject masterPrefab, GameObject bodyPrefab, CharacterMasterParams masterParams)
        {
            CharacterMaster characterMaster = null;
            if (NeedToAddCharacterMaster())
            {
                characterMaster = masterPrefab.GetOrAddComponent<CharacterMaster>();
                characterMaster.bodyPrefab = bodyPrefab;
                characterMaster.spawnOnStart = masterParams.spawnOnStart;
                characterMaster.teamIndex = masterParams.teamIndex;
                characterMaster.destroyOnBodyDeath = masterParams.destroyBodyOnDeath;
                characterMaster.isBoss = masterParams.isBoss;
                characterMaster.preventGameOver = masterParams.preventGameOver;
            }
            return characterMaster;
        }
    }
}
