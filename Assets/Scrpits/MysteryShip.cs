using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryShip : MonoBehaviour
{

    public float speed = 5f;
    public int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "Untagged") return;

        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (transform.position.x > 12f || transform.position.x < -12f)
        {
            Destroy(gameObject);
        }

    }

    //buff shield when hit
void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet")) 
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if(player != null)
            {
                player.ActivateShield();
            }

            Destroy(other.gameObject);
        
            Enemy enemyScript = GetComponent<Enemy>();
            if(enemyScript != null)
            {
                enemyScript.Die();
            }
            else 
            {
                Destroy(gameObject); 
            }
        }
    }
}
