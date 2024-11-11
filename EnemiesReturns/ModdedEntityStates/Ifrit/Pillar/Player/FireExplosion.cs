using RoR2.CharacterAI;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Player
{
    public class FireExplosion : BaseFireExplosion
    {
        public override float damage => EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillDamage.Value;

        public override float radius => EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillRadius.Value;

        public override float force => EnemiesReturns.Configuration.Ifrit.PillarExplosionForce.Value;

        public override bool ignoresLoS => EnemiesReturns.Configuration.Ifrit.PillarExplosionIgnoesLoS.Value;

        public override float damagePerStack => EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillDamagePerStack.Value;

        public override GameObject GetAttacker()
        {
            if (characterBody.master)
            {
                var aiOwnership = characterBody.master.gameObject.GetComponent<AIOwnership>();
                if (aiOwnership && aiOwnership.ownerMaster)
                {
                    var body = aiOwnership.ownerMaster.GetBody();
                    if (body)
                    {
                        return body.gameObject;
                    }
                }
            }
            return base.GetAttacker();
        }
    }
}
