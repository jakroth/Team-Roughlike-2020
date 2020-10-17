using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{

    // will need to access the instance of the DungeonManager to remake the Dungeon when colliding with an exit door
    private DungeonManager dungeonManager;

    //needs access to the PlayerSoundManager to play audio
    private PlayerSoundManager playerSoundManager;

    //needs access to the FadeController to fade the notes in and out
    private FadeController fadeController;

    [Header("Player Attributes")]
    // this is the speed pf the player
    public float speed;

    // Player health setting;
    public int playerHealth;

    // Player ammo;
    public int playerAmmo;

    public Text playerHealthNum;
    public Text playerAmmoNum;

    public int HealthItemID = 0;
    public int AmmoItemID = 1;

    [Header("Player Animation Sprites")]
    // tile set for the Normal Tiles
    public List<Sprite> animationList;
    public int jockDirection;
    public int jockForm;
    public int formChangeDirection;
    public float animSpeed;
    public float animTime;
    public bool stopped;
    private bool directionChangeX = false;
    private bool directionChangeY = false;


    // for collisions
    private RaycastHit2D[] rayArray = new RaycastHit2D[4];
    private float laserLength = 0.4f;
    private CircleCollider2D coll;



    // Start is called before the first frame update
    void Start()
    {        
        playerSoundManager = gameObject.GetComponent<PlayerSoundManager>();
        fadeController = GameObject.FindGameObjectWithTag("UI").GetComponent<FadeController>();

        coll = GetComponent<CircleCollider2D>();

    }



    // Update is called once per frame
    void Update()
    {

        updateAnimation();
        if (!stopped)
            playerSoundManager.PlayFootsteps();
        else
            playerSoundManager.EndFootsteps();

    }


    // FixedUpdate is called in line with the physics engine. Can be set in the project settings. 
    void FixedUpdate()
    {
        rayCollisionChecking();
        move();
    }



    // checks for collisions around the player, and bounces the player away if they get too close
    private void rayCollisionChecking()
    {
        float shortestDistance = 100;
        RaycastHit2D closestHit = new RaycastHit2D();

        //Get the closest object hit by the rays
        for (int i = 0; i < 4; i++)
        {
            // work out directions (0,1 or -1,0, etc)
            Vector2 direction = new Vector2(i % 2 * Mathf.Pow(-1, i % 3), (i + 1) % 2 * Mathf.Pow(-1, (i + 1) % 3));
            //Debug.Log(direction);
            Debug.DrawRay(coll.bounds.center, direction * laserLength, Color.red);

            // send out the ray
            rayArray[i] = Physics2D.Raycast(coll.bounds.center, direction, laserLength, LayerMask.GetMask("Wall"));
            if (rayArray[i].collider != null)
            {
                float distance = Vector2.Distance(rayArray[i].collider.transform.position, transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestHit = rayArray[i];
                }
            }
        }
        //Debug.Log(shortestDistance);
        if (closestHit.collider != null)
        {
            if (closestHit.collider.gameObject.tag == "door")
            {
                if (closestHit.collider.gameObject.GetComponent<DungeonTile>().isFinalDoor)
                {
                    Debug.Log("finalDoor");
                    GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadNextScene();
                }
                else
                    moveAway(closestHit);
            }
            else
                moveAway(closestHit);
        }
    }


    // the bounce function
    public void moveAway(RaycastHit2D r)
    {
        float deltaX = r.point.x - coll.bounds.center.x;
        float deltaY = r.point.y - coll.bounds.center.y;

        transform.position = new Vector2(transform.position.x - deltaX / 3, transform.position.y - deltaY / 3);
    }



    // checks player move input
    public void move()
    {

        var newXPos = transform.position.x + Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
        var newYPos = transform.position.y + Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;
        // move left
        if (newXPos < transform.position.x)
        {
            stopped = false;
            jockDirection = 9;
            if (directionChangeX)
            {
                GetComponent<SpriteRenderer>().sprite = animationList[jockDirection + 2];
                directionChangeX = false;
            }
        }
        // move right
        else if (newXPos > transform.position.x)
        {
            stopped = false;
            jockDirection = 6;
            if (!directionChangeX)
            {
                GetComponent<SpriteRenderer>().sprite = animationList[jockDirection + 2];
                directionChangeX = true;
            }
        }
        // move up
        else if (newYPos > transform.position.y)
        {
            stopped = false;
            jockDirection = 3;
            if (directionChangeY)
            {
                GetComponent<SpriteRenderer>().sprite = animationList[jockDirection + 2];
                directionChangeY = false;
            }
        }
        // move down
        else if (newYPos < transform.position.y)
        {
            stopped = false;
            jockDirection = 0;
            if (!directionChangeY)
            {
                GetComponent<SpriteRenderer>().sprite = animationList[jockDirection + 2];
                directionChangeY = true;
            }
        }
        else
        {
            stopped = true;
        }
        transform.position = new Vector2(newXPos, newYPos);
    }


    // makes the players legs move and the player change direction
    public void updateAnimation()
    {
        if (!stopped)
        {
            animTime += Time.deltaTime;
            if (animTime > 2f / animSpeed)
            {
                animTime = 0;
                jockForm += formChangeDirection;
                if (jockForm >= 2 || jockForm <= 0)
                    formChangeDirection *= -1;
                GetComponent<SpriteRenderer>().sprite = animationList[jockDirection + jockForm];
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = animationList[jockDirection + 1];
        }
    }


    /*
        // check for player input
        public void updateMoveInput()
        {
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                move(new Vector2Int(0, 1));
                this.gameObject.GetComponent<SpriteRenderer>().sprite = backFace;
                backMove = true;
                faceUp = true;
                faceLeft = faceRight = faceDown = false;

            }
            else if(Input.GetKeyDown(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                move(new Vector2Int(-1, 0));
                transform.localScale = new Vector3(-1, 1, 1);//Flip when move to left
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sideFace;
                sideMove = true;
                faceLeft = true;
                faceUp = faceRight = faceDown = false;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                move(new Vector2Int(0, -1));
                this.gameObject.GetComponent<SpriteRenderer>().sprite = frontFace;
                faceMove = true;
                faceDown = true;
                faceLeft = faceRight = faceUp = false;
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                move(new Vector2Int(1, 0));
                transform.localScale = new Vector3(1, 1, 1);//Flip when move to right
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sideFace;
                sideMove = true;
                faceRight = true;
                faceLeft = faceUp = faceDown = false;
            }
        }*/



    /*    // this starts a move action. Sets start and destination positions. called once per player key press. 
        public void move(Vector2Int moveAction)
        {
            playerSoundManager.PlayFootsteps();
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
                target = new Vector2(transform.position.x + moveAction.x * dungeonRenderer.cellDimensions, 
                                     transform.position.y + moveAction.y * dungeonRenderer.cellDimensions);
            }
        }*/





    // controls collision with objects and enemies
    // also makes player go behind lower walls
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision");

        if (other.tag == "collection")
        {
            Destroy(other.gameObject);
            switch (other.GetComponent<ObjectBehaviour>().spriteID)
            {
                case 0:
                    if ((playerHealth != 100) || (playerHealth < 100))
                    {
                        playerHealth += 10;
                        playerHealthNum.text = playerHealth.ToString();
                    }
                    break;

                case 1:
                    playerAmmo += 10;
                    playerAmmoNum.text = playerAmmo.ToString();
                    break;

                case 2:
                    TextboxController.UpdateText(NoteDictionary.RandomNote());
                    fadeController.FadeInAndOut();
                    break;
                default:
                    break;
            }
        }
        else if (other.tag == "enemy")
        {
            if (other.GetComponent<EnemyBehaviour>().spriteID == 1)
            {
                playerHealth -= 70;
                playerHealthNum.text = playerHealth.ToString();
                if (playerHealth <= 0)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                //playerHealth -= 45;
                playerHealthNum.text = playerHealth.ToString();
                if (playerHealth <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (other.tag == "horoWall")
        {
            Debug.Log("horoWall");
            if (other.transform.position.y < transform.position.y)
            {
                other.GetComponent<SpriteRenderer>().sortingLayerName = "TopLayer";
            }
        }
        else
        {
            //
        }
    }


    public void objectFunction()
    {
        if (GetComponent<ObjectBehaviour>().spriteID == 0)
        {
            if ((playerHealth != 100) || (playerHealth < 100))
            {

                playerHealth += 5;
                playerHealthNum.text = playerHealth.ToString();
            }
        }
        else
        {
            playerAmmo += 10;
            playerAmmoNum.text = playerAmmo.ToString();
        }
    }



   


}


   

