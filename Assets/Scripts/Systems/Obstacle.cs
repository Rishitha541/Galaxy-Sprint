using UnityEngine;

public class Obstacle : MonoBehaviour, IPooledObject
{
    private bool hasTriggeredGameOver;

    private void OnTriggerEnter(Collider other)
    {
        TryGameOver(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryGameOver(collision.gameObject);
    }

    private void TryGameOver(GameObject other)
    {
        if (hasTriggeredGameOver || !other.CompareTag("Player"))
        {
            return;
        }

        hasTriggeredGameOver = true;
        GameManager.Instance?.GameOver();
    }

    public void OnObjectSpawn()
    {
        hasTriggeredGameOver = false;
    }

    public void OnObjectDespawn()
    {
        hasTriggeredGameOver = false;
    }
}
