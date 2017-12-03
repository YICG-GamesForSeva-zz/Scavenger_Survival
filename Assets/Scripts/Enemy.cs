using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage; // The number of food points to subtract

    private Animator animator; // Helps to store a reference for the enemy animation
    private Transform target;  // A transform to aid the enemy in moving towards the player
    private bool skipMove;     // Tell the enemy when exactly to move

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start(); 
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return; 
        }

        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // Checking the position of the target, against the position
        // of the transform - to determine whether or not the enemy
        // has to move
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > target.position.y ? 1 : -1; 
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1; 
        }

        AttemptMove<Player>(xDir, yDir); 
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.LoseFood(playerDamage);
    }
}