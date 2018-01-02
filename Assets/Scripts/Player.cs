using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

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

    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    // Use this for initialization
    protected override void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        //Get the current food point total stored in GameManager.instance between levels.
        food = GameManager.instance.playerFoodPoints;

        //Set the foodText to reflect the current player food total.
        foodText.text = "Food: " + food;

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
        //Every time player moves, subtract from food points total.
        food--;

        //Update food text display to reflect current score.
        foodText.text = "Food: " + food;

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove<T>(xDir, yDir);

        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;

        //If Move returns true, meaning Player was able to move into an empty space.
        if (Move(xDir, yDir, out hit))
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        //Since the player has moved and lost food points, check if the game has ended.
        CheckIfGameOver();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }

    private void CheckIfGameOver()
    {
        //Check if food point total is less than or equal to zero.
        if (food <= 0)
        {
            //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            SoundManager.instance.PlaySingle(gameOverSound);

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        //Set hitWall to equal the component passed in as a parameter.
        Wall hitWall = component as Wall;

        //Call the DamageWall function of the Wall we are hitting.
        hitWall.DamageWall(wallDamage);

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(0); 
    }

    public void LoseFood(int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        //Subtract lost food points from the players total.
        food -= loss;

        //Update the food display with the new total.
        foodText.text = "-" + loss + " Food: " + food;

        //Check to see if game has ended.
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
        }

        //Check if the tag of the trigger collided with is Food.
        else if (other.tag == "Food")
        {
            //Add pointsPerFood to the players current food total.
            food += pointsPerFood;

            //Update foodText to represent current total and notify player that they gained points
            foodText.text = "+" + pointsPerFood + " Food: " + food;

            //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);

            //Disable the food object the player collided with.
            other.gameObject.SetActive(false);
        }

        //Check if the tag of the trigger collided with is Soda.
        else if (other.tag == "Soda")
        {
            //Add pointsPerSoda to players food points total
            food += pointsPerSoda;

            //Update foodText to represent current total and notify player that they gained points
            foodText.text = "+" + pointsPerSoda + " Food: " + food;

            //Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

            //Disable the soda object the player collided with.
            other.gameObject.SetActive(false);
        }
    }
}