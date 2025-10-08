using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.VoidlingWeapon
{
    public class Spawn : BaseNetworkedBodyAttachmentState
    {
        public static GameObject voidlingWeaponVisualsPrefab;

        public override void OnEnter()
        {
            base.OnEnter();

            var body = bodyAttachment.attachedBody;
            var weaponInstance = UnityEngine.Object.Instantiate(voidlingWeaponVisualsPrefab, bodyGameObject.transform);
            weaponInstance.transform.position = body.corePosition + body.transform.right * body.bestFitActualRadius;
            weaponInstance.transform.rotation = Quaternion.Euler(body.inputBank.aimDirection);
        }



    }
}
