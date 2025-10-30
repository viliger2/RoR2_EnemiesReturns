using RoR2;
using RoR2.UI;
using UnityEngine;
using static RoR2.BossGroup;

namespace EnemiesReturns.Behaviors.Judgement
{
    public class SceneSpecificHooks : MonoBehaviour
    {
        private void Start()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate += HUDBossHealthBarController_LateUpdate;
            On.RoR2.PickupDropletController.CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 += PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3;
            On.RoR2.BossGroup.ReportObjective += BossGroup_ReportObjective;
            if (MusicController.Instance)
            {
                AkSoundEngine.PostEvent("ER_Play_Music_System", MusicController.Instance.gameObject);
            }
        }

        private void BossGroup_ReportObjective(On.RoR2.BossGroup.orig_ReportObjective orig, BossGroup self, CharacterMaster master, System.Collections.Generic.List<RoR2.UI.ObjectivePanelController.ObjectiveSourceDescriptor> output)
        {
            if (self.combatSquad.readOnlyMembersList.Count != 0 || self.combatSquad.membersList.Count != 0)
            {
                output.Add(new ObjectivePanelController.ObjectiveSourceDescriptor
                {
                    source = self,
                    master = master,
                    objectiveType = typeof(DefeatBossObjectiveTracker)
                });
            }
        }

        private void PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3(On.RoR2.PickupDropletController.orig_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 orig, GenericPickupController.CreatePickupInfo pickupInfo, Vector3 position, Vector3 velocity)
        {
            if (RoR2.Stage.instance.sceneDef.cachedName == "enemiesreturns_outoftime")
            {
                var pickupDef = PickupCatalog.GetPickupDef(pickupInfo.pickupIndex);
                if (pickupDef != null)
                {
                    if (pickupDef.equipmentIndex != Content.Equipment.EliteAeonian.equipmentIndex
                        && pickupDef.equipmentIndex != Content.Equipment.MithrixHammer.equipmentIndex
                        && pickupDef.equipmentIndex != Content.Equipment.VoidlingWeapon.equipmentIndex)
                    {
                        return;
                    }
                }
            }
            orig(pickupInfo, position, velocity);
        }

        private void HUDBossHealthBarController_LateUpdate(On.RoR2.UI.HUDBossHealthBarController.orig_LateUpdate orig, RoR2.UI.HUDBossHealthBarController self)
        {
            orig(self);
            BossGroupHealthColorOverride.ReplaceColor(self);
            BossGroupTextOverride.ReplaceNames(self);
        }

        private void OnDestroy()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate -= HUDBossHealthBarController_LateUpdate;
            On.RoR2.PickupDropletController.CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 -= PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3;
            On.RoR2.BossGroup.ReportObjective -= BossGroup_ReportObjective;
            if (MusicController.Instance)
            {
                AkSoundEngine.PostEvent("ER_Stop_Music", MusicController.Instance.gameObject);
            }
        }
    }
}
