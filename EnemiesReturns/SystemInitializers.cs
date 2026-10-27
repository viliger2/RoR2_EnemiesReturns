using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns
{
    public static class SystemInitializers
    {
        [SystemInitializer(new Type[] { typeof(BodyCatalog) })]
        public static void InitBodyCatalog()
        {
            if (!EnemiesReturnsPlugin.ModIsLoaded)
            {
                return;
            }

            Enemies.Judgement.AnointedSkins.CreateAnointedSkinsSecondIteration();
            Items.LynxFetish.LynxFetishFactory.InitBodyCatalog();
            EnemiesReturns.Enemies.Judgement.SetupJudgementPath.InitBodyCatalog();
        }

        [SystemInitializer(new Type[] { typeof(MasterCatalog) })]
        public static void InitMasterCatalog()
        {
            if (!EnemiesReturnsPlugin.ModIsLoaded)
            {
                return;
            }

            Items.AdrenalineCore.AdrenalineCoreMasterComponent.InitMasterCatalog();
            EnemiesReturns.Skills.Engi.MechanicalSpiderTurret.SetupSkill.InitMasterCatalog();
            EnemiesReturns.Enemies.Judgement.SetupJudgementPath.InitMasterCatalog();
        }

        [SystemInitializer(new Type[] { typeof(ItemCatalog) })]
        public static void InitItemCatalog()
        {
            if (!EnemiesReturnsPlugin.ModIsLoaded)
            {
                return;
            }

            EnemiesReturns.ModCompats.AncientScepterCompat.InitItemCatalog();
            EnemiesReturns.ModCompats.RiskyModCompat.InitItemCatalog();
        }
    }
}
