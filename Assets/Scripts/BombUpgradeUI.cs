using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombUpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text levelText;     
    public TMP_Text damageText;     
    public TMP_Text costText;       
    public TMP_Text coinsText;      
    public Button enhanceButton;

    [Header("Progression Data (Per Bomb)")]
    public int maxLevel = 5;
    public int[] damageProgression = { 50, 60, 75, 90, 110 };
    public int[] upgradeCosts = { 200, 250, 350, 400 };   

    public int currentBombIndex;
    private int currentLevel;
    private int currentDamage;
    private int coins;

    public GameObject normalBomb;
    public GameObject timeBomb;
    public GameObject clusterBomb;
    public GameObject freezeBomb;
    public GameObject stickyBomb;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("BombLevel_" + currentBombIndex, 1);
        currentDamage = PlayerPrefs.GetInt("BombDamage_" + currentBombIndex, damageProgression[0]);
        coins = PlayerPrefs.GetInt("Coins", 1000);

        enhanceButton.onClick.AddListener(OnEnhanceClicked);

        UpdateUI();
    }

    public void UpdateUI()
    {
        // Show level
        levelText.text = $"Level {currentLevel} / {maxLevel}";

        // Show damage for this bomb
        damageText.text = $"Damage: {currentDamage}";

        // Show upgrade cost or Max Level
        if (currentLevel < maxLevel)
        {
            costText.text = $"Cost: {upgradeCosts[currentLevel - 1]} Coins";
            enhanceButton.interactable = true;
        }
        else
        {
            costText.text = "Max Level";
            enhanceButton.interactable = false;
        }

        coinsText.text = coins.ToString();
    }

    private void OnEnhanceClicked()
    {
        if (currentLevel >= maxLevel) return;

        int cost = upgradeCosts[currentLevel - 1];

        if (coins >= cost)
        {
            coins -= cost;

            currentLevel++;
            currentDamage = damageProgression[currentLevel - 1];

            PlayerPrefs.SetInt("BombLevel_" + currentBombIndex, currentLevel);
            PlayerPrefs.SetInt("BombDamage_" + currentBombIndex, currentDamage);

            PlayerPrefs.SetInt("Coins", coins);

            UpdateUI();
        }
        else
        {
            Debug.Log("Not enough coins to upgrade!");
        }
    }

    public void OnBombChanged(int newIndex)
    {
        currentBombIndex = newIndex;

        currentLevel = PlayerPrefs.GetInt("BombLevel_" + currentBombIndex, 1);
        currentDamage = PlayerPrefs.GetInt("BombDamage_" + currentBombIndex, damageProgression[0]);

        UpdateUI();
    }
}
