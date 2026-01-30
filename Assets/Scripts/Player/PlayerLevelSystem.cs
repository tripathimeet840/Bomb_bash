using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("XP Growth")]
    public float xpMultiplier = 1.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider xpBar;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", currentLevel);
        currentXP = PlayerPrefs.GetInt("PlayerXP", currentXP);
        xpToNextLevel = PlayerPrefs.GetInt("XPToNextLevel", xpToNextLevel);

        int pendingXP = PlayerPrefs.GetInt("PendingXP", 0);
        if (pendingXP > 0)
        {
            AddXP(pendingXP);
            PlayerPrefs.SetInt("PendingXP", 0);
        }

        UpdateUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        SaveProgress();
        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);

        Debug.Log("Level Up! Now Level " + currentLevel);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        PlayerPrefs.SetInt("PlayerXP", currentXP);
        PlayerPrefs.SetInt("XPToNextLevel", xpToNextLevel);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "Level " + currentLevel;

        if (xpBar != null)
            xpBar.value = (float)currentXP / xpToNextLevel;
    }

    private void Update()
    {
        if (levelText != null && levelText.text != "Level " + currentLevel)
        {
            levelText.text = "Level " + currentLevel;
        }
    }
}
