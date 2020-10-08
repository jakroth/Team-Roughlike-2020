using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    // 2D array of enemies (made from the enemyPrefab)
    public GameObject[,] enemyMap;

    // the prefab for all the Enemies, set in the Inspector
    public GameObject EnemyPrefab;

    // links enemy sprites to enemyIDs, set in the Inspector
    public List<Sprite> enemySprites;

    public int spiderID;
    public int bossID;

    // grab these for some of their elements
    DungeonManager dungeonManager;
    DungeonGenerator dungeonGenerator;
    DungeonRenderer dungeonRenderer;

    // cell dimensions, grabbed from DungeonRenderer
    private float cellDim;

    // the map dimensions, grabbed from DungeonManager
    private int mapWidth = 0, mapHeight = 0;



    // ******** ENTRY POINT for this SCRIPT ********
    public void makeEnemies()
    {
        // clear map of enemies to start
        deleteEnemies();

        // grab the instance of the Dungeon Manager
        if (dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }
        dungeonGenerator = GetComponent<DungeonGenerator>();
        dungeonRenderer = GetComponent<DungeonRenderer>();

        // make new enemyMap
        enemyMap = new GameObject[dungeonManager.mapWidth, dungeonManager.mapHeight];

        // grab cellDimensions and map size
        cellDim = dungeonRenderer.cellDimensions;
        mapWidth = dungeonManager.mapWidth;
        mapHeight = dungeonManager.mapHeight;

        // generate and spawn Enemies
        generateEnemyCoordinates();
        spawnRandomEnemies();
        spawnFinalBoss();
    }



    // create coordinates of enemies
    private void generateEnemyCoordinates()
    {
        // use this if we want more interesting coordinates for enemies

    }



    // instantiate enemies
    private void spawnRandomEnemies()
    {
        DungeonGenerator.Room r;
        // generate an enemy in the centre of rooms (not including first and last), with probability 0.6667
        for(int i = 1; i < dungeonGenerator.rooms.Count - 2; i++)
        {
            if (Random.Range(0, 3) > 0)
            {
                r = dungeonGenerator.rooms[i];
                enemyMap[r.centre.x, r.centre.y] = Instantiate(EnemyPrefab, new Vector3(r.centre.x * cellDim, r.centre.y * cellDim, 0),
                                                    Quaternion.identity, dungeonManager._enemyParent);
                enemyMap[r.centre.x, r.centre.y].GetComponent<EnemyBehaviour>().setEnemy(spiderID, new Vector2Int(r.centre.x, r.centre.y));
            }
        }
    }


    // instantiate enemies
    private void spawnFinalBoss()
    {
        // generate a final boss in the final room
        DungeonGenerator.Room r = dungeonGenerator.rooms[0];
        enemyMap[r.centre.x, r.centre.y] = Instantiate(EnemyPrefab, new Vector3(r.centre.x * cellDim, r.centre.y * cellDim, 0),
                                                Quaternion.identity, dungeonManager._enemyParent);
        enemyMap[r.centre.x, r.centre.y].GetComponent<EnemyBehaviour>().setEnemy(bossID, new Vector2Int(r.centre.x, r.centre.y));

    }



    // clear map of enemies
    private void deleteEnemies()
    {
        if (enemyMap != null) { 
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (enemyMap[x, y] != null)
                    {
                        Destroy(enemyMap[x, y]);
                    }
                }
            } 
        }
    }

}
