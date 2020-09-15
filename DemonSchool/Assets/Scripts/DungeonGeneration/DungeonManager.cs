using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{

    // dimensions of the map to generate
    // max number of rooms we want (will usually end up with a lot less than this, depending on how they fit in the map)
    // set in the inspector
    public int mapWidth, mapHeight, maxRooms;

    // check if this is the first update for the program, to populate the map
    private bool firstUpdate = true;

    // save created prefabs under these parents in the Hierarchy
    [HideInInspector] public Transform _tileParent;
    [HideInInspector] public Transform _objectParent;
    [HideInInspector] public Transform _enemyParent;


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
        if (Input.GetKeyDown(KeyCode.G))
        {
            makeDungeon();
        }
    }



    // call three methods in one call
    public void makeDungeon()
    {
        applyProcGen();
        applyObjectGen();
        applyEnemyGen();
    }



    // *********** CALCULATE the TILE COORDINATES, MAKE the MAP, PLACE the PLAYER, SET the CAMERA **********
    // called when Dungeon Manager is first updated, or when G is pressed, or when the Player walks through a door
    // the main method for generating the dungeon, from which a bunch of DungeonGenerator methods are called
    private void applyProcGen()
    {
        // get the DungeonGenerator object attached to the same gameObject as this DungeonManager
        // below we call a bunch of the methods in the DungeonGenerator class
        DungeonGenerator mapGenerator = GetComponent<DungeonGenerator>();
        DungeonRenderer mapRenderer = GetComponent<DungeonRenderer>();


        //print("Populating Rooms");
        mapGenerator.populateRooms();
        //print("Rooms Populated");


        //print("Spawning Map");
        mapRenderer.spawnMap();
        //print("Map Spawned");


        // print the room coordinates and IDs, then print the doormat location
        mapGenerator.printRooms();


        // put player outside door of first room created (or 1,1 in empty map)
        if (mapGenerator._roomCount > 0)
        {
            print(mapGenerator.rooms[0].doorMat);
            GameObject.Find("Player").transform.position = new Vector3(mapGenerator.rooms[0].doorMat.x * mapRenderer.cellDimensions, mapGenerator.rooms[0].doorMat.y * mapRenderer.cellDimensions, 0);
        }
        else
        {
            print("player placed at 1,1");
            GameObject.Find("Player").transform.position = new Vector3(1 * mapRenderer.cellDimensions, 1 * mapRenderer.cellDimensions, 0);
        }

        // put camera on player
        Camera.main.GetComponent<FollowCameraBehaviour>().setMap(new Vector2((mapWidth) * mapRenderer.cellDimensions, (mapHeight) * mapRenderer.cellDimensions), 
                                                                 new Vector2(mapRenderer.cellDimensions / 2, mapRenderer.cellDimensions / 2));
    }






    // *********** CALCULATE the OBJECT COORDINATES, PLACE THEM on MAP **********
    // called when Dungeon Manager is first updated, or when G is pressed, or when the Player walks through a door
    // the main method for generating objects in the dungeon
    private void applyObjectGen()
    {
        ObjectGenerator objGen = GetComponent<ObjectGenerator>();


    }





    // *********** CALCULATE the ENEMY COORDINATES, PLACE THEM on MAP **********
    // called when Dungeon Manager is first updated, or when G is pressed, or when the Player walks through a door
    // the main method for generating enemies in the dungeon
    private void applyEnemyGen()
    {
        EnemyGenerator enemyGen = GetComponent<EnemyGenerator>();


    }



}
