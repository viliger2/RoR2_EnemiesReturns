﻿using RoR2;
using UnityEngine;

namespace EnemiesReturns.Enemies.Judgement
{
    public class HiddenSkinDef : SkinDef
    {
        public bool hideInLobby = true;

        public static HiddenSkinDef FromSkinDef(SkinDef skinDef)
        {
            var hiddenSkinDef = ScriptableObject.CreateInstance<HiddenSkinDef>();
            (hiddenSkinDef as ScriptableObject).name = skinDef.name + "Hidden";
            hiddenSkinDef.rootObject = skinDef.rootObject;
            hiddenSkinDef.gameObjectActivations = skinDef.gameObjectActivations;
            hiddenSkinDef.meshReplacements = skinDef.meshReplacements;
            hiddenSkinDef.projectileGhostReplacements = skinDef.projectileGhostReplacements;
            hiddenSkinDef.minionSkinReplacements = skinDef.minionSkinReplacements;
            hiddenSkinDef.rendererInfos = skinDef.rendererInfos;
            hiddenSkinDef.baseSkins = skinDef.baseSkins;
            hiddenSkinDef.icon = skinDef.icon;
            hiddenSkinDef.nameToken = skinDef.nameToken;
            hiddenSkinDef.skinDefParamsAddress = skinDef.skinDefParamsAddress;
            hiddenSkinDef.skinDefParams = skinDef.skinDefParams;

            return hiddenSkinDef;
        }
    }
}
