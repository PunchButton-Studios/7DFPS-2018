using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    private const int CHUNK_SIZE = 8;

    private MapTile[] map;
    private int mapSize;

    public ChunkHandler chunkHandler;
    public GameObject floorPrefab, wallPrefab, ceilingPrefab, orePrefab, holePrefab;
    public Transform playerBase, player;

    public float scale = 1.0f;

    public Vector2Int startingRoomSize = new Vector2Int(3, 3);

    public int minOrigins = 3, maxOrigins = 5;
    [Range(0, 1)] public float pathStopChance = 0.0025f;
    [Range(0, 1)] public float pathBendChance = 0.05f;
    [Range(0, 1)] public float pathSplitChance = 0.07f;
    [Range(0, 1)] public float roomChance = 0.02f;
    public int minRoomSize = 3, maxRoomSize = 8;
    [Range(0, 1)] public float roomDoorwayChance = 0.35f;
    [Range(0, 1)] public float roomOreChance = 0.05f;
    [Range(0, 1)] public float roomHoleChance = 0.01f;

    public event WorldgenComplete worldGenCompleteEvent;

    private void Awake()
    {
        GameManager.Main.saveGameEvent += OnSaveGame;
        GameManager.Main.loadGameEvent += OnGameLoaded;
    }

    private void OnGameLoaded(SaveData saveData)
    {
        map = System.Array.ConvertAll(saveData.worldData.map, (b) => (MapTile)b);
        mapSize = saveData.worldData.mapSize;
        StartCoroutine(PlaceTiles(saveData.worldData.extraData));
    }

    private void OnSaveGame(SaveData saveData)
    {
        saveData.worldData.map = System.Array.ConvertAll(map, (mt) => (byte)mt);
        saveData.worldData.mapSize = mapSize;
    }

    public void GenerateLevel(int size)
    {
        Vector2Int startPos = new Vector2Int(Random.Range(0, size), Random.Range(0, size));
        GenerateLevel(size, startPos, true);
    }

    public void GenerateLevel(int size, Vector2Int startPos, bool startPosBaseRoom = false)
    {
        map = new MapTile[size * size];
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

            GenerateRoom(startPos, startingRoomSize, ref queuedTiles, true, true);
            Vector2Int roomCenter = new Vector2Int((int)(startingRoomSize.x * 0.5), (int)(startingRoomSize.y * 0.5));
            Vector3 playerPosition = new Vector3(startPos.x + roomCenter.x, 0, startPos.y + roomCenter.y) * scale;
            playerPosition.y = player.position.y;
            player.position = playerPosition;
            playerPosition.y = playerBase.position.y;
            playerBase.position = playerPosition;
        }

        Vector2Int[] origins = new Vector2Int[Random.Range(minOrigins, maxOrigins)];
        for (int i = 0; i < origins.Length; i++)
        {
            if (i == 0)
                origins[i] = startPos;
            else
                origins[i] = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));

            Vector2Int direction = new Vector2Int(0, 0);

            if (Random.value < 0.5f)
                direction.x = (origins[i].x < size * 0.5) ? 1 : -1;
            else
                direction.y = (origins[i].y < size * 0.5) ? 1 : -1;

            Debug.Log($"{origins[i]} ~ ({origins[i].x + (origins[i].y * mapSize)})");
            map[origins[i].x + (origins[i].y * mapSize)] = i == 0 ? MapTile.EmptyTile : MapTile.HoleTile;
            queuedTiles.Enqueue(new QueuedTile(origins[i] + direction, direction));
        }

        while (queuedTiles.Count > 0)
        {
            QueuedTile queuedTile = queuedTiles.Dequeue();
            GeneratePath(queuedTile.pos, queuedTile.direction, ref queuedTiles);
        }

        ConnectOrigins(origins);

        StartCoroutine(PlaceTiles());
    }

    private void ConnectOrigins(Vector2Int[] origins)
    {
        for(int i = 0; i < origins.Length; i++)
        {
            Vector2Int difference = origins[(i + 1) % origins.Length] - origins[i];
            for(int x = 0; x < Mathf.Abs(difference.x); x++)
            {
                Vector2Int pos = origins[i] + new Vector2Int((int)Mathf.Sign(difference.x) * x, 0);
                int mapIndex = pos.x + (pos.y * mapSize);
                if (mapIndex > 0 && mapIndex < map.Length && map[mapIndex] == MapTile.Void)
                    map[mapIndex] = MapTile.EmptyTile;
            }

            for (int y = 0; y < Mathf.Abs(difference.y); y++)
            {
                Vector2Int pos = origins[(i + 1) % origins.Length] + new Vector2Int(0, (int)Mathf.Sign(difference.y) * -y);
                int mapIndex = pos.x + (pos.y * mapSize);
                if (mapIndex > 0 && mapIndex < map.Length && map[mapIndex] == MapTile.Void)
                    map[mapIndex] = MapTile.EmptyTile;
            }
        }
    }

    private void GeneratePath(Vector2Int pos, Vector2Int direction, ref Queue<QueuedTile> queuedTiles)
    {
        int i = pos.x + (pos.y * mapSize);
        if (i < 0 || i >= map.Length || map[i] != MapTile.Void)
            return;

        map[i] = MapTile.EmptyTile;

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
                if (!IsOutOfBounds(splitPosition))
                    queuedTiles.Enqueue(new QueuedTile(splitPosition, splitDirection));

                if(Random.value < pathSplitChance)
                {
                    splitDirection = RotateVector2Int(newDirection, !clockwiseSplit);
                    splitPosition = pos + splitDirection;
                    if (!IsOutOfBounds(splitPosition))
                        queuedTiles.Enqueue(new QueuedTile(splitPosition, splitDirection));
                }
            }

            if(Random.value < roomChance)
            {
                Vector2Int roomSize = new Vector2Int(
                    Random.Range(minRoomSize, maxRoomSize),
                    Random.Range(minRoomSize, maxRoomSize)
                    );

                Vector2Int offset = new Vector2Int(
                    Random.Range(0, roomSize.x),
                    Random.Range(0, roomSize.y)
                    );

                GenerateRoom(pos - offset, roomSize, ref queuedTiles);
            }

            int attempts = 0;
            while (IsOutOfBounds(nextPosition) && attempts < 4)
            {
                newDirection = RotateVector2Int(newDirection, true);
                nextPosition = pos + newDirection;
                attempts++;
            }

            if (!IsOutOfBounds(nextPosition))
                queuedTiles.Enqueue(new QueuedTile(nextPosition, newDirection));
        }
    }

    private void GenerateRoom(Vector2Int pos, Vector2Int size, ref Queue<QueuedTile> queuedTiles, bool mustHaveDoorway = false, bool allEmpty = false)
    {
        for(int lx = 0; lx < size.x; lx++)
        {
            for(int ly = 0; ly < size.y; ly++)
            {
                int x = pos.x + lx;
                int y = pos.y + ly;

                int i = x + (y * mapSize);

                if (i < map.Length && i > 0 && map[i] == MapTile.Void)
                {
                    map[i] = MapTile.EmptyTile;

                    if (!allEmpty)
                    {
                        if (Random.value < roomHoleChance)
                            map[i] = MapTile.HoleTile;
                        else if (Random.value < roomOreChance)
                            map[i] = MapTile.OreTile;
                    }
                }
            }
        }

        Vector2Int halfSize = new Vector2Int((int)(size.x * 0.5), (int)(size.y * 0.5));
        Vector2Int center = pos + halfSize;

        bool hasDoorway = false;
        int forcedDoorway = Random.Range(0, 3);

        Vector2Int direction = new Vector2Int(1, 0);
        for(int i = 0; i < 4; i++)
        {
            if(Random.value < roomDoorwayChance || (!hasDoorway && mustHaveDoorway && (i == forcedDoorway || i == 3)))
            {
                Vector2Int newPos = center + direction * new Vector2Int(halfSize.x + 1, halfSize.y + 1);
                if (!IsOutOfBounds(newPos))
                {
                    queuedTiles.Enqueue(new QueuedTile(newPos, direction));
                    hasDoorway = true;
                }
            }
            direction = RotateVector2Int(direction, true);
        }
    }

    private bool IsOutOfBounds(Vector2Int pos) => pos.x < 0 || pos.x >= mapSize || pos.y < 0 || pos.y >= mapSize;

    private Vector2Int RotateVector2Int(Vector2Int v2Int, bool clockwise)
    {
        if (clockwise)
            return new Vector2Int(v2Int.y, -v2Int.x);
        else
            return new Vector2Int(-v2Int.y, v2Int.x);
    }

    private IEnumerator PlaceTiles(byte[] saveData = null)
    {
        GameManager.isLoading = true;
        Time.timeScale = 0.0f;

        Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

        for(int y = 0; y < mapSize; y++)
        {
            for(int x = 0; x < mapSize; x++)
            {
                Vector2Int chunk = new Vector2Int(x / CHUNK_SIZE, y / CHUNK_SIZE);
                int mapIndex = x + (y * mapSize);
                MapTile mapTile = map[mapIndex];
                if (mapTile != MapTile.Void)
                {
                    Vector3 worldPosition = new Vector3(x, 0, y) * scale;
                    GameObject floorObject = Instantiate(floorPrefab, worldPosition, floorPrefab.transform.rotation);
                    GameObject ceilingObject = Instantiate(ceilingPrefab, worldPosition + Vector3.up * scale, ceilingPrefab.transform.rotation);
                    if (!chunks.ContainsKey(chunk))
                        chunks.Add(chunk, new Chunk());
                    chunks[chunk].meshFilters.AddRange(floorObject.GetComponentsInChildren<MeshFilter>());
                    chunks[chunk].meshFilters.AddRange(ceilingObject.GetComponentsInChildren<MeshFilter>());
                    PlaceWalls(x, y, chunks[chunk]);

                    byte tileSaveData = 0;
                    if (saveData != null && mapIndex < saveData.Length)
                        tileSaveData = saveData[mapIndex];
                    PopulateTile(mapTile, new Vector2Int(x, y), tileSaveData);
                }
            }
            yield return new WaitForEndOfFrame();
        }

        GameObject chunkHolder = new GameObject("Chunks");
        chunkHolder.transform.parent = chunkHandler.transform;
        foreach (Vector2Int key in chunks.Keys)
        {
            CombineMeshes(chunks[key], key, chunkHolder.transform);
            yield return new WaitForEndOfFrame();
        }

        GameManager.isLoading = false;
        Time.timeScale = 1.0f;
        worldGenCompleteEvent?.Invoke();
    }

    private void PlaceWalls(int x, int y, Chunk chunk)
    {
        Vector2Int pos = new Vector2Int(x, y);
        Vector2Int direction = new Vector2Int(1, 0);
        for(int i = 0; i < 4; i++)
        {
            Vector2Int checkPos = pos + direction;
            int mapIndex = checkPos.x + (checkPos.y * mapSize);
            if(IsOutOfBounds(checkPos) || map[mapIndex] == MapTile.Void)
            {
                Vector3 directionV3 = new Vector3(direction.x, 0, direction.y);
                Quaternion rotation = Quaternion.LookRotation(directionV3);

                GameObject wallObject = Instantiate(
                    wallPrefab,
                    new Vector3(pos.x, 0, pos.y) * scale,
                    rotation
                    );

                chunk.meshFilters.AddRange(wallObject.GetComponentsInChildren<MeshFilter>());
                chunk.destroyObjects.Add(wallObject);
            }

            direction = RotateVector2Int(direction, true);
        }
    }

    private void PopulateTile(MapTile mapTile, Vector2Int pos, byte saveData)
    {
        Vector2 randomOffset = new Vector2(
                Mathf.PerlinNoise(pos.x * 53.791f, pos.y * 29.314f) * Mathf.PerlinNoise((pos.y + 23.4f) * 11.172f, (pos.x + 62.6f) * 86.148f),
                Mathf.PerlinNoise(pos.x * -21.218f, pos.y * -58.057f) * Mathf.PerlinNoise((pos.y - 23.4f) * 62.127f, (pos.x - 62.6f) * 10.150f)
                ) * scale * 0.35f;
        Quaternion randomRotation = Quaternion.Euler(0, (Mathf.PerlinNoise(pos.y * 83.163f, pos.x * -65.273f) * Mathf.PerlinNoise(pos.y * 3.12f, pos.x * 7.31f)) * 360, 0);

        if (mapTile == MapTile.OreTile)
        {
            Vector3 position = new Vector3(
                pos.x * scale - randomOffset.x,
                0,
                pos.y * scale - randomOffset.y
                );

            GameObject oreObject = Instantiate(orePrefab, position, randomRotation);
            oreObject.transform.parent = chunkHandler.transform;
            IWorldGenObject worldGenObject = oreObject.GetComponent<IWorldGenObject>();
            if (worldGenObject != null)
            {
                worldGenObject.Id = (pos.x + (pos.y * mapSize));
                worldGenObject.OnWorldLoaded(saveData);
            }
        }
        else if(mapTile == MapTile.HoleTile)
        {
            Vector3 position = new Vector3(
                pos.x * scale,
                holePrefab.transform.position.y,
                pos.y * scale
                );
            GameObject holeObject = Instantiate(holePrefab, position, randomRotation);
            holeObject.transform.parent = chunkHandler.transform;
            holeObject.GetComponent<ActivatableHole>().tilePos = pos;
        }
    }

    private void CombineMeshes(Chunk chunk, Vector2Int pos, Transform chunkHolder)
    {
        CombineInstance[] combineInstances = new CombineInstance[chunk.meshFilters.Count];
        List<Material> materials = new List<Material>();
        for(int i = 0; i < chunk.meshFilters.Count; i++)
        {
            combineInstances[i].mesh = chunk.meshFilters[i].sharedMesh;
            combineInstances[i].transform = chunk.meshFilters[i].transform.localToWorldMatrix;

            MeshRenderer meshRenderer = chunk.meshFilters[i].GetComponent<MeshRenderer>();
            foreach(Material mat in meshRenderer.materials)
            {
                if (!materials.Contains(mat))
                    materials.Add(mat);
            }

            Destroy(chunk.meshFilters[i].gameObject);
        }

        GameObject chunkObject = new GameObject($"Chunk ({pos.ToString()})");
        MeshRenderer chunkRenderer = chunkObject.AddComponent<MeshRenderer>();
        chunkRenderer.materials = materials.ToArray();
        MeshFilter chunkMesh = chunkObject.AddComponent<MeshFilter>();
        chunkMesh.mesh = new Mesh();
        chunkMesh.mesh.CombineMeshes(combineInstances, false);
        MeshCollider collider = chunkObject.AddComponent<MeshCollider>();
        collider.sharedMesh = chunkMesh.sharedMesh;
        chunkObject.transform.parent = chunkHolder;

        foreach (GameObject destroyObject in chunk.destroyObjects)
            Destroy(destroyObject);

        chunkHandler.scale = CHUNK_SIZE * scale;
        chunkHandler.chunkRenderers.Add(pos, chunkRenderer);

        StaticBatchingUtility.Combine(chunkHandler.gameObject);
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

    private class Chunk
    {
        public List<MeshFilter> meshFilters = new List<MeshFilter>();
        public List<GameObject> destroyObjects = new List<GameObject>();
    }

    public enum MapTile : byte
    {
        Void,
        EmptyTile,
        BaseReserved,
        OreTile,
        HoleTile,
    }
}