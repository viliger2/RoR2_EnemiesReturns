using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class PreEnding : EntityState
    {
        public static float duration = 10f;

        public static string phaseControllerChildString = "PreEnding";

        public override void OnEnter()
        {
            base.OnEnter();
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                var phaseControllerObject = childLocator.FindChild(phaseControllerChildString);
                if (phaseControllerObject)
                {
                    phaseControllerObject.gameObject.SetActive(true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > duration)
            {
                outer.SetNextState(new Ending());
            }
        }
    }
}
