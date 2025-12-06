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
    public int coinsPerGhost = 5;
    private int totalCoinsCollected = 0;
    
    // Sistem Nyawa
    public int maxLives = 3;
    public int currentLives;
    public GameObject[] playerSpawnPoints; // Kumpulan posisi spawn player
    private GameObject player;
   

    // Coin Spawn System
    public GameObject coinPrefab; // Prefab coin yang akan di-spawn
    public GameObject[] coinSpawnPoints; // Kumpulan posisi spawn coin
    public int maxCoinsOnMap = 5; // Jumlah maksimal koin yang ada di map secara bersamaan
    private System.Collections.Generic.HashSet<GameObject> occupiedCoinSpawns = new System.Collections.Generic.HashSet<GameObject>();

    // Ghost Spawn System
    public GameObject[] ghostSpawnPoints; // Kumpulan posisi spawn ghost
    private System.Collections.Generic.HashSet<GameObject> occupiedGhostSpawns = new System.Collections.Generic.HashSet<GameObject>();
    public GameObject[] ghostPrefabs; // Array untuk multiple ghost models
    public string[] ghostTypes = { "Standard", "Fast"}; // Type masing-masing prefab



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
        InitializeCoins();
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

    // Coin Spawn System Methods
    void InitializeCoins()
    {
        if (coinSpawnPoints == null || coinSpawnPoints.Length == 0)
        {
            Debug.LogWarning("Coin spawn points belum di-set di GameManager!");
            return;
        }

        if (coinPrefab == null)
        {
            Debug.LogWarning("Coin prefab belum di-set di GameManager!");
            return;
        }

        // Spawn coin sejumlah maxCoinsOnMap dari spawn points yang tersedia
        int coinsToSpawn = Mathf.Min(maxCoinsOnMap, coinSpawnPoints.Length);
        
        for (int i = 0; i < coinsToSpawn; i++)
        {
            GameObject spawnPoint = GetRandomAvailableSpawnPoint();
            if (spawnPoint != null)
            {
                SpawnCoinAtPoint(spawnPoint);
            }
        }
    }

    void SpawnCoinAtPoint(GameObject spawnPoint)
    {
        if (spawnPoint == null || occupiedCoinSpawns.Contains(spawnPoint))
            return;

        GameObject coin = Instantiate(coinPrefab, spawnPoint.transform.position, Quaternion.identity);
        occupiedCoinSpawns.Add(spawnPoint);
    }

    public void OnCoinCollected(GameObject coin)
    {
        // Cari spawn point terdekat dengan coin yang diambil
        GameObject closestSpawnPoint = FindClosestSpawnPoint(coin.transform.position);
        
        if (closestSpawnPoint != null)
        {
            // Tandai spawn point ini sebagai kosong
            occupiedCoinSpawns.Remove(closestSpawnPoint);
            
            // Spawn coin baru di spawn point yang berbeda dan belum terpakai
            GameObject availableSpawnPoint = GetRandomAvailableSpawnPoint();
            if (availableSpawnPoint != null)
            {
                SpawnCoinAtPoint(availableSpawnPoint);
            }
        }
    }

    GameObject FindClosestSpawnPoint(Vector3 position)
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject spawnPoint in coinSpawnPoints)
        {
            if (spawnPoint != null)
            {
                float distance = Vector3.Distance(position, spawnPoint.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = spawnPoint;
                }
            }
        }

        return closest;
    }

    GameObject GetRandomAvailableSpawnPoint()
    {
        System.Collections.Generic.List<GameObject> availableSpawns = new System.Collections.Generic.List<GameObject>();

        foreach (GameObject spawnPoint in coinSpawnPoints)
        {
            if (spawnPoint != null && !occupiedCoinSpawns.Contains(spawnPoint))
            {
                availableSpawns.Add(spawnPoint);
            }
        }

        if (availableSpawns.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpawns.Count);
            return availableSpawns[randomIndex];
        }

        return null;
    }

    // Ghost Spawn System Methods
    void SpawnGhost()
    {
        if (ghostSpawnPoints == null || ghostSpawnPoints.Length == 0)
        {
            Debug.LogWarning("Ghost spawn points belum di-set di GameManager!");
            return;
        }

        GameObject spawnPoint = GetRandomAvailableGhostSpawnPoint();
        if (spawnPoint != null)
        {
            // Pilih random ghost prefab
            int ghostIndex = Random.Range(0, ghostPrefabs.Length);
            GameObject ghostPrefab = ghostPrefabs[ghostIndex];
            
            GameObject ghost = Instantiate(ghostPrefab, spawnPoint.transform.position, Quaternion.identity);
            
            // Set tipe ghost
            EnemyAI enemyAI = ghost.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.ghostType = ghostTypes[ghostIndex];
            }
            
            occupiedGhostSpawns.Add(spawnPoint);
        }
        else
        {
            Debug.LogWarning("Tidak ada ghost spawn point yang tersedia!");
        }
    }

    GameObject GetRandomAvailableGhostSpawnPoint()
    {
        System.Collections.Generic.List<GameObject> availableSpawns = new System.Collections.Generic.List<GameObject>();

        foreach (GameObject spawnPoint in ghostSpawnPoints)
        {
            if (spawnPoint != null && !occupiedGhostSpawns.Contains(spawnPoint))
            {
                availableSpawns.Add(spawnPoint);
            }
        }

        if (availableSpawns.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpawns.Count);
            return availableSpawns[randomIndex];
        }

        return null;
    }

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
        // Jika ada spawn points yang ditentukan, gunakan yang terjauh dari ghost
        if (playerSpawnPoints != null && playerSpawnPoints.Length > 0)
        {
            GameObject farthestSpawnPoint = null;
            float maxDistance = 0f;

            foreach (GameObject spawnPoint in playerSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    float distance = Vector3.Distance(spawnPoint.transform.position, ghostPos);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestSpawnPoint = spawnPoint;
                    }
                }
            }

            if (farthestSpawnPoint != null)
            {
                return farthestSpawnPoint.transform.position;
            }
        }

        // Fallback: cari posisi random yang jauh dari ghost
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

        // Fallback terakhir
        return new Vector3(0, 1f, 0);
    }

    public void PlayerDied()
    {
        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f;

        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
        
        // Tambahkan GraphicRaycaster untuk UI
        GraphicRaycaster raycaster = losePanel.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = losePanel.AddComponent<GraphicRaycaster>();
        }
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