using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public string PoolId { get; private set; }

    private ObjectPool ownerPool;

    public void Configure(ObjectPool pool, string poolId)
    {
        ownerPool = pool;
        PoolId = poolId;
    }

    public void ReturnToPool()
    {
        if (ownerPool != null)
        {
            ownerPool.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
