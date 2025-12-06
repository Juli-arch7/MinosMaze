using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUpUIManager : MonoBehaviour
{
    [SerializeField] private Text powerUpNotificationText; // Tampilan aktivasi power-up
    [SerializeField] private Image ghostKillerDurationImage; // Circular progress bar
    [SerializeField] private Text ghostKillerDurationText; // Teks durasi
    [SerializeField] private float notificationDuration = 2f; // Durasi notifikasi ditampilkan

    void Start()
    {
        // Inisialisasi - sembunyikan elemen
        if (ghostKillerDurationImage) ghostKillerDurationImage.gameObject.SetActive(false);
        if (ghostKillerDurationText) ghostKillerDurationText.gameObject.SetActive(false);
        if (powerUpNotificationText) powerUpNotificationText.gameObject.SetActive(false);
    }

    public void ShowPowerUpActivated(string powerUpName)
    {
        if (powerUpNotificationText)
        {
            powerUpNotificationText.text = powerUpName + " Activated!";
            powerUpNotificationText.gameObject.SetActive(true);
            StartCoroutine(HideNotificationAfterDelay());
        }
    }

    public void UpdateGhostKillerDuration(float timeRemaining, float totalDuration)
    {
        // Tampilkan panel durasi
        if (ghostKillerDurationImage && !ghostKillerDurationImage.gameObject.activeSelf)
        {
            ghostKillerDurationImage.gameObject.SetActive(true);
        }
        
        if (ghostKillerDurationText && !ghostKillerDurationText.gameObject.activeSelf)
        {
            ghostKillerDurationText.gameObject.SetActive(true);
        }

        // Update circular progress bar (fillAmount)
        if (ghostKillerDurationImage)
        {
            ghostKillerDurationImage.fillAmount = timeRemaining / totalDuration;
            ghostKillerDurationImage.color = Color.yellow; // Warna untuk Ghost Killer
        }

        // Update teks durasi
        if (ghostKillerDurationText)
        {
            ghostKillerDurationText.text = Mathf.Ceil(timeRemaining).ToString() + "s";
        }
    }

    public void HideGhostKillerDuration()
    {
        if (ghostKillerDurationImage) ghostKillerDurationImage.gameObject.SetActive(false);
        if (ghostKillerDurationText) ghostKillerDurationText.gameObject.SetActive(false);
    }

    IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);
        if (powerUpNotificationText) powerUpNotificationText.gameObject.SetActive(false);
    }
}