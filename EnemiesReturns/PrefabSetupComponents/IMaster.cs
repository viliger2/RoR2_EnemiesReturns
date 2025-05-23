﻿using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.MasterComponents;
using EnemiesReturns.PrefabSetupComponents.MasterComponents;
using R2API;
using Rewired.Utils;
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

            return masterPrefab;
        }
    }
}
