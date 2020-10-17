﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRenderer : MonoBehaviour
{
    // INSTANTIATES the MAP and the TILES, based on the coordinates and room IDs generated in DungeonGenerator

    // will hold these objects from the hierarchy
    DungeonManager dungeonManager;
    DungeonGenerator dungeonGenerator;

    // the prefab for all the Tiles, set in the Inspector
    public GameObject tilePrefab;

    // cell dimensions, set in the Inspector (needs to always be 2 for now)
    // used everywhere we need to deal with size and movement
    public float cellDimensions;

    private int[,] map;

    // 2D array of floor tiles (made from the tilePrefab)
    [HideInInspector] public GameObject[,] floorTileMap;

    // 2D array of wall tiles (made from the tilePrefab)
    [HideInInspector] public GameObject[,] wallTileMap;

    // 2D array of wall shadow tiles (made from the tilePrefab)
    [HideInInspector] public GameObject[,] shadowTileMap;


    [Header("Hell Sprites")]
    // tile set for the Hell Tiles
    public List<Sprite> hellTiles;

    [Header("Normal Sprites")]
    // tile set for the Normal Tiles
    public List<Sprite> normalTiles;

    [Header("Wall Sprite Indexes")]
    public int wallTopLeftTile;
    public int wallTopRightTile;
    public int wallBottomLeftTile;
    public int wallBottomRightTile;
    public int wallHorizontalTile;
    public int wallVerticalTile;
    public int wallVerticalEndTile; 
    public int wallTopTJoinTile;
    public int wallBottomTJoinTile;
    public int wallLeftTJoinTile;
    public int wallRightTJoinTile;
    public int wallFourWayJoinTile;

    [Header("Door Sprite Indexes")]
    public int doorTopClosedTile;
    public int doorTopOpenTile;
    public int doorVerticalClosedTile;
    public int doorBottomClosedTile; 
    public int doorBottomOpenTile;

    [Header("Shadow/Fill Sprite Indexes")]
    public int shadowWallHorizontalTile; 
    public int shadowWallLeftTile;
    public int shadowWallRightTile;
    public int shadowWallVerticalBottomTile; 

    [Header("Other Sprite Indexes")]
    public int floorTile;
    public int fireTile;
    public int halfFireTile;
    public int quarterFireTile;


    [Header("Sorting Layers")]
    public string floorLayer;
    public string betweenLayer;
    public string wallLayer;
    public string shadowLayer;
    public string tileTopLayer;
    public string actualTopLayer;

    [Header("Game Layers")]
    public int wallGameLayer;

    [Header("Tags")]
    public string mapTag;
    public string horoWallTag;


    // the map dimensions
    // private, set in DungeonManager, obtained in spawnMap() method
    private int mapWidth = 0, mapHeight = 0;

    // map coordinate names, from DungeonGenerator
    //private readonly int BORDER = -4;
    private readonly int DOOR = -3;
    private readonly int WALL = -2;
    private readonly int CORRIDOR = -1;
    //private readonly int EMPTY = 0;
    private readonly int ROOM = 1;



    // **************** ENTRY POINT for the DUNGEON RENDERER ****************
    // RENDERS the appropriate textures/sprites for each of the map coordinates generated by the DungeonGenerator
    public void spawnMap()
    {
        // grab the instance of the Dungeon Manager
        if (dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }
        // grab the Dungeon Generator object
        dungeonGenerator = GetComponent<DungeonGenerator>();

        map = dungeonGenerator.map;

        // create the base map with background tiles and border (wall) tiles only
        createBaseMap();

        // create all the special (ID'd) tiles
        createFeaturedMap();

    }


    // CREATES BASE MAP with empty TILES ******
    // called by the spawnMap() method above
    // calls three other methods in this Class
    private void createBaseMap()
    {
        // delete old map, set new map size, reinstantiate map 2D array
        setMapSize();
        // fill the map with the basic fill tiles
        fillGridFloor();
        // fill the border with wall tiles
        createBorderWall();
    }



    private void createFeaturedMap()
    {
        //creates the floor tiles for the rooms and corridors, at layer 1, not collidable
        createRoomsAndCorridors();

        // creates the tiles for the walls, at layer 2, generally collidable
        createWalls();

        // creates the tiles for the doors, at layer 2, generally collidable
        createDoors();

        // creates the tiles for the shadows, at layer 3, sometimes collidable
        createShadows();

    }



    // called by the createBaseMap() method above
    // setup a new map 2D array with no tile game objects
    private void setMapSize()
    {
        // delete old map
        deleteMap();

        // get map dimensions from DungeonManager
        mapHeight = dungeonManager.mapHeight;
        mapWidth = dungeonManager.mapWidth;

        // make new maps
        floorTileMap = new GameObject[mapWidth, mapHeight];
        wallTileMap = new GameObject[mapWidth, mapHeight];
        shadowTileMap = new GameObject[mapWidth, mapHeight];
    }



    // called by the setMapSize() method above, to clear the maps before making new maps 
    // destroys all the tile objects in the 2D "map" arrays 
    private void deleteMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (floorTileMap[x, y] != null)
                {
                    Destroy(floorTileMap[x, y]);
                }
                if (wallTileMap[x, y] != null)
                {
                    Destroy(wallTileMap[x, y]);
                }
                if (shadowTileMap[x, y] != null)
                {
                    Destroy(shadowTileMap[x, y]);
                }
            }
        }
    }


    // called by the createBaseMap() method above
    // fills the map with the basic fill tiles
    private void fillGridFloor()
    {
        // setup variables
        int left_X = 1;
        int bottom_Y = 1;
        int right_X = mapWidth - 2;
        int top_Y = mapHeight - 2;

        // main loop for this method
        // creates basic fill tiles across the whole map
        for (int x = left_X; x <= right_X; x++)
        {
            for (int y = bottom_Y; y <= top_Y; y++)
            {
                if (map[x, y] == WALL || map[x,y] == DOOR || x == 0 || y == 0)
                {
                    createTile(floorTile, new Vector2Int(x, y), floorTileMap, floorLayer, false);
                }
                else {
                    if (dungeonManager.hellTiles)
                    {
                        int random = Random.Range(0, 3);
                        if (random == 0)
                            createTile(fireTile, new Vector2Int(x, y), floorTileMap, tileTopLayer, false);
                        else if (random == 1)
                            createTile(halfFireTile, new Vector2Int(x, y), floorTileMap, tileTopLayer, false);
                        else
                            createTile(quarterFireTile, new Vector2Int(x, y), floorTileMap, tileTopLayer, false);
                    }
                    else
                    {
                        createTile(fireTile, new Vector2Int(x, y), floorTileMap, floorLayer, false);
                    }
                }
            }
        }
    }


    // called by the createBaseMap() method above
    // creates border tiles around the edges of the map
    private void createBorderWall()
    {
        // setup variables
        int left_X = 0;
        int bottom_Y = 0;
        int right_X = mapWidth - 1;
        int top_Y = mapHeight - 1;

        // a useful variable for the loops below
        bool needsTJoin;

        // make corners
        createTile(wallTopLeftTile, new Vector2Int(left_X, top_Y), wallTileMap, wallLayer, true);
        createTile(wallTopRightTile, new Vector2Int(right_X, top_Y), wallTileMap, wallLayer, true);
        createTile(wallBottomLeftTile, new Vector2Int(left_X, bottom_Y), wallTileMap, wallLayer, true);
        createTile(wallBottomRightTile, new Vector2Int(right_X, bottom_Y), wallTileMap, wallLayer, true); ;

        // make borders
        for (int x = left_X + 1; x < right_X; x++)
        {
            // top border
            // check if TJoin needed (if adjacent tile is a wall or door, and either side of adjacent is a room, corridor or empty)
            needsTJoin = (map[x, top_Y - 1] == WALL || map[x, top_Y - 1] == DOOR) &&
                                (map[x - 1, top_Y - 1] >= CORRIDOR || map[x + 1, top_Y - 1] >= CORRIDOR);
            createTile(needsTJoin ? wallTopTJoinTile : wallHorizontalTile, new Vector2Int(x, top_Y), wallTileMap, wallLayer, true);

            // bottom border
            needsTJoin = (map[x, bottom_Y + 1] == WALL || map[x, bottom_Y + 1] == DOOR) &&
                                (map[x - 1, bottom_Y + 1] >= CORRIDOR || map[x + 1, bottom_Y + 1] >= CORRIDOR);
            createTile(needsTJoin ? wallBottomTJoinTile : wallHorizontalTile, new Vector2Int(x, bottom_Y), wallTileMap, wallLayer, true);
        }

        for (int y = bottom_Y + 1; y < top_Y; y++)
        {
            // left border
            needsTJoin = (map[left_X + 1, y] == WALL || map[left_X + 1, y] == DOOR) &&
                                (map[left_X + 1, y - 1] >= CORRIDOR || map[left_X + 1, y + 1] >= CORRIDOR);
            createTile(needsTJoin ? wallLeftTJoinTile : wallVerticalTile, new Vector2Int(left_X, y), wallTileMap, wallLayer, true);

            // right border
            needsTJoin = (map[right_X - 1, y] == WALL || map[right_X - 1, y] == DOOR) &&
                                (map[right_X - 1, y - 1] >= CORRIDOR || map[right_X - 1, y + 1] >= CORRIDOR);
            createTile(needsTJoin ? wallRightTJoinTile : wallVerticalTile, new Vector2Int(right_X, y), wallTileMap, wallLayer, true);
        }
    }


    // create room and corridor tiles at layer 1
    // called by the spawnMap() method above
    private void createRoomsAndCorridors()
    {
        for (int x = 1; x <= mapWidth - 2; x++)
        {
            for (int y = 1; y <= mapHeight - 2; y++)
            {
                // if mapID is a corridor or room, isCollision = false;
                if (map[x, y] == CORRIDOR || map[x, y] >= ROOM)
                {
                    // send the (x,y) coordinates as a Vector2Int, along with the tileID, to the createTile method in the DungeonManager
                    createTile(floorTile, new Vector2Int(x, y), floorTileMap, floorLayer, false);
                }
            }
        }
    }


    // creates all the interior walls of the map at layer 2
    // these are generally collidable
    private void createWalls()
    {
        for (int x = 1; x <= mapWidth - 2; x++)
        {
            for (int y = 1; y <= mapHeight - 2; y++)
            {
                // CORNER WALLS x4
                // top left corner
                if (map[x, y] == WALL && (map[x + 1, y] <= WALL) && (map[x, y - 1] <= WALL) && !(map[x - 1, y] <= WALL) && !(map[x, y + 1] <= WALL))
                    createTile(wallTopLeftTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);

                // top right corner
                else if (map[x, y] == WALL && (map[x - 1, y] <= WALL) && (map[x, y - 1] <= WALL) && !(map[x + 1, y] <= WALL) && !(map[x, y + 1] <= WALL))
                    createTile(wallTopRightTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);

                // bottom left corner
                else if (map[x, y] == WALL && (map[x + 1, y] <= WALL) && (map[x, y + 1] <= WALL) && !(map[x - 1, y] <= WALL) && !(map[x, y - 1] <= WALL))
                    createTile(wallBottomLeftTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);

                // bottom right corner
                else if (map[x, y] == WALL && (map[x - 1, y] <= WALL) && (map[x, y + 1] <= WALL) && !(map[x + 1, y] <= WALL) && !(map[x, y - 1] <= WALL))
                    createTile(wallBottomRightTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);


                // STRAIGHT WALLS and T-JOINS (HORIZONTAL)
                else if (map[x, y] == WALL && (map[x + 1, y] <= WALL) && (map[x - 1, y] <= WALL))
                {
                    // horizontal straight
                    if (!(map[x, y + 1] == WALL || map[x, y + 1] == DOOR) && !(map[x, y - 1] == WALL || map[x, y - 1] == DOOR))
                        createTile(wallHorizontalTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                    // top TJoin
                    else if ((map[x, y - 1] <= WALL) && !(map[x, y + 1] == WALL || map[x, y + 1] == DOOR))
                        createTile(wallTopTJoinTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                    // bottom TJoin
                    else if ((map[x, y + 1] <= WALL) && !(map[x, y - 1] == WALL || map[x, y - 1] == DOOR))
                        createTile(wallBottomTJoinTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                    // four way join
                    else if ((map[x, y + 1] <= WALL) && (map[x, y - 1] <= WALL))
                        createTile(wallFourWayJoinTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                }


                // STRAIGHT WALLS and T-JOINS (VERTICAL)
                else if (map[x, y] == WALL && (map[x, y + 1] <= WALL) && (map[x, y - 1] <= WALL))
                {
                    // vertical straight
                    if (!(map[x + 1, y] == WALL || map[x + 1, y] == DOOR) && !(map[x - 1, y] == WALL || map[x - 1, y] == DOOR))
                        createTile(wallVerticalTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                    // left TJoin
                    else if ((map[x + 1, y] <= WALL) && !(map[x - 1, y] == WALL || map[x - 1, y] == DOOR))
                        createTile(wallLeftTJoinTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                    // right TJoin
                    else if ((map[x - 1, y] <= WALL) && !(map[x + 1, y] == WALL || map[x + 1, y] == DOOR))
                        createTile(wallRightTJoinTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                }
                                

                // ENDINGS
                else if (map[x, y] == WALL) 
                { 
                    int count = 0;
                    if (map[x - 1, y] == WALL) count += 1;
                    if (map[x + 1, y] == WALL) count += 3;
                    if (map[x, y - 1] == WALL) count += 5;
                    if (map[x, y + 1] == WALL) count += 7;
                    if (count == 0 || count == 1 || count == 3)
                        createTile(wallHorizontalTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                    else if (count == 5)
                        createTile(wallVerticalTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                    else if (count == 7)
                        createTile(wallVerticalEndTile, new Vector2Int(x, y), wallTileMap, wallLayer, true);
                }


                    // door ways?? Ie passages with door shaped holes but no doors? 

                    // anything else? 
                                    
            }
        }
    }


    private void createDoors()
    {
        int finalRoom = dungeonGenerator.rooms.Count - 1;
        if (finalRoom > -1)
        {
            Vector2Int door = dungeonGenerator.rooms[finalRoom].door;
            int twice = 0;
            while (twice < 2)
            {
                // doors on the border
                if (door.y <= 0 || door.y >= mapHeight - 1)
                    createTile(doorTopClosedTile, new Vector2Int(door.x, door.y), wallTileMap, wallLayer, true);
                else if (door.x <= 0 || door.x >= mapWidth - 1)
                    createTile(doorVerticalClosedTile, new Vector2Int(door.x, door.y), wallTileMap, wallLayer, true);

                // doors on interior walls
                else if (map[door.x - 1, door.y] == WALL || map[door.x + 1, door.y] == WALL)
                    createTile(doorTopClosedTile, new Vector2Int(door.x, door.y), wallTileMap, wallLayer, true);
                else if (map[door.x, door.y - 1] == WALL || map[door.x, door.y + 1] == WALL)
                    createTile(doorVerticalClosedTile, new Vector2Int(door.x, door.y), wallTileMap, wallLayer, true);

                door = dungeonGenerator.rooms[0].door;
                twice++;
            }
            // set finalDoor variable in the assigned tile in the first/Boss room
            wallTileMap[dungeonGenerator.rooms[0].door.x, dungeonGenerator.rooms[0].door.y].GetComponent<DungeonTile>().isFinalDoor = true;
        }

    }


    // creates all the wall shadows underneath the wall layer. 
    // these are generally not collidable
    // shadows generally only needed for horizontal walls, or vertical wall endings
    private void createShadows()
    {
        for (int x = 0; x <= mapWidth - 1; x++)
        {
            for (int y = 1; y <= mapHeight - 1; y++)
            {
                
                if (wallTileMap[x, y] != null)
                {
                    DungeonTile current = wallTileMap[x, y].GetComponent<DungeonTile>();

                    // horizontal wall shadows
                    if (current.spriteID == wallHorizontalTile)
                        createTile(shadowWallHorizontalTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // vertical wall shadows
                    else if (current.spriteID == wallVerticalEndTile)
                        createTile(shadowWallVerticalBottomTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // door shadows
                    else if (current.spriteID == doorTopClosedTile)
                        createTile(doorBottomClosedTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // top left corner wall shadows
                    else if (current.spriteID == wallTopLeftTile)
                        createTile(shadowWallRightTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // top right corner wall shadows
                    else if (current.spriteID == wallTopRightTile)
                        createTile(shadowWallLeftTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // bottom left corner wall shadows
                    else if (current.spriteID == wallBottomLeftTile)
                        createTile(shadowWallRightTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // bottom right corner wall shadows
                    else if (current.spriteID == wallBottomRightTile)
                        createTile(shadowWallLeftTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // top t-junction wall shadows
                    else if (current.spriteID == wallTopTJoinTile)
                        createTile(shadowWallHorizontalTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // bottom t-junction wall shadows
                    else if (current.spriteID == wallBottomTJoinTile)
                        createTile(shadowWallHorizontalTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // left t-junction wall shadows
                    else if (current.spriteID == wallLeftTJoinTile)
                        createTile(shadowWallRightTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // right t-junction wall shadows
                    else if (current.spriteID == wallRightTJoinTile)
                        createTile(shadowWallLeftTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                    // fourway junction wall shadows
                    else if (current.spriteID == wallFourWayJoinTile)
                        createTile(shadowWallHorizontalTile, new Vector2Int(x, y - 1), shadowTileMap, betweenLayer, false);

                }
            }
        }
    }


    // ************* TILE INSTANTIATION ************
    // actually instantiates the game objects/textures that are the map tiles
    // called by createBaseMap and it's helper methods to setup the basic floor and border tiles    
    // then called by populateMapWithTiles to make the room, inner wall, door and other special tiles
    private void createTile(int spriteIndex, Vector2Int pos, GameObject[,] map, string sortingLayer, bool isCollision, int gameLayer = 0, string tag = "map")
    {
        // check if this tile has already been instantiated (e.g. by the fillGrid or fillBorder methods)
        if (map[pos.x, pos.y] == null)
        {
            // instantiate new tilePrefab game object on the map
            map[pos.x, pos.y] = Instantiate(tilePrefab, new Vector3(pos.x * cellDimensions, pos.y * cellDimensions, 0), Quaternion.identity, dungeonManager._tileParent);
        }
        // set the tile ID, position and isWall to the one passed in
        map[pos.x, pos.y].GetComponent<DungeonTile>().setTile(spriteIndex, pos, sortingLayer, isCollision, gameLayer, tag);
    }


}
