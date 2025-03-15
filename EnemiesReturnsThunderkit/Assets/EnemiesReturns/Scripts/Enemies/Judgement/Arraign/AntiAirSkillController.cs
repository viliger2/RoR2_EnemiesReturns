using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class AntiAirSkillController : MonoBehaviour
    {
        public float checkTimer = 0.25f;

        public GenericSkill antiAirSkill;

        public CharacterBody body;

        private float timer;

        private void FixedUpdate()
        {
            if (!body.hasEffectiveAuthority)
            {
                return;
            }

            timer += Time.fixedDeltaTime;
            if(timer > checkTimer)
            {
                if (antiAirSkill.IsReady())
                {
                    // var bodies = Utils.GetActiveAndAlivePlayerBodies();
                    // foreach (var body in bodies)
                    // {
                    //     if (body && body.characterMotor && !body.characterMotor.isGrounded)
                    //     {
                    //         antiAirSkill.ExecuteIfReady();
                    //     }
                    // }
                }
                timer -= checkTimer;
            }
        }
    }
}
