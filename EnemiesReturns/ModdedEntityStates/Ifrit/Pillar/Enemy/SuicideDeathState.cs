using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Enemy
{
    [RegisterEntityState]
    public class SuicideDeathState : BaseDeathState
    {
        public override void OnEnter()
        {
            var childLocator = GetModelChildLocator();
            if (childLocator)
            {
                var fireball = childLocator.FindChild("Fireball");
                if (fireball)
                {
                    fireball.gameObject.SetActive(false);
                }
            }
            base.OnEnter();
        }
    }
}
