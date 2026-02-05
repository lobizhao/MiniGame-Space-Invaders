using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrid : MonoBehaviour
{
    //enemy prefabs 
    public GameObject[] prefabs;

    public int rows = 5;
    public int columns = 11;
    public float spacingX = 0f;
    public float spacingY = 0f;

    //how matrix move
    public float moveSpeed = 2.0f;
    public float dropDistance = 1.0f;
    public float speedMultiplier = 1.05f;

    //shoot set
    public GameObject enemyBulletPrefab;
    public float minShootInterval = 1.0f;
    public float maxShootInterval = 3.0f;
    //bullet sum
    public int shotsPerWave = 2;


    private int direction = 1;
    private Vector3 currentVelocity;
    private float shootTimer;
    private int totalEnemies;

    //TIME BUFF
    public float moveRate = 1.0f;
    private float originalMoveRate;

    private float originalMoveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        originalMoveSpeed = moveSpeed;

        originalMoveRate = moveRate;
        GenerateGrid();
        totalEnemies = rows * columns;
        shootTimer = Random.Range(minShootInterval, maxShootInterval);

        //InvokeRepeating("MoveEnemy", 0.1f, moveRate);
    }   

    void GenerateGrid(){

        float gridWidth = (columns - 1) * spacingX;
        Vector3 startPos = new Vector3(-gridWidth / 2, 0, 0);

        for(int row = 0; row < rows; row++){
            GameObject prefabToSpawn;

            if(row == 0){
                prefabToSpawn = prefabs[0];
            }else if(row == 1 || row == 2){
                prefabToSpawn = prefabs[1];
            }else{
                prefabToSpawn = prefabs[2];
            }

            for(int col = 0; col < columns; col++){
                Vector3 spawnPos = new Vector3(
                    startPos.x + (col * spacingX), 
                    startPos.y - (row * spacingY),
                    0
                );

                GameObject newEnemy = Instantiate(prefabToSpawn, transform);
                newEnemy.transform.localPosition = spawnPos;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        foreach (Transform enemy in transform){
            if (!enemy.gameObject.activeInHierarchy) {
                continue;
            }
            if (direction == 1 && enemy.position.x > 8.1f){
                ChangeDirection(-1);
                break; 
            }else if (direction == -1 && enemy.position.x < -8.1f){
                ChangeDirection(1);
                break;
            }
        }

        HandleShooting();
    }

    public void OnEnemyKilled(){
        totalEnemies--;

        if (totalEnemies <= 0)
        {
            GameManager.Instance.Victory();
        }
    }

    void HandleShooting(){
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0){
            Attack();
            shootTimer = Random.Range(minShootInterval, maxShootInterval);
        }
    }

    void Attack(){

        if (transform.childCount == 0){
            return;
        }

        for(int i=0; i< shotsPerWave; i++){
            Transform randomEnemy = null;
            int attempts = 0;
            
            while (randomEnemy == null && attempts < 10){
                int index = Random.Range(0, transform.childCount);
                Transform candidate = transform.GetChild(index);
            
                if (candidate.gameObject.activeSelf) {
                    randomEnemy = candidate;
                }
                attempts++;
            }

            if (randomEnemy != null){
                Instantiate(enemyBulletPrefab, randomEnemy.position, Quaternion.identity);
            }
        }
    }

    void ChangeDirection(int newDir)
    {
        direction = newDir;
        Vector3 pos = transform.position;
        pos.y -= dropDistance;
        transform.position = pos;
        moveSpeed *= speedMultiplier; 
    }

    public void SetSlowMotion(bool active)
    {
        if (active){
            moveSpeed = originalMoveSpeed * 0.1f;
            Debug.Log("Grid Slow Motion ON");
        }
        else{

            moveSpeed = originalMoveSpeed;
            Debug.Log("Grid Slow Motion OFF");
        }
    }
}
