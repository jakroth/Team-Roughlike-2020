﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    //needs access to the PlayerSoundManager to play audio
    private PlayerSoundManager playerSoundManager;

    //needs access to the FadeController to fade the notes in and out
    private FadeController fadeController;

    [Header("Player Attributes")]
    // this is the speed pf the player
    public float speed;
    public float bounce;

    // Player health, ammo and score settings;
    public int playerHealth;
    public int playerAmmo;
    public int playerScore;

    public Text playerHealthNum;
    public Text playerAmmoNum;
    public Text playerScoreNum;

    public int HealthItemID = 0;
    public int AmmoItemID = 1;
    public int ScoreItemID = 2;

    [Header("Player Animation Sprites")]
    // tile set for the Normal Tiles
    public List<Sprite> animationList;
    public int jockDirection;
    public int jockForm;
    public int formChangeDirection;
    public float animSpeed;
    public float animTime;
    public bool stopped;
    public bool pauseState = false;
    private bool directionChangeX = false;
    private bool directionChangeY = false;


    // for collisions
    private RaycastHit2D[] rayArray = new RaycastHit2D[4];
    private float laserLength = 0.4f;
    private CircleCollider2D coll;

    public int keyPart = 0;

    private Transform bossDis;

    private SpriteRenderer srP;//11
    private Color originalColor;//11
    public float PlayerFlashTime = 0.25f;//11

    // Start is called before the first frame update
    void Start()
    {       
        pauseState = GameController.instance.GetPauseState(); 
        playerSoundManager = gameObject.GetComponent<PlayerSoundManager>();
        fadeController = GameObject.FindGameObjectWithTag("UI").GetComponent<FadeController>();
        coll = GetComponent<CircleCollider2D>();

        srP = GetComponent<SpriteRenderer>();//11
        originalColor = srP.color;//11

        // set up player stats
        if (!pauseState)
        {
            playerAmmoNum.text = playerAmmo.ToString();
            playerHealthNum.text = playerHealth.ToString();
            playerScoreNum.text = playerScore.ToString();
        }
    }



    // Update is called once per frame
    void Update()
    {
        pauseState = GameController.instance.GetPauseState();
        if(!pauseState)
            updateAnimation();
        if (!stopped && !pauseState)
            playerSoundManager.PlayFootsteps();
        else
            playerSoundManager.EndFootsteps();

        

    }


    // FixedUpdate is called in line with the physics engine. Can be set in the project settings. 
    void FixedUpdate()
    {
        if(!pauseState)
        {
            rayCollisionChecking();
            move();
        }
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

        transform.position = new Vector2(transform.position.x - deltaX / bounce, transform.position.y - deltaY / bounce);
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
                    TextboxController.UpdateText(NoteDictionary.RandomNote());
                    fadeController.FadeInAndOut(5f);
                    break;

                case 1:
                case 2:
                case 3:
                    playerAmmo += 10;
                    playerAmmoNum.text = playerAmmo.ToString();
                    break;

                case 4:
                case 5:
                case 6:
                    if ((playerHealth != 100) || (playerHealth < 100))
                    {
                        playerHealth += 10;
                        playerHealthNum.text = playerHealth.ToString();
                    }
                    break;
                default:
                    break;
            }
        }
        else if (other.tag == "key")
        {
            Destroy(other.gameObject);
            keyPart += 1;
        }
        else if (other.tag == "student")
        {
            Destroy(other.gameObject);
            TextboxController.UpdateText(NoteDictionary.StudentRescue());
            fadeController.FadeInAndOut(1f);
            playerScore += 10;
            playerScoreNum.text = playerScore.ToString();
        }
        else if (other.tag == "enemy")
        {
            if (other.GetComponent<EnemyBehaviour>().enemyHealth > 0)
            {
                playerHealth -= 45;
                FlashColor(PlayerFlashTime);//11
                playerHealthNum.text = playerHealth.ToString();
            }

            if (playerHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if (other.tag == "boss")
        {
            bossDis = GameObject.FindGameObjectWithTag("boss").transform;
            float distanceFromBoss = Vector2.Distance(bossDis.position, GameObject.FindGameObjectWithTag("Player").transform.position);

            if (distanceFromBoss <= other.GetComponent<EnemyBehaviour>().lineOfSiteBossDmg) {

                if (other.GetComponent<EnemyBehaviour>().enemyHealth > 0)
                {
                    playerHealth -= 5;
                    FlashColor(PlayerFlashTime);//11
                    playerHealthNum.text = playerHealth.ToString();

                    if (playerHealth <= 0)
                    {
                        Destroy(gameObject);
                    }

                    Invoke("BossSkill", 1.06f);
                    FlashColor(PlayerFlashTime);//11
                    FlashColor(PlayerFlashTime);//11
                }
            }

            if (playerHealth <= 0)
            {
                Destroy(gameObject);
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


    public void BossSkill()
    {
        playerHealth -= 30;
        playerHealthNum.text = playerHealth.ToString();

        if (playerHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void FlashColor(float flashTime)//11
    {
        srP.color = Color.red;
        Invoke("ResetColor", flashTime);
    }

    void ResetColor()//11
    {
        srP.color = originalColor;
    }

}


   

