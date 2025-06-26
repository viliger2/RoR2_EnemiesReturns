using EntityStates;
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
#if DEBUG == true || NOWEAVER == true
        public static InitialState initialState => Configuration.General.JudgementInitialState.Value;

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
                    outer.SetNextState(new PrePhase2());
                    break;
                case InitialState.Phase3:
                    outer.SetNextState(new Phase3());
                    break;
                case InitialState.Ending:
                    outer.SetNextState(new Ending());
                    break;
            }
        }
#endif
    }
}
