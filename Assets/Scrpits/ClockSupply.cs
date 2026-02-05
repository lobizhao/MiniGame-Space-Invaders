using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSupply : MonoBehaviour
{
    public float speed = 4f;
    public int direction = 1;
    public GameObject explosionFX;

    void Update()
    {
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (transform.position.x > 12f || transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ActivateTimeSlow(5.0f);
            }

            Destroy(other.gameObject);

            if (explosionFX != null)
            {
                GameObject vfx = Instantiate(explosionFX, transform.position, Quaternion.identity);
                Destroy(vfx, 1.0f);
            }
            Destroy(gameObject);
        }
    }
}