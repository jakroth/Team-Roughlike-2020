using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    // for grabbing these components from this object or the DungeonManager instance
    private DungeonManager dungeonManager;
    private DungeonGenerator dungeonGenerator;
    private DungeonRenderer dungeonRenderer;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collisionBox;

    // the tileID for this tile
    public int spriteID;
    // the location of this tile in the map
    public Vector2Int pos;
    // if this tile is a collision tile
    public bool isCollision;
    // is this tile is the final door
    public bool isFinalDoor = false;


    // set up the tile
    public void setTile(int spriteID, Vector2Int pos, string layer, bool isCollision, bool oldTiles)
    {
        // make sure these links exist
        if (dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }
        if (dungeonGenerator == null)
        {
            dungeonGenerator = GameObject.Find("DungeonManager").GetComponent<DungeonGenerator>();
        }
        if (dungeonRenderer == null)
        {
            dungeonRenderer = GameObject.Find("DungeonManager").GetComponent<DungeonRenderer>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (collisionBox == null)
        {
            collisionBox = GetComponent<BoxCollider2D>();
        }

        // set up this tile with tileID and position
        this.spriteID = spriteID;
        this.pos = pos;
        this.isCollision = isCollision;
        collisionBox.enabled = isCollision;

        // give the tile a name in the Hierarchy
        gameObject.name = "Pos (" + pos.x + "," + pos.y + "); mapID: " + dungeonGenerator.map[pos.x,pos.y] + (isCollision ? ", Coll; " : ", NColl; ") + "spriteID: " + spriteID + "; L: " + layer;

     
        // render the tile with the correct sprite, and check if Hell tile or Normal tile
        if (dungeonManager.hellTiles)
        {
            if (oldTiles && isCollision)
                spriteRenderer.sprite = dungeonRenderer.wallTileTextures[spriteID];
            else if (oldTiles)
                spriteRenderer.sprite = dungeonRenderer.floorTileTextures[spriteID];
            else
                spriteRenderer.sprite = dungeonRenderer.hellTiles[spriteID];
        }
        spriteRenderer.sortingLayerName = layer;
    }

    // set up the tile
    public void setLayer(string layer)
    {
        spriteRenderer.sortingLayerName = layer;
    }


}
