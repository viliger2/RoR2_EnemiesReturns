using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.MithrixHammer
{
    public class BaseMithrixHammerState : EntityState
    {
        protected NetworkedBodyAttachment bodyAttachment;

        protected EquipmentSlot equipmentSlot;

        protected GameObject bodyGameObject;

        protected CharacterMaster master;

        public override void OnEnter()
        {
            base.OnEnter();
            bodyAttachment = GetComponent<NetworkedBodyAttachment>();
            if (!bodyAttachment)
            {
                return;
            }

            var body = bodyAttachment.attachedBody;
            master = body.master;
            bodyGameObject = bodyAttachment.attachedBodyObject;
            if (bodyGameObject)
            {
                equipmentSlot = bodyGameObject.GetComponent<EquipmentSlot>();
            }
        }
    }
}
