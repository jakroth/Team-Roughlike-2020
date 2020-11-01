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
    public void setTile(int spriteID, Vector2Int pos, string sortingLayer, bool isCollision, int gameLayer, string tag)
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
        gameObject.name = "Pos (" + pos.x + "," + pos.y + "); mapID: " + dungeonGenerator.map[pos.x,pos.y] + (isCollision ? ", Coll; " : ", NColl; ") + "spriteID: " + spriteID + "; L: " + sortingLayer;

     
        // render the tile with the correct sprite, and check if Hell tile or Normal tile
        if (dungeonManager.hellTiles)
            spriteRenderer.sprite = dungeonRenderer.hellTiles[spriteID];
        else
            spriteRenderer.sprite = dungeonRenderer.normalTiles[spriteID];


        // set the layers and tags
        gameObject.layer = gameLayer;
        gameObject.tag = tag;
        spriteRenderer.sortingLayerName = sortingLayer;

        
        // set the collider dimensions
        switch (spriteID)
        {
            // Corners, Horos and Verts
            case 0:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.41f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.8f);
                BoxCollider2D newBC0 = gameObject.AddComponent<BoxCollider2D>();
                newBC0.offset = new Vector2(0f, -0.76f);
                newBC0.size = new Vector2(0.36f, 0.5f);
                newBC0.isTrigger = true;
                break;
            case 1:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-0.41f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.8f);
                BoxCollider2D newBC1 = gameObject.AddComponent<BoxCollider2D>();
                newBC1.offset = new Vector2(0f, -0.75f);
                newBC1.size = new Vector2(0.36f, 0.53f);
                newBC1.isTrigger = true;
                break;
            case 2:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.41f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.8f);
                BoxCollider2D newBC2 = gameObject.AddComponent<BoxCollider2D>();
                newBC2.offset = new Vector2(0f, 0.65f);
                newBC2.size = new Vector2(0.36f, 0.72f);
                newBC2.isTrigger = true;
                break;
            case 3:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-0.41f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.8f);
                BoxCollider2D newBC3 = gameObject.AddComponent<BoxCollider2D>();
                newBC3.offset = new Vector2(0f, 0.65f);
                newBC3.size = new Vector2(0.36f, 0.72f);
                newBC3.isTrigger = true;
                break;
            case 4:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.8f);
                break;
            case 5:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                break;


            // T-Joins
            case 6:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.8f);
                BoxCollider2D newBC6 = gameObject.AddComponent<BoxCollider2D>();
                newBC6.offset = new Vector2(0f, -0.76f);
                newBC6.size = new Vector2(0.36f, 0.5f);
                newBC6.isTrigger = true;
                break;
            case 7:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.8f);
                BoxCollider2D newBC7 = gameObject.AddComponent<BoxCollider2D>();
                newBC7.offset = new Vector2(0f, 0.65f);
                newBC7.size = new Vector2(0.36f, 0.72f);
                newBC7.isTrigger = true;
                break;
            case 8:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                BoxCollider2D newBC8 = gameObject.AddComponent<BoxCollider2D>();
                newBC8.offset = new Vector2(0.59f, -0.1f);
                newBC8.size = new Vector2(0.82f, 0.8f);
                newBC8.isTrigger = true;
                break;
            case 9:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                BoxCollider2D newBC9 = gameObject.AddComponent<BoxCollider2D>();
                newBC9.offset = new Vector2(-0.59f, -0.1f);
                newBC9.size = new Vector2(0.82f, 0.8f);
                newBC9.isTrigger = true;
                break;
            case 10:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                BoxCollider2D newBC10 = gameObject.AddComponent<BoxCollider2D>();
                newBC10.offset = new Vector2(0f, -0.1f);
                newBC10.size = new Vector2(2f, 0.8f);
                newBC10.isTrigger = true;
                break;


            // DOORS
            case 11:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.8f);
                break;
            case 12:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.8f);
                break;
            case 13:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                break;
            case 14:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                break;
            case 15:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 1f);
                break;
            case 16:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 1f);
                break;


            // Wall Shadows
            case 17:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 1f);
                break;
            case 18:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.41f, 1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 1f);
                break;
            case 19:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-0.41f, 1f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 1f);
                break;


            // Vertical Wall Ends
            case 20:
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0.25f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 1.5f);
                break;
            case 21:
                // no collider
                break;


            // Floor and Fire
            case 22:
                break;
            case 23:
                break;
            case 24:
                break;
            case 25:
                break;

        }

    }


}
