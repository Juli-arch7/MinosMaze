using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { ExtraLife, GhostKiller }
    public PowerUpType powerUpType;
    public float powerUpDuration = 10f; // Durasi power-up
    public float moveInterval = 5f; // Interval perpindahan dalam detik
    private bool isCollected = false;
    private float moveTimer = 0f;

    void Start()
    {
        moveTimer = moveInterval;
    }

    void Update()
    {
        // Perpindahan otomatis setiap interval
        if (!isCollected)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                MoveToRandomPosition();
                moveTimer = moveInterval;
            }
        }
    }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            ApplyPowerUp(other.gameObject);
            
            // Reset untuk dapat diambil kembali
            isCollected = false;
            moveTimer = moveInterval;
            MoveToRandomPosition();
        }
    }

    void ApplyPowerUp(GameObject player)
    {
        switch (powerUpType)
        {
            case PowerUpType.ExtraLife:
                if (GameManager.Instance.currentLives < GameManager.Instance.maxLives)
                {
                    GameManager.Instance.currentLives++;
                    GameManager.Instance.UpdateUI();
                    Debug.Log("Extra Life! Nyawa: " + GameManager.Instance.currentLives);
                }
                break;

            case PowerUpType.GhostKiller:
                StartCoroutine(ActivateGhostKiller(player));
                break;
        }
    }

    IEnumerator ActivateGhostKiller(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController)
        {
            playerController.SetGhostKillerMode(true);
        }

        yield return new WaitForSeconds(powerUpDuration);

        if (playerController)
        {
            playerController.SetGhostKillerMode(false);
        }
        Debug.Log("Ghost Killer mode expired!");
    }
}