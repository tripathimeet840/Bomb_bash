using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerProgression : MonoBehaviour
{
    public Image progressBar;
    public Text levelText;
    public TMP_Text upgradePriceText;

    private int maxLevel = 5;
    public int[] powerProgression = { 20, 25, 30, 35, 40 };
    public int[] upgradeCosts = { 200, 250, 350, 400, 500 };

    private int currentPlayerIndex;
    public Sprite Yellow, Orange, red;


    private void Start()
    {
        currentPlayerIndex = PlayerPrefs.GetInt("PlayerIndex", 0);

        UpdateUI();
    }

    public void IncreasePower()
    {
        int currentLevel = PlayerPrefs.GetInt("PlayerLevel" + currentPlayerIndex, 1);

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

            int nextPower = powerProgression[nextLevel - 1];

            PlayerPrefs.SetInt("PlayerLevel" + currentPlayerIndex, nextLevel);
            PlayerPrefs.SetInt("PlayerPower" + currentPlayerIndex, nextPower);

            UpdateUI();
        }
        else
        {
            SoundManager.soundManager.PlayButtonSound();

            UIManager.instance.ShowWatchAdsToUpgradeWindow(true);
            //UIManager.instance.EnableInAppWindow(true);
        }
    }

    public void IncreaseByForce()
    {
        int currentLevel = PlayerPrefs.GetInt("PlayerLevel" + currentPlayerIndex, 1);
        SoundManager.soundManager.PlayButtonSound();
        if (currentLevel >= maxLevel) return;
        int nextLevel = currentLevel + 1;

        int nextPower = powerProgression[nextLevel - 1];

        PlayerPrefs.SetInt("PlayerLevel" + currentPlayerIndex, nextLevel);
        PlayerPrefs.SetInt("PlayerPower" + currentPlayerIndex, nextPower);

        UpdateUI();

    }


    public void UpdateUI()
    {
        currentPlayerIndex = PlayerPrefs.GetInt("PlayerIndex", 0);

        int currentLevel = PlayerPrefs.GetInt("PlayerLevel" + currentPlayerIndex, 1);
        int currentPower = PlayerPrefs.GetInt("PlayerPower" + currentPlayerIndex, powerProgression[0]);

        if (currentLevel >= 1 && currentLevel < 3)
            progressBar.sprite = Yellow;
        if (currentLevel >= 3 && currentLevel < 5)
            progressBar.sprite = Orange;
        if (currentLevel >= 5)
            progressBar.sprite = red;

        // Update UI elements
        levelText.text = "Level " + currentLevel;
        progressBar.fillAmount = (float)currentLevel / maxLevel;
        upgradePriceText.text = currentLevel < maxLevel ? "" + upgradeCosts[currentLevel - 1]+"<sprite=0>" : "Max Level"; // Update upgrade cost text

        // Set player's power in PlayerMovement script
        FindObjectOfType<PlayerMovement>().PlayerPower = currentPower;
        UIManager.instance.SetCoinsText();
    }
}
