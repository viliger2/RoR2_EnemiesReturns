using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    public class PrePhase2 : BaseState
    {
        public static float phaseDuration = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
            GetComponent<ChildLocator>().FindChild("Phase1").gameObject.SetActive(false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > phaseDuration)
            {
                outer.SetNextState(new Phase2());
            }
        }
    }
}
