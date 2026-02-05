using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    //public float lifeTime = 2f;
    // Start is called before the first frame update

    public float maxHeight = 5.88f;
    public float debrisLifeTime = 5f;

    private bool isSpent = false;
    private Rigidbody rb;
    private Collider col;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        //Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpent) return;

        //transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);

        if (transform.position.y >= maxHeight)
        {
            BecomeDebris();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isSpent) return;


        if (other.CompareTag("Shield")){
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void BecomeDebris()
    {
        isSpent = true;
        gameObject.tag = "Untagged"; 
        gameObject.layer = LayerMask.NameToLayer("Debris");

        if (col != null){
            col.isTrigger = false;
        }

        if (rb != null){
            rb.isKinematic = false;
            rb.useGravity = true;
            
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            rb.velocity = Vector3.down * 1f; 
        }

        Destroy(gameObject, debrisLifeTime);
    }
}
