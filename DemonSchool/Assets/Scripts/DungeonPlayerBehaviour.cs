﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonPlayerBehaviour : MonoBehaviour
{
    // this is the time we want the player to take to move 1 tile. 
    // it is set in the Unity Inspector
    public float timeToMove;

    // this is the amount of time that has passed as a fraction of our timeToMove variable
    // when it reaches 1, the player will have reached the destination tile and isMoving (below) will be set to false. 
    public float moveProgress;

    // this boolean stores whether the player should still be moving and needs their position updated this frame. 
    public bool isMoving;

    // this represents the change in position required
    public Vector2Int moveAction;

    // these represent the absolute start position and target destination of the character for a move action
    public Vector2 start;
    public Vector2 target;

    // will need to access the instance of the DungeonManager to see how big cell dimensions are to move the correct distance
    private DungeonManager dungeonManager;


    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        moveProgress = 0;
        moveAction = new Vector2Int();
    }

    // Update is called once per frame
    void Update()
    {
        //find the instance of the DungeonManager, if it's not already assigned
        if(dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }

        // check if the player is moving towards a target destination already
        // if player is already moving, don't accept new command until finished
        if(isMoving)
        {
            updateMovement();
        } else
        {
            updateMoveInput();
        }
    }

    // check for player input
    public void updateMoveInput()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            move(new Vector2Int(0, 1));
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            move(new Vector2Int(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            move(new Vector2Int(0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            move(new Vector2Int(1, 0));
        }
    }

    // this starts a move action. Sets start and destination positions. called once per player key press. 
    public void move(Vector2Int moveAction)
    {
        // set the current moveAction to whatever key the player just pressed
        this.moveAction = moveAction;

        // check to make sure at least one of the dimensions is non-zero (will they ever both be zero?)
        if(moveAction.x != 0 || moveAction.y != 0)
        {
            // resets move progress (this will go from 0 to 1)
            moveProgress = 0;
            
            // tells the update function there is movement to be done
            isMoving = true;

            // sets the starting location and the target destination
            start = new Vector2(transform.position.x, transform.position.y);
            target = new Vector2(transform.position.x + moveAction.x * dungeonManager.cellDimensions, 
                                 transform.position.y + moveAction.y * dungeonManager.cellDimensions);
        }
    }

    // actually moves the character between start and destination points. called once each frame if necessary. 
    public void updateMovement()
    {
        // adds the fraction of timeToMove time that the last frame took.
        // makes the movement time independent of frame rate.
        // moveProgress will progress from 0 to 1. 
        moveProgress += Time.deltaTime / timeToMove;

        // checks to see if the move action should be completed (if time passed equals or exceeds our timeToMove setting)
        if(moveProgress >= 1)
        {
            moveProgress = 1;
            isMoving = false;
        }
        // moves the character towards the destination target, based on the percent of timeToMove time passed since the move action commenced.
        transform.position = new Vector3(Mathf.Lerp(start.x, target.x, moveProgress),
                                         Mathf.Lerp(start.y, target.y, moveProgress), 0);
    }


    // stop the character from walking through walls
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision");

        // stop moving forward any more on collision, and run the returnToOriginalPos() method
        isMoving = false;
        returnToOriginPos();

    }

    // when the character tries to walk through a wall, will them to their original position (gives a nice "bump" effect). 
    public void returnToOriginPos()
    {
        // set target as the original start position before walking into a wall (would have been set in the move() method)
        target = start;
        // set start as the current position, where the character collided with the wall
        start = new Vector2(transform.position.x, transform.position.y);
        // send the character on their way
        moveProgress = 0;
        isMoving = true;
    }

}
