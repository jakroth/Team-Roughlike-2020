using JetBrains.Annotations;
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
    public float bounceFactor;
    public int maxLevels = 3;

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
    private RaycastHit2D[] rayArray = new RaycastHit2D[8];
    private float laserLength = 0.4f;
    private CircleCollider2D coll;



    public int keyPart = 0;

    private Transform bossDis;

    private SpriteRenderer srP;//11
    private Color originalColor;//11
    public float PlayerFlashTime = 0.25f;//11

    [SerializeField] private bool isTutorial = false;
    private TutorialManager tutorialManager;
    [SerializeField] private Sprite backpackSprite = null;
    [SerializeField] private Sprite studentSprite = null;



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

        coll = GetComponent<CircleCollider2D>();
        srP = GetComponent<SpriteRenderer>();//11
        originalColor = srP.color;//11

        // load player stats from tutorial, if not tutorial
        if (!isTutorial)
        {
            playerHealth = PlayerStats.health;
            playerAmmo = PlayerStats.ammo;
            playerScore = PlayerStats.score;
            playerLevel = PlayerStats.level;
        }


        // set up player stats
        playerAmmoNum.text = playerAmmo.ToString();
        playerHealthNum.text = playerHealth.ToString();
        playerScoreNum.text = playerScore.ToString();
        playerLevelNum.text = playerLevel.ToString();
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
                        // update static player stats
                        PlayerStats.health = playerHealth;
                        PlayerStats.ammo = playerAmmo;
                        PlayerStats.score = playerScore;
                        PlayerStats.level = playerLevel;  // increment level

                        // handle any transitions needed
                        if (!isTutorial)
                        {
                            Debug.Log("finalDoor");
                            playerLevel++;
                            // check if dungeon max level reached
                            if (playerLevel > maxLevels)
                            {
                                PlayerStats.level = maxLevels;
                                GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadScene(4);
                                return;
                            }
                            else
                            {   // TODO: handle this better with a Coroutine, as in the the tutorial
                                playerLevelNum.text = playerLevel.ToString();
                                GameObject.Find("DungeonManager").GetComponent<DungeonManager>().makeDungeon();
                                return;
                            }
                        }
                        else
                        {
                            tutorialManager.StartFinalTransition();
                        }
                    }
                    else
                        moveAway(rayArray[i]);
                }
                // otherwise move away from wall
                else
                    moveAway(rayArray[i]);
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
                    TextboxController.UpdateTextAndImage(NoteDictionary.RandomNote(), backpackSprite);
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
            TextboxController.UpdateTextAndImage(NoteDictionary.StudentRescue(), studentSprite);
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
                // update static player stats
                PlayerStats.health = playerHealth;
                PlayerStats.ammo = playerAmmo;
                PlayerStats.score = playerScore;
                PlayerStats.level = playerLevel;

                // load game over scene
                GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadNextScene();
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
                       
                        GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadNextScene();
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

        else if (other.tag == "horoWall" || other.tag == "door")
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
            GameObject.Find("SceneLoader").GetComponent<SceneLoader>().LoadNextScene();
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


   

