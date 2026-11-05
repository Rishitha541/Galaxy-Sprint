using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CoinSystem : MonoBehaviour, IPooledObject
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private float rotationSpeed = 150f;

    private bool collected;
    private PooledObject pooledObject;

    private void Awake()
    {
        pooledObject = GetComponent<PooledObject>();

        Collider coinCollider = GetComponent<Collider>();
        coinCollider.isTrigger = true;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
        {
            return;
        }

        collected = true;
        GameManager.Instance?.AddCoin(coinValue);

        if (pooledObject != null)
        {
            pooledObject.ReturnToPool();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnObjectSpawn()
    {
        if (pooledObject == null)
        {
            pooledObject = GetComponent<PooledObject>();
        }

        collected = false;
    }

    public void OnObjectDespawn()
    {
        collected = false;
    }
}
