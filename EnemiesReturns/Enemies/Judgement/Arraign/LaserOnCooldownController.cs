using RoR2;
using UnityEngine;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class LaserOnCooldownController : MonoBehaviour
    {
        public float checkTimer = 0.25f;

        public GenericSkill laserSkill;

        public CharacterBody body;

        public EntityStateMachine[] esms;

        private float timer;

        private void FixedUpdate()
        {
            if (!body.hasEffectiveAuthority)
            {
                return;
            }

            timer += Time.fixedDeltaTime;
            if (timer > checkTimer)
            {
                timer -= checkTimer;
                if (!laserSkill.IsReady())
                {
                    return;
                }

                bool emssFree = true;
                foreach (var esm in esms)
                {
                    emssFree = emssFree && esm.IsInMainState();
                }

                if (emssFree)
                {
                    laserSkill.ExecuteIfReady();
                }
            }
        }
    }
}
