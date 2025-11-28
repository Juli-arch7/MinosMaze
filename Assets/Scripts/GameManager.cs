using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int currentCoins;
    public Text CoinsText;
    public Text LivesText; // Tambah untuk menampilkan nyawa
    public GameObject losePanel;
    public GameObject pauseMenuPanel;
    public GameObject ghostPrefab;
    public int coinsPerGhost = 5;
    private int totalCoinsCollected = 0;
    
    // Sistem Nyawa
    public int maxLives = 3;
    public int currentLives;
    public GameObject playerSpawnPoint; // Posisi spawn player
    private GameObject player;
    public GameObject powerUpPrefab;

    // Difficulty Settings
    public float ghostSpeedMultiplier = 1f;
    public bool isPaused = false;


    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    void Start()
    {
        currentLives = maxLives;
        player = GameObject.FindGameObjectWithTag("Player");
        UpdateUI();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void AddCoins(int amount)
{
    currentCoins += amount;
    totalCoinsCollected += amount;
    
    // Spawn ghost hanya jika totalCoinsCollected adalah kelipatan coinsPerGhost dan bukan 0
    if (totalCoinsCollected > 0 && totalCoinsCollected % coinsPerGhost == 0)
    {
        SpawnGhost();
    }
    
    UpdateUI();
}

    // ...existing code...

    void SpawnGhost()
    {
        Vector3 spawnPos = GetRandomValidPosition();
        Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
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
                1f,
                Random.Range(minZ, maxZ)
            );

            if (IsPositionValid(randomPos))
            {
                return randomPos;
            }
        }

        return new Vector3(0, 1f, 0);
    }

    bool IsPositionValid(Vector3 position)
    {
        float checkRadius = 1f;

        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Wall") || col.name.Contains("Wall"))
            {
                return false;
            }
        }

        return true;
    }

    // ...existing code...

    public void UpdateUI()
    {
        if (CoinsText) CoinsText.text = currentCoins.ToString();
        if (LivesText) LivesText.text = "Lives: " + currentLives.ToString();
    }

    // Fungsi ketika player terkena ghost
    public void PlayerHitByGhost(GameObject ghost)
    {
        currentLives--;
        UpdateUI();

        if (currentLives <= 0)
        {
            PlayerDied();
        }
        else
        {
            // Respawn player jauh dari ghost
            StartCoroutine(RespawnPlayer(ghost));
        }
    }

    IEnumerator RespawnPlayer(GameObject ghost)
    {
        if (player)
        {
            player.SetActive(false);
            yield return new WaitForSeconds(1f); // Delay sebelum respawn

            Vector3 respawnPos = GetRespawnPosition(ghost.transform.position);
            player.transform.position = respawnPos;
            player.SetActive(true);
        }
    }

    Vector3 GetRespawnPosition(Vector3 ghostPos)
    {
        Vector3 respawnPos;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            float minX = -10f, maxX = 10f, minZ = -10f, maxZ = 10f;
            respawnPos = new Vector3(
                Random.Range(minX, maxX),
                1f,
                Random.Range(minZ, maxZ)
            );

            // Jarak minimal dari ghost
            float minDistance = 15f;
            if (Vector3.Distance(respawnPos, ghostPos) > minDistance)
            {
                return respawnPos;
            }
        }

        return playerSpawnPoint ? playerSpawnPoint.transform.position : new Vector3(0, 1f, 0);
    }

    public void PlayerDied()
    {
        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Pause Menu Functions
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); // Kembali ke main menu
    }

    public void SetDifficulty(float multiplier)
    {
        ghostSpeedMultiplier = multiplier;
    }
}