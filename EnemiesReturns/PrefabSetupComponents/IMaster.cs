using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.MasterComponents;
using EnemiesReturns.PrefabSetupComponents.MasterComponents;
using R2API;
using Rewired.Utils;
using RoR2;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Components
{
    public interface IMaster : INetworkIdentity, ICharacterMaster, IInventory, IEntityStateMachine, IBaseAI, IMinionOwnership, IAISkillDriver, IAIOwnership, ISetDontDestroyOnLoad, IDeployable
    {
        public GameObject CreateMaster(GameObject masterPrefab, GameObject bodyPrefab)
        {
            AddNetworkIdentity(masterPrefab, GetNetworkIdentityParams());
            AddCharacterMaster(masterPrefab, bodyPrefab, GetCharacterMasterParams());
            AddInventory(masterPrefab);
            var esms = AddEntityStateMachines(masterPrefab, GetEntityStateMachineParams());
            AddBaseAI(masterPrefab, !esms.IsNullOrDestroyed() && esms.Count() > 0 ? esms[0] : null, GetBaseAIParams());
            AddMinionOwnership(masterPrefab);
            AddAISkillDrivers(masterPrefab, GetAISkillDriverParams());
            AddAIOwnership(masterPrefab);
            AddSetDontDestroyOnLoad(masterPrefab);
            AddDeployable(masterPrefab);

            if ((this as INetworkIdentity).NeedToAddNetworkIdentity())
            {
                masterPrefab.RegisterNetworkPrefab();
            }

            if (Items.PartyHat.PartyHatFactory.ShouldThrowParty())
            {
                var pickups = masterPrefab.AddComponent<GivePickupsOnStart>();
                pickups.itemDefInfos = new GivePickupsOnStart.ItemDefInfo[]
                {
                    new GivePickupsOnStart.ItemDefInfo()
                    {
                        count = 1,
                        dontExceedCount = true,
                        itemDef = Content.Items.PartyHat
                    }
                };
            }

            return masterPrefab;
        }
    }
}
