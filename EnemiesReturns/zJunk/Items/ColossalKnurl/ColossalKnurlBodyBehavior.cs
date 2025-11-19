using RoR2;

namespace EnemiesReturns.Junk.Items.ColossalKnurl
{
    public class ColossalKnurlBodyBehavior : CharacterBody.ItemBehavior
    {
        public CharacterMaster master;

        private DeployableMinionSpawner golemAllySpawner;

        private void Awake()
        {
            enabled = false;
        }
        private void OnEnable()
        {
            master = body.master;
            golemAllySpawner = new DeployableMinionSpawner(master, ColossalKnurlFactoryJunk.deployableSlot, RoR2Application.rng)
            {
                respawnInterval = 10f,
                spawnCard = ColossalKnurlFactoryJunk.cscGolemAlly
            };
            golemAllySpawner.onMinionSpawnedServer += OnGolemAllySpawned;
        }

        private void OnDisable()
        {
            golemAllySpawner?.Dispose();
            golemAllySpawner = null;
        }

        private void OnGolemAllySpawned(SpawnCard.SpawnResult result)
        {
            var spawnedObject = result.spawnedInstance;
            if (spawnedObject)
            {
                var master = spawnedObject.GetComponent<CharacterMaster>();
                if (master)
                {
                    master.inventory.GiveItemPermanent(RoR2Content.Items.BoostDamage,
                        30 + 30 * (stack - 1));
                    master.inventory.GiveItemPermanent(RoR2Content.Items.BoostHp,
                        10 + 10 * (stack - 1));
                    master.inventory.GiveItemPermanent(RoR2Content.Items.Hoof,
                        5 + 5 * (stack - 1)); // maybe too much
                    var deployable = master.GetComponent<Deployable>();
                    if (deployable)
                    {
                        body.master.AddDeployable(deployable, ColossalKnurlFactoryJunk.deployableSlot);
                    }
                }
            }
        }

        public static int GetModdedCount(CharacterMaster self, int count)
        {
            return count;
        }
    }
}
