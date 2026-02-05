using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    public float baseSpeed = 10f;
    //BUFF
    public float currentSpeed;

    private bool isDebris = false;

    private Rigidbody rb;
    private Collider col;

    //add audio
    public AudioClip shieldBreakClip;
    public AudioClip ricochetClip;
    public float soundVolume = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        //TIME BUFF
        if (GameManager.Instance != null && GameManager.Instance.isTimeSlowActive){
            currentSpeed = baseSpeed * 0.5f;
        }else{
            currentSpeed = baseSpeed;
        }

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        Destroy(gameObject, 6f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDebris){
            return;
        }

        //transform.Translate(Vector3.down * speed * Time.deltaTime);
        //TIME BUFF
        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);

        if (transform.position.y < -15f) {
            Destroy(gameObject);
        }
    }

    public void SetSlowMotion(bool active)
    {
        if (active){
            currentSpeed = baseSpeed * 0.5f;
        }
        else{
            currentSpeed = baseSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDebris){
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShield")){
            PlaySound(ricochetClip);
            BecomeDebris();
        }else if (other.CompareTag("Shield")){
            PlaySound(shieldBreakClip);
            Destroy(other.gameObject); 
            Destroy(gameObject);       
        }else if(other.CompareTag("Ground")){
            BecomeDebris();
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null){
            
            AudioSource.PlayClipAtPoint(clip, transform.position, soundVolume);
        }
    }


    void BecomeDebris()
    {
        isDebris = true;
        gameObject.layer = LayerMask.NameToLayer("Debris");
        gameObject.tag = "Untagged";

        if (col != null)
        {
            col.isTrigger = false;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            rb.velocity = Vector3.up * 2f + Random.insideUnitSphere * 1f;
        }
    }
}
