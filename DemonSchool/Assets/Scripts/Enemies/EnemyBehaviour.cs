using System.Collections;
using System.Collections.Generic;
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
    }

    //check where player is
        //if plyaer is inside these coordinates ( for enemy room)
        //then chasingPlayer();
        //otherwise, guardingRoom();


    public void guardingRoom()
    {
        // randommly run around room
    }

    public void chasingPlayer()
    {
        // chase player

    }


}
