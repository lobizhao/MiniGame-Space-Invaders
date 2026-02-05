using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
//SWITCH CAMERA
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int score = 0;
    public static bool shouldAutoStart = false;

    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject gameWorld;
    //lost panel
    public GameObject gameOverPanel;
    //win panel
    public GameObject victoryPanel;

    //set score
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    //TIME BUFF
    public bool isTimeSlowActive = false;

    //private int score = 0;
    private int highScore = 0;

    //Toggle for camera
    public Toggle view3DToggle;
    public GameObject camera3DObj;

    //game lives
    public GameObject[] heartIcons;
    public GameObject playerShip;
    public int maxLives = 3;

    private int currentLives;


    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        //load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();

        //ShowMainMenu();
        if (shouldAutoStart == true){
            shouldAutoStart = false;
            StartGame(); 
        } else{
            //reset score as 0
            score = 0;
            ShowMainMenu(); 
        }
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        hudPanel.SetActive(true);
        //hide game world
        gameWorld.SetActive(false);
        //gameOverPanel.SetActive(false);
        if(gameOverPanel != null) gameOverPanel.SetActive(false);
        if(victoryPanel != null) victoryPanel.SetActive(false);
        //reset score
        scoreText.text = "SCORE\n0000";

    }

    // public void StartGame()
    // {
    //     //score = 0;
    //     //scoreText.text = "SCORE\n0000";
    //     scoreText.text = "SCORE\n" + score.ToString("D4");

    //     mainMenuPanel.SetActive(false); 
    //     hudPanel.SetActive(true);       
    //     //Level begin, show game world
    //     gameWorld.SetActive(true);
    //     //gameOverPanel.SetActive(false);
        
    //     Debug.Log("Game Started!");
    // }

    public void StartGame()
    {
        scoreText.text = "SCORE\n" + score.ToString("D4");
        if (view3DToggle != null && view3DToggle.isOn)
        {
            if(camera3DObj != null) camera3DObj.SetActive(true);
            if(camera3DObj != null && camera3DObj.GetComponent<AudioListener>() != null)
            {
                camera3DObj.GetComponent<AudioListener>().enabled = true;
            }

            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            if(camera3DObj != null) camera3DObj.SetActive(false);
            GetComponent<Camera>().enabled = true;
            GetComponent<AudioListener>().enabled = true;
        }

        mainMenuPanel.SetActive(false); 
        hudPanel.SetActive(true);       
        gameWorld.SetActive(true);

        currentLives = maxLives;
        foreach (GameObject heart in heartIcons)
        {
            heart.SetActive(true);
        }
        
        if(playerShip != null) 
        {
            playerShip.SetActive(true);
            playerShip.transform.position = new Vector3(0f, -3f, 0.3f);
        }
        
        Debug.Log("Game Started!");
    }

    //game over
    public void GameOver()
    {
        Debug.Log("Game Over!");
        gameOverPanel.SetActive(true);
        //game stop
        Time.timeScale = 0f; 
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "SCORE\n" + score.ToString("D4");

        //check high score
        if(score > highScore){
            highScore = score;
            UpdateHighScoreUI();
            //save high score
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreUI();
        }
    }

    //win
    public void Victory()
    {
        Debug.Log("You Win!");
        if(victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void NextLevel()
    {
        shouldAutoStart = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateHighScoreUI(){
        if(highScoreText != null){
            highScoreText.text = "HIGH SCORE\n" + highScore.ToString("D4");
        }
    
    }

    //TIME BUFF
    public void ActivateTimeSlow(float duration)
    {
        if (isTimeSlowActive){
            StopCoroutine("TimeSlowRoutine");
        }
        StartCoroutine(TimeSlowRoutine(duration));
    }

    IEnumerator TimeSlowRoutine(float duration)
    {
        isTimeSlowActive = true;
        Debug.Log("Time Slow Started!");
        EnemyGrid grid = FindObjectOfType<EnemyGrid>();
        if (grid != null) grid.SetSlowMotion(true);
        EnemyBullet[] bullets = FindObjectsOfType<EnemyBullet>();
        foreach (EnemyBullet b in bullets)
        {
            b.SetSlowMotion(true);
        }

        yield return new WaitForSeconds(duration);

        isTimeSlowActive = false;
        Debug.Log("Time Slow Ended!");
        if (grid != null) grid.SetSlowMotion(false);

        bullets = FindObjectsOfType<EnemyBullet>(); 
        foreach (EnemyBullet b in bullets)
        {
            b.SetSlowMotion(false);
        }
    }

    public void OnPlayerDied()
    {
        currentLives--;
        if (currentLives >= 0 && currentLives < heartIcons.Length)
        {
            heartIcons[currentLives].SetActive(false);
        }

        if (currentLives > 0)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            GameOver();
        }
    }

    IEnumerator RespawnRoutine(){
        yield return new WaitForSeconds(1.0f);

        if (playerShip != null)
        {
            playerShip.transform.position = new Vector3(0f, -3f, 0.3f);
            playerShip.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
