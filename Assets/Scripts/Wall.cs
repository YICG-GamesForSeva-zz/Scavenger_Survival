using Completed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public AudioClip chopSound1; // 1 of 2 audio clips that play when the wall is attacked by the player
    public AudioClip chopSound2; // 2 of 2 audio clips that play when the wall is attacked by the player
    public Sprite dmgSprite;     // Alternate sprite to display after the wall has been attacked by the player.
    public int hitPoints = 3;    // Hit points for the wall.

    private SpriteRenderer spriteRenderer; // Store a component reference to the attached SpriteRenderer. \

    void Awake()
    {
        // Get a component reference to the SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    /// <summary>
    /// The DamageWall method is called when the player
    /// attacks a wall. 
    /// </summary>
    /// <param name="loss">Integer value which will be passed</param>
    public void DamageWall(int loss)
    {
        // Call the RandomizeSfx function of SoundManager
        // to play one of two chop sounds. 
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

        // Set spriteRenderer to the damaged wall sprite. 
        spriteRenderer.sprite = dmgSprite;

        // Subtract loss from the hit point total
        hitPoints -= loss;

        // If the hit points are less than or equal to zero
        if (hitPoints <= 0)
        {
            // Disable the gameObject
            gameObject.SetActive(false); 
        }
    }
}