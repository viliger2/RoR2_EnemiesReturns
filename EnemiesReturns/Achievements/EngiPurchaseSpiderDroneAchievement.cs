using Assets.RoR2.Scripts.Platform;
using EnemiesReturns.Enemies.MechanicalSpider.Drone;
using RoR2;
using RoR2.Achievements;

namespace EnemiesReturns.Achievements
{
    [RegisterAchievement("EnemiesReturnsEngiPurchaseSpiderDroneAchievement", "Skills.Engi.EnemiesReturnsSpiderTurret", "Complete30StagesCareer", 3u, typeof(EngiPurchaseSpiderDroneServerAchievement))]
    public class EngiPurchaseSpiderDroneAchievement : BaseAchievement
    {
        private class EngiPurchaseSpiderDroneServerAchievement : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            }

            private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, UnityEngine.GameObject interactableObject)
            {
                var summonMasterBehaviour = interactableObject.GetComponent<SummonMasterBehavior>();
                if (IsCurrentBody(interactor.gameObject) && summonMasterBehaviour && summonMasterBehaviour.masterPrefab == MechanicalSpiderDroneMaster.MasterPrefab)
                {
                    Grant();
                }
            }

            public override void OnUninstall()
            {
                GlobalEventManager.OnInteractionsGlobal -= GlobalEventManager_OnInteractionsGlobal;
                base.OnUninstall();
            }
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("EngiBody");
        }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            SetServerTracked(shouldTrack: true);
        }

        public override void OnBodyRequirementBroken()
        {
            SetServerTracked(shouldTrack: false);
            base.OnBodyRequirementBroken();
        }

        public override void TryToCompleteActivity()
        {
            if (localUser.id == LocalUserManager.GetFirstLocalUser().id && shouldGrant)
            {
                BaseActivitySelector baseActivitySelector = new BaseActivitySelector();
                baseActivitySelector.activityAchievementID = achievementDef.identifier;
                PlatformSystems.activityManager.TryToCompleteActivity(baseActivitySelector);
            }
        }
    }
}
