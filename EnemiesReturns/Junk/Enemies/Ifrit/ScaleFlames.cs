using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Junk.Enemies.Ifrit
{
    public class ScaleFlames : NetworkBehaviour
    {
        public Vector3 scaleFactor;

        public CharacterBody characterBody;

        public Transform[] flames;

        private void OnEnable()
        {
            if (!characterBody)
            {
                characterBody = GetComponent<CharacterBody>();
                if (!characterBody)
                {
                    Destroy(this);
                }
            }

            characterBody.onSkillActivatedServer += CharacterBody_onSkillActivatedServer;
        }

        private void CharacterBody_onSkillActivatedServer(GenericSkill skill)
        {
            ScaleTransforms();
            RpcScaleTransforms();
        }

        private void ScaleTransforms()
        {
            foreach (Transform t in flames)
            {
                if (t)
                {
                    t.localScale += scaleFactor;
                }
            }
        }

        private void OnDisable()
        {
            characterBody.onSkillActivatedServer -= CharacterBody_onSkillActivatedServer;
        }

        [ClientRpc]
        private void RpcScaleTransforms()
        {
            ScaleTransforms();
        }
    }
}
