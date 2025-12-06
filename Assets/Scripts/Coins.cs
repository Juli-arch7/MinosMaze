using UnityEngine;

public class Coins : MonoBehaviour
{
    public int value = 1;
    public ParticleSystem collectEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddCoins(value);
            if (collectEffect) Instantiate(collectEffect, transform.position, Quaternion.identity);
            
            // Beritahu GameManager bahwa coin ini sudah diambil
            GameManager.Instance.OnCoinCollected(this.gameObject);
            
            // Destroy coin ini
            Destroy(gameObject);
        }
    }
}