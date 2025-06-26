using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    [RegisterEntityState]
    public class BaseJudgementIntaractable : BaseState
    {
        internal PickupPickerController pickupPickerController;

        internal ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            pickupPickerController = GetComponent<PickupPickerController>();
            childLocator = gameObject.GetComponent<ChildLocator>();
        }
    }
}
