using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayerBehaviour : MonoBehaviour
{

    // this is the time we want the player to take to move 1 tile. 
    // it is set in the Unity Inspector
    public float speed;

    //needs access to the PlayerSoundManager to play audio
    private PlayerSoundManager playerSoundManager;

    //needs access to the FadeController to fade the notes in and out
    private FadeController fadeController;

    //Player health setting;
    public int playerHealth;

    //Player ammo;
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
    float laserLength = 0.4f;
    private CircleCollider2D coll;

    //Collider2D nearestWall;


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
    }


    void FixedUpdate()
    {
        rayCollisionChecking();
        move();
    }


    private void rayCollisionChecking()
    {
        float shortestDistance = 100;
        RaycastHit2D closestHit = new RaycastHit2D();

        //Get the closest object hit by the rays
        for(int i = 0; i < 4; i++)
        {
            // work out directions (0,1 or -1,0, etc)
            Vector2 direction = new Vector2(i % 2 * Mathf.Pow(-1,i%3), (i + 1) % 2 * Mathf.Pow(-1, (i+1)%3));
            //Debug.Log(direction);
            Debug.DrawRay(coll.bounds.center, direction * laserLength, Color.red);

            // send out the ray
            rayArray[i] = Physics2D.Raycast(coll.bounds.center, direction, laserLength,LayerMask.GetMask("Wall"));
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
        Debug.Log(shortestDistance);
        if (closestHit.collider != null)
            moveAway(closestHit);
    }


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
        else if(newXPos > transform.position.x)
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


    public void moveAway(RaycastHit2D r)
    {
        float deltaX = r.point.x - coll.bounds.center.x;
        float deltaY = r.point.y - coll.bounds.center.y;

        transform.position = new Vector2(transform.position.x - deltaX/3, transform.position.y - deltaY/3);
    }



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


    // stop the character from walking through walls
    // loads a new map if collision is with the final door
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("collision");
        // stop moving forward any more on collision

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
            //

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
            if(other.transform.position.y < transform.position.y)
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


   

