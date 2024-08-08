using R2API;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace EnemiesReturns.Enemies.Colossus
{
    public class FloatingRocksController : MonoBehaviour
    {
        public static float distance = 6f; // TODO

        public static int rockCount = 10; // TODO

        public GameObject[] floatingRocks { get; private set; }

        public Transform initialPosition;

        private GameObject rockThing;

        private void Awake()
        {
            // creating a point to attach to
            rockThing = new GameObject();
            rockThing.name = "RockThing";
            rockThing.transform.parent = this.transform;
            rockThing.transform.localPosition = new Vector3(0, 1f, 0);
            rockThing.transform.rotation = Quaternion.identity;

            GameObject modelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Grandparent/GrandparentMiniBoulderGhost.prefab").WaitForCompletion().InstantiateClone("RockFloaters", false);
            UnityEngine.Object.Destroy(modelPrefab.GetComponent<ProjectileGhostController>());

            List<GameObject> floatingRocks = new List<GameObject>();

            float angle = 360 / rockCount;
            // creating rocks
            for (int i = 0; i < rockCount; i++)
            {
                var x = distance * Mathf.Cos(angle * i * Mathf.Deg2Rad);
                var z = distance * Mathf.Sin(angle * i * Mathf.Deg2Rad);

                var newRock = UnityEngine.Object.Instantiate(modelPrefab);
                newRock.transform.parent = rockThing.transform;
                newRock.transform.position = rockThing.transform.position + new Vector3(x, Random.Range(-2f, 2f), z);
                newRock.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
                floatingRocks.Add(newRock);
            }
            this.floatingRocks = floatingRocks.ToArray();

            this.enabled = false;
        }

        private void OnEnable()
        {
            rockThing.SetActive(true);
        }

        private void OnDisable()
        {
            rockThing.transform.position = initialPosition.position;
            rockThing.SetActive(false);
        }

        private void FixedUpdate()
        {
            rockThing.transform.Rotate(new Vector3(0f, 90f * Time.deltaTime, 0f));
        }

        public void SetRockThingPosition(Vector3 position)
        {
            rockThing.transform.position = position;
        }
    }
}
