using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{

    // to hold the DungeonGenerator and DungeonRenderer object attached to the same gameObject as this DungeonManager
    private DungeonGenerator mapGenerator;
    private DungeonRenderer mapRenderer;

    // dimensions of the map to generate
    // max number of rooms we want (will usually end up with a lot less than this, depending on how they fit in the map)
    // set in the inspector
    public int mapWidth, mapHeight, maxRooms;

    // which tile set to use, can be set in the inspector or through code
    public bool hellTiles = true;

    // check if this is the first update for the program, to populate the map
    private bool firstUpdate = true;

    // save created prefabs under these parents in the Hierarchy
    [HideInInspector] public Transform _tileParent;
    [HideInInspector] public Transform _objectParent;
    [HideInInspector] public Transform _enemyParent;

    // flag that can be set at any time during dungeon creation, if anything goes wrong. Dungeon will be rebuilt at the end if it's checked. 
    [HideInInspector] public bool needsRegen = false;

    // flag that stops other processes if the dungeon is in the process of generating (such as player movement)
    [HideInInspector] public bool makingDungeon = false;

    // location to put the player after dungeon generation
    private Vector2Int doorMat;


    // Start is called before the first frame update
    void Start()
    {
        // this references the parent "Singleton.cs", and sets the protected "instance" variable to this object. 
        // it allows the Instance() method to be called...
        instance = this;

        // returns the child objects of the DungeonManager (which will be "Tiles"/"Objects"/"Enemies" in the hierarchy)
        _tileParent = transform.GetChild(0);
        _objectParent = transform.GetChild(1);
        _enemyParent = transform.GetChild(2);

        //get the DungeonGenerator and DungeonRenderer
        mapGenerator = GetComponent<DungeonGenerator>();
        mapRenderer = GetComponent<DungeonRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        // apply procedural generation only on the first update
        if (firstUpdate)
        {
            makeDungeon();
            firstUpdate = false;
        }

        // or if the user presses 'G' (this is probably only during testing)
        if (Input.GetKeyDown(KeyCode.G) && !makingDungeon)
        {
            GameObject.Find("Player").GetComponent<PlayerBehaviour>().isMoving = false;
            makeDungeon();
        }

    }



    // call all the other methods 
    public void makeDungeon()
    {
        makingDungeon = true;

        // run the procedural generation methods
        applyMapGen();
        applyObjectGen();
        applyEnemyGen();

        // check to see if the doorMat is not in a room (any ID < 1)
        if (mapGenerator._roomCount > 0)
            if (mapGenerator.map[doorMat.x, doorMat.y] < 1 || needsRegen)
            {
                needsRegen = false;
                makeDungeon();
            }


        // place player and camera on doorMat
        placePlayerAndCamera();

        makingDungeon = false;
    }



    // *********** CALCULATE the TILE COORDINATES, MAKE the MAP, PLACE the PLAYER, SET the CAMERA **********
    // called when Dungeon Manager is first updated, or when G is pressed, or when the Player walks through a door
    // the main method for generating the dungeon, from which a bunch of DungeonGenerator methods are called
    private void applyMapGen()
    {
        print("Populating Rooms");
        mapGenerator.populateRooms(); 
        doorMat = mapGenerator.rooms[mapGenerator.rooms.Count - 1].doorMat;
        print("Rooms Populated");


        print("Spawning Map");
        mapRenderer.spawnMap();
        print("Map Spawned");


        // print the room coordinates and IDs
        mapGenerator.printRooms();
    }




    // *********** CALCULATE the OBJECT COORDINATES, PLACE THEM on MAP **********
    // called when Dungeon Manager is first updated, or when G is pressed, or when the Player walks through a door
    // the main method for generating objects in the dungeon
    private void applyObjectGen()
    {
        ObjectGenerator objGen = GetComponent<ObjectGenerator>();
        objGen.makeObjects();
    }




    // *********** CALCULATE the ENEMY COORDINATES, PLACE THEM on MAP **********
    // called when Dungeon Manager is first updated, or when G is pressed, or when the Player walks through a door
    // the main method for generating enemies in the dungeon
    private void applyEnemyGen()
    {
        EnemyGenerator enemyGen = GetComponent<EnemyGenerator>();
        enemyGen.makeEnemies();
    }



    // Place player and Camera on the DoorMat of the First Room
    private void placePlayerAndCamera()
    {
        // put player outside door of first room created (or 1,1 in empty map)
        if (mapGenerator._roomCount > 0)
        {
            print("Player placed on DoorMat at " + doorMat);
            GameObject.Find("Player").transform.position = new Vector3(doorMat.x * mapRenderer.cellDimensions, doorMat.y * mapRenderer.cellDimensions, 0);
        }
        else
        {
            print("Player placed at 1,1");
            GameObject.Find("Player").transform.position = new Vector3(1 * mapRenderer.cellDimensions, 1 * mapRenderer.cellDimensions, 0);
        }

        // put camera on player
        Camera.main.GetComponent<FollowCameraBehaviour>().setMap(new Vector2((mapWidth) * mapRenderer.cellDimensions, (mapHeight) * mapRenderer.cellDimensions),
                                                                 new Vector2(mapRenderer.cellDimensions / 2, mapRenderer.cellDimensions / 2));
    }


}
