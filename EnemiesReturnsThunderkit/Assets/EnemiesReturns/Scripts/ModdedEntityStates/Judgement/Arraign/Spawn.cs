﻿using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign
{
    public class Spawn : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 5.5f;
            base.OnEnter();
        }
    }
}
