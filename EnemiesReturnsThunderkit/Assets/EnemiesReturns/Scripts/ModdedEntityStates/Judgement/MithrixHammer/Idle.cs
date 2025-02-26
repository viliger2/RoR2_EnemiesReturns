using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.MithrixHammer
{
    public class Idle : BaseMithrixHammerState
    {
        public static EquipmentDef equipmentToCheck;

        private InputBankTest bodyInputBank;

        public override void OnEnter()
        {
            base.OnEnter();
            if (bodyGameObject)
            {
                bodyInputBank = bodyGameObject.GetComponent<InputBankTest>();
            }
        }

        public override void Update()
        {
            base.Update();
            if(isAuthority && equipmentSlot && bodyInputBank)
            {
                if(equipmentSlot.equipmentIndex != equipmentToCheck.equipmentIndex)
                {
                    return;
                }

                if(equipmentSlot.stock == 0)
                {
                    return;
                }

                if (bodyInputBank.activateEquipment.justReleased)
                {
                    outer.SetNextState(new Fire());
                }
            }
        }
    }
}
