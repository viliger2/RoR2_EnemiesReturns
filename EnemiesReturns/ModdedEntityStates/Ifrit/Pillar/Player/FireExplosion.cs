using RoR2.CharacterAI;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Player
{
    public class FireExplosion : BaseFireExplosion
    {
        public override float damage => EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillDamage.Value;

        public override float radius => EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillRadius.Value;

        public override float force => EnemiesReturnsConfiguration.Ifrit.PillarExplosionForce.Value;

        public override bool ignoresLoS => EnemiesReturnsConfiguration.Ifrit.PillarExplosionIgnoesLoS.Value;

        public override float damagePerStack => EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillDamagePerStack.Value;

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
