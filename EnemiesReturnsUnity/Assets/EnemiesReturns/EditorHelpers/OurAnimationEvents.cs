﻿using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    // man I love writing this garbage
    public class OurAnimationEvents : MonoBehaviour
    {

        // public static Dictionary<int, GameObject> effectDictionary = new Dictionary<int, GameObject>(); // we will need to keep this stuff very scructured, yes, it is a shitshow

        // private GameObject bodyObject;

        // private CharacterModel characterModel;

        // private ChildLocator childLocator;

        // private EntityLocator entityLocator;

        // private Renderer meshRenderer;

        // private ModelLocator modelLocator;

        private void Start()
        {
            // childLocator = GetComponent<ChildLocator>();
            // entityLocator = GetComponent<EntityLocator>();
            // meshRenderer = GetComponentInChildren<Renderer>();
            // characterModel = GetComponent<CharacterModel>();
            // if ((bool)characterModel && (bool)characterModel.body)
            // {
            //     bodyObject = characterModel.body.gameObject;
            //     modelLocator = bodyObject.GetComponent<ModelLocator>();
            // }
        }

        // int is key of effect in dictionary
        // string is name of child transform to attach effect to
        public void OurCreateEffect(AnimationEvent animationEvent)
        {
            // if (string.IsNullOrEmpty(animationEvent.stringParameter))
            // {
            //     Log.Error("animationEvent belonging to " + bodyObject + " has been passed without childName in string paremeter.");
            //     return;
            // }

            // if(!effectDictionary.TryGetValue(animationEvent.intParameter, out var effect))
            // {
            //     Log.Error("animationEvent couldn't find effect with key" + animationEvent.intParameter + " in dictionary.");
            //     return;
            // }

            // EffectManager.SimpleMuzzleFlash(effect, modelLocator.modelTransform.gameObject, animationEvent.stringParameter, true);
        }
    }
}