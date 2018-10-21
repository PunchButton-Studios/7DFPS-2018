using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    private bool[] map;
    private int mapSize;

    public GameObject floorPrefab, wallPrefab;

    public float scale = 1.0f;

    [Range(0, 1)] public float pathStopChance = 0.0025f;
    [Range(0, 1)] public float pathBendChance = 0.05f;
    [Range(0, 1)] public float pathSplitChance = 0.07f;

    private void Awake()
    {
        GameManager.Main.saveGameEvent += OnSaveGame;
        GameManager.Main.loadGameEvent += OnGameLoaded;
    }

    private void OnGameLoaded(SaveData saveData)
    {
        map = saveData.worldData.map;
        PlaceTiles(saveData.worldData.mapSize);
    }

    private void OnSaveGame(SaveData saveData)
    {
        saveData.worldData.map = map;
        saveData.worldData.mapSize = mapSize;
    }

    public void GenerateLevel(int size)
    {
        Vector2Int startPos = new Vector2Int(Random.Range(0, size - 1), Random.Range(0, size - 1));
        GenerateLevel(size, startPos, true);
    }

    public void GenerateLevel(int size, Vector2Int startPos, bool startPosBaseRoom = false)
    {
        map = new bool[size * size];
        this.mapSize = size;
        Queue<QueuedTile> queuedTiles = new Queue<QueuedTile>();

        if(startPosBaseRoom)
        {
            if (startPos.x > (size - 3))
                startPos.x -= 3;
            else if (startPos.x < 3)
                startPos.x += 3;

            if (startPos.y > (size - 3))
                startPos.y -= 3;
            else if (startPos.y < 3)
                startPos.y += 3;

            //GenerateRoom(startPos, new Vector2Int(3, 3), size);
        }

        Vector2Int direction = new Vector2Int(0, 0);

        if (Random.value < 0.5f)
            direction.x = (startPos.x < size * 0.5) ? 1 : -1;
        else
            direction.y = (startPos.y < size * 0.5) ? 1 : -1;

        queuedTiles.Enqueue(new QueuedTile(startPos, direction));

        while (queuedTiles.Count > 0)
        {
            QueuedTile queuedTile = queuedTiles.Dequeue();
            GeneratePath(queuedTile.pos, queuedTile.direction, size, ref queuedTiles);
        }

        PlaceTiles(size);
    }

    private void GeneratePath(Vector2Int pos, Vector2Int direction, int mapSize, ref Queue<QueuedTile> queuedTiles)
    {
        int i = pos.x + (pos.y * mapSize);
        if (i < 0 || i >= map.Length || map[i])
            return;

        map[i] = true;

        if(Random.value >= pathStopChance)
        {
            Vector2Int newDirection = direction;
            Vector2Int nextPosition = pos + newDirection;

            if (Random.value < pathBendChance)
            {
                newDirection = RotateVector2Int(newDirection, Random.value < 0.5f);
                nextPosition = pos + newDirection;
            }

            if(Random.value < pathSplitChance)
            {
                bool clockwiseSplit = Random.value < 0.5f;
                Vector2Int splitDirection = RotateVector2Int(newDirection, clockwiseSplit);
                Vector2Int splitPosition = pos + splitDirection;
                if (!IsOutOfBounds(splitPosition, mapSize))
                    queuedTiles.Enqueue(new QueuedTile(splitPosition, splitDirection));

                if(Random.value < pathSplitChance)
                {
                    splitDirection = RotateVector2Int(newDirection, !clockwiseSplit);
                    splitPosition = pos + splitDirection;
                    if (!IsOutOfBounds(splitPosition, mapSize))
                        queuedTiles.Enqueue(new QueuedTile(splitPosition, splitDirection));
                }
            }

            int attempts = 0;
            while (IsOutOfBounds(nextPosition, mapSize) && attempts < 4)
            {
                newDirection = RotateVector2Int(newDirection, true);
                nextPosition = pos + newDirection;
                attempts++;
            }

            if (!IsOutOfBounds(nextPosition, mapSize))
                queuedTiles.Enqueue(new QueuedTile(nextPosition, newDirection));
        }
    }

    private void GenerateRoom(Vector2Int pos, Vector2Int size, int mapSize)
    {
        for(int lx = 0; lx < size.x; lx++)
        {
            for(int ly = 0; ly < size.y; ly++)
            {
                int x = pos.x + lx;
                int y = pos.y + ly;

                int i = x + (y * mapSize);

                if (i < map.Length && i > 0)
                    map[i] = true;
            }
        }
    }

    private bool IsOutOfBounds(Vector2Int pos, int mapSize) => pos.x < 0 || pos.x >= mapSize || pos.y < 0 || pos.y >= mapSize;

    private Vector2Int RotateVector2Int(Vector2Int v2Int, bool clockwise)
    {
        if (clockwise)
            return new Vector2Int(v2Int.y, -v2Int.x);
        else
            return new Vector2Int(-v2Int.y, v2Int.x);
    }

    private void PlaceTiles(int mapSize)
    {
        for(int y = 0; y < mapSize; y++)
        {
            for(int x = 0; x < mapSize; x++)
            {
                if (map[x + (y * mapSize)])
                {
                    Vector3 worldPosition = new Vector3(x, 0, y) * scale;
                    Instantiate(floorPrefab, worldPosition, Quaternion.identity);
                }
            }
        }
    }

    private struct QueuedTile
    {
        public Vector2Int pos, direction;

        public QueuedTile(Vector2Int position, Vector2Int direction)
        {
            this.pos = position;
            this.direction = direction;
        }
    }
}