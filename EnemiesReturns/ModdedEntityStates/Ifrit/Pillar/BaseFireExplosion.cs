using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar
{
    public abstract class BaseFireExplosion : BaseState
    {
        public static float duration = 0.1f; // just to be safe

        public abstract float damage { get; }

        public abstract float radius { get; }

        public abstract float force { get; }

        public abstract bool ignoresLoS { get; }

        public abstract float damagePerStack { get; }

        public static GameObject explosionPrefab;

        private int stackCount = 1;

        private BlastAttack blastAttack;

        public override void OnEnter()
        {
            base.OnEnter();

            var childLocator = GetModelChildLocator();
            var fireball = childLocator.FindChild("Fireball");
            if (NetworkServer.active)
            {
                if (explosionPrefab)
                {
                    EffectManager.SpawnEffect(explosionPrefab, new EffectData { origin = fireball ? fireball.position : gameObject.transform.position, scale = 5f * (radius / 30f) }, true);
                }

                if (characterBody.master)
                {
                    var aiOwnership = characterBody.master.gameObject.GetComponent<AIOwnership>();
                    if (aiOwnership && aiOwnership.ownerMaster)
                    {
                        stackCount = aiOwnership.ownerMaster.inventory.GetItemCount(Items.SpawnPillarOnChampionKill.SpawnPillarOnChampionKillFactory.itemDef);
                    }
                }

                blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.radius = radius;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = transform.position;
                blastAttack.crit = false;
                blastAttack.baseDamage = damageStat * (damage + damagePerStack * (stackCount - 1));
                blastAttack.canRejectForce = false;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = force;
                blastAttack.losType = ignoresLoS ? BlastAttack.LoSType.None : BlastAttack.LoSType.NearestHit;
                blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                blastAttack.damageType.damageType = DamageType.IgniteOnHit;
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.Fire();
            }
            if (fireball)
            {
                fireball.gameObject.SetActive(false);
            }
            var areaIndicator = childLocator.FindChild("TeamAreaIndicator");
            if (areaIndicator)
            {
                areaIndicator.gameObject.SetActive(false);
            }
            var lineRenderer = childLocator.FindChild("LineOriginPoint");
            if (lineRenderer)
            {
                lineRenderer.gameObject.SetActive(false);
            }
            Util.PlaySound("ER_Ifrit_Pillar_Explosion_Play", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && fixedAge >= duration)
            {
                base.healthComponent.Suicide();
            }
        }
    }
}
