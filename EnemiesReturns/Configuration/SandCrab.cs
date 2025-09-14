using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration
{
    internal class SandCrab : IConfiguration
    {
        public static ConfigEntry<float> BubbleSize;

        public static ConfigEntry<float> BubbleExplosionSize;

        public static ConfigEntry<float> BubbleSpeed;

        public static ConfigEntry<int> BubbleCountPerShot;

        public static ConfigEntry<int> BubbleShotCount;

        public void PopulateConfig(ConfigFile config)
        {
            BubbleSize = config.Bind<float>("Bubbles", "Bubble Size", 2.25f, "Bubble Size");
            BubbleCountPerShot = config.Bind("Bubbles", "Bubble Count Per Shot", 1, "Bubble Count Per Shot");
            BubbleShotCount = config.Bind("Bubbles", "Bubble Shots Count", 6, "Bubble Shots Count");
            BubbleExplosionSize = config.Bind("Bubbles", "Bubble Explosion Radius", 2.5f, "Bubble Explosion Radius");
            BubbleSpeed = config.Bind("Bubbles", "Bubble Speed", 9f, "Bubble Speed");
        }
    }
}
