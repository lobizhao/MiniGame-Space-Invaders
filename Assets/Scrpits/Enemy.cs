using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //set score value
    public int scoreValue = 10;
    public GameObject explosionEffect;
    
    public float floatStrength = 0.1f;
    public float floatSpeed = 2.0f;

    private Vector3 initialLocalPosition;
    private float randomOffset;
    //no UFO
    public bool enableFloating = true;

    //after die

    public Material deadMaterial;

    public float tumbleForce = 5.0f;
    private bool isDead = false;

    private BoxCollider boxCol;
    private MeshCollider meshCol;
    private Rigidbody rb;

    private Renderer meshRenderer;

    //touch red line 
    public float invasionLine = -3.75f;


    void Start()
    {
        initialLocalPosition = transform.localPosition;
        randomOffset = Random.Range(0f, 100f);

        //start
        boxCol = GetComponent<BoxCollider>();
        meshCol = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();

        meshRenderer = GetComponent<Renderer>();

        if(rb != null) {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        //set init
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        if(meshCol != null) meshCol.enabled = false;
        if(boxCol != null) boxCol.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(isDead){
            //destroy when fall out of screen
            if (transform.position.y < -10f){
                Destroy(gameObject);
            }
            return;
        }
        if (enableFloating){
            float newX = Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatStrength;
            float newY = Mathf.Cos(Time.time * floatSpeed * 0.8f + randomOffset) * floatStrength;
            transform.localPosition = initialLocalPosition + new Vector3(newX, newY, 0);
        }

        if (transform.position.y <= invasionLine)
        {
            TriggerInvasionGameOver();
        }
    }

    void TriggerInvasionGameOver()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("Invasion Successful! Game Over.");
            GameManager.Instance.GameOver();
        }
    }

    void OnTriggerEnter(Collider other){

        if(isDead){
            return;
        } 

        //check tag bullet
        if( !isDead && other.CompareTag("Bullet") || other.GetComponent<Bullet>()){
            Die();
            Destroy(other.gameObject);
        }
    }

    public void Die(){

        if(isDead){
            return;
        }

        isDead = true;

        //wind score
        if (GameManager.Instance != null){
            GameManager.Instance.AddScore(scoreValue);
        }

        //notify grid
        EnemyGrid grid = GetComponentInParent<EnemyGrid>();
        
        if (grid != null) 
        {
            grid.OnEnemyKilled();
        }

        if (explosionEffect != null){
            GameObject vfx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            //destroy vf
            Destroy(vfx, 1.0f); 
        }


        //die and fall
        transform.parent = null;

        gameObject.layer = LayerMask.NameToLayer("Debris");

        //smaller if dead
        transform.localScale = transform.localScale * 0.8f;

        if(meshRenderer != null && deadMaterial != null){
            meshRenderer.material = deadMaterial;
        }

        //switch colliders
        if(boxCol != null) boxCol.enabled = false;
        if(meshCol != null) meshCol.enabled = true;
        //set protect player
        gameObject.tag = "Untagged";

        if(rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.AddTorque(Random.insideUnitSphere * tumbleForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 1.0f, ForceMode.Impulse);
        }

        //Destroy(gameObject);
    }


}
