using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // for grabbing these components from this object or the DungeonManager instance
    private EnemyGenerator enemyGenerator;
    private SpriteRenderer spriteRenderer;

    // the spriteID for this enemy
    public int spriteID;

    // the location of this enemy on the map
    public Vector2Int pos;

    public int enemyHealth;

    public float speed;
    public float lineOfSite, lineOfSite1, lineOfSiteBossDmg;
    private GameObject jock;

    public Animator eneAnim;

    public GameObject keyPrefeb;

    public bool isBoss, isAttack, BossDie, SpiderDie,SpiderLeft,SpiderRight;

    private SpriteRenderer sr;
    private Color originalColor;
    public float flashTime = 0.25f;

    private Rigidbody2D enemyBody;
    private Vector2 newPosition;
    private Vector2 originalPosition;

    [SerializeField] private bool isTutorial = false;
    private TutorialManager tutorialManager;
    private GameController gameController;
    private DungeonGenerator dungeonGen;

    void Start()
    {
        // set up the managers
        if(isTutorial)
            tutorialManager = GameObject.FindGameObjectWithTag("manager").GetComponent<TutorialManager>();
        else
            gameController = GameObject.Find("GameController").GetComponent<GameController>();

        // find the player & dungeon
        jock = GameObject.FindGameObjectWithTag("Player");
        dungeonGen = GameObject.Find("DungeonManager").GetComponent<DungeonGenerator>();

        // set up the sprite colours
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        //set initial position
        originalPosition = transform.position;
        newPosition = transform.position;

        enemyBody = GetComponent<Rigidbody2D>();

        // set initial behaviour
        guardingRoom();

    }

    void Update()
    {
        if (!GameController.instance.GetPauseState())
        {
            chasingPlayer();

            if (this.gameObject.tag == "boss")
            {
                isBoss = true;
                eneAnim.SetBool("isBoss", isBoss);
                eneAnim.SetBool("isAttack", isAttack);
                eneAnim.SetBool("BossDie", BossDie);

                if (isAttack)
                    splashDamage();

            }
            else if (this.gameObject.tag == "enemy")
            {
                eneAnim.SetFloat("speed", speed);
                eneAnim.SetBool("SpiderDie", SpiderDie);
                eneAnim.SetBool("SpiderLeft", SpiderLeft);
                eneAnim.SetBool("SpiderRight", SpiderRight);
            }
        }
    }


    void FixedUpdate()
    {
        enemyBody.MovePosition(newPosition);
    }


    public void setEnemy(int spriteID, Vector2Int pos)
    {
        // make sure these links exist
        if (enemyGenerator == null)
        {
            enemyGenerator = GameObject.Find("DungeonManager").GetComponent<EnemyGenerator>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        enemyHealth = 100;

        //set sprite and position
        this.spriteID = spriteID;

        if (this.spriteID == 1)
        {
            isBoss = true;
            this.lineOfSite = 10f;
            this.lineOfSite1 = 5.44f;
            this.lineOfSiteBossDmg = 3.7f;
            this.gameObject.tag = "boss";
            this.GetComponent<CapsuleCollider2D>().offset = new Vector2(0.18f, -0.17f);
            this.GetComponent<CapsuleCollider2D>().size = new Vector2(7f, 4.75f);
            enemyHealth += (PlayerStats.level * 50); // level 1 boss has 150 health, level 2 has 200, level 3 has 250, etc. 
        }
        else if (this.spriteID == 0)
        {
            this.gameObject.tag = "enemy";
        }

        this.pos = pos;

        // update sprite on enemy
        spriteRenderer.sprite = enemyGenerator.enemySprites[spriteID];

        // give the enemy a name in the Hierarchy
        gameObject.name = "Enemy (" + pos.x + "," + pos.y + "): " + spriteID;

    }


    private void OnTriggerEnter2D(Collider2D hit)
    {
        if(gameObject.tag == "enemy")
        {
            if (hit.tag == "bullet")
            {
                if (!SpiderDie)
                {
                    enemyHealth -= 30;
                    FlashColor(flashTime);
                }

                if (enemyHealth <= 0 && !SpiderDie)
                {
                    SpiderDie = true;
                    speed = 0;
                    GetComponent<CapsuleCollider2D>().enabled = false;
                    Destroy(gameObject,2);
                    Instantiate(keyPrefeb, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
                    PlayerBehaviour player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
                    player.playerScore += 10;
                    player.playerScoreNum.text = player.playerScore.ToString();
                    if (isTutorial)
                        tutorialManager.StartDeathDialogue();
                }
            }
            else if (hit.tag == "Player")
            {
                transform.position = Vector2.MoveTowards(transform.position, jock.transform.position, speed * Time.deltaTime);
            }
        }
        else if (gameObject.tag == "boss")
        {

            if (hit.tag == "bullet")
            {
                if (!BossDie)
                {
                    enemyHealth -= 10;
                    FlashColor(flashTime);
                }

                if (enemyHealth <= 0 & !BossDie)
                {
                    BossDie = true;
                    speed = 0;
                    GetComponent<CapsuleCollider2D>().enabled = false;
                    Destroy(gameObject,2);
                    Vector2 key1 = transform.position;
                    Vector2 key2 = transform.position;
                    key1.x -= 0.2f;
                    key2.x += 0.2f;
                    Instantiate(keyPrefeb, key1, new Quaternion(0, 0, 0, 0));
                    Instantiate(keyPrefeb, key2, new Quaternion(0, 0, 0, 0));
                    PlayerBehaviour player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
                    player.playerScore += 100;
                    player.playerScoreNum.text = player.playerScore.ToString();
                }
            }
            else if (hit.tag == "Player")
            {
                transform.position = Vector2.MoveTowards(transform.position, jock.transform.position, speed * Time.deltaTime);
            }
        }

    }

    //check where player is
        //if plyaer is inside these coordinates ( for enemy room)
        //then chasingPlayer();
        //otherwise, guardingRoom();


    public void guardingRoom()// randommly run around room
    {
        //
    }


    public void chasingPlayer()// chase player
    {
        float distanceFromPlayer = Vector2.Distance(jock.transform.position, transform.position);

        if (this.gameObject.tag == "enemy")
        {
            if (distanceFromPlayer < lineOfSite)
            {
                // move towards player
                newPosition = Vector2.MoveTowards(this.transform.position, jock.transform.position, speed * Time.deltaTime);

                if (jock.transform.transform.position.x > this.transform.position.x)
                {
                    SpiderRight = true;
                    SpiderLeft = false;
                }
                else if (jock.transform.position.x < this.transform.position.x)
                {
                    SpiderLeft = true;
                    SpiderRight = false;
                }

                if (jock.transform.localPosition.x < this.transform.localPosition.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                // move back to original position
                newPosition = Vector2.MoveTowards(this.transform.position, originalPosition, speed * Time.deltaTime);
            }
        }
            //getPlayerTile();

        else if (this.gameObject.tag == "boss")
        {
            // if player is in Boss room
            if (getPlayerTile() == 1)
            {
                // move towards player
                newPosition = Vector2.MoveTowards(this.transform.position, jock.transform.position, speed * Time.deltaTime);

                if (distanceFromPlayer < lineOfSite1)
                {
                    isAttack = true;
                }
                else
                {
                    isAttack = false;
                }

                if (jock.transform.localPosition.x < this.transform.localPosition.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else
            {
                // move back to original position
                newPosition = Vector2.MoveTowards(this.transform.position, originalPosition, speed * Time.deltaTime);
            }
        }
        
    }

   

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lineOfSite1);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSiteBossDmg);
    }

    void FlashColor(float flashTime)
    {
        sr.color = Color.red;
        Invoke("ResetColor", flashTime);
    }

    void ResetColor()
    {
        sr.color = originalColor;   
    }


    void splashDamage() {
        BoxCollider2D test = gameObject.GetComponent<BoxCollider2D>();
        if ((sr.sprite.ToString().Contains("demon_attack09") || sr.sprite.ToString().Contains("demon_attack10")))
        {
            if (test == null)
            {
                BoxCollider2D splashAttack = gameObject.AddComponent<BoxCollider2D>();
                //splashAttack.direction = CapsuleDirection2D.Horizontal;
                splashAttack.offset = new Vector2(-0.25f, -1.45f);
                splashAttack.size = new Vector2(9.75f, 2.25f);
            }
        }
        else if (test != null)
        {
            Destroy(test);
        }
    }


    int getPlayerTile()
    {
        int xCoor = (int)jock.transform.position.x;
        int yCoor = (int)jock.transform.position.y;

        xCoor = (xCoor % 2 == 0) ? (xCoor / 2) : (xCoor / 2 + 1);

        yCoor = (yCoor % 2 == 0) ? (yCoor / 2) : (yCoor / 2 + 1);

        int mapID = dungeonGen.map[xCoor, yCoor];
        //Debug.Log(mapID);

        return mapID;
    }


}
