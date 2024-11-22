using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.MasterComponents;
using UnityEngine;

namespace EnemiesReturns.Components
{
    public abstract class MasterBase : IMaster
    {
        protected abstract IAISkillDriver.AISkillDriverParams[] AISkillDriverParams();

        protected virtual IBaseAI.BaseAIParams BaseAIParams()
        {
            var wander = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            return new IBaseAI.BaseAIParams(RoR2.Navigation.MapNodeGroup.GraphType.Ground, wander);
        }

        protected virtual ICharacterMaster.CharacterMasterParams CharacterMasterParams()
        {
            return new ICharacterMaster.CharacterMasterParams();
        }

        protected virtual IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams()
                {
                    name = "AI",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander))
                }
            };
        }

        protected virtual INetworkIdentity.NetworkIdentityParams NetworkIdentityParams()
        {
            return new INetworkIdentity.NetworkIdentityParams();
        }

        public abstract GameObject AddMasterComponents(GameObject masterPrefab, GameObject bodyPrefab);

        protected virtual bool AddNetworkIdentity => true;
        protected virtual bool AddAISkillDriver => true;
        protected virtual bool AddBaseAI => true;
        protected virtual bool AddCharacterMaster => true;
        protected virtual bool AddInventory => true;
        protected virtual bool AddEntityStateMachine => true;
        protected virtual bool AddMinionOwnership => true;


        IAISkillDriver.AISkillDriverParams[] IAISkillDriver.GetAISkillDriverParams() => AISkillDriverParams();
        IBaseAI.BaseAIParams IBaseAI.GetBaseAIParams() => BaseAIParams();
        ICharacterMaster.CharacterMasterParams ICharacterMaster.GetCharacterMasterParams() => CharacterMasterParams();
        IEntityStateMachine.EntityStateMachineParams[] IEntityStateMachine.GetEntityStateMachineParams() => EntityStateMachineParams();
        INetworkIdentity.NetworkIdentityParams INetworkIdentity.GetNetworkIdentityParams() => NetworkIdentityParams();

        bool IAISkillDriver.NeedToAddAISkillDriver() => AddAISkillDriver;
        bool IBaseAI.NeedToAddBaseAI() => AddBaseAI;
        bool ICharacterMaster.NeedToAddCharacterMaster() => AddCharacterMaster;
        bool IEntityStateMachine.NeedToAddEntityStateMachines() => AddEntityStateMachine;
        bool IInventory.NeedToAddInventory() => AddInventory;
        bool IMinionOwnership.NeedToAddMinionOwnership() => AddMinionOwnership;
        bool INetworkIdentity.NeedToAddNetworkIdentity() => AddNetworkIdentity;
    }
}
