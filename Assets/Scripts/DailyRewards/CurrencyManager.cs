using TMPro;
using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("UI")]
    public TMP_Text coinsText;
    public TMP_Text gemsText;

    private int coins;
    private int gems;

    private const string CoinsKey = "Coins";
    private const string GemsKey = "Gems";
    private const string LastClaimDateKey = "LastClaimDate";

    public bool canClaim = true;

    void Awake()
    {
        canClaim = true;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadCurrency();
        CheckIfNewDay();
        UpdateUI();
    }

    public void AddCoinsOncePerDay(int amount)
    {
        if (!canClaim) return;

        coins += amount;
        SaveCurrency();
        SaveClaimDate();
        canClaim = false;
        UpdateUI();
    }

    public void AddGemsOncePerDay(int amount)
    {
        if (!canClaim) return;

        gems += amount;
        SaveCurrency();
        SaveClaimDate();
        canClaim = false;
        UpdateUI();
    }

    public void AddXPOncePerDay(int amount)
    {
        if (!canClaim) return;

        PlayerLevelSystem levelSystem = FindObjectOfType<PlayerLevelSystem>();
        if (levelSystem != null)
        {
            levelSystem.AddXP(amount);
            Debug.Log($" Daily XP claimed: +{amount} XP");
        }
        else
        {
            Debug.LogWarning(" PlayerLevelSystem not found in scene!");
        }

        SaveClaimDate();
        canClaim = false;
    }

    public void ClaimMegaReward()
    {
        if (!canClaim) return;

        coins += 1000;

        gems += 50;

        PlayerLevelSystem levelSystem = FindObjectOfType<PlayerLevelSystem>();
        if (levelSystem != null)
        {
            levelSystem.AddXP(500);
        }
        else
        {
            Debug.LogWarning(" PlayerLevelSystem not found in scene!");
        }

        SaveCurrency();
        SaveClaimDate();
        canClaim = false;
        UpdateUI();
    }


    public void CheckIfNewDay()
    {
        string lastClaimDate = PlayerPrefs.GetString(LastClaimDateKey, "");
        if (string.IsNullOrEmpty(lastClaimDate))
        {
            canClaim = true;
            return;
        }

        DateTime lastDate = DateTime.Parse(lastClaimDate);
        canClaim = (lastDate.Date != DateTime.Now.Date);
    }

    //  Save today's claim date
    private void SaveClaimDate()
    {
        PlayerPrefs.SetString(LastClaimDateKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    //  Save coins & gems
    public void SaveCurrency()
    {
        PlayerPrefs.SetInt(CoinsKey, coins);
        PlayerPrefs.SetInt(GemsKey, gems);
        PlayerPrefs.Save();
    }

    //  Load data
    public void LoadCurrency()
    {
        coins = PlayerPrefs.GetInt(CoinsKey, 0);
        gems = PlayerPrefs.GetInt(GemsKey, 0);
    }

    //  Update UI text
    private void UpdateUI()
    {
        if (coinsText) coinsText.text = coins.ToString();
        if (gemsText) gemsText.text = gems.ToString();
    }
}
