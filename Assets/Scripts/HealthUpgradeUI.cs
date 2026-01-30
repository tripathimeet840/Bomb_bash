using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public int[] healthPerLevel;    
    public int[] upgradeCosts;     
    public string rarity;
}

public class HealthUpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text levelText;
    public TMP_Text hpText;
    public TMP_Text costText;
    public TMP_Text coinsText;
    public TMP_Text rarityText;
    public Button enhanceButton;

    [Header("Characters")]
    public CharacterData[] characters;

    [Header("Character Prefabs (assign in same order as characters[])")]
    public GameObject[] characterPrefabs;

    private int currentCharacterIndex = 0;
    private int[] currentLevels;

    [Header("References")]
    public PlayerDataManager playerDataManager;

    private void Awake()
    {
        if (characterPrefabs != null && characterPrefabs.Length != characters.Length)
            Debug.LogWarning("characterPrefabs length does not match characters length. Be sure they correspond.");

        currentLevels = new int[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            currentLevels[i] = PlayerPrefs.GetInt("CharacterLevel_" + i, 1);
        }
    }

    private void Start()
    {
        if (enhanceButton != null)
            enhanceButton.onClick.AddListener(OnEnhanceClicked);

        RefreshCoins(playerDataManager != null ? playerDataManager.coins : 0);
        UpdateUI();

        ApplyAllSavedHealthValues();
    }

    public void OnCharacterChanged(int index)
    {
        if (index < 0 || index >= characters.Length) return;
        currentCharacterIndex = index;
        UpdateUI();
    }

    private void OnEnhanceClicked()
    {
        int level = currentLevels[currentCharacterIndex];
        CharacterData c = characters[currentCharacterIndex];

        if (level >= c.healthPerLevel.Length) return;

        int cost = c.upgradeCosts[level - 1];

        if (playerDataManager != null && playerDataManager.coins >= cost)
        {
            playerDataManager.DeductCoins(cost);

            currentLevels[currentCharacterIndex]++;
            PlayerPrefs.SetInt("CharacterLevel_" + currentCharacterIndex, currentLevels[currentCharacterIndex]);
            PlayerPrefs.Save();

            float newMaxHealth = c.healthPerLevel[currentLevels[currentCharacterIndex] - 1];
            ApplyHealthToPrefabAndInstances(currentCharacterIndex, newMaxHealth);

            UpdateUI();
            RefreshCoins(playerDataManager.coins);
        }
        else
        {
            Debug.Log("Not enough coins to upgrade!");
        }
    }

    private void UpdateUI()
    {
        CharacterData c = characters[currentCharacterIndex];
        int level = currentLevels[currentCharacterIndex];

        levelText.text = $"Level {level} / {c.healthPerLevel.Length}";
        hpText.text = $"HP: {c.healthPerLevel[level - 1]}";
        rarityText.text = c.rarity;

        if (level < c.healthPerLevel.Length)
            costText.text = $"Cost: {c.upgradeCosts[level - 1]} Coins";
        else
            costText.text = "Max Level";

        enhanceButton.interactable = level < c.healthPerLevel.Length;
    }

    public void RefreshCoins(int currentCoins)
    {
        if (coinsText != null)
            coinsText.text = currentCoins.ToString();
    }

    private void ApplyHealthToPrefabAndInstances(int characterIndex, float newMaxHealth)
    {
        if (characterPrefabs != null && characterIndex < characterPrefabs.Length)
        {
            GameObject prefab = characterPrefabs[characterIndex];
            if (prefab != null)
            {
                Health prefabHealth = prefab.GetComponent<Health>();
                if (prefabHealth != null)
                {
                    prefabHealth.maxHealth = newMaxHealth;
                    prefabHealth.currentHealth = Mathf.Min(prefabHealth.currentHealth, newMaxHealth);
                }
            }
        }

        CharacterIdentifier[] allIdentifiers = FindObjectsOfType<CharacterIdentifier>(true);
        foreach (var id in allIdentifiers)
        {
            if (id.characterIndex == characterIndex)
            {
                Health h = id.GetComponent<Health>();
                if (h != null)
                {
                    h.maxHealth = newMaxHealth;

                    h.currentHealth = Mathf.Min(h.currentHealth, h.maxHealth);

                    if (h.healthBar != null)
                        h.healthBar.fillAmount = h.currentHealth / h.maxHealth;
                }
            }
        }
    }

    private void ApplyAllSavedHealthValues()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            int level = PlayerPrefs.GetInt("CharacterLevel_" + i, 1);
            float maxHealth = characters[i].healthPerLevel[level - 1];
            ApplyHealthToPrefabAndInstances(i, maxHealth);
        }
    }

    public void OnCoinsChanged(int newCoins)
    {
        RefreshCoins(newCoins);
    }
}
