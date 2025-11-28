using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public Button resumeButton;
    public Button restartButton;
    public Dropdown difficultyDropdown;

    void Start()
    {
        resumeButton.onClick.AddListener(() => GameManager.Instance.Resume());
        restartButton.onClick.AddListener(() => GameManager.Instance.Restart());
        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
    }

    void OnDifficultyChanged(int index)
    {
        float multiplier = index switch
        {
            0 => 0.5f,   // Easy
            1 => 1f,     // Normal
            2 => 1.5f,   // Hard
            _ => 1f
        };
        GameManager.Instance.SetDifficulty(multiplier);
    }
}