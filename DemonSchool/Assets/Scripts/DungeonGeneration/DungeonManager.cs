using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    // links tile texture sprites to tileIDs, set in the Inspector
    public List<Sprite> floorTileTextures;

    // links wall texture sprites to wallIDs, set in the Inspector
    public List<Sprite> wallTileTextures;

    // the prefab for all the Tiles, set in the Inspector
    public GameObject tilePrefab;

    // cell dimensions, set in the Inspector (needs to always be 2 for now)
    // used everywhere we need to deal with size and movement
    public float cellDimensions;

    // dimensions of the map to generate
    private int mapWidth, mapHeight;

    // 2D array of tile objects (made from the tilePrefab)
    public GameObject[,] map;

    // check if this is the first update for the program, to populate the map
    private bool firstUpdate = true;

    // save created tiles under this parent in the Hierarchy
    private Transform _tileParent;


    // Start is called before the first frame update
    void Start()
    {
        // this references the parent "Singleton.cs", and sets the protected "instance" variable to this object. 
        // it allows the Instance() method to be called...
        instance = this;

        // make a new 2D array to hold tilePrefab game objects
        // they can be referenced/accessed in this array by their (x,y) coordinates
        map = new GameObject[mapWidth, mapHeight];

        // returns the 1st child object of the DungeonManager (which will be "Tiles" in the hierarchy)
        _tileParent = transform.GetChild(0);
    }


    // Update is called once per frame
    void Update()
    {
        // apply procedural generation only on the first update
        if (firstUpdate)
        {
            applyProcGen();
            firstUpdate = false;
        }

        // of if the user presses 'G' (this is probably only during testing)
        if (Input.GetKeyDown(KeyCode.G))
        {
            applyProcGen();
        }
    }


    // called when Dungeon Manager is first created
    // the main method for generating the dungeon, from which a bunch of DungeonGenerator methods are called
    public void applyProcGen()
    {
        // get the DungeonGenerator object attached to the same gameObject as this DungeonManager
        // below we call a bunch of the methods in the DungeonGenerator class
        DungeonGenerator mapGen = GetComponent<DungeonGenerator>();
        
        
        // the populateRooms() method 
        //print("Populating Rooms");
        mapGen.populateRooms();

        //print("Spawning Map");
        mapGen.spawnMap();

        //print("Map Spawned");
        mapGen.printRooms();

        // print center of first room
        print(mapGen.rooms[0].doorMat);

        // put player outside door of first room created
        GameObject.Find("Player").transform.position = new Vector3(mapGen.rooms[0].doorMat.x * cellDimensions,
                                                                   mapGen.rooms[0].doorMat.y * cellDimensions, 0);
        // put camera on player
        Camera.main.GetComponent<FollowCameraBehaviour>().setMap(new Vector2((mapWidth) * cellDimensions, (mapHeight) * cellDimensions), 
                                                                 new Vector2(cellDimensions / 2, cellDimensions / 2));
    }



    // called by the Dungeon Generator after setting up map coordinate IDs
    // calls three other methods in this Class
    public void createBaseMap(int mapWidth, int mapHeight, int backgroundTileID, int borderTileID)
    {
        // delete old map, set new map size, reinstantiate map 2D array
        setMapSize(mapWidth, mapHeight);
        // fill the map with the basic fill tiles
        fillGridObject(backgroundTileID, new Vector2Int(1, 1), new Vector2Int(mapWidth - 2, mapHeight - 2));
        // make the border all wall tiles
        fillBorderObject(borderTileID, new Vector2Int(0, 0), new Vector2Int(mapWidth - 1, mapHeight - 1));
    }


    // called by the createBaseMap() method above
    // setup a new map 2D array with no tile game objects
    public void setMapSize(int mapWidth, int mapHeight)
    {
        if (this.mapWidth == mapWidth && this.mapHeight == mapHeight)
            return;
        deleteMap();
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        map = new GameObject[mapWidth, mapHeight];
    }


    // called by the setMapSize() method above, to clear the map before making a new map 
    // destroys all the tile objects in the 2D array "map"
    private void deleteMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (map[x, y] != null)
                {
                    Destroy(map[x, y]);
                }
            }
        }
    }


    // called by the createBaseMap() method above
    // fills the map with the basic fill tiles
    public void fillGridObject(int tileID, Vector2Int start, Vector2Int end)
    {
        // check if proposed grid is outside map dimensions
        if (isPositionOutsideMap(start) || isPositionOutsideMap(end))
        {
            Debug.Log("Error fillGridObject. Grid outside map area. (" + start.x + ", " + start.y + ") to (" + end.x + ", " + end.y + ")");
            return;
        }

        // in case end is less than start
        int start_X = Mathf.Min(start.x, end.x);
        int start_Y = Mathf.Min(start.y, end.y);
        int end_X = Mathf.Max(start.x, end.x);
        int end_Y = Mathf.Max(start.y, end.y);


        // main loop for this method
        // creates basic fill tiles across the whole map
        for (int x = start_X; x <= end_X; x++)
        {
            for (int y = start_Y; y <= end_Y; y++)
            {
                createTile(tileID, new Vector2Int(x, y), false);
            }
        }
    }

    // called by the createBaseMap() method above
    // creates border tiles around the edges of the map
    public void fillBorderObject(int tileID, Vector2Int start, Vector2Int end)
    {
        // check if proposed border is outside map dimensions
        if (isPositionOutsideMap(start) || isPositionOutsideMap(end))
        {
            Debug.Log("Error fillGridObject. Grid outside map area. (" + start.x + ", " + start.y + ") to (" + end.x + ", " + end.y + ")");
            return;
        }

        // in case end is less than start
        int start_X = Mathf.Min(start.x, end.x);
        int start_Y = Mathf.Min(start.y, end.y);
        int end_X = Mathf.Max(start.x, end.x);
        int end_Y = Mathf.Max(start.y, end.y);


        // main loop for this method
        // creates border tiles around the edges
        // top and bottom borders
        for (int x = start_X; x <= end_X; x++)
        {
            createTile(tileID, new Vector2Int(x, start_Y), true);
            createTile(tileID, new Vector2Int(x, end_Y), true);
        }
        // left and right borders
        for (int y = start.y + 1; y < end_Y; y++)
        {
            createTile(tileID, new Vector2Int(start_X, y), true);
            createTile(tileID, new Vector2Int(end_X, y), true);
        }
    }


    // used by the fill...Object() methods above, to check if any of the tile (x,y) variables are outside the map boundaries
    public bool isPositionOutsideMap(Vector2Int pos)
    {
        return (pos.x > mapWidth - 1 || pos.x < 0 || pos.y > mapHeight - 1 || pos.y < 0);
    }



    // actually instantiates the game objects/textures that are the map tiles
    // called by createBaseMap method to setup the basic (usually water) and border tiles    
    // then called by the Dungeon Generator to make the room, inner wall and other special tiles
    public void createTile(int tileID, Vector2Int pos, bool isDoorOrWall)
    {
        // check if this tile has already been instantiated (e.g. by the fillGrid or fillBorder methods)
        if (map[pos.x, pos.y] == null)
        {
            // instantiate new tilePrefab game objects all over the map, where they don't already exists
            map[pos.x, pos.y] = Instantiate(tilePrefab, new Vector3(pos.x * cellDimensions, pos.y * cellDimensions, 0), Quaternion.identity, _tileParent);
        }
        // set the tile ID, position and isWall to the one passed in
        map[pos.x, pos.y].GetComponent<DungeonTile>().setTile(tileID, pos, isDoorOrWall);
    }






}
