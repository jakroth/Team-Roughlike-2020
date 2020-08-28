using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private DungeonManager dungeonManager;

    // max number of rooms we want (usually will be a lot less than this, depending on how that fit in the map)
    public int maxRooms;
    // the map dimensions
    public int width, height;
    // the room min and max dimensions
    public int minRoomWidth, minRoomHeight;
    public int maxRoomWidth, maxRoomHeight;
    
    // how many rooms we actually got
    public int _roomCount;

    // some IDs for the textures and types of rooms
    public int wallID;
    public int basicID;
    public int fillID;

    //??
    public bool basicOnly;

    public bool makeCorridorsRooms;
    // only used if makeCorridorsRooms is true
    public int corridorID;

    //??
    public List<int> validIDs;

    // a 2D array of all the x,y coordinates on the map amd which roomID they are part of. Will be set to the height and width of the map.
    public int[,] map;

    // this is the basic room (it's a nested struct/class)
    public struct Room
    {
        // x and y values will be the bottom and left coordinates of each room
        // w and h are width and height of each room
        public int x, y, w, h;
        public int x2, y2;
        public Vector2Int center;

        public Room(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;

            // these values will be the right (x2) and top (y2) coordinates of each room
            x2 = x + w;
            y2 = y + h;

            // center is halfway between x and x2 and y and y2
            center = new Vector2Int((x + x2) / 2,
                (y + y2) / 2);
        }

        // return true if this room intersects the room passed in as a parameter
        public bool intersects(Room room)
        {

            return (x <= room.x2 && x2 >= room.x &&
                y <= room.y2 && room.y2 >= room.y);
        }

    }
    // will hold the list of rooms in the map
    public List<Room> rooms;


    // the CONSTRUCTOR for the dungeon class
    public DungeonGenerator(int maxRooms, int width, int height, int minRoomWidth, int maxRoomWidth, int minRoomHeight, int maxRoomHeight)
    {
        this.maxRooms = maxRooms;
        this.width = width;
        this.height = height;
        this.minRoomHeight = minRoomHeight;
        this.maxRoomHeight = maxRoomHeight;
        this.minRoomWidth = minRoomWidth;
        this.maxRoomWidth = maxRoomWidth;
    }


    // method to put all the rooms on the map
    public void populateRooms(bool applyWalls = true)
    {
        // create a new List of rooms
        rooms = new List<Room>();

        //reset room count to 0
        _roomCount = 0;

        // create a new 2D array of coordinates to hold the room IDs (array size set to map width and height)
        map = new int[width, height];

        // variable for tracking center of each room; initialised to (0,0)
        Vector2Int newCenter = Vector2Int.zero;

        // main loop to populate rooms
        for (int i = 0; i < maxRooms; i++)
        {
            // generate some random width, height, x and y for each room
            int w = Random.Range(minRoomWidth, maxRoomWidth + 1);
            int h = Random.Range(minRoomHeight, maxRoomHeight + 1);
            // this will range from 1 to the width of the dungeon minus the width of the room generated above (remember x is the left hand coordinate of each room)
            int x = Random.Range(1, width - w);
            int y = Random.Range(1, height - h);

            // create newRoom with randomized values
            Room newRoom = new Room(x, y, w, h);

            // check for intersection with an existing room (there will be no rooms to test against initially)
            bool failed = false;
            foreach (Room otherRoom in rooms)
            {
                if (newRoom.intersects(otherRoom))
                {
                    failed = true;
                    break;
                }
            }

            // if there is NO INTERSECTION with an existing room, add this new room to the list of rooms
            // otherwise, go to next loop
            if (!failed)
            {
                // no overlap, so good to go; increment our room count
                _roomCount++;

                // local function to carve out new room
                // sets up the map array with the coordinates of this room and the roomID (based on roomCount)
                createRoom(newRoom, _roomCount);

                // THIS SECTION IS TO MAKE A CORRIDOR
                // this first part creates a newCenter somewhere inside newRoom
                // store the center from newRoom in the newCenter Vector2
                newCenter = newRoom.center;
                // create two offsets that are half the width and height of the newRoom
                int offsetW = newRoom.w / 2;
                int offsetH = newRoom.h / 2;
                // set the x value of newCenter to be somewhere along the x coordinates of newRoom
                newCenter.x = Random.Range(newCenter.x - offsetW, newCenter.x + offsetW);
                // set the y value of newCenter to be somewhere along the y coordinates of newRoom
                newCenter.y = Random.Range(newCenter.y - offsetH, newCenter.y + offsetH);

                // only make a CORRIDOR if a room already exists
                if (rooms.Count != 0)
                {
                    // grab the center of the previous room added to the list
                    Vector2Int prevCenter = rooms[rooms.Count - 1].center;
                    //as above, put a new prevCenter somewhere inside the previousRoom
                    offsetW = rooms[rooms.Count - 1].w / 2;
                    offsetH = rooms[rooms.Count - 1].h / 2;
                    prevCenter.x = Random.Range(prevCenter.x - offsetW, prevCenter.x + offsetW);
                    prevCenter.y = Random.Range(prevCenter.y - offsetH, prevCenter.y + offsetH);

                    // carve out CORRIDORS between rooms based on new centers
                    // randomly start with horizontal or vertical corridors
                    if (Random.Range(0, 2) == 1)
                    {
                        hCorridor(prevCenter.x, newCenter.x, prevCenter.y);
                        vCorridor(prevCenter.y, newCenter.y, newCenter.x);
                    }
                    else
                    {
                        vCorridor(prevCenter.y, newCenter.y, prevCenter.x);
                        hCorridor(prevCenter.x, newCenter.x, newCenter.y);
                    }
                }

                // add new room to rooms list
                rooms.Add(newRoom);
            }
        }

        //??
        if(applyWalls)
        {
            this.applyWalls();
        }

    }


    // creates a map with one room that fills the whole map
    public void createDefaultMap()
    {
        map = new int[width, height];
        createRoom(new Room(0, 0, width, height), 0);
    }


    // sets all the coordinates of this room on the map to the roomID
    public void createRoom(Room room, int roomID)
    {
        for (int x = room.x; x < room.x2; x++)
        {
            for (int y = room.y; y < room.y2; y++)
            {
                map[x, y] = roomID;
            }
        }
    }

    // prints out all the room IDs
    public void printRooms()
    {
        string result = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                result += map[x, y];
            }
            result += "\n";
        }
        Debug.Log(result);
    }

    // make a horizontal corridor (y is fixed)
    private void hCorridor(int x1, int x2, int y)
    {
        //start at the most left hand room centre
        int startX = Mathf.Min(x1, x2);
        //end at the most right hand centre
        int endX = Mathf.Max(x1, x2);

        bool firstEdit = true;

        // set the roomID of corridor coordinates to -2
        for (int x = startX; x <= endX; x++)
        {
            // check if the space is empty
            if (map[x, y] == 0)
            {
                //??
                if (firstEdit)
                {
                    //??
                    if (makeCorridorsRooms)
                    {
                        _roomCount++;
                    }
                    firstEdit = false;
                }
                // set corridors to roomID -2
                map[x, y] = makeCorridorsRooms ? _roomCount : -2;
            }
        }
    }

    //as above, but for vertical corridor
    private void vCorridor(int y1, int y2, int x)
    {
        int startY = Mathf.Min(y1, y2);
        int endY = Mathf.Max(y1, y2);

        bool firstEdit = true;

        for (int y = startY; y <= endY; y++)
        {
            if (map[x, y] == 0)
            {
                if (firstEdit)
                {
                    if (makeCorridorsRooms)
                    { 
                        _roomCount++;
                    }
                    firstEdit = false;
                }
                map[x, y] = makeCorridorsRooms ? _roomCount : -2;
            }
        }
    }

    // runs through every coordinate on the map
    // tests if a wall is required
    // sets the roomID of all the walls on the map to -1
    private void applyWalls()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (testWallRequired(x, y))
                {
                    map[x, y] = -1;
                }
            }
        }
    }



    private bool testWallRequired(int x, int y)
    {
        // save the roomID of the given coordinates into v
        int v = map[x, y];
        //if v is already occupied (with wall (-1), room (>1) or corridor (-2)) exit method with false
        if (v != 0) return false;
        // otherwise v must be equal to 0 (so empty)
        // for the 3 x 3 box around x,y...
        for (int ix = x - 1; ix <= x + 1; ix++)
        {
            for (int iy = y - 1; iy <= y + 1; iy++)
            {
                // if any of the x,y coordinates around the position being tested are a room or corridor, return true (wall required)
                if (map[ix, iy] != 0 && map[ix, iy] != -1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // ...
    public void spawnMap()
    {
        // instantiate instance of this Singleton
        if(dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }

        //??
        dungeonManager.createBaseMap(width, height, fillID, wallID);

        // create dictionary to hold the mapping of roomIDs to int values
        Dictionary<int, int> roomTilePairs = new Dictionary<int, int>();

        // set all Walls to have the wallID
        roomTilePairs.Add(-1, wallID);

        // set all Corridors to have the CorridorID
        if (!makeCorridorsRooms)
        {
            roomTilePairs.Add(-2, corridorID);
        }

        // Set basic tile for each room if ticked, otherwise random tile types for each room
        for (int i = 1; i <= _roomCount; i++)
        {
            roomTilePairs.Add(i, basicOnly ? basicID : validIDs[Random.Range(0, validIDs.Count)]);
        }

        // Populate blocks with assigned tile types
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != 0)
                {
                    dungeonManager.createTile(roomTilePairs[map[x, y]], new Vector2Int(x, y));
                }
            }
        }
    }
}
