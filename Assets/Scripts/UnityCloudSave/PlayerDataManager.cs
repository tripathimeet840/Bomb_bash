using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI status;
    public TextMeshProUGUI playerName; 
    public TextMeshProUGUI playerNameDisplayPanel; 
    public TMP_InputField inputField;
    public TextMeshProUGUI levelText;

    // Global Player Data
    public int coins = 0;
    public int gems = 0;
    public float playTime = 0f;
    public string name = "Player";

    // New fields
    public bool removeAds = false;
    public int dayNumber = 1;
    public int levelNumber = 1;

    // First-time check
    public bool isFirstTime = false;

    [Header("Coin/Gem UI")]
    public TextMeshProUGUI coinTexts;
    public TextMeshProUGUI gemTexts;


    private async void Start()
    {
        // Load local PlayerPrefs first
        coins = PlayerPrefs.GetInt("Coins", 0);
        gems = PlayerPrefs.GetInt("Gems", 0);
        levelNumber = PlayerPrefs.GetInt("LevelNumber", 1);
        name = PlayerPrefs.GetString("PlayerName", "Player");
        removeAds = PlayerPrefs.GetInt("RemoveAds", 0) == 1;
        isFirstTime = PlayerPrefs.GetInt("IsFirstTime", 1) == 1;

        // Update UI immediately
        if (playerName != null) playerName.text = name;
        if (playerNameDisplayPanel != null) playerNameDisplayPanel.text = name;
        if (levelText != null) levelText.text = "Level " + levelNumber;

        Debug.Log("Loaded local data: Coins=" + coins + ", Gems=" + gems);

        // Then do cloud init
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in. PlayerID: " + AuthenticationService.Instance.PlayerId);
        }

        // Load cloud data
        await LoadData();

        UpdateCoinGemUI(); // Update UI immediately
    }


    private void ShowEnterNamePanel()
    {
        if (UIManager.instance != null && UIManager.instance.changeNamePanel != null)
        {
            UIManager.instance.changeNamePanel.SetActive(true);
        }
    }

    private void Update()
    {
        // Track playtime every frame
        playTime += Time.deltaTime;
    }

    // Save All Data
    public async void SaveData()
    {
        try
        {
            // 1. Update local values
            if (!string.IsNullOrEmpty(inputField.text))
            {
                name = inputField.text;
                isFirstTime = false;
            }

            // Save locally first
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.SetInt("LevelNumber", levelNumber);
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.SetInt("Gems", gems);
            PlayerPrefs.SetInt("IsFirstTime", isFirstTime ? 1 : 0);
            PlayerPrefs.SetInt("RemoveAds", removeAds ? 1 : 0);
            PlayerPrefs.SetInt("DayNumber", dayNumber);
            PlayerPrefs.Save();

            // Update UI immediately
            if (playerName != null) playerName.text = name;
            if (playerNameDisplayPanel != null) playerNameDisplayPanel.text = name;
            if (levelText != null) levelText.text = "Level " + levelNumber;
            if (status != null) status.text = "Saved locally!";

            Debug.Log("Local data updated -> Name: " + name);

            // 2. Prepare cloud data
            var data = new Dictionary<string, object>
        {
            { "Name", name },
            { "Coins", coins },
            { "Gems", gems },
            { "PlayTime", playTime },
            { "RemoveAds", removeAds },
            { "DayNumber", dayNumber },
            { "LevelNumber", levelNumber },
            { "IsFirstTime", isFirstTime }
        };

            // 3. Save to cloud
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);

            if (status != null) status.text = "Synced with cloud!";
            Debug.Log("Cloud sync successful");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Cloud save failed: " + ex.Message);
            if (status != null) status.text = "Cloud Save Failed!";
        }
    }




    // Load All Data
    private async Task LoadData()
    {
        try
        {
            var keys = new HashSet<string> { "Name", "Coins", "Gems", "PlayTime", "RemoveAds", "DayNumber", "LevelNumber", "IsFirstTime" };
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(keys);

            if (savedData.ContainsKey("Name")) name = savedData["Name"].ToString();
            if (savedData.ContainsKey("Coins")) coins = int.Parse(savedData["Coins"].ToString());
            if (savedData.ContainsKey("Gems")) gems = int.Parse(savedData["Gems"].ToString());
            if (savedData.ContainsKey("PlayTime")) playTime = float.Parse(savedData["PlayTime"].ToString());
            if (savedData.ContainsKey("RemoveAds")) removeAds = bool.Parse(savedData["RemoveAds"].ToString());
            if (savedData.ContainsKey("DayNumber")) dayNumber = int.Parse(savedData["DayNumber"].ToString());
            if (savedData.ContainsKey("LevelNumber")) levelNumber = int.Parse(savedData["LevelNumber"].ToString());
            if (savedData.ContainsKey("IsFirstTime")) isFirstTime = bool.Parse(savedData["IsFirstTime"].ToString());

            // Update UI
            if (playerName != null) playerName.text = name;
            if (playerNameDisplayPanel != null) playerNameDisplayPanel.text = name;
            levelText.text = "Level " + levelNumber;

            // Save to PlayerPrefs after loading
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.SetInt("LevelNumber", levelNumber);
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.SetInt("Gems", gems);
            PlayerPrefs.SetInt("IsFirstTime", isFirstTime ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log($"Loaded Data -> Name: {name}, Coins: {coins}, Gems: {gems}, PlayTime: {playTime}, RemoveAds: {removeAds}, Day: {dayNumber}, Level: {levelNumber}, FirstTime: {isFirstTime}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Load failed: " + ex.Message);
        }
    }

    public async void SaveName()
    {
        try
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                name = inputField.text;
                isFirstTime = false;

                // Save locally first
                PlayerPrefs.SetString("PlayerName", name);
                PlayerPrefs.SetInt("IsFirstTime", 0);
                PlayerPrefs.Save();

                // Update UI immediately
                if (playerName != null) playerName.text = name;
                if (playerNameDisplayPanel != null) playerNameDisplayPanel.text = name;
                if (status != null) status.text = "Name saved locally!";

                // Save to cloud
                var data = new Dictionary<string, object>
            {
                { "Name", name },
                { "IsFirstTime", isFirstTime }
            };
                await CloudSaveService.Instance.Data.ForceSaveAsync(data);

                if (status != null) status.text = "Name synced to cloud!";
                Debug.Log("Name saved to cloud!");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Save name failed: " + ex.Message);
            if (status != null) status.text = "Name cloud save failed!";
        }
    }

    public void SaveLocalData()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Gems", gems);
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.SetInt("LevelNumber", levelNumber);
        PlayerPrefs.SetInt("IsFirstTime", isFirstTime ? 1 : 0);
        PlayerPrefs.SetInt("RemoveAds", removeAds ? 1 : 0);
        PlayerPrefs.SetInt("DayNumber", dayNumber);
        PlayerPrefs.Save();

        // Update UI immediately
        if (playerName != null) playerName.text = name;
        if (playerNameDisplayPanel != null) playerNameDisplayPanel.text = name;
        if (levelText != null) levelText.text = "Level " + levelNumber;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinGemUI();
        SaveData();
    }

    public void DeductCoins(int amount)
    {
        coins -= amount;
        UpdateCoinGemUI();
        SaveLocalData();
        SaveData();
    }

    public void AddGems(int amount)
    {
        gems += amount;
        UpdateCoinGemUI();
        SaveData();
    }

    public void IncreaseGems()
    {
        AddGems(10000);
        AddCoins(10000);
    }

    public void DeductGems(int amount)
    {
        gems -= amount;
        UpdateCoinGemUI();
        SaveLocalData();
        SaveData();
    }


    public void NextDay()
    {
        dayNumber++;
        SaveData();
    }

    public void NextLevel()
    {
        levelNumber++;

        // Update level text immediately
        if (levelText != null)
            levelText.text = "Level " + levelNumber;

        SaveData(); // Save to PlayerPrefs + Cloud
    }

    public void PurchaseRemoveAds()
    {
        removeAds = true;
        SaveData();
    }

    private void UpdateCoinGemUI()
    {
        coinTexts.text = coins.ToString();
        gemTexts.text = gems.ToString();
    }

}
