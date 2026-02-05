using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float xLimit = 8.5f;

    //bullet    
    public GameObject bulletPrefab; 
    public Transform firePoint;

    //die effect
    public GameObject explosionFX;

    //buff shield 
    public GameObject umbrellaShield;
    public float shieldDuration = 5.0f;
    private bool isShieldActive = false;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        //close shield at beginning
        if(umbrellaShield != null) umbrellaShield.SetActive(false);
    }

    void OnEnable()
    {
        isDead = false;
        isShieldActive = false;
        if(umbrellaShield != null) umbrellaShield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        float moveInput = Input.GetAxis("Horizontal");
        //speed
        Vector3 moveVector = Vector3.right * moveInput * moveSpeed * Time.deltaTime;

        // transform.Translate(moveVector);
        transform.Translate(moveVector, Space.World);

        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, -xLimit, xLimit);
        transform.position = currentPos;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;
        Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

    if (isShieldActive && other.CompareTag("EnemyBullet"))
        {
            return; 
        }

        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            Die();
            Destroy(other.gameObject);
        }
    }

    public void Die(){

        if (isDead) return;

        if(explosionFX != null) 
        {
            GameObject vfx = Instantiate(explosionFX, transform.position, Quaternion.identity);
            Destroy(vfx, 1.0f);
        }
        if (GameManager.Instance != null){
            GameManager.Instance.OnPlayerDied();
        }
        gameObject.SetActive(false);
    }


    public void ActivateShield()
    {
        if(umbrellaShield == null || isDead) return;

        if (isShieldActive)
        {
            CancelInvoke("DeactivateShield");
        }

        isShieldActive = true;
        umbrellaShield.SetActive(true);

        Invoke("DeactivateShield", shieldDuration);
    }


    void DeactivateShield()
    {
        isShieldActive = false;
        if(umbrellaShield != null) umbrellaShield.SetActive(false);
    }
}
