using UnityEngine;

public class Coins : MonoBehaviour
{
    public int value = 1;
    public ParticleSystem collectEffect;

    void Start()
    {
        
    }

    // ...existing code...

void MoveToRandomPosition()
{
    Vector3 newPos = GetRandomValidPosition();
    transform.position = newPos;
}

Vector3 GetRandomValidPosition()
{
    Vector3 randomPos;
    int maxAttempts = 10;
    
    for (int i = 0; i < maxAttempts; i++)
    {
        float minX = -10f, maxX = 10f, minZ = -10f, maxZ = 10f;
        randomPos = new Vector3(
            Random.Range(minX, maxX),
            transform.position.y,
            Random.Range(minZ, maxZ)
        );

        if (IsPositionValid(randomPos))
            return randomPos;
    }
    
    return transform.position; // Jika gagal, tetap di posisi lama
}

bool IsPositionValid(Vector3 position)
{
    Collider[] colliders = Physics.OverlapSphere(position, 0.5f);
    foreach (Collider col in colliders)
    {
        if (col.CompareTag("Wall"))
            return false;
    }
    return true;
}

// ...existing code...

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoins(value);
            if (collectEffect) Instantiate(collectEffect, transform.position, Quaternion.identity);
            
            
            MoveToRandomPosition();
        }
    }
}