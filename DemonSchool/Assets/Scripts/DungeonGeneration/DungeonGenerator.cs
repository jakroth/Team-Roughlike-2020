using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    // This class sets up IDs (as ints in a 2D array) for a map of the room using (x,y,) coordinates
    // The IDs are -3 (doors), -2 (walls), -1 (corridors), 1+ (unique for each room)
    // Texture sprites are (mostly) added in the DungeonManager


    // this will hold the DungeonManager instance, so elements inside it can be accessed
    private DungeonManager dungeonManager;

    // the map dimensions
    // set in the Inspector
    public int mapWidth, mapHeight;

    // max number of rooms we want (will usually end up with a lot less than this, depending on how they fit in the map)
    // set in the Inspector
    public int maxRooms;

    // how many rooms we actually get
    // view in the Inspector, is set in populateRooms method
    public int _roomCount;

    // the min and max room dimensions
    // set in the Inspector
    public int minRoomWidth, minRoomHeight;
    public int maxRoomWidth, maxRoomHeight;
    
    // some IDs for the texture/sprite of rooms
    // set in the Inspector
    public int basicFloorTileID;
    public int corridorTileID;
    public int backgroundTileID;

    public int doorTileID;
    public int wallTileID;

    // toggles between basic floor tiles and fire/ice/water floor tiles
    public bool basicFloorTilesOnly;

    // used to randomly pick a room tileset if basicOnly is false
    public List<int> validIDs;

    // a 2D array of all the IDs and the (x,y) coordinates they correspond to. 
    public int[,] map;

    // will hold the list of "rooms" (see nested calls below) in the map
    public List<Room> rooms;



    // this is the basic ROOM (it's a nested struct/class)
    public struct Room
    {
        // x and y values will be the bottom and left coordinates of each room
        // w and h are width and height of each room
        public int x, y, w, h;
        public int x2, y2;
        public Vector2Int centre;
        public Vector2Int door;
        public Vector2Int doorMat;

        //room constructor
        public Room(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;

            // these values will be the right (x2) and top (y2) coordinates of each room
            x2 = x + w;
            y2 = y + h;

            // centre is halfway between x and x2 and y and y2
            centre = new Vector2Int((x + x2) / 2,
                (y + y2) / 2);

            // door is set to bottom left wall by default
            door = new Vector2Int(x-1,y-1);

            // doorFloor is set to bottom left floor by default
            doorMat = new Vector2Int(x, y);
        }

        // return true if this room intersects the room passed in as a parameter
        public bool intersects(Room room)
        {

            return (x <= room.x2 && x2 >= room.x &&
                y <= room.y2 && room.y2 >= room.y);
        }

    }


    // method to make ROOMS:
    // 1. populate a list of room objects
    // 2. populate the map with roomIDs for each coordinate in a room
    // 3. populate the map with corridorIDs for each coordinate in a corridor
    // 4. populate the map with wallIDs for each coordinate in a wall
    public void populateRooms()
    {
        // grab the instance of the Dungeon Manager
        if (dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }

        // create a new List of rooms
        rooms = new List<Room>();

        //reset room count to 0
        _roomCount = 0;

        // create a new 2D array of coordinates to hold the mapIDs (array size set to map width and height)
        map = new int[mapWidth, mapHeight];

        // variable for tracking centre of each room; initialised to (0,0)
        Vector2Int newCentre = Vector2Int.zero;

        // main loop to populate rooms
        for (int i = 0; i < maxRooms; i++)
        {
            // generate some random width, height, x and y for each room
            int w = UnityEngine.Random.Range(minRoomWidth, maxRoomWidth + 1);
            int h = UnityEngine.Random.Range(minRoomHeight, maxRoomHeight + 1);
            // this will range from 1 to the width of the dungeon minus the width of the room generated above (remember x is the left hand coordinate of each room)
            int x = UnityEngine.Random.Range(1, mapWidth - w);
            int y = UnityEngine.Random.Range(1, mapHeight - h);

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

                // adds the coordinates of this room and the roomID (based on roomCount) to the map array
                createRoom(newRoom, _roomCount);

                // make a CORRIDOR only if a room already exists
                if (rooms.Count != 0)
                {
                    createCorridor(newRoom,rooms[rooms.Count-1]);
                }

                // add the new room to rooms list
                rooms.Add(newRoom);
            }
        }
        // work out where walls should be and set all the coordinates to the wallID
        createWalls();
        createDoors();
    }


    // sets all the coordinates of this room on the map to the roomID for this room (each room has a unique ID)
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

    // method to make CORRIDORS
    // sets all the coordinates of the corridors between rooms to the corridorID (all corridors have the same ID)
    public void createCorridor(Room newRoom, Room prevRoom)
    {
        // this first part creates a newCentre somewhere inside the newRoom
        // grabs .centre from newRoom and puts it into newCentre (so .centre isn't affected)
        Vector2Int newCentre = newRoom.centre;

        // create two offsets that are half the width and height of the newRoom
        int offsetW = newRoom.w / 2;
        int offsetH = newRoom.h / 2;
        // set the x value of newCentre to be somewhere along the x coordinates of newRoom
        newCentre.x = UnityEngine.Random.Range(newCentre.x - offsetW, newCentre.x + offsetW);
        // set the y value of newCentre to be somewhere along the y coordinates of newRoom
        newCentre.y = UnityEngine.Random.Range(newCentre.y - offsetH, newCentre.y + offsetH);

        // this second part creates a prevCentre somewhere inside the previous room
        // grab the centre of the last room added to the list
        Vector2Int prevCentre = prevRoom.centre;
        //create two offsets that are half the width and height of the previous room
        offsetW = prevRoom.w / 2;
        offsetH = prevRoom.h / 2;
        prevCentre.x = UnityEngine.Random.Range(prevCentre.x - offsetW, prevCentre.x + offsetW);
        prevCentre.y = UnityEngine.Random.Range(prevCentre.y - offsetH, prevCentre.y + offsetH);

        // assign corridorIDs to coordinates between rooms based on new centres
        // randomly start with horizontal or vertical corridors
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            hCorridor(prevCentre.x, newCentre.x, prevCentre.y);
            vCorridor(prevCentre.y, newCentre.y, newCentre.x);
        }
        else
        {
            vCorridor(prevCentre.y, newCentre.y, prevCentre.x);
            hCorridor(prevCentre.x, newCentre.x, newCentre.y);
        }
    }


    // make a horizontal corridor (y is fixed)
    private void hCorridor(int x1, int x2, int y)
    {
        //start at the most left hand room centre
        int startX = Mathf.Min(x1, x2);
        //end at the most right hand centre
        int endX = Mathf.Max(x1, x2);

        // set the roomID of corridor coordinates to -1
        for (int x = startX; x <= endX; x++)
        {
            // check if the space is empty
            // this way corridoors aren't made over existing rooms, only the space between them
            if (map[x, y] == 0)
            {
                // set corridor mapID to -1
                map[x, y] = -1;
            }
        }
    }

    // make a vertical corridor (x is fixed)
    private void vCorridor(int y1, int y2, int x)
    {
        int startY = Mathf.Min(y1, y2);
        int endY = Mathf.Max(y1, y2);

        for (int y = startY; y <= endY; y++)
        {
            if (map[x, y] == 0)
            {
                // set corridor mapID to -1
                map[x, y] = -1;
            }
        }
    }



    // creates WALLS wherever required    
    // runs through every coordinate on the map
    // sets the ID of all the walls on the map to the WallID (-2)
    private void createWalls()
    {
        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int y = 1; y < mapHeight - 1; y++)
            {
                if (testWallRequired(x, y))
                {
                    map[x, y] = -2;
                }
            }
        }
    }


    // returns true if the (x,y) coordinate is currently empty and has an adjacent (or diagonal) wall or corridor
    private bool testWallRequired(int x, int y)
    {
        // get the mapID for the given coordinates
        int v = map[x, y];
        //if v is already occupied (with wall (-2), corridor (-1) or room (>0)) exit method with false
        if (v != 0) return false;
        // otherwise v must be equal to 0 (so empty)
        // for the 3 x 3 box around x,y...
        for (int ix = x - 1; ix <= x + 1; ix++)
        {
            for (int iy = y - 1; iy <= y + 1; iy++)
            {
                // if any of the x,y coordinates around the position being tested are a room (1+) or corridor (-1) but not already a wall (-2) return true (wall required)
                if (map[ix, iy] != 0 && map[ix, iy] != -2)
                {
                    return true;
                }
            }
        }
        return false;
    }




    // adds DOORS to the first and last room
    // sets the ID of the doors to the DoorID (-3)
    private void createDoors()
    {
        // method to make door in first room
        // randomnly choose x or y walls
        if (UnityEngine.Random.Range(0, 2) == 0) 
            findDoorInXWalls(0);
        else
            findDoorInYWalls(0);

        // make door in last room
        if (UnityEngine.Random.Range(0, 2) == 0)
            findDoorInXWalls(rooms.Count - 1);
        else
            findDoorInYWalls(rooms.Count - 1);
    }


    // helper method for createDoors() method
    private void findDoorInXWalls(int rmNum)
    {
        Room room = rooms[rmNum];

        //check in case room is against bottom wall
        int bottom = room.y <= 1 ? 1 : 2;

        //check in case room is against top wall
        int top = room.y2 >= (mapHeight - 1) ? 0 : 1;

        int count = 0;
        while (true)
        {
            // generate random x along the walls of the room
            int wallX = UnityEngine.Random.Range(room.x, room.x2);

            // check if the beneath the wall is unassigned (background) or is the border
            if (map[wallX, room.y - bottom] == 0 || (room.y - 1) <= 0)
            {
                // set this coordinate to be a door
                map[wallX, room.y - 1] = -3;
                // change the door and doorMat variables in this room
                room.door.x = wallX;
                room.doorMat.x = wallX;
                // assign the edited room back to the room list
                try {
                    rooms[rmNum] = room;
                } catch (ArgumentOutOfRangeException e) {
                    Debug.Log(e.Message);
                    dungeonManager.makeDungeon();
                }
                break;
            }
            // check if above the wall is unassigned (background) or is the border
            else if (room.y2 >= mapHeight || map[wallX, room.y2 + top] == 0)
            {
                // set this coordinate to be a door
                map[wallX, room.y2] = -3;
                // change the door and doorMat variables in this room
                room.door.x = wallX;
                room.door.y = room.y2;
                room.doorMat.x = wallX;
                room.doorMat.y = room.y2-1;
                // assign the edited room back to the room list
                try {
                    rooms[rmNum] = room;
                }
                catch (ArgumentOutOfRangeException e) {
                    Debug.Log(e.Message);
                    dungeonManager.makeDungeon();
                }
                break;
            }

            count++;
            if (count >= 50)
            {
                //TODO: generate a new dungeon
                dungeonManager.makeDungeon();
            }
        }
    }

    // helper method for createDoors() method
    private void findDoorInYWalls(int rmNum)
    {
        Room room = rooms[rmNum];

        //check in case room is against left side wall
        int lSide = room.x <= 1 ? 1 : 2;

        //check in case room is against right side wall
        int rSide = room.x2 >= (mapWidth - 1) ? 0 : 1;

        int count = 0;
        while (true)
        {
            // generate random y along the walls of the room
            int wallY = UnityEngine.Random.Range(room.y, room.y2);

            // check if the left of the wall is unassigned (background)
            if (map[room.x - lSide, wallY] == 0 || (room.x - 1) <= 0)
            {
                // set this coordinate to be a door
                map[room.x - 1, wallY] = -3;
                // change the door and doorMat variables in this room
                room.door.y = wallY;
                room.doorMat.y = wallY;
                // assign the edited room back to the room list
                try {
                    rooms[rmNum] = room;
                } catch (ArgumentOutOfRangeException e) {
                    Debug.Log(e.Message);
                    dungeonManager.makeDungeon();
                }
                break;
            }
            // check if the right of the wall is unassigned (background)
            else if (room.x >= mapWidth || map[room.x2 + rSide, wallY] == 0)
            {
                // set this coordinate to be a door
                map[room.x2, wallY] = -3;
                // change the door and doorMat variables in this room
                room.door.y = wallY;
                room.door.x = room.x2;
                room.doorMat.y = wallY;
                room.doorMat.x = room.x2-1;
                // assign the edited room back to the room list
                try {
                    rooms[rmNum] = room;
                } catch (ArgumentOutOfRangeException e) {
                    Debug.Log(e.Message);
                    dungeonManager.makeDungeon();
                }
                break;
            }
            
            count++;
            if (count >= 50)
            {
                //TODO: generate a new dungeon
                dungeonManager.makeDungeon();
            }
        }
    }




    // RENDERS the appropriate textures/sprites for each of the map coordinates
    public void spawnMap()
    {
        // grab the instance of the Dungeon Manager
        if(dungeonManager == null)
        {
            dungeonManager = DungeonManager.Instance;
        }

        // create the base map with background tiles and border (wall) tiles only
        dungeonManager.createBaseMap(mapWidth, mapHeight, backgroundTileID, wallTileID);

        // create dictionary to hold the mapping of mapIDs to tileIDs/textures
        Dictionary<int, int> roomTilePairs = new Dictionary<int, int>();

        // set all Corridors to have the CorridorTileID
        roomTilePairs.Add(-1, corridorTileID);

        // set all Walls to have the WallTileID
        roomTilePairs.Add(-2, wallTileID);

        // set all Doors to have the DoorTileID
        roomTilePairs.Add(-3, doorTileID);


        // Set basic floor tiles for each room if ticked, otherwise random tile types for each room
        for (int i = 1; i <= _roomCount; i++)
        {
            roomTilePairs.Add(i, basicFloorTilesOnly ? basicFloorTileID : validIDs[UnityEngine.Random.Range(0, validIDs.Count)]);
        }


        // Render the associated tile type/texture for each map coordinate
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // if mapID is a corridor or room, isWall = false
                if (map[x, y] == -1 || map[x, y] > 0)
                {   
                    // grab the mapID from map[x,y] and check what tileID it's associated with in roomTilePairs
                    // send the (x,y) coordinates as a Vector2Int, along with the tileID, to the createTile method in the DungeonManager
                    dungeonManager.createTile(roomTilePairs[map[x, y]], new Vector2Int(x, y), false);
                }
                // if mapID is a wall or door, isWall = true
                else if (map[x, y] < -1)
                {
                    dungeonManager.createTile(roomTilePairs[map[x, y]], new Vector2Int(x, y), true);               
                }
            }
        }

        // set finalDoor variable in the assigned tile in the final room
        dungeonManager.map[rooms[rooms.Count - 1].door.x, rooms[rooms.Count - 1].door.y].GetComponent<DungeonTile>().isFinalDoor = true;
    }



    // PRINTS out all the map IDs
    public void printRooms()
    {
        string result = "";
        for (int y = mapHeight-1; y >= 0; y--)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                result += map[x, y];
            }
            result += "\n";
        }
        Debug.Log(result);
    }



    // creates a map with one room that fills the whole map
    public void createEmptyMap()
    {
        map = new int[mapWidth, mapHeight];
        createRoom(new Room(0, 0, mapWidth, mapHeight), 0);
    }

}
