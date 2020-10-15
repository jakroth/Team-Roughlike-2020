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
    public float lineOfSite;
    private Transform joke;

    public Animator eneAnim;
    //public bool eneUpMove, eneRightMove, eneLeftMove, eneDownMove;




    void Start()
    {
        guardingRoom();
        //eneUpMove = eneRightMove = eneLeftMove = eneDownMove = false;
    }

    void Update()
    {
        chasingPlayer();
        eneAnim.SetFloat("isMove", speed);
        /*eneAnim.SetBool("eneLeftMove", eneLeftMove);
        eneAnim.SetBool("eneRightMove", eneRightMove);
        eneAnim.SetBool("eneDownMove", eneDownMove);
        eneAnim.SetBool("eneUpMove", eneUpMove);*/
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
        this.pos = pos;

        // update sprite on enemy
        spriteRenderer.sprite = enemyGenerator.enemySprites[spriteID];

        // give the enemy a name in the Hierarchy
        gameObject.name = "Enemy (" + pos.x + "," + pos.y + "): " + spriteID;

        enemyHealth = 100;

    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.tag == "bullet")
        {
            enemyHealth -= 30;
            if (enemyHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
        else if(hit.tag == "Player")
        {
            transform.position = Vector2.MoveTowards(this.transform.position, joke.position, speed * Time.deltaTime);
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
            transform.position = Vector2.MoveTowards(this.transform.position, joke.position, speed * Time.deltaTime);
            if (joke.transform.localPosition.x < this.transform.localPosition.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else {
                transform.localScale = new Vector3(1, 1, 1);
            }
            /*if (joke.transform.localPosition.x < this.transform.localPosition.x)
            {
                eneLeftMove = true;

            }
            else if (joke.transform.localPosition.x > this.transform.localPosition.x)
            {
                eneRightMove = true;

            }
            else if (joke.transform.localPosition.y < this.transform.localPosition.y)
            {
                eneDownMove = true;

            }
            else if (joke.transform.localPosition.y > this.transform.localPosition.y)
            {
                eneUpMove = true;

            }
            else
            {
                eneUpMove = eneRightMove = eneLeftMove = eneDownMove = false;
            }*/
        }



    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
    }


}
