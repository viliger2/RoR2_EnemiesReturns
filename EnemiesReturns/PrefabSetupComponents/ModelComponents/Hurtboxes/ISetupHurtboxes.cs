using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents.Hurtboxes
{
    public interface ISetupHurtboxes
    {
        public bool NeedToSetupHurtboxes();

        public SurfaceDef GetSurfaceDef();

        internal HurtBox[] SetupHurtboxes(GameObject bodyPrefab, SurfaceDef surfaceDef, HealthComponent healthComponent)
        {
            List<HurtBox> hurtBoxes = new List<HurtBox>();

            if (NeedToSetupHurtboxes())
            {
                var hurtBoxesTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "HurtBox").ToArray();
                foreach (Transform t in hurtBoxesTransform)
                {
                    var hurtBox = t.gameObject.AddComponent<HurtBox>();
                    hurtBox.healthComponent = healthComponent;
                    hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                    hurtBoxes.Add(hurtBox);

                    t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
                }

                var sniperHurtBoxes = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "SniperHurtBox").ToArray();
                foreach (Transform t in sniperHurtBoxes)
                {
                    var hurtBox = t.gameObject.AddComponent<HurtBox>();
                    hurtBox.healthComponent = healthComponent;
                    hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                    hurtBox.isSniperTarget = true;
                    hurtBoxes.Add(hurtBox);

                    t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
                }

                var mainHurtboxTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "MainHurtBox").First();
                var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
                mainHurtBox.healthComponent = healthComponent;
                mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                mainHurtBox.isBullseye = true;
                hurtBoxes.Add(mainHurtBox);

                mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            }

            return hurtBoxes.ToArray();
        }
    }
}
