using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRigidBody : MonoBehaviour
{
    public float speed = 5f;

    public GameObject prefab;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
        //rb.angularVelocity = Vector3.zero;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.contacts.Length > 0)
        {
            transform.forward = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
            // Collision normal
            // Vector3 normal = collision.contacts[0].normal;

            // // Reflect
            // var newForward = Vector3.Reflect(transform.forward, normal);

            // var newObject = UnityEngine.Object.Instantiate(prefab, transform.position + newForward, Quaternion.identity);
            // newObject.transform.forward = newForward;

            // UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
