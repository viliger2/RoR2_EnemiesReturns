using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class IdleSoundPlayback : MonoBehaviour
    {
        public string soundEventPlay = "";
        public string soundEventStop = "";

        public CharacterModel characterModel;

        private CharacterMaster master;

        private void OnEnable()
        {
            if(!characterModel)
            {
                characterModel = GetComponent<CharacterModel>();
            }

            if(characterModel.body)
            {
                master = characterModel.body.master;
            }

            if(master)
            {
                master.onBodyStart += Master_onBodyStart;
                master.onBodyDeath.AddListener(OnBodyDeath);
            }
            Util.PlaySound(soundEventPlay, this.gameObject);
        }

        private void OnDisable()
        {
            if (master)
            {
                master.onBodyStart -= Master_onBodyStart;
                master.onBodyDeath.RemoveListener(OnBodyDeath);
            }
            Util.PlaySound(soundEventStop, this.gameObject);
        }

        public void OnBodyDeath()
        {
            Util.PlaySound(soundEventStop, this.gameObject);
        }

        private void Master_onBodyStart(CharacterBody obj)
        {
            Util.PlaySound(soundEventPlay, this.gameObject);
        }
    }
}
