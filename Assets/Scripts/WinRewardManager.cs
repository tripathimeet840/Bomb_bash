using UnityEngine;
using TMPro;

public class WinRewardManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI totalCoinsText;

    [Header("Reward Settings")]
    public int minReward = 150;
    public int maxReward = 300;

    private int rewardEarned;   
    private int finalReward; 

    void OnEnable()
    {
        GrantWinReward();
    }

    public void GrantWinReward()
    {
        rewardEarned = Random.Range(minReward, maxReward + 1);
        finalReward = rewardEarned;

        UpdateUI(finalReward);

        Debug.Log($"Win Reward: {rewardEarned}");
    }

    public void ApplyMultiplier(int multiplier)
    {
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);
        int newCoins = rewardEarned * multiplier;

        PlayerPrefs.SetInt("Coins", currentCoins + newCoins);
        PlayerPrefs.Save();

        if (totalCoinsText != null)
            totalCoinsText.text = (currentCoins + newCoins).ToString();

        if (rewardText != null)
            rewardText.text = "+" + newCoins.ToString();

        Debug.Log($"Reward after multiplier {multiplier}X: {newCoins}, Total Coins: {currentCoins + newCoins}");
    }

    public void ConfirmRewardWithoutAd()
    {
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);
        PlayerPrefs.SetInt("Coins", currentCoins + rewardEarned);
        PlayerPrefs.Save();

        if (totalCoinsText != null)
            totalCoinsText.text = (currentCoins + rewardEarned).ToString();

        if (rewardText != null)
            rewardText.text = "+" + rewardEarned.ToString();

        Debug.Log($"Reward without ad: {rewardEarned}, Total Coins: {currentCoins + rewardEarned}");
    }


    private void UpdateUI(int rewardAmount, int totalCoins = -1)
    {
        if (rewardText != null)
            rewardText.text = "+" + rewardAmount.ToString();

        if (totalCoinsText != null)
        {
            if (totalCoins >= 0)
                totalCoinsText.text = totalCoins.ToString();
            else
                totalCoinsText.text = (PlayerPrefs.GetInt("Coins", 0)).ToString();
        }
    }

    public int GetRewardAmount()
    {
        return rewardEarned;
    }
}
