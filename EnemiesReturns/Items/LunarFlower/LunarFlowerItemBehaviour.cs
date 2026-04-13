using RoR2;
using UnityEngine.Networking;
using static RoR2.CharacterBody;
using static RoR2.CharacterMasterNotificationQueue;

namespace EnemiesReturns.Items.LunarFlower
{
    public class LunarFlowerItemBehaviour : ItemBehavior, IOnKilledServerReceiver
    {
        public bool hasVoidDied;

        public bool trueKill = false;

        private void Start()
        {
            hasVoidDied = false;
        }

        public void OnKilledServer(DamageReport damageReport)
        {
            if (damageReport != null && damageReport.damageInfo != null && IsDamageVoidDeath(damageReport.damageInfo))
            {
                hasVoidDied = true;
            }
        }

        private bool IsDamageVoidDeath(DamageInfo damageInfo)
        {
            return (damageInfo.damageType.damageType & DamageType.VoidDeath) == DamageType.VoidDeath;
        }

        public static void SetTrueKill(CharacterMaster master)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!master)
            {
                return;
            }

            var body = master.GetBody();
            if (!body)
            {
                return;
            }


            var lunarFlowerComponent = body.GetComponent<LunarFlowerItemBehaviour>();
            if (!lunarFlowerComponent)
            {
                return;
            }

            lunarFlowerComponent.trueKill = true;
        }

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.General.EnableJudgement.Value)
            {
                if (NetworkServer.active)
                {
                    if (body && body.inventory)
                    {
                        body.AddItemBehavior<LunarFlowerItemBehaviour>(body.inventory.GetItemCountPermanent(Content.Items.LunarFlower));
                    }
                }
            }
        }

        public static void Hooks()
        {
            if (Configuration.General.EnableJudgement.Value)
            {
                On.RoR2.CharacterMaster.TryReviveOnBodyDeath += CharacterMaster_TryReviveOnBodyDeath;
                On.RoR2.CharacterMaster.TrueKill_GameObject_GameObject_DamageTypeCombo += CharacterMaster_TrueKill_GameObject_GameObject_DamageTypeCombo;
            }
        }

        private static void CharacterMaster_TrueKill_GameObject_GameObject_DamageTypeCombo(On.RoR2.CharacterMaster.orig_TrueKill_GameObject_GameObject_DamageTypeCombo orig, CharacterMaster self, UnityEngine.GameObject killerOverride, UnityEngine.GameObject inflictorOverride, DamageTypeCombo damageTypeOverride)
        {
            LunarFlowerItemBehaviour.SetTrueKill(self);
            orig(self, killerOverride, inflictorOverride, damageTypeOverride);
        }

        private static bool CharacterMaster_TryReviveOnBodyDeath(On.RoR2.CharacterMaster.orig_TryReviveOnBodyDeath orig, CharacterMaster self, CharacterBody body)
        {
            if (LunarFowerExtraLifeBehaviour.CanRevive(self))
            {
                Inventory.ItemTransformation itemTransformation = default(Inventory.ItemTransformation);
                itemTransformation.originalItemIndex = Content.Items.LunarFlower.itemIndex;
                itemTransformation.newItemIndex = Content.Items.VoidFlower.itemIndex;
                itemTransformation.transformationType = (ItemTransformationTypeIndex)TransformationType.ContagiousVoid;
                if (itemTransformation.TryTake(self.inventory, out var result))
                {
                    LunarFowerExtraLifeBehaviour extraLifeServerBehavior = self.gameObject.AddComponent<LunarFowerExtraLifeBehaviour>();
                    extraLifeServerBehavior.pendingTransformation = result;
                    extraLifeServerBehavior.consumedItemIndex = Content.Items.VoidFlower.itemIndex;
                    extraLifeServerBehavior.completionTime = Run.FixedTimeStamp.now + 2f;
                    extraLifeServerBehavior.completionCallback = extraLifeServerBehavior.LunarFlowerRevive;
                    extraLifeServerBehavior.soundTime = Run.FixedTimeStamp.positiveInfinity;
                    return true;
                }
            }
            return orig(self, body);
        }
    }

}
