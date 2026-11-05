using System.Collections.Generic;
using UnityEngine;

public interface IPooledObject
{
    void OnObjectSpawn();
    void OnObjectDespawn();
}

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class PoolConfig
    {
        public string id;
        public GameObject prefab;
        public int initialSize = 12;
        public bool expandable = true;
    }

    [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();

    private readonly Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
    private readonly Dictionary<string, PoolConfig> configLookup = new Dictionary<string, PoolConfig>();
    private readonly Dictionary<string, Transform> containerLookup = new Dictionary<string, Transform>();

    private void Awake()
    {
        BuildPools();
    }

    public GameObject SpawnFromPool(string poolId, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(poolId))
        {
            Debug.LogWarning($"Pool '{poolId}' does not exist.");
            return null;
        }

        if (pools[poolId].Count == 0)
        {
            PoolConfig config = configLookup[poolId];

            if (!config.expandable)
            {
                return null;
            }

            CreateObjectForPool(config);
        }

        GameObject pooledObject = pools[poolId].Dequeue();
        pooledObject.transform.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        NotifySpawn(pooledObject);

        return pooledObject;
    }

    public void ReturnToPool(GameObject pooledObject)
    {
        if (pooledObject == null)
        {
            return;
        }

        PooledObject marker = pooledObject.GetComponent<PooledObject>();

        if (marker == null || string.IsNullOrWhiteSpace(marker.PoolId) || !pools.ContainsKey(marker.PoolId))
        {
            pooledObject.SetActive(false);
            return;
        }

        NotifyDespawn(pooledObject);
        pooledObject.SetActive(false);
        pooledObject.transform.SetParent(containerLookup[marker.PoolId]);
        pools[marker.PoolId].Enqueue(pooledObject);
    }

    private void BuildPools()
    {
        foreach (PoolConfig config in poolConfigs)
        {
            if (config == null || string.IsNullOrWhiteSpace(config.id) || config.prefab == null)
            {
                Debug.LogWarning("Skipped invalid pool configuration.");
                continue;
            }

            if (pools.ContainsKey(config.id))
            {
                Debug.LogWarning($"Duplicate pool id '{config.id}' skipped.");
                continue;
            }

            Transform container = new GameObject($"{config.id}_Pool").transform;
            container.SetParent(transform);

            pools[config.id] = new Queue<GameObject>();
            configLookup[config.id] = config;
            containerLookup[config.id] = container;

            for (int i = 0; i < config.initialSize; i++)
            {
                CreateObjectForPool(config);
            }
        }
    }

    private GameObject CreateObjectForPool(PoolConfig config)
    {
        GameObject instance = Instantiate(config.prefab, containerLookup[config.id]);
        instance.SetActive(false);

        PooledObject marker = instance.GetComponent<PooledObject>();
        if (marker == null)
        {
            marker = instance.AddComponent<PooledObject>();
        }

        marker.Configure(this, config.id);
        pools[config.id].Enqueue(instance);

        return instance;
    }

    private static void NotifySpawn(GameObject pooledObject)
    {
        MonoBehaviour[] behaviours = pooledObject.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IPooledObject poolable)
            {
                poolable.OnObjectSpawn();
            }
        }
    }

    private static void NotifyDespawn(GameObject pooledObject)
    {
        MonoBehaviour[] behaviours = pooledObject.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IPooledObject poolable)
            {
                poolable.OnObjectDespawn();
            }
        }
    }
}
