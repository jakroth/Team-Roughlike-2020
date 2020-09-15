using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    // 2D array of objects (made from the objectPrefab)
    public GameObject[,] objectMap;

    // the prefab for all the Object, set in the Inspector
    public GameObject objectPrefab;

    // links object sprites to objectIDs, set in the Inspector
    public List<Sprite> objectSprites;

    public int backpackID;
    public int otherID;

    // grab these for some of their elements
    DungeonManager dungeonManager;
    DungeonGenerator dungeonGenerator;
    DungeonRenderer dungeonRenderer;

    // cell dimensions, grabbed from DungeonRenderer
    private float cellDim;

    // the map dimensions, grabbed from DungeonManager
    private int mapWidth = 0, mapHeight = 0;



    // ******** ENTRY POINT for this SCRIPT ********
    public void makeObjects()
    {
        // clear map of objects to start
        deleteObjects();

        // grab the instance of the Dungeon Manager
        if (dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }
        dungeonGenerator = GetComponent<DungeonGenerator>();
        dungeonRenderer = GetComponent<DungeonRenderer>();

        // make new enemyMap
        objectMap = new GameObject[dungeonManager.mapWidth, dungeonManager.mapHeight];

        // grab cellDimensions and map size
        cellDim = dungeonRenderer.cellDimensions;
        mapWidth = dungeonManager.mapWidth;
        mapHeight = dungeonManager.mapHeight;

        // generate and spawn Objects
        generateObjectCoordinates();
        spawnObjects();
    }



    // create coordinates of objects
    private void generateObjectCoordinates()
    {
        // use this if we want more interesting coordinates for objects

    }



    // instantiate objects
    private void spawnObjects()
    {
        DungeonGenerator.Room r;
        // generate an object on each tile in a room, with probability 0.1 (except centre - where enemies currently are, and doorMat - where Player starts)
        for (int i = 0; i < dungeonGenerator.rooms.Count - 1; i++)
        {
            r = dungeonGenerator.rooms[i];
            for (int x = r.x; x < r.x2; x++)
            {
                for (int y = r.y; y < r.y2; y++)
                {          // probability of 0.1           // not in centre of each room               // not on doorMat of first room
                    if (Random.Range(0, 10) == 0 && !(x == r.centre.x && y == r.centre.y) && !(i == 0 && x == r.doorMat.x && y == r.doorMat.y))
                    {
                        // randomise object type
                        int randomObjectID = Random.Range(0, objectSprites.Count);
                        objectMap[x, y] = Instantiate(objectPrefab, new Vector3(x * cellDim, y * cellDim, 0),
                                                            Quaternion.identity, dungeonManager._objectParent);
                        objectMap[x, y].GetComponent<ObjectBehaviour>().setObject(randomObjectID, new Vector2Int(x, y));
                    }
                }
            }
        }

    }



    // clear map of objects
    private void deleteObjects()
    {
        if (objectMap != null)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (objectMap[x, y] != null)
                    {
                        Destroy(objectMap[x, y]);
                    }
                }
            }
        }
    }

}
