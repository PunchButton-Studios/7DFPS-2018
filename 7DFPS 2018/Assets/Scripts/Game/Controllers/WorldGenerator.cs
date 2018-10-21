using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public Room[] rooms;

    public float roomScale = 1.0f;

    public void GenerateLevel(int size)
    {
        Vector2Int startPos = new Vector2Int(0, 0);
        GenerateLevel(size, startPos, true);
    }

    public void GenerateLevel(int size, Vector2Int startPos, bool startPosBaseRoom = false)
    {
        Dictionary<Vector2Int, bool> map = new Dictionary<Vector2Int, bool>();
        bool hasEndPoint = false;

        Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

        if(startPosBaseRoom)
        {
            Room[] baseRooms = rooms.Where((r) => r.canPlaceBase).ToArray();
            WeightedRandom<Room>.Item[] baseItems = new WeightedRandom<Room>.Item[baseRooms.Length];
            for (int i = 0; i < baseItems.Length; i++)
                baseItems[i] = new WeightedRandom<Room>.Item(baseRooms[i], baseRooms[i].weight);
            PlaceRandomRoom(startPos, baseItems, ref map, ref roomQueue);
        }

        WeightedRandom<Room>.Item[] items = new WeightedRandom<Room>.Item[rooms.Length];
        for (int i = 0; i < items.Length; i++)
            items[i] = new WeightedRandom<Room>.Item(rooms[i], rooms[i].weight);

        while(roomQueue.Count > 0)
        {
            Vector2Int pos = roomQueue.Dequeue();
            if(pos.magnitude < size)
                PlaceRandomRoom(pos, items, ref map, ref roomQueue);
        }
    }

    private void PlaceRandomRoom(Vector2Int pos, IEnumerable<WeightedRandom<Room>.Item> collection, ref Dictionary<Vector2Int, bool> map, ref Queue<Vector2Int> queue)
    {
        if (map.ContainsKey(pos))
            return;

        Room selectedRoom = null;
        Vector2Int size = new Vector2Int();
        while(selectedRoom == null)
        {
            selectedRoom = WeightedRandom<Room>.GetRandom(collection);
            size = GetSize(selectedRoom.size);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if(map.ContainsKey(pos + new Vector2Int(x, y)))
                    {
                        selectedRoom = null;
                        break;
                    }
                }
                if (selectedRoom == null)
                    break;
            }
        }
        Vector3 worldPosition = new Vector3(pos.x * roomScale, 0, pos.y * roomScale);
        Instantiate(selectedRoom.roomPrefab, worldPosition, Quaternion.identity);
        
        for(int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
                map.Add(pos + new Vector2Int(x, y), true);
        }

        Vector2Int center = GetCenter(selectedRoom.size);
        int offset = GetNextRoomOffset(selectedRoom.size);
        for(int s = 0; s < 4; s++)
        {
            Room.Sides side = (Room.Sides)(1 << s);
            if (selectedRoom.doorways.HasFlag(side))
            {
                Vector2Int direction = GetDirection(side);
                Vector2Int nextRoom = pos + center + (direction * offset);
                if(!map.ContainsKey(nextRoom))
                    queue.Enqueue(nextRoom);
            }
        }
    }

    private Vector2Int GetSize(Room.RoomSize roomSize)
    {
        switch(roomSize)
        {
            case Room.RoomSize.Big:
                return new Vector2Int(3, 3);
            default:
                return new Vector2Int(1, 1);
        }
    }

    private Vector2Int GetCenter(Room.RoomSize roomSize)
    {
        switch(roomSize)
        {
            case Room.RoomSize.Big:
                return new Vector2Int(1, 1);
            default:
                return new Vector2Int(0, 0);
        }
    }

    private int GetNextRoomOffset(Room.RoomSize roomSize)
    {
        switch(roomSize)
        {
            case Room.RoomSize.Big:
                return 2;
            default:
                return 1;
        }
    }

    private Vector2Int GetDirection(Room.Sides side)
    {
        switch(side)
        {
            case Room.Sides.North:
                return new Vector2Int(0, 1);
            case Room.Sides.South:
                return new Vector2Int(0, -1);
            case Room.Sides.East:
                return new Vector2Int(1, 0);
            case Room.Sides.West:
                return new Vector2Int(-1, 0);
            default:
                return new Vector2Int();
        }
    }
}