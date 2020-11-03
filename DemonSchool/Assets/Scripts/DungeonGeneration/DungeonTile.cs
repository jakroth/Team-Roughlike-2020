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
            case 0: // Wall_Top_Left
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.41f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.388f);
                break;
            case 1: // Wall_Top_Right
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-0.41f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.388f);
                break;
            case 2: // Wall_Bottom_Left
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.41f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.388f);
                BoxCollider2D newBC2 = gameObject.AddComponent<BoxCollider2D>();
                newBC2.offset = new Vector2(0f, 0.195f);
                newBC2.size = new Vector2(0.36f, 1.61f);
                break;
            case 3: // Wall_Bottom_Right
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(-0.41f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1.18f, 0.388f);
                BoxCollider2D newBC3 = gameObject.AddComponent<BoxCollider2D>();
                newBC3.offset = new Vector2(0f, 0.195f);
                newBC3.size = new Vector2(0.36f, 1.61f);
                break;
            case 4: // Wall_Horizontal
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.388f);
                break;
            case 5: // Wall_Vertical
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                break;


            // T-Joins
            case 6: // Wall_Top_T_Join
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.388f);
                break;
            case 7: // Wall_Bottom_T_Join
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.388f);
                BoxCollider2D newBC7 = gameObject.AddComponent<BoxCollider2D>();
                newBC7.offset = new Vector2(0f, 0.195f);
                newBC7.size = new Vector2(0.36f, 1.61f);
                break;
            case 8: // Wall_Left_T_Join
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                BoxCollider2D newBC8 = gameObject.AddComponent<BoxCollider2D>();
                newBC8.offset = new Vector2(0.59f, -0.805f);
                newBC8.size = new Vector2(0.82f, 0.388f);
                break;
            case 9:// Wall_Right_T_Join
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                BoxCollider2D newBC9 = gameObject.AddComponent<BoxCollider2D>();
                newBC9.offset = new Vector2(-0.59f, -0.805f);
                newBC9.size = new Vector2(0.82f, 0.388f);
                break;
            case 10: // Wall_Fourway_Join
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                BoxCollider2D newBC10 = gameObject.AddComponent<BoxCollider2D>();
                newBC10.offset = new Vector2(0f, -0.1f);
                newBC10.size = new Vector2(2f, 0.8f);
                break;


            // DOORS
            case 11: // Door_Top_Closed
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.388f);
                break;
            case 12: // Door_Top_Open
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.805f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(2f, 0.388f);
                break;
            case 13: // Door_Vertical_Closed
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                break;
            case 14: // Door_Vertical_Closed2
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                break;
            case 15: // Door_Bottom_Closed
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case 16: // Door_Bottom_Closed
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                break;


            // Wall Shadows
            case 17: // ShadowWall_Horizontal
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case 18: // ShadowWall_Left
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                break;
            case 19: // ShadowWall_Right
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                break;


            // Vertical Wall Ends
            case 20: // Wall_Vertical_End
                gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
                gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.36f, 2f);
                break;
            case 21: // ShadowWall_Vertical_Bottom
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
