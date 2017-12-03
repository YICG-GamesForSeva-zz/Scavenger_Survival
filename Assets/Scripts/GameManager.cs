using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public int playerFoodPoints = 100;
    public static GameManager instance = null;
    [HideInInspector] public bool playersTurn = true;

    public BoardManager boardScript;
    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving; 

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject); 
        }

        // Sets this not to be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        // Assign enemies to a new List of Enemy objects
        enemies = new List<Enemy>(); 

        boardScript = GetComponent<BoardManager>();
        InitGame(); 
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // Add one to our level number.
        level++;

        // Call InitGame to initialize our level.
        InitGame(); 
    }

    void OnEnable()
    {
        // Tell our 'OnLevelFinishedLoading' function
        // to start listening for a scene change event
        // as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        // Telling our 'OnLevelFinishedLoading' function
        // to stop listening for a scene change event as
        // soon as this script is disabled.

        // Remember to always have an unsubscription for
        // every delegate that is subscribed to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame()
    {
        // Clear off enemies
        enemies.Clear(); 

        boardScript.SetupScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        // Check that playersTurn or enemiesMoving
        // or doingSetup are not currently true. 
        if (playersTurn || enemiesMoving)
        {
            return;
        }

        // Start moving enemies
        StartCoroutine(MoveEnemies()); 
    }

    IEnumerator MoveEnemies()
    {
        // While enemiesMoving is true, player is unable to move
        enemiesMoving = true;

        // Wait for turnDelay seconds, defaults to 0.1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        // If there are no enemies spawned (IE in first level). 
        if (enemies.Count == 0)
        {
            // Wait for turnDelay seconds between moves,
            // replaces delay caused by enemies moving when
            // there are none.
            yield return new WaitForSeconds(turnDelay); 
        }

        // Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            // Call the MoveEnemy function of Enemy at
            // index i, in the enemies List
            enemies[i].MoveEnemy();

            // Wait for Enemy's moveTime before moving
            // the next Enemy
            yield return new WaitForSeconds(enemies[i].moveTime); 
        }

        // Once enemies are done moving, set the playersTurn to true
        // so that the player can move.
        playersTurn = true;

        // Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }

    public void GameOver()
    {
        // Enable the black background image gameObject.
        // levelImage.SetActive(true); 

        // Disable the GameManager.
        enabled = false;
    }

    // Call this to add the passed in Enemy script to
    // the List of Enemy objects
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script); 
    }
}