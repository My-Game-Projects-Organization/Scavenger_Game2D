using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;

    public BoardManager boardScript;
    public int level = 0;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private Button replayBtn;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private bool isPlaying = false;
    private bool isGameRestart = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null) 
            Destroy(gameObject);

        isPlaying = true;
        level = 0;
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
            instance.level++;
            instance.InitGame();
    }
    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        replayBtn.gameObject.SetActive(true);
        isPlaying = false;

        enabled = false;
    }

    private void InitGame()
    {
        if (isGameRestart)
        {
            playerFoodPoints = 100;
            isGameRestart = false;
            enabled = true;
            isPlaying = true;
            SoundManager.instance.musicSource.Play();
        }
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        replayBtn = GameObject.Find("BtnReplay").GetComponent<Button>();   
        replayBtn.onClick.RemoveAllListeners();
        replayBtn.onClick.AddListener(() => Restart());
        replayBtn.gameObject.SetActive(false);
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }


    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;  
    }

    // Update is called once per frame
    void Update()
    {
        if(playersTurn || enemiesMoving || !isPlaying)
        {
            return;
        }
       
        StartCoroutine(MoveEnemies());
    }
    private void Restart()
    {
        // Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        instance.playerFoodPoints = 100;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

        instance.level = 0;
        isGameRestart = true;
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
}
