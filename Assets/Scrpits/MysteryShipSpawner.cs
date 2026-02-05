using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryShipSpawner : MonoBehaviour
{

    public GameObject ufoPrefab;
    public float minSpawnTime = 15f;
    public float maxSpawnTime = 30f;
    public float spawnY = 6f;

    private float timer;
    private float nextSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        SetNextSpawnTime();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnUFO();
            SetNextSpawnTime();
        }
    }

    void SetNextSpawnTime()
    {
        timer = 0;
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

void SpawnUFO()
    {

        int side = Random.Range(0, 2); 
        
        float spawnX = (side == 0) ? -11f : 11f;
        int direction = (side == 0) ? 1 : -1;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        GameObject ufo = Instantiate(ufoPrefab, spawnPos, Quaternion.identity);
        
        MysteryShip shipScript = ufo.GetComponent<MysteryShip>();
        if (shipScript != null)
        {
            shipScript.direction = direction;
        }
    }

}
