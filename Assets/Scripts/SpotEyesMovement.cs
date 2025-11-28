using UnityEngine;

public class SpotlightMovement : MonoBehaviour
{
    // Sudut maksimum kepala menoleh (dalam derajat)
    public float maxRotationAngle = 45f;

    // Kecepatan pergerakan kepala
    public float speed = 2f;

    private Quaternion initialRotation;

    void Start()
    {
        // Simpan rotasi awal lampu sorot saat game dimulai
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Hitung nilai sinusoidal berdasarkan waktu dan kecepatan
        // Math.Sin akan menghasilkan nilai antara -1 dan 1
        float rotationY = Mathf.Sin(Time.time * speed) * maxRotationAngle;

        // Terapkan rotasi pada sumbu Y untuk efek kepala menoleh
        Quaternion newRotation = initialRotation * Quaternion.Euler(0f, rotationY, 0f);
        transform.rotation = newRotation;
    }
}