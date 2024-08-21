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
    public class DeathBoner : DeathFallBase
    {
        public override string fallAnimation => "DeathBoner";
    }
}
