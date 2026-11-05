using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private ObjectPool objectPool;

    [Header("Pool IDs")]
    [SerializeField] private string roadPoolId = "RoadTile";
    [SerializeField] private string coinPoolId = "Coin";
    [SerializeField] private string[] obstaclePoolIds = { "Barrier", "Box", "Wall" };
    [SerializeField] private string[] decorationPoolIds = { "Tree", "Rock" };

    [Header("Lanes")]
    [SerializeField] private float laneDistance = 3f;

    [Header("Road Generation")]
    [SerializeField] private float roadLength = 30f;
    [SerializeField] private float roadAheadDistance = 150f;
    [SerializeField] private float cleanupDistance = 45f;

    [Header("Gameplay Spawning")]
    [SerializeField] private float gameplaySpawnAheadDistance = 110f;
    [SerializeField] private Vector2 spawnSpacingRange = new Vector2(9f, 16f);
    [SerializeField, Range(0f, 1f)] private float obstacleSpawnChance = 0.78f;
    [SerializeField, Range(0f, 1f)] private float coinSpawnChance = 0.68f;
    [SerializeField] private Vector2Int coinLineCountRange = new Vector2Int(3, 6);
    [SerializeField] private float coinSpacing = 2.2f;
    [SerializeField] private float coinHeight = 1.3f;
    [SerializeField] private float obstacleHeightOffset = 0f;

    [Header("Decorations")]
    [SerializeField, Range(0f, 1f)] private float decorationChancePerSide = 0.42f;
    [SerializeField] private Vector2 decorationXRange = new Vector2(6.5f, 11f);

    private readonly List<GameObject> activeRoads = new List<GameObject>();
    private readonly List<GameObject> activeObstacles = new List<GameObject>();
    private readonly List<GameObject> activeCoins = new List<GameObject>();
    private readonly List<GameObject> activeDecorations = new List<GameObject>();

    private float nextRoadZ;
    private float nextGameplayZ;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (objectPool == null)
        {
            objectPool = FindObjectOfType<ObjectPool>();
        }

        BuildStartingRoad();
    }

    private void Update()
    {
        if (player == null || objectPool == null)
        {
            return;
        }

        GenerateRoadAhead();
        CleanupBehindPlayer(activeRoads);
        CleanupBehindPlayer(activeObstacles);
        CleanupBehindPlayer(activeCoins);
        CleanupBehindPlayer(activeDecorations);

        if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
        {
            GenerateGameplayAhead();
        }
    }

    private void BuildStartingRoad()
    {
        if (player == null || objectPool == null)
        {
            return;
        }

        nextRoadZ = Mathf.Floor(player.position.z / roadLength) * roadLength;
        nextGameplayZ = player.position.z + 35f;

        GenerateRoadAhead();
    }

    private void GenerateRoadAhead()
    {
        while (nextRoadZ < player.position.z + roadAheadDistance)
        {
            SpawnRoadSegment(nextRoadZ);
            nextRoadZ += roadLength;
        }
    }

    private void GenerateGameplayAhead()
    {
        while (nextGameplayZ < player.position.z + gameplaySpawnAheadDistance)
        {
            SpawnGameplayChunk(nextGameplayZ);
            nextGameplayZ += Random.Range(spawnSpacingRange.x, spawnSpacingRange.y);
        }
    }

    private void SpawnRoadSegment(float zPosition)
    {
        GameObject road = objectPool.SpawnFromPool(roadPoolId, new Vector3(0f, 0f, zPosition), Quaternion.identity);
        if (road != null)
        {
            activeRoads.Add(road);
        }

        SpawnDecorationsForSegment(zPosition);
    }

    private void SpawnGameplayChunk(float zPosition)
    {
        bool spawnObstacle = obstaclePoolIds.Length > 0 && Random.value <= obstacleSpawnChance;
        bool spawnCoins = Random.value <= coinSpawnChance;
        int obstacleLane = Random.Range(0, 3);

        if (spawnObstacle)
        {
            string obstacleId = obstaclePoolIds[Random.Range(0, obstaclePoolIds.Length)];
            Vector3 obstaclePosition = new Vector3(LaneToX(obstacleLane), obstacleHeightOffset, zPosition);
            GameObject obstacle = objectPool.SpawnFromPool(obstacleId, obstaclePosition, Quaternion.identity);

            if (obstacle != null)
            {
                activeObstacles.Add(obstacle);
            }
        }

        if (spawnCoins)
        {
            int coinLane = Random.Range(0, 3);
            if (spawnObstacle && coinLane == obstacleLane)
            {
                coinLane = (coinLane + Random.Range(1, 3)) % 3;
            }

            int coinCount = Random.Range(coinLineCountRange.x, coinLineCountRange.y + 1);
            for (int i = 0; i < coinCount; i++)
            {
                Vector3 coinPosition = new Vector3(LaneToX(coinLane), coinHeight, zPosition + i * coinSpacing);
                GameObject coin = objectPool.SpawnFromPool(coinPoolId, coinPosition, Quaternion.identity);

                if (coin != null)
                {
                    activeCoins.Add(coin);
                }
            }
        }
    }

    private void SpawnDecorationsForSegment(float segmentZ)
    {
        if (decorationPoolIds.Length == 0)
        {
            return;
        }

        TrySpawnDecoration(segmentZ, -1);
        TrySpawnDecoration(segmentZ, 1);
    }

    private void TrySpawnDecoration(float segmentZ, int side)
    {
        if (Random.value > decorationChancePerSide)
        {
            return;
        }

        string decorationId = decorationPoolIds[Random.Range(0, decorationPoolIds.Length)];
        float x = Random.Range(decorationXRange.x, decorationXRange.y) * side;
        float z = segmentZ + Random.Range(4f, roadLength - 4f);
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject decoration = objectPool.SpawnFromPool(decorationId, new Vector3(x, 0f, z), rotation);
        if (decoration != null)
        {
            activeDecorations.Add(decoration);
        }
    }

    private void CleanupBehindPlayer(List<GameObject> activeObjects)
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject activeObject = activeObjects[i];

            if (activeObject == null || !activeObject.activeInHierarchy)
            {
                activeObjects.RemoveAt(i);
                continue;
            }

            if (activeObject.transform.position.z < player.position.z - cleanupDistance)
            {
                objectPool.ReturnToPool(activeObject);
                activeObjects.RemoveAt(i);
            }
        }
    }

    private float LaneToX(int lane)
    {
        return (Mathf.Clamp(lane, 0, 2) - 1) * laneDistance;
    }
}
