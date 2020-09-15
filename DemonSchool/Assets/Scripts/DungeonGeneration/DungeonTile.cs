using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    // for grabbing these components from this object or the DungeonManager instance
    private DungeonRenderer dungeonRenderer;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collisionBox;

    // the tileID for this tile
    public int tileID;
    // the location of this tile in the map
    public Vector2Int pos;
    // if this tile is a collision tile
    public bool isCollision;
    // if this tile is a door or wall tile
    public bool isDoorOrWall;
    // is this tile is the final door
    public bool isFinalDoor = false;


    // set up the tile
    public void setTile(int tileID, Vector2Int pos, bool isDoorOrWall)
    {
        // make sure these links exist
        if(dungeonRenderer == null)
        {
            dungeonRenderer = GameObject.Find("DungeonManager").GetComponent<DungeonRenderer>();
        }
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if(collisionBox == null)
        {
            collisionBox = GetComponent<BoxCollider2D>();
        }

        // set up this tile with tileID and position
        this.tileID = tileID;
        this.pos = pos;
        this.isDoorOrWall = isDoorOrWall;


        // give the tile a name in the Hierarchy
        gameObject.name = "Tile (" + pos.x + "," + pos.y + "): " + (isDoorOrWall ? "Wall, " : "Floor, ") + tileID;


        // render the sprite with either a door/wall tile or floor tile
        // and enable the collision mechanics for walls
        if (isDoorOrWall)
        {
            spriteRenderer.sprite = dungeonRenderer.wallTileTextures[tileID];
            collisionBox.enabled = true;
        }
        else
        {
            spriteRenderer.sprite = dungeonRenderer.floorTileTextures[tileID];
            collisionBox.enabled = false;
        }

    }

}
