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
    
    // Spawn points yang dapat dikonfigurasi di Inspector
    public Transform[] spawnPoints;
    
    // UI References
    public PowerUpUIManager powerUpUIManager;

    void Start()
    {
        moveTimer = moveInterval;
        powerUpUIManager = FindObjectOfType<PowerUpUIManager>();
        
        // Jika tidak ada spawn points yang ditentukan, pindahkan ke spawn point random saat start
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            MoveToRandomSpawnPoint();
        }
    }

    void Update()
    {
        // Perpindahan otomatis setiap interval
        if (!isCollected && spawnPoints != null && spawnPoints.Length > 0)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                MoveToRandomSpawnPoint();
                moveTimer = moveInterval;
            }
        }
    }

    void MoveToRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Tidak ada spawn points yang ditentukan untuk PowerUp!");
            return;
        }

        // Pilih spawn point random
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        transform.position = randomSpawnPoint.position;
        transform.rotation = randomSpawnPoint.rotation;
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
            MoveToRandomSpawnPoint();
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
                    
                    // Update UI
                    if (powerUpUIManager)
                    {
                        powerUpUIManager.ShowPowerUpActivated("Extra Life");
                    }
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

        // Tampilkan aktivasi power-up
        if (powerUpUIManager)
        {
            powerUpUIManager.ShowPowerUpActivated("Ghost Killer");
        }

        float timeRemaining = powerUpDuration;
        
        // Update durasi setiap frame
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            
            if (powerUpUIManager)
            {
                powerUpUIManager.UpdateGhostKillerDuration(timeRemaining, powerUpDuration);
            }
            
            yield return null;
        }

        if (playerController)
        {
            playerController.SetGhostKillerMode(false);
        }
        
        // Sembunyikan UI durasi
        if (powerUpUIManager)
        {
            powerUpUIManager.HideGhostKillerDuration();
        }
        
        Debug.Log("Ghost Killer mode expired!");
    }
}