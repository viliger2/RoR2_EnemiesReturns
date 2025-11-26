using R2API;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemiesReturns.Enemies.Colossus
{
    public class FloatingRocksController : MonoBehaviour
    {
        public static float distance = Configuration.Colossus.RockClapProjectileSpawnDistance.Value;

        public static int rockCount = Configuration.Colossus.RockClapProjectileCount.Value;

        public static GameObject flyingRockPrefab;

        public GameObject[] floatingRocks { get; private set; }

        public Transform initialPosition;

        private GameObject rockThing;

        private void Awake()
        {
            // creating a point to attach to
            rockThing = new GameObject();
            rockThing.name = "RockThing";
            rockThing.transform.parent = transform;
            rockThing.transform.localPosition = new Vector3(0, 0f, 0);
            rockThing.transform.rotation = Quaternion.identity;

            List<GameObject> floatingRocks = new List<GameObject>();

            float angle = 360 / rockCount;
            // creating rocks
            for (int i = 0; i < rockCount; i++)
            {
                var x = distance * Mathf.Cos(angle * i * Mathf.Deg2Rad);
                var z = distance * Mathf.Sin(angle * i * Mathf.Deg2Rad);

                var newRock = Instantiate(flyingRockPrefab);
                newRock.transform.parent = rockThing.transform;
                //newRock.transform.position = rockThing.transform.position + new Vector3(x, Random.Range(-2f, 2f), z);
                newRock.transform.localPosition = new Vector3(x, Random.Range(-2f, 2f), z);
                newRock.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
                floatingRocks.Add(newRock);
            }
            this.floatingRocks = floatingRocks.ToArray();

            enabled = false;
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
            rockThing.transform.Rotate(new Vector3(0f, 90f * Time.fixedDeltaTime, 0f));
        }

        public Vector3 GetRockThingPosition()
        {
            return rockThing.transform.position;    
        }

        public void SetRockThingPosition(Vector3 position)
        {
            rockThing.transform.position = position;
        }
    }
}
