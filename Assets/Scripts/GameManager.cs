using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public int playerFoodPoints = 100;
    public static GameManager instance = null;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    public BoardManager boardScript;
    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup = true;

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

    //void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    //{
    //    // Add one to our level number.
    //    level++;

    //    // Call InitGame to initialize our level.
    //    InitGame(); 
    //}

    void OnEnable()
    {
        // Tell our 'OnLevelFinishedLoading' function
        // to start listening for a scene change event
        // as soon as this script is enabled.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Telling our 'OnLevelFinishedLoading' function
        // to stop listening for a scene change event as
        // soon as this script is disabled.

        // Remember to always have an unsubscription for
        // every delegate that is subscribed to!
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void InitGame()
    {
        // While doing setup is true, the player can't move
        doingSetup = true;

        // Get a reference to our image LevelImage by finding it by name
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Day " + level;

        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", levelStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();

        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);
    }

    void HideLevelImage()
    {
        // Disable the levelImage gameObject
        levelImage.SetActive(false);

        doingSetup = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check that playersTurn or enemiesMoving
        // or doingSetup are not currently true. 
        if (playersTurn || enemiesMoving || doingSetup)
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
        //Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " days, you starved.";

        //Enable black background image gameObject.
        levelImage.SetActive(true);

        //Disable this GameManager.
        enabled = false;
    }

    // Call this to add the passed in Enemy script to
    // the List of Enemy objects
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script); 
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }
}