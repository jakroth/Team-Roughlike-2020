using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    // 2D arrays for objects and students
    public GameObject[,] objectMap;
    public GameObject[,] studentMap;

    // the prefab for all the Object, set in the Inspector
    public GameObject objectPrefab;

    // the prefab for all the Object, set in the Inspector
    public GameObject studentPrefab;

    // links object sprites to objectIDs, set in the Inspector
    public List<Sprite> objectSprites;

    // links student sprites to studentIDs, set in the Inspector
    public List<Sprite> studentSprites;

    private int objectID = 0;
    private int studentID = 1;
    

    // grab these for some of their elements
    DungeonManager dungeonManager;
    DungeonGenerator dungeonGenerator;
    DungeonRenderer dungeonRenderer;

    // cell dimensions, grabbed from DungeonRenderer
    private float cellDim;

    // the map dimensions, grabbed from DungeonManager
    private int mapWidth = 0, mapHeight = 0;


    // ******** ENTRY POINT for this SCRIPT ********
    public void makeObjectsAndStudents()
    {
        // grab the instance of the Dungeon Manager
        if (dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }
        dungeonGenerator = GetComponent<DungeonGenerator>();
        dungeonRenderer = GetComponent<DungeonRenderer>();

        // clear maps of objects and students to start
        deleteObjectsAndStudents();

        if (dungeonManager.hellTiles)
        {
            // make new objectMap
            objectMap = new GameObject[dungeonManager.mapWidth, dungeonManager.mapHeight];
            // make new studentMap
            studentMap = new GameObject[dungeonManager.mapWidth, dungeonManager.mapHeight];

            // grab cellDimensions and map size
            cellDim = dungeonRenderer.cellDimensions;
            mapWidth = dungeonManager.mapWidth;
            mapHeight = dungeonManager.mapHeight;

            // generate and spawn Objects
            //generateObjectCoordinates();
            //generateStudentCoordinates();
            spawnObjectsAndStudents();
        }
    }



    // create coordinates of objects
    private void generateObjectCoordinates()
    {
        // use this if we want more interesting coordinates for objects

    }


    // create coordinates of students
    private void generateStudentCoordinates()
    {
        // use this if we want more interesting coordinates for students

    }


    // instantiate objects
    private void spawnObjectsAndStudents()
    {
        DungeonGenerator.Room r;
        // generate an object on each tile in a room, with probability 0.1 (except centre - where enemies currently are, and doorMat - where Player starts)
        for (int i = 0; i < dungeonGenerator.rooms.Count - 1; i++)
        {
            r = dungeonGenerator.rooms[i];
            for (int x = r.x; x < r.x2; x++)
            {
                for (int y = r.y; y < r.y2; y++)
                    // GENERATE OBJECTS
                {          // probability of 0.1           // not in centre of each room               // not on doorMat of first room
                    if (Random.Range(0, 10) == 0 && !(x == r.centre.x && y == r.centre.y) && !(i == 0 && x == r.doorMat.x && y == r.doorMat.y))
                    {
                        // randomise object type
                        int randomObjectID = Random.Range(0, objectSprites.Count);
                        objectMap[x, y] = Instantiate(objectPrefab, new Vector3(x * cellDim, y * cellDim, 0), Quaternion.identity, dungeonManager._objectParent);
                        objectMap[x, y].GetComponent<ObjectBehaviour>().setObject(randomObjectID, new Vector2Int(x, y), objectID);
                    }
                    // GENERATE STUDENTS (but not on same location as objects)
                              // probability of 0.066           // not in centre of each room               // not on doorMat of first room
                    else if (Random.Range(0, 16) == 0 && !(x == r.centre.x && y == r.centre.y) && !(i == 0 && x == r.doorMat.x && y == r.doorMat.y))
                    {
                        // randomise student type
                        int randomObjectID = Random.Range(0, studentSprites.Count);
                        objectMap[x, y] = Instantiate(studentPrefab, new Vector3(x * cellDim, y * cellDim, 0), Quaternion.identity, dungeonManager._studentParent);
                        objectMap[x, y].GetComponent<ObjectBehaviour>().setObject(randomObjectID, new Vector2Int(x, y), studentID);
                    }
                }
            }
        }
    }



    // clear maps of objects
    private void deleteObjectsAndStudents() 
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
                    if (studentMap[x, y] != null)
                    {
                        Destroy(studentMap[x, y]);
                    }
                }
            }
        }
    }

}
