using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Player : MovingObject
{
    // The damage that the player is going to apply to the wall object(s) when it is chopping them
    public int wallDamage = 1;

    // The number of points when food or soda items are collected by the player
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;

    // Delay in restarting the level, used for level transitions
    public float restartLevelDelay = 1f;

    // Creating private variables
    private Animator animator;
    private int food; 

    // Use this for initialization
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        base.Start(); 
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn)
        {
            return; 
        }

        // Storing the direction(s) that we are moving
        int horizontal = 0;
        int vertical = 0;

        // Getting the value of the movements both horizontally and vertically
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        // Try to prevent the player from moving diagonally
        if (horizontal != 0)
        {
            vertical = 0; 
        }

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical); 
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false; 
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            GameManager.instance.GameOver(); 
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop"); 
    }

    private void Restart()
    {
        SceneManager.LoadScene(0); 
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false; 
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false); 
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false); 
        }
    }
}