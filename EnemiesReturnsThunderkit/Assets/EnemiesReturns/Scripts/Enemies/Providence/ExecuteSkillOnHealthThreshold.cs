using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.ContactLight.Providence
{
    public class ExecuteSkillOnHealthThreshold : MonoBehaviour
    {
        public HealthComponent healthComponent;

        public GenericSkill skillToExecute;

        [Range(0f, 1f)]
        [SerializeField]
        public float[] healthThresholds;

        public EntityStateMachine[] esms;

        public float checkFrequency;

        private int timesExecuted;

        private float timer;

        private void OnEnable()
        {
            timesExecuted = 0;

            if (!healthComponent)
            {
                this.enabled = false;
                return;
            }

            if (!skillToExecute)
            {
                this.enabled = false;
                return;
            }
        }

        private void FixedUpdate()
        {
            if (timer > checkFrequency)
            {
                timer -= checkFrequency;

                if (!skillToExecute.IsReady())
                {
                    return;
                }

                if(timesExecuted >= healthThresholds.Length)
                {
                    return;
                }

                int reachedThresholdCount = 0;
                foreach(var healthTheshold in healthThresholds)
                {
                    if(healthComponent.healthFraction <= healthTheshold)
                    {
                        reachedThresholdCount++;
                    }
                }

                // if we executed skill as many times as there are reached thresholds then we don't need to do anything
                if(reachedThresholdCount <= timesExecuted)
                {
                    return;
                }

                var esmsAreFree = true;
                foreach(var esm in esms)
                {
                    esmsAreFree = esmsAreFree && esm.IsInMainState();
                }

                if (!esmsAreFree)
                {
                    return;
                }

                if (skillToExecute.ExecuteIfReady())
                {
                    timesExecuted++;
                }

            }
            timer += Time.fixedDeltaTime;
        }
    }
}
