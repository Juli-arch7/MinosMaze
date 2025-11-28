using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
public float walkSpeed = 4f;
public float runSpeed = 7f;
public float gravity = -9.81f;
public float mouseSensitivity = 2f;
public Transform cameraTransform;
private bool isGhostKillerMode = false;
private Renderer playerRenderer;
private Color originalColor;
private GameManager gameManager;
CharacterController cc;
Vector3 velocity;
float pitch = 0f;
private float lastGhostHitTime = 0f;
private float ghostHitCooldown = 1f; // 1 detik cooldown antara hit ghost


void Start()
{
    playerRenderer = GetComponent<Renderer>();
        if (playerRenderer)
        {
            originalColor = playerRenderer.material.color;
        }
    cc = GetComponent<CharacterController>();
    gameManager = GameManager.Instance;
    Cursor.lockState = CursorLockMode.Locked;
}


void Update()
{
    // Check if game is paused
    if (gameManager != null && gameManager.isPaused)
    {
        // Unlock cursor saat pause sehingga player bisa klik button
        Cursor.lockState = CursorLockMode.Confined;
        return; // Jangan proses input saat game pause
    }
    
    // Lock cursor kembali saat game berjalan
    Cursor.lockState = CursorLockMode.Locked;

    // Mouse look
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
    transform.Rotate(Vector3.up * mouseX);
    pitch -= mouseY;
    pitch = Mathf.Clamp(pitch, -85f, 85f);
    cameraTransform.localEulerAngles = Vector3.right * pitch;


    // Movement
    float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
    Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
    cc.Move(move * speed * Time.deltaTime);


    // Gravity
    if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
    velocity.y += gravity * Time.deltaTime;
    cc.Move(velocity * Time.deltaTime);
}
public void SetGhostKillerMode(bool active)
    {
        isGhostKillerMode = active;
        
        // Ubah warna player saat mode aktif
        if (playerRenderer)
        {
            if (active)
            {
                playerRenderer.material.color = Color.yellow;
                Debug.Log("Ghost Killer Mode AKTIF! (10 detik)");
            }
            else
            {
                playerRenderer.material.color = originalColor;
                Debug.Log("Ghost Killer Mode NONAKTIF");
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastGhostHitTime < ghostHitCooldown)
        {
            return;
        }
        if (other.CompareTag("Ghost"))
        {
            if (isGhostKillerMode)
            {
                // Player mengalahkan ghost dan dapat nyawa tambahan
                Destroy(other.gameObject);
                GameManager.Instance.currentLives++;
                GameManager.Instance.UpdateUI();
                Debug.Log("Ghost defeated! Extra life gained! Nyawa: " + GameManager.Instance.currentLives);
            }
            else
            {
                // Player terkena ghost normal
                GameManager.Instance.PlayerHitByGhost(other.gameObject);
            }
        }
    }
}