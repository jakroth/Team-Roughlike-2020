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

    [Header("Player Controls")]
    // this is the speed pf the player
    public float speed;
    public float bounceFactor;
    public int maxLevels = 3;
    public bool isDead = false;
    public bool foundKeys = false;

    [Header("Player Attributes")]
    // Player health, ammo and score settings;
    public int playerHealth;
    public int playerAmmo;
    public int playerScore;
    public int playerLevel;

    public Text playerHealthNum;
    public Text playerAmmoNum;
    public Text playerScoreNum;
    public Text playerLevelNum;

    public int HealthItemID = 0;
    public int AmmoItemID = 1;
    public int ScoreItemID = 2;

    [Header("Player Animation Sprites")]
    // tile set for the Normal Tiles
    public List<Sprite> animationList;

    [Header("Player Animation Attributes")]
    public int jockDirection;
    public int jockForm;
    public int formChangeDirection;
    public float animSpeed;
    public float animTime;
    private SpriteRenderer sprite;

    [Header("Player Movement Attributes")]
    public bool stopped;
    public bool pauseState = false;
    private bool bossInRange = false;
    public bool isFiring = false;
    private Rigidbody2D playerBody;

    [Header("Player Collision Attributes")]
    public float pushSpeed = 3;
    public float pushDistance = 3;
    public bool isPushed = false;
    private Vector2 pushDestination = Vector2.zero;
    private float pushTime = 0;
    private bool directionChangeX = false;
    private bool directionChangeY = false;

    [Header("Other Attributes")]
    public int keyPart = 0;
    private Transform bossDis;
    private SpriteRenderer playerSprite;//11
    private Color originalColor;//11
    public float PlayerFlashTime = 0.25f;//11

    [SerializeField] private bool isTutorial = false;
    private TutorialManager tutorialManager;
    [SerializeField] private Sprite backpackSprite = null;
    [SerializeField] private Sprite studentSprite0 = null;
    [SerializeField] private Sprite studentSprite1 = null;
    [SerializeField] private Sprite keySprite = null;
    [SerializeField] private AudioClip deathSound = null;



    // Start is called before the first frame update
    void Start()
    {
        if (isTutorial)
        {
            tutorialManager = GameObject.FindGameObjectWithTag("manager").GetComponent<TutorialManager>();
            playerLevel = 0;
        }

        pauseState = GameController.instance.GetPauseState(); 
        playerSoundManager = gameObject.GetComponent<PlayerSoundManager>();
        fadeController = GameObject.FindGameObjectWithTag("UI").GetComponent<FadeController>();


        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        originalColor = playerSprite.color;//11


        // load player stats from tutorial, if not tutorial
        if (!isTutorial)
        {
            playerHealth = PlayerStats.health;
            playerAmmo = PlayerStats.ammo;
            playerScore = PlayerStats.score;
            playerLevel = PlayerStats.level;
        }


        // set up player stats on the screen
        playerAmmoNum.text = playerAmmo.ToString();
        playerHealthNum.text = playerHealth.ToString();
        playerScoreNum.text = playerScore.ToString();
        playerLevelNum.text = playerLevel.ToString() + " / " + maxLevels;
    }



    // Update is called once per frame
    void Update()
    {
        pauseState = GameController.instance.GetPauseState();

        if(!pauseState)
            updateAnimation();
            checkPlayer();
        
        if(!pauseState && !bossInRange)
            checkBossMusic(false);
        else if(!pauseState && bossInRange)
            checkBossMusic(true);

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
            if(!isPushed)
                move();
            else
                push();
        }
    }


    // loads next scene if player is dead
    private void checkPlayer()
    {
        if (playerHealth <= 0 && isDead == false)
        {
            isDead = true;
            // update static player stats
            PlayerStats.health = playerHealth;
            PlayerStats.ammo = playerAmmo;
            PlayerStats.score = playerScore;
            PlayerStats.level = playerLevel;

            // load game over scene
            GameObject.Find("GameController").GetComponent<GameController>().UpdatePauseState(true);
            GameObject.Find("MenuPrefab").GetComponent<FadeController>().FadeInAndOut(2f);
            GameObject.Find("GameController").GetComponent<SceneLoader>().LoadScene(3);
        }
        if (keyPart >= 1 && foundKeys == false) //That one will causing final door unfunctional after level 2 :( 
        {
            foundKeys = true;
            GameObject.FindGameObjectWithTag("finalDoor").GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    private void checkBossMusic(bool inRange)
    {
        if(GameObject.FindGameObjectWithTag("boss") != null)
        {
            GameObject boss = GameObject.FindGameObjectWithTag("boss");
            if(!inRange)
            {
                if(Vector2.Distance(gameObject.transform.position, boss.transform.position) < 10)
                {
                    StartCoroutine(FadeAudioSource.StartFade(MusicController.instance.GetAudioSource(), 0.5f, 0f));
                    bossInRange = true;
                    StartCoroutine(BossMusicFade(3));
                }
            }
            else
            {
                if(Vector2.Distance(gameObject.transform.position, boss.transform.position) > 20)
                {
                    StartCoroutine(FadeAudioSource.StartFade(MusicController.instance.GetAudioSource(), 0.5f, 0f));
                    bossInRange = false;
                    StartCoroutine(BossMusicFade(2));
                }
            }
        }

    }

    private IEnumerator BossMusicFade(int index)
    {
        yield return new WaitForSeconds(0.5f);
        MusicController.instance.SetMusic(index);
    }


    
    // make dungeon with pause
    private IEnumerator makeNextDungeonLevel()
    {
        yield return new WaitForSeconds(2f);
        playerLevelNum.text = playerLevel.ToString() + " / " + maxLevels;
        keyPart = 0;
        foundKeys = false;
        GameObject.Find("GameController").GetComponent<GameController>().UpdatePauseState(false);
        GameObject.Find("DungeonManager").GetComponent<DungeonManager>().makeDungeon();
    }


    // checks player move input
    public void move()
    {
        var newXPos = transform.position.x + Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime * speed;
        var newYPos = transform.position.y + Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime * speed;

        // move left
        if (newXPos < transform.position.x)
        {
            stopped = false;
            jockDirection = 15;
            if (directionChangeX)
            {
                playerSprite.sprite = animationList[jockDirection + 2];
                directionChangeX = false;
            }
        }
        // move right
        else if (newXPos > transform.position.x)
        {
            stopped = false;
            jockDirection = 10;
            if (!directionChangeX)
            {
                playerSprite.sprite = animationList[jockDirection + 2];
                directionChangeX = true;
            }
        }
        // move up
        else if (newYPos > transform.position.y)
        {
            stopped = false;
            jockDirection = 5;
            if (directionChangeY)
            {
                playerSprite.sprite = animationList[jockDirection + 2];
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
                playerSprite.sprite = animationList[jockDirection + 2];
                directionChangeY = true;
            }
        }
        else
        {
            stopped = true;
        }
        playerBody.MovePosition(new Vector2(newXPos, newYPos));
    }


    // makes the players legs move and the player change direction
    public void updateAnimation()
    {
        if (isFiring)
        {
            animTime += Time.deltaTime;
            if (animTime > 2f / animSpeed) 
                isFiring = false;
        }
        else if (!stopped)
        {
            animTime += Time.deltaTime;
            if (animTime > 2f / animSpeed)
            {
                animTime = 0;
                jockForm += formChangeDirection;
                if (jockForm >= 2 || jockForm <= 0)
                    formChangeDirection *= -1;
                playerSprite.sprite = animationList[jockDirection + jockForm];
            }
        }
        else
        {
            playerSprite.sprite = animationList[jockDirection + 1];
        }
    }

    public void attackAnimation()
    {
        isFiring = true;
        animTime = 0;
        playerSprite.sprite = animationList[jockDirection + 3];
    }


    public void daggerAttackAnimation()
    {
        isFiring = true;
        animTime = 0;
        playerSprite.sprite = animationList[jockDirection + 4];
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
                    TextboxController.UpdateTextAndImage(NoteDictionary.RandomNote(), backpackSprite);
                    fadeController.FadeInAndOut(5f);
                    break;

                case 1:
                case 2:
                case 3:
                case 4:
                    playerAmmo += 10;
                    playerAmmoNum.text = playerAmmo.ToString();
                    break;

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


        else if (other.tag == "boyStudent" || other.tag == "girlStudent")
        {
            Destroy(other.gameObject);
            TextboxController.UpdateTextAndImage(NoteDictionary.StudentRescue(), other.tag == "boyStudent" ? studentSprite0 : studentSprite1);
            fadeController.FadeInAndOut(1f);
            playerScore += 10;
            PlayerStats.students++;
            playerScoreNum.text = playerScore.ToString();
        }

        // if it's the final door, load the next scene
        if (other.tag == "finalDoor")
        {
            if (keyPart < 1)
            {
                TextboxController.UpdateTextAndImage("You need a key to exit this dungeon level!!", keySprite);
                fadeController.FadeInAndOut(1f);
            }
            else
            {
                Debug.Log("finalDoor");

                // pause game and fade out
                GameObject.Find("GameController").GetComponent<GameController>().UpdatePauseState(true);
                GameObject.Find("MenuPrefab").GetComponent<FadeController>().FadeInAndOut(2f);

                // update static player stats
                PlayerStats.health = playerHealth;
                PlayerStats.ammo = playerAmmo;
                PlayerStats.score = playerScore;
                PlayerStats.level = playerLevel;

                // check if dungeon max level reached
                playerLevel++;
                if (playerLevel > maxLevels)
                {
                    PlayerStats.level = maxLevels;
                    GameObject.Find("GameController").GetComponent<SceneLoader>().LoadScene(4);
                    return;
                }
                else
                {
                    StartCoroutine(makeNextDungeonLevel());
                    return;
                }
            }
            
        }     
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.tag == "enemy")
        {
            if (other.GetComponent<EnemyBehaviour>().enemyHealth > 0)
            {
                playerHealth -= 20;
                FlashColor();  //11
                playerHealthNum.text = playerHealth.ToString();
                pushBack(collision.gameObject);
            }

        }

        else if (other.tag == "boss")
        {
            if (other.GetComponent<EnemyBehaviour>().enemyHealth > 0)
            {
                playerHealth -= 30;
                FlashColor();
                playerHealthNum.text = playerHealth.ToString();
                pushBack(other);
                FlashColor();
            }
        }

        // if it's the final door, check keys and display message
        else if (other.tag == "finalDoor")
        {
            if (keyPart < 2)
            {
                TextboxController.UpdateTextAndImage("You need a keys to exit this dungeon level!!", keySprite);
                fadeController.FadeInAndOut(1f);
            }
        }
    }

    public void pushBack(GameObject other)
    {
        isPushed = true;
        float distance = Vector2.Distance(transform.position, other.transform.position);
        float deltaX = (other.transform.position.x - transform.position.x)/distance;
        float deltaY = (other.transform.position.y - transform.position.y)/distance;
        pushDestination = new Vector2(transform.position.x - (deltaX * pushDistance), transform.position.y - (deltaY * pushDistance));
        pushTime = 0f;
        push();
    }


    private void push()
    {
        if (pushTime < 0.15f)
        {
            Vector2 pos = Vector2.MoveTowards(transform.position, pushDestination, pushSpeed * Time.fixedDeltaTime);
            playerBody.MovePosition(pos);
            pushTime += Time.deltaTime;
        }
        else
            isPushed = false;
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

    // why this function?
    public void BossSkill()
    {
        playerHealth -= 30;
        playerHealthNum.text = playerHealth.ToString();

        if (playerHealth <= 0)
        {
            Camera.main.GetComponent<AudioSource>().PlayOneShot(deathSound);
            GameObject.Find("GameController").GetComponent<SceneLoader>().LoadNextScene();
        }
    }

    void FlashColor()//11
    {
        Debug.Log("flash player");
        playerSprite.color = Color.red;
        Invoke("ResetColor", PlayerFlashTime);
    }

    void ResetColor()//11
    {
        playerSprite.color = originalColor;
    }


    public void restartLevel()
    {
        StartCoroutine(restartLevelRoutine());
    }

    private IEnumerator restartLevelRoutine()
    {
        GameObject.Find("GameController").GetComponent<GameController>().UpdatePauseState(true);
        GameObject.Find("MenuPrefab").GetComponent<FadeController>().FadeInAndOut(2f);
        keyPart = 0;
        yield return new WaitForSeconds(2f);
        GameObject.Find("GameController").GetComponent<GameController>().UpdatePauseState(false);
        GameObject.Find("DungeonManager").GetComponent<DungeonManager>().makeDungeon();
    }






    // *********LEGACY COLLISION CODE, NOT USED ANYMORE***********

    // for collisions NOT USED ANYMORE
    //private RaycastHit2D[] rayArray = new RaycastHit2D[8];
    //private float laserLength = 0.4f;
    //private CircleCollider2D coll;

    //coll = GetComponent<CircleCollider2D>();


    // this is only used for door collisions now, used to be all physics collisions
    //rayCollisionChecking();


    // checks for collisions around the player, and bounces the player away if they get too close
    /*private void rayCollisionChecking()
    {
        // Create the directions for the rays
        for (int i = 0; i < 8; i++)
        {
            Vector2 direction = Vector2.zero;
            switch (i)
            {
                case 0:
                    direction = new Vector2(1, 0);
                    laserLength = 0.4f;
                    break;
                case 1:
                    direction = new Vector2(0.7f, 0.8f);
                    laserLength = 0.55f;
                    break;
                case 2:
                    direction = new Vector2(0, 1);
                    laserLength = 0.6f;
                    break;
                case 3:
                    direction = new Vector2(0.7f, -0.8f);
                    laserLength = 0.55f;
                    break;
                case 4:
                    direction = new Vector2(0, -1);
                    laserLength = 0.4f;
                    break;
                case 5:
                    direction = new Vector2(-0.7f, -0.8f);
                    laserLength = 0.55f;
                    break;
                case 6:
                    direction = new Vector2(-1, 0);
                    laserLength = 0.4f;
                    break;
                case 7:
                    direction = new Vector2(-0.7f, 0.8f);
                    laserLength = 0.55f;
                    break;

            }
            // draw the ray for debugging
            Debug.DrawRay(coll.bounds.center, direction * laserLength, Color.red);

            // send out the ray
            rayArray[i] = Physics2D.Raycast(coll.bounds.center, direction, laserLength, LayerMask.GetMask("Wall"));
            if (rayArray[i].collider != null)
            {
                // check if the ray hits a door
                if (rayArray[i].collider.gameObject.tag == "door")
                {
                    // if it's the final door, load the next scene
                    if (rayArray[i].collider.gameObject.GetComponent<DungeonTile>().isFinalDoor)
                    {
                        if (keyPart < 2)
                        {
                            TextboxController.UpdateTextAndImage("You need 2 keys to exit this dungeon level!!", keySprite);
                            fadeController.FadeInAndOut(1f);
                            //moveAway(rayArray[i]);
                        }
                        else
                        {
                            Debug.Log("finalDoor");

                            // pause game and fade out
                            GameObject.Find("GameController").GetComponent<GameController>().UpdatePauseState(true);
                            GameObject.Find("MenuPrefab").GetComponent<FadeController>().FadeInAndOut(2f);

                            // update static player stats
                            PlayerStats.health = playerHealth;
                            PlayerStats.ammo = playerAmmo;
                            PlayerStats.score = playerScore;
                            PlayerStats.level = playerLevel;

                            // check if dungeon max level reached
                            playerLevel++;
                            if (playerLevel > maxLevels)
                            {
                                PlayerStats.level = maxLevels;
                                GameObject.Find("GameController").GetComponent<SceneLoader>().LoadScene(4);
                                return;
                            }
                            else
                            {
                                StartCoroutine(makeNextDungeonLevel());
                                return;
                            }
                        }
                    }
                    //else
                    //moveAway(rayArray[i]);
                }
                // otherwise move away from wall
                //else
                //moveAway(rayArray[i]);
            }
        }
    }
    
     
    // the bounce function
    public void moveAway(RaycastHit2D r)
    {
        float deltaX = r.point.x - coll.bounds.center.x;
        float deltaY = r.point.y - coll.bounds.center.y;

        transform.position = new Vector2(transform.position.x - deltaX / bounceFactor, transform.position.y - deltaY / bounceFactor);
    }
     
     
     */
}




