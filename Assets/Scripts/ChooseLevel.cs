using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using Firebase.Analytics;

public class ChooseLevel : MonoBehaviour
{
    [SerializeField] Level[] levels;
    int totalLevels;
    [SerializeField] int currentLevelIndex;
    [SerializeField] TMP_Text levelNameText;
    [SerializeField] Image levelImage;
    [SerializeField] TMP_Text levelButtonText;
    [SerializeField] int unlockCost = 1000;
    [SerializeField] BombPrice mapPriceData;

    [SerializeField] TMP_SpriteAsset coinSpriteAsset, gemSpriteAsset;
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("LevelUnlocked0"))
        {
            PlayerPrefs.SetInt("LevelUnlocked0", 1);
        }

        currentLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        totalLevels = levels.Length;
        ShowLevel(currentLevelIndex);
    }

    private void OnEnable()
    {
        currentLevelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
        totalLevels = levels.Length;

        ShowLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        SoundManager.soundManager.PlayButtonSound();
        currentLevelIndex++;
        if (currentLevelIndex > totalLevels - 1)
            currentLevelIndex = 0;
        ShowLevel(currentLevelIndex);
    }

    public void PreviousLevel()
    {
        SoundManager.soundManager.PlayButtonSound();

        currentLevelIndex--;
        if (currentLevelIndex < 0)
            currentLevelIndex = totalLevels - 1;
        ShowLevel(currentLevelIndex);
    }

    public void ShowLevel(int index)
    {
        levelNameText.text = levels[index].levelName;
        levelImage.sprite = levels[index].levelImage;
        UpdateLevelButton();
    }

    public void SelectLevel()
    {
        SoundManager.soundManager.PlayButtonSound();

        if (!IsLevelUnlocked(currentLevelIndex))
        {
            bool purchaseSuccessful = PurchaseLevel();
            if (purchaseSuccessful)
            {
                PlayerPrefs.SetInt("LevelUnlocked" + currentLevelIndex, 1);
                UpdateLevelButton();
                CheckAchievement();
                //FirebaseAnalytics.LogEvent("Map Unlocked", new Parameter("Map Name: ",levelNameText.text));
            }
            else
            {
                // Handle failed purchase, e.g., show an error message or prompt for more currency

                Debug.Log("Not enough currency to unlock the level.");
            }
        }
        else
        {
            LoadLevel();
        }
    }

    void CheckAchievement()
    {
        AchievementsManager achievementsManager = FindObjectOfType<AchievementsManager>();
        PlayerPrefs.SetInt("MapCount", PlayerPrefs.GetInt("MapCount") + 1);
        int mapUnlocked = PlayerPrefs.GetInt("MapCount");

        if (mapUnlocked >= 1 && PlayerPrefs.GetInt("Map1Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.UnlockMap);
            PlayerPrefs.SetInt("Map1Achieved", 1); 
        }

        if (mapUnlocked >= 5 && PlayerPrefs.GetInt("Map5Achieved", 0) != 1)
        {
            achievementsManager.UnlockAchievement(AchievementID.Unlock5Map); 
            PlayerPrefs.SetInt("Map5Achieved", 1); 
        }

        PlayerPrefs.Save();
    }

    bool IsLevelUnlocked(int index)
    {
        return PlayerPrefs.GetInt("LevelUnlocked" + index, 0) == 1;
    }

    bool PurchaseLevel()
    {
        CurrencyItem priceInfo = mapPriceData.currencyItems[currentLevelIndex];

        int currentCoins = PlayerPrefs.GetInt("Coins", 0);
        int currentGems = PlayerPrefs.GetInt("Gems", 0);

        if (priceInfo.curreny == PriceType.Coins && currentCoins >= priceInfo.price)
        {
            PlayerPrefs.SetInt("Coins", currentCoins - priceInfo.price);
            UIManager.instance.SetCoinsText();
            return true;
        }
        else if (priceInfo.curreny == PriceType.Gems && currentGems >= priceInfo.price)
        {
            PlayerPrefs.SetInt("Gems", currentGems - priceInfo.price);
            UIManager.instance.SetCoinsText();
            return true;
        }
        else
        {
            UIManager.instance.EnableInAppWindow(true);
            return false;
        }
    }

    void UpdateLevelButton()
    {
        if (IsLevelUnlocked(currentLevelIndex))
        {
            levelButtonText.text = "Play";
        }
        else
        {
            CurrencyItem priceInfo = mapPriceData.currencyItems[currentLevelIndex];
            string priceText;

            if (priceInfo.curreny == PriceType.Coins)
            {
                levelButtonText.spriteAsset = coinSpriteAsset;
                priceText = $"{priceInfo.price} <sprite=0>"; 
            }
            else
            {
                levelButtonText.spriteAsset = gemSpriteAsset;
                priceText = $"{priceInfo.price} <sprite=0>";
            }

            levelButtonText.text = $"{priceText}";
        }
    }

    void LoadLevel()
    {
        PlayerPrefs.SetInt("LevelIndex", currentLevelIndex);
        // SceneManager.LoadScene(levels[currentLevelIndex].levelName);
        Debug.Log($"Loading level: {levels[currentLevelIndex].levelName}");
        SceneManager.LoadScene(levels[currentLevelIndex].levelName);
    }

    
}
[System.Serializable]
public class Level
{
    public string levelName;
    public Sprite levelImage;
}