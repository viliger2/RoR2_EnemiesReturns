using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using Rewired.HID;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Death
{
    public class InitialDeathState : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var awooga = characterBody.gameObject.GetComponent<ColossusAwooga>();
            if (awooga && awooga.boing)
            {
                Debug.Log("hehehe boner");
            }

            var rockController = GetModelTransform().gameObject.GetComponent<FloatingRocksController>();

            if (rockController)
            {
                rockController.enabled = false;
            }

            if (NetworkServer.active)
            {
                int value = UnityEngine.Random.Range(0, 100);
                if (value > 50)
                {
                    outer.SetNextState(new Death1());
                }
                else
                {
                    outer.SetNextState(new Death2());
                }
            }
        }
    }
}
