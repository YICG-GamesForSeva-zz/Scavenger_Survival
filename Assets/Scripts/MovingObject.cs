using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f; // Time taken for an object to move
    public LayerMask blockingLayer; // Layer on which collision(s) will be checked

    private BoxCollider2D boxCollider; //The BoxCollider2D component attached to the script
    private Rigidbody2D rb2D;          //The RigidBody2D component attached to the script
    private float inverseMoveTime;     //Used to make the movement more efficient

    // Protected, virtual functions can be overridden by inheriting classes
    protected virtual void Start()
    {
        // Get a component reference to this object's BoxCollider2D
        boxCollider = GetComponent<BoxCollider2D>();

        // Get a component reference to this object's Rigidbody2D
        rb2D = GetComponent<Rigidbody2D>();

        // By storing the reciprocal of the move time, we can use it
        // by multiplying instead of dividing. 
        inverseMoveTime = 1f / moveTime;
    }

    // Move will return true if the object is able to move, and return
    // false if the object cannot move. The move method will take parameters
    // for x, y directions, and a RaycastHit2D to check collisions.
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        // Store the start position to move from, based on
        // the objects current transform position
        Vector2 start = transform.position;

        // Calculate the end position based on the direction parameters
        // passed in when calling Move. 
        Vector2 end = start + new Vector2(xDir, yDir);

        // Disable the boxCollider so that the linecast 
        // doesn't hit this object's own collider
        boxCollider.enabled = false;

        // Cast a line from start point to end point checking
        // the collision on the blocking layer
        hit = Physics2D.Linecast(start, end, blockingLayer);

        // Re-enable the boxCollider after the linecast
        boxCollider.enabled = true;

        // Check if anything was hit
        if (hit.transform == null)
        {
            // If nothing was hit, start SmoothMovement co-routine
            // passing in the Vector2 end as destination
            StartCoroutine(SmoothMovement(end));

            // Return true to signal that the move was successful
            return true; 
        }
        else
        {
            // If something was hit, return false, the move
            // was not successful
            return false;
        }
    }

    /// <summary>
    /// Co-routine for moving units from one space to next, 
    /// takes a parameter end to specify from where to move
    /// to.
    /// </summary>
    /// <param name="end">Vector 2D</param>
    /// <returns></returns>
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        // Calculate the remaining distance to move based on the
        // square magnitude of the difference between current position
        // and end parameter.
        // Square magnitude is used of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            // Find a new position proportionally closer
            // to the end, based on moveTime
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            // Call MovePosition on the attached Rigidbody2D and move it to the calculated
            // position
            rb2D.MovePosition(newPosition);

            // Recalculate the remaining distance after moving. 
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            // Return and loop until sqrRemainingDistance is close enough to zero
            // to end the function
            yield return null; 
        }
    }

    // The virtual keyword means that AttemptMove can be overridden
    // by inheriting classes using the override keyword.
    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        // Hit will store whatever our linecast hits when Move is called
        RaycastHit2D hit;

        // Set canMove to true if Move was successful, false if failed
        bool canMove = Move(xDir, yDir, out hit);

        // Check if nothing was hit by the linecast
        if (hit.transform == null)
        {
            // If nothing was hit, return and don't execute
            // further code. 
            return; 
        }

        // Get a component reference to the component of type T
        // attached to the object that was hit
        T hitComponent = hit.transform.GetComponent<T>();

        // If canMove is false and hitComponent is not equal to null, 
        // meaning that if MovingObject is blocked and has hit something
        // it can interact with. 
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent); 
        }
    }

    /// <summary>
    /// The abstract modifier will indicate that 
    /// the thing being modified has a missing or incomplete 
    /// implementation. OnCantMove will be overridden by functions
    /// in the inheriting classes.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="hitComponent">Component that is hit</param>
    protected abstract void OnCantMove<T>(T hitComponent) 
        where T : Component; 
}