using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    private DungeonManager dungeonManager;
    public int tileID;
    public Vector2Int pos;
    public bool isCollision;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collisionBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTile(int tileID, Vector2Int pos)
    {
        if(dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if(collisionBox == null)
        {
            collisionBox = GetComponent<BoxCollider2D>();
        }

        this.tileID = tileID;
        this.pos = pos;
        this.isCollision = tileID == 2 ? true : false;
        gameObject.name = "Tile (" + pos.x + "," + pos.y + "): " + tileID;

        spriteRenderer.sprite = dungeonManager.tileTextures[tileID];

        if(isCollision)
            collisionBox.enabled = true;
        else
            collisionBox.enabled = false;

    }
}
