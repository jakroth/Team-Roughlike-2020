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
    public float lineOfSite, lineOfSite1, lineOfSiteBossDmg;//11
    private Transform joke;

    public Animator eneAnim;

    public GameObject keyPrefeb;

    public bool isBoss, isAttack, BossDie, SpiderDie,SpiderLeft,SpiderRight;

    void Start()
    {
        guardingRoom();
        if(this.gameObject.tag == "boss")
        {
            this.GetComponent<BoxCollider2D>().size = new Vector2(6f, 4f);
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0.13f, -0.9f);
        }
        
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
                enemyHealth -= 30;
                if (enemyHealth <= 0)
                {
                    SpiderDie = true;
                    speed = 0;
                    Destroy(gameObject,2);
                    Instantiate(keyPrefeb, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
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
                }
                else if(joke.transform.position.x > this.transform.position.x)
                {
                    SpiderLeft = true;
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

        Gizmos.color = Color.green;//11
        Gizmos.DrawWireSphere(transform.position, lineOfSiteBossDmg);//11
    }


}
