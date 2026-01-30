using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthProgression : MonoBehaviour
{
    public Image progressBar;
    public Text levelText;
    public TMP_Text upgradePriceText; 

    private int maxLevel = 5;
    public int[] healthProgression; 
    public int[] upgradeCosts = { 200, 250, 350, 400, 500 }; 

    private int currentPlayerIndex; 
    public Sprite Yellow, Orange, Red;

    private void Start()
    {
        currentPlayerIndex = PlayerPrefs.GetInt("PlayerIndex", 0);
        UpdateUI();
    }

    public void IncreasePower()
    {
        int currentLevel = PlayerPrefs.GetInt("PlayerHealthLevel" + currentPlayerIndex, 1);

        if (currentLevel >= maxLevel)
        {
            SoundManager.soundManager.PlayButtonSound();
            return;
        }

        int currentUpgradeCost = upgradeCosts[currentLevel - 1];
        if (PlayerPrefs.GetInt("Coins") >= currentUpgradeCost)
        {
            SoundManager.soundManager.PlayButtonSound();
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - currentUpgradeCost);

            int nextLevel = currentLevel + 1;
            int nextHealth = healthProgression[nextLevel - 1];

            PlayerPrefs.SetInt("PlayerHealthLevel" + currentPlayerIndex, nextLevel);
            PlayerPrefs.SetInt("PlayerHealth" + currentPlayerIndex, nextHealth);

            UpdateUI();
        }
        else
        {
            // UIManager.instance.EnableInAppWindow(true);
            SoundManager.soundManager.PlayButtonSound();

            UIManager.instance.ShowWatchAdsToUpgradeWindow(false);

        }
    }

    public void IncreaseByForce()
    {
        int currentLevel = PlayerPrefs.GetInt("PlayerHealthLevel" + currentPlayerIndex, 1);
        SoundManager.soundManager.PlayButtonSound();
        if (currentLevel >= maxLevel) return;
        int nextLevel = currentLevel + 1;
        int nextHealth = healthProgression[nextLevel - 1];

        PlayerPrefs.SetInt("PlayerHealthLevel" + currentPlayerIndex, nextLevel);
        PlayerPrefs.SetInt("PlayerHealth" + currentPlayerIndex, nextHealth);

        UpdateUI();

    }

    public void UpdateUI()
    {
        currentPlayerIndex = PlayerPrefs.GetInt("PlayerIndex", 0);

        int currentLevel = PlayerPrefs.GetInt("PlayerHealthLevel" + currentPlayerIndex, 1);
        int currentHealth = PlayerPrefs.GetInt("PlayerHealth" + currentPlayerIndex, healthProgression[0]);

        if (currentLevel >= 1 && currentLevel < 3)
            progressBar.sprite = Yellow;
        else if (currentLevel >= 3 && currentLevel < 5)
            progressBar.sprite = Orange;
        else if (currentLevel >= 5)
            progressBar.sprite = Red;

        levelText.text = "Level " + currentLevel;
        progressBar.fillAmount = (float)currentLevel / maxLevel;

        upgradePriceText.text = currentLevel < maxLevel ? "" + upgradeCosts[currentLevel - 1] + "<sprite=0>" : "Max Level"; // Display the upgrade cost or Max Level

        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().HealthInc(currentHealth);
        UIManager.instance.SetCoinsText();
    }
}
