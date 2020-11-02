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
    private Transform joke;

    public Animator eneAnim;

    public GameObject keyPrefeb;

    public bool isBoss, isAttack, BossDie, SpiderDie,SpiderLeft,SpiderRight;

    private SpriteRenderer sr;
    private Color originalColor;
    public float flashTime = 0.25f;

    [SerializeField] private bool isTutorial = false;
    private TutorialManager tutorialManager;

    void Start()
    {
        if(isTutorial)
            tutorialManager = GameObject.FindGameObjectWithTag("manager").GetComponent<TutorialManager>();
        guardingRoom();

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        
    }

    void Update()
    {
        chasingPlayer();
        
        if (this.gameObject.tag == "boss")
        {
            isBoss = true;
            eneAnim.SetBool("isBoss", isBoss);
            eneAnim.SetBool("isAttack", isAttack);
            eneAnim.SetBool("BossDie", BossDie);

            if(isAttack)
                splashDamage();
            
        }
        else if(this.gameObject.tag == "enemy")
        {
            eneAnim.SetFloat("speed", speed);
            eneAnim.SetBool("SpiderDie", SpiderDie);
            eneAnim.SetBool("SpiderLeft", SpiderLeft);
            eneAnim.SetBool("SpiderRight", SpiderRight);
        }

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

        //set sprite and position
        this.spriteID = spriteID;

        if (this.spriteID == 1)
        {
            this.lineOfSite = 8f;
            this.lineOfSite1 = 5.44f;
            this.lineOfSiteBossDmg = 3.7f;
            this.gameObject.tag = "boss";
            isBoss = true;
            this.GetComponent<BoxCollider2D>().size = new Vector2(6.35f, 4.3f);
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0.24f, 0.14f);
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

        enemyHealth = 100;

    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if(this.gameObject.tag == "enemy")
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
                    Destroy(gameObject,2);
                    Instantiate(keyPrefeb, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
                    PlayerBehaviour player = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
                    player.playerScore += 10;
                    player.playerScoreNum.text = player.playerScore.ToString();
                    if (isTutorial)
                        tutorialManager.StartDeathDialogue();
                }
            }
            else if (hit.tag == "Player")
            {
                transform.position = Vector2.MoveTowards(this.transform.position, joke.position, speed * Time.deltaTime);
            }
        }
        else if (this.gameObject.tag == "boss")
        {

            if (hit.tag == "bullet")
            {
                enemyHealth -= 10;

                FlashColor(flashTime);

                if (enemyHealth <= 0)
                {
                    BossDie = true;
                    speed = 0;
                    Destroy(gameObject,2);
                }
            }
            else if (hit.tag == "Player")
            {
                transform.position = Vector2.MoveTowards(this.transform.position, joke.position, speed * Time.deltaTime);
            }
        }

    }

    //check where player is
        //if plyaer is inside these coordinates ( for enemy room)
        //then chasingPlayer();
        //otherwise, guardingRoom();


    public void guardingRoom()// randommly run around room
    {
        joke = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void chasingPlayer()// chase player
    {
        float distanceFromPlayer = Vector2.Distance(joke.position, transform.position);

        if(distanceFromPlayer < lineOfSite)
        {
            if(this.gameObject.tag == "enemy")
            {
                transform.position = Vector2.MoveTowards(this.transform.position, joke.position, speed * Time.deltaTime);

                if (joke.transform.position.x > this.transform.position.x)
                {
                    SpiderRight = true;
                    SpiderLeft = false;
                }
                else if(joke.transform.position.x < this.transform.position.x)
                {
                    SpiderLeft = true;
                    SpiderRight = false;
                }

                if (joke.transform.localPosition.x < this.transform.localPosition.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }

            else if (this.gameObject.tag == "boss")
            {
                transform.position = Vector2.MoveTowards(this.transform.position, joke.position, speed * Time.deltaTime);

                if(distanceFromPlayer < lineOfSite1)
                {
                    isAttack = true;
                }
                else
                {
                    isAttack = false;
                }

                if (joke.transform.localPosition.x < this.transform.localPosition.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
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
        CapsuleCollider2D test = gameObject.GetComponent<CapsuleCollider2D>();
        if ((sr.sprite.ToString().Contains("demon_attack09") || sr.sprite.ToString().Contains("demon_attack10")))
        {
            if (test == null)
            {
                CapsuleCollider2D splashAttack = gameObject.AddComponent<CapsuleCollider2D>();
                splashAttack.direction = CapsuleDirection2D.Horizontal;
                splashAttack.offset = new Vector2(-0.25f, -1.45f);
                splashAttack.size = new Vector2(9.75f, 2.25f);
            }
        }
        else if (test != null)
        {
            Destroy(test);
        }
    }

}
