using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.SkyLaser
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect;

        public override void OnEnter()
        {
            spawnSoundString = "ER_Colossus_Barrage_Charge_Play";
            duration = 4f;
            base.OnEnter();
            EffectManager.SimpleEffect(spawnEffect, base.transform.position, Quaternion.identity, false);
        }

        public override void OnExit()
        {
            base.OnExit();
            Util.PlaySound("Play_voidRaid_superLaser_start", base.gameObject);
        }
    }
}
