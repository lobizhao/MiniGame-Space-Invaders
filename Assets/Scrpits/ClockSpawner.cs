using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSpawner : MonoBehaviour
{
    public GameObject clockPrefab;
    public float spawnInterval = 20f;
    public float startDelay = 10f;

    void Start()
    {
        InvokeRepeating("SpawnClock", startDelay, spawnInterval);
    }

    void SpawnClock()
    {
        float spawnY = Random.Range(4.3f, 4.6f); 
        
        Vector3 spawnPos;
        int direction;

        if (Random.value > 0.5f)
        {

            spawnPos = new Vector3(-12f, spawnY, 0f);
            direction = 1;
        }
        else
        {
            spawnPos = new Vector3(14f, spawnY, 0f);
            direction = -1;
        }

        GameObject clock = Instantiate(clockPrefab, spawnPos, Quaternion.identity);
        ClockSupply supplyScript = clock.GetComponent<ClockSupply>();
        if (supplyScript != null)
        {
            supplyScript.direction = direction;
        }
    }
}