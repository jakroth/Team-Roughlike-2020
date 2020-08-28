using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    // maps tile texture sprites to tileIDs
    // set in the Inspector
    public List<Sprite> tileTextures;

    // map of tilePrefab objects
    private GameObject[,] map;
    public int mapWidth, mapHeight;

    // the prefab for all the Tiles, set in the Inspector
    public GameObject tilePrefab;

    //??
    public float cellDim = 2;

    // check if this is the first update for the program, to populate the map
    private bool firstUpdate = true;

    // save tiles under this parent in the Hierarchy
    private Transform _tileParent;


    // Start is called before the first frame update
    void Start()
    {
        // ??
        instance = this;
        // make a new 2D array of tilePrefab game objects
        map = new GameObject[mapWidth, mapHeight];

        // returns the transforms of the children of DungeonManager
        _tileParent = transform.GetChild(0);
    }


    // Update is called once per frame
    void Update()
    {
        if (firstUpdate)
        {
            applyProcGen();
            firstUpdate = false;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            applyProcGen();
        }
    }


    // called when Dungeon Manager is first created
    private void applyProcGen()
    {
        // generate the random dungeon
        DungeonGenerator mapGen = GetComponent<DungeonGenerator>();
        //print("Populating Rooms");
        mapGen.populateRooms();
        //print("Spawning Map");
        mapGen.spawnMap();
        //print("Map Spawned");
        mapGen.printRooms();


        // print center of first room
        print(mapGen.rooms[0].center);
        // put player at center of first room
        GameObject.Find("PlayerPlaceholder").transform.position = new Vector3(mapGen.rooms[0].center.x * cellDim,
                                                                   mapGen.rooms[0].center.y * cellDim, 0);
        Camera.main.GetComponent<FollowCameraBehaviour>().setMap(new Vector2((mapWidth) * cellDim, (mapHeight) * cellDim), new Vector2(cellDim / 2, cellDim / 2));
    }

    // setup a new map with no tile game objects
    public void setMapSize(int mapWidth, int mapHeight)
    {
        if (this.mapWidth == mapWidth && this.mapHeight == mapHeight)
            return;
        deleteMap();
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        map = new GameObject[mapWidth, mapHeight];
    }


    // called by the Dungeon Generator after setting up map coordinate IDs
    public void createBaseMap(int width, int height, int basicID, int wallID)
    {
        setMapSize(width, height);
        // fill the map with the basic fill tiles
        fillGridObject(basicID, new Vector2Int(1, 1), new Vector2Int(width - 2, height - 2));
        // make the border all wall tiles
        fillBorderObject(wallID, new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1));
    }



    // actually makes the game objects that are the background tiles
    // called by createBaseMap method to setup the basic (usually water) and border tiles
    // called by the Dungeon Generator to make the room, inner wall and other special tiles
    public void createTile(int tileID, Vector2Int pos)
    {
        if (map[pos.x, pos.y] == null)
        {
            // instantiate new tilePrefab game objects all over the map, where they don't already exists
            map[pos.x, pos.y] = Instantiate(tilePrefab, new Vector3(pos.x * cellDim, pos.y * cellDim, 0), Quaternion.identity, _tileParent);
        }
        // if they already exist, set the tile ID to the one passed in
        map[pos.x, pos.y].GetComponent<DungeonTile>().setTile(tileID, pos);
    }


    public bool isPositionOutsideMap(Vector2Int pos)
    {
        return (pos.x > mapWidth - 1 || pos.x < 0 || pos.y > mapHeight - 1 || pos.y < 0);
    }



    public void fillGridObject(int tileID, Vector2Int start, Vector2Int end)
    {
        if (isPositionOutsideMap(start) || isPositionOutsideMap(end))
        {
            Debug.Log("Error fillGridObject. Grid outside map area. (" + start.x + ", " + start.y + ") to (" + end.x + ", " + end.y + ")");
            return;
        }

        // in case end is less than start??
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
                createTile(tileID, new Vector2Int(x, y));
            }
        }
    }

    public void fillBorderObject(int tileID, Vector2Int start, Vector2Int end)
    {
        if (isPositionOutsideMap(start) || isPositionOutsideMap(end))
        {
            Debug.Log("Error fillGridObject. Grid outside map area. (" + start.x + ", " + start.y + ") to (" + end.x + ", " + end.y + ")");
            return;
        }

        int start_X = Mathf.Min(start.x, end.x);
        int start_Z = Mathf.Min(start.y, end.y);
        int end_X = Mathf.Max(start.x, end.x);
        int end_Z = Mathf.Max(start.y, end.y);

        //creates wall tiles around the border
        for (int x = start.x; x <= end_X; x++)
        {
            createTile(tileID, new Vector2Int(x, start_Z));
            createTile(tileID, new Vector2Int(x, end_Z));
        }
        for (int z = start.y + 1; z < end_Z; z++)
        {
            createTile(tileID, new Vector2Int(start_X, z));
            createTile(tileID, new Vector2Int(end_X, z));
        }
    }


    // destroys all the tile objects on the map
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


}
