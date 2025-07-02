using Assets.RoR2.Scripts.Platform;
using RoR2;
using RoR2.Achievements;

namespace EnemiesReturns.Achievements
{
    // TODO: redo cheevo so it triggers on ending instead of Arraign's death
    // although maybe some players prefer cheevo trigger when Arraign is killed to give sense of acomplishment
    public class JudgementClearedAchievement : BaseAchievement
    {
        public class JudgementClearedServerAchievement : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                ModdedEntityStates.Judgement.Mission.Ending.onArraignDefeated += Ending_onArraignDefeated;
            }

            public override void OnUninstall()
            {
                base.OnUninstall();
                ModdedEntityStates.Judgement.Mission.Ending.onArraignDefeated -= Ending_onArraignDefeated;
            }

            private void Ending_onArraignDefeated()
            {
                Grant();
            }
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            var unlockableDef = UnlockableCatalog.GetUnlockableDef(achievementDef.unlockableRewardIdentifier);
            if (!unlockableDef)
            {
                return BodyIndex.None;
            }

            if (Enemies.Judgement.AnointedSkins.AnointedSkinsUnlockablesAchivements.TryGetValue(unlockableDef, out string bodyName))
            {
                return BodyCatalog.FindBodyIndex(bodyName);
            }

            return BodyIndex.None;
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
            bool flag = base.localUser.id == LocalUserManager.GetFirstLocalUser().id;
            if (shouldGrant && flag)
            {
                BaseActivitySelector baseActivitySelector = new BaseActivitySelector();
                baseActivitySelector.activityAchievementID = achievementDef.identifier;
                PlatformSystems.activityManager.TryToCompleteActivity(baseActivitySelector);
            }
        }
    }
}
