﻿using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    public class DebugIntiialState : BaseState
    {
        public enum InitialState
        {
            Phase1,
            Phase2,
            Phase3,
            Ending
        }

        //public static InitialState initialState => Configuration.General.JudgementInitialState.Value;
        public static InitialState initialState;

        public override void OnEnter()
        {
            base.OnEnter();
            switch (initialState)
            {
                case InitialState.Phase1:
                default:
                    outer.SetNextState(new Phase1());
                    break;
                case InitialState.Phase2:
                    outer.SetNextState(new Phase2());
                    break;
                case InitialState.Phase3:
                    outer.SetNextState(new Phase3());
                    break;
                case InitialState.Ending:
                    outer.SetNextState(new Ending());
                    break;
            }
        }
    }
}
