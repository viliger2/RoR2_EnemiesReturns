using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class DeployableLineRendererToOwner : NetworkBehaviour
    {
        public string childOriginName;

        public string ownerTargetName;

        public LineRenderer lineRenderer;

        private GameObject ownerBodyObject;

        private Transform originPoint;

        private Transform targetPoint;

        private const uint OWNER_BODY_OBJECT_DIRTY_BIT = 1u;

        private bool foundOwnerChild;

        private void Start()
        {
            if (!lineRenderer)
            {
                this.enabled = false;
                return;
            }

            originPoint = gameObject.transform;
            var modelLocator = GetComponent<ModelLocator>();
            if (modelLocator)
            {
                var modelTransform = modelLocator.modelTransform;
                if (modelTransform)
                {
                    var childLocator = modelTransform.GetComponent<ChildLocator>();
                    if (childLocator)
                    {
                        var child = childLocator.FindChild(childOriginName);
                        if (child)
                        {
                            originPoint = child;
                        }
                    }
                }
            }

            FindOwnerBodyGameObject();
        }

        private void FixedUpdate()
        {
            FindOwnerBodyGameObject();
            if(!foundOwnerChild && ownerBodyObject)
            {
                var modelLocator = ownerBodyObject.GetComponent<ModelLocator>();
                if(!modelLocator || !modelLocator.modelTransform)
                {
                    return;
                }

                var childLocator = modelLocator.modelTransform.GetComponent<ChildLocator>();
                if (!childLocator)
                {
                    return;
                }

                var child = childLocator.FindChild(ownerTargetName);
                if (!child)
                {
                    return;
                }

                targetPoint = child;
                foundOwnerChild = true;
            }
        }

        private void LateUpdate()
        {
            if (lineRenderer && originPoint && targetPoint)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, originPoint.position);
                lineRenderer.SetPosition(1, targetPoint.position);
            }
        }

        private void FindOwnerBodyGameObject()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (this.ownerBodyObject)
            {
                return;
            }

            var body = GetComponent<CharacterBody>();
            if (!body || !body.master)
            {
                return;
            }

            var deployable = body.master.GetComponent<Deployable>();
            if (!deployable || !deployable.ownerMaster)
            {
                return;
            }

            var ownerBodyObject = deployable.ownerMaster.GetBodyObject();
            if (!ownerBodyObject)
            {
                return;
            }

            this.ownerBodyObject = ownerBodyObject;
            SetDirtyBit(OWNER_BODY_OBJECT_DIRTY_BIT);
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            uint num = syncVarDirtyBits;
            if (initialState && ownerBodyObject)
            {
                num = OWNER_BODY_OBJECT_DIRTY_BIT;
            }

            writer.WritePackedUInt32(num);
            if ((num & OWNER_BODY_OBJECT_DIRTY_BIT) != 0)
            {
                var networkIdentity = ownerBodyObject.GetComponent<NetworkIdentity>();
                writer.Write(networkIdentity.netId);
            }

            return num != 0;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            uint num = reader.ReadPackedUInt32();
            if ((num & OWNER_BODY_OBJECT_DIRTY_BIT) != 0)
            {
                var netId = reader.ReadNetworkId();
                ownerBodyObject = Util.FindNetworkObject(netId);
            }
        }

    }
}
