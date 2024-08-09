using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public Transform initialPoint;

    public Transform spawnPoint;

    public GameObject box;

    public float repeatEach = 3f;

    public float speed;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer >= repeatEach) {
            
            var initialPosition = (spawnPoint.position - initialPoint.position) * 0.8f;
            initialPosition = new Vector3(initialPoint.position.x + initialPosition.x, initialPoint.position.y, initialPoint.position.z + initialPosition.z);


            var rotation = Quaternion.LookRotation(spawnPoint.position - initialPosition, Vector3.up);
            var newObject = UnityEngine.Object.Instantiate(box, spawnPoint.position, rotation);

            var rigidBodyComponent = newObject.GetComponent<Rigidbody>();
            rigidBodyComponent.velocity = newObject.transform.forward * speed;
            timer = 0;
        }
    }
}
