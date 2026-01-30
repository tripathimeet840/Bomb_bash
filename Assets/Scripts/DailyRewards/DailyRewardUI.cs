using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject dailyRewardPanel;
    //public GameObject profilePanel;
    //public GameObject notificationIcon;

    [Header("Rewards")]
    public Image[] rewardImages;             
    public Color claimedColor = Color.green; 
    public Color unclaimedColor = Color.white;

    [Header("Countdown")]
    public TMP_Text countdownText;
    public float rewardIntervalHours = 24f;

    [Header("Progress Slider")]
    public Slider progressSlider;

    private DateTime nextRewardTime;
    private int currentDay;

    private const string CurrentDayKey = "CurrentDailyRewardDay";
    private const string NextRewardTimeKey = "NextDailyRewardTime";

    public CurrencyManager currencyManager;

    public Button chest1;
    public Button chest2;
    public Button chest3;
    public Button chest4;
    public Button chest5;
    public Button chest6;
    public Button chest7;

    void Start()
    {
        UpdateChestInteractable(); 
        LoadData();
        UpdateRewardUI();
        UpdateProgressSlider();
    }

    void Update()
    {
        UpdateCountdown();
    }

    #region Panel Open/Close
    public void OpenDailyReward()
    {
        dailyRewardPanel.SetActive(true);
        //notificationIcon.SetActive(false);
        //profilePanel.SetActive(false);
    }

    public void CloseDailyReward()
    {
        dailyRewardPanel.SetActive(false);
    }
    #endregion

    #region Claim Reward
    public void ClaimReward(int rewardIndex)
    {
        if (rewardIndex != currentDay || DateTime.Now < nextRewardTime)
            return;

        Debug.Log("Reward claimed for day " + (currentDay + 1));

        rewardImages[currentDay].color = claimedColor;
        rewardImages[currentDay].GetComponent<Button>().interactable = false;

        currentDay++;

        if (currentDay >= rewardImages.Length)
        {
            Debug.Log("All 7 rewards claimed! Resetting...");
            ResetAllRewards();
            return;
        }

        nextRewardTime = DateTime.Now.AddHours(rewardIntervalHours);

        SaveData();
        UpdateRewardUI();
        UpdateProgressSlider();
        UpdateChestInteractable();
    }


    private void ResetAllRewards()
    {
        currentDay = 0;
        nextRewardTime = DateTime.Now.AddHours(rewardIntervalHours);

        foreach (var img in rewardImages)
            img.color = unclaimedColor;

        if (progressSlider != null)
            progressSlider.value = 0;

        SaveData();
        UpdateRewardUI();
        UpdateProgressSlider();
        UpdateChestInteractable();

        Debug.Log("Daily rewards have been reset for a new cycle!");
    }


    void UpdateChestInteractable()
    {
        chest1.interactable = false;
        chest2.interactable = false;
        chest3.interactable = false;
        chest4.interactable = false;
        chest5.interactable = false;
        chest6.interactable = false;
        chest7.interactable = false;

        switch (currentDay)
        {
            case 1:
                chest1.interactable = true;
                break;
            case 2:
                chest2.interactable = true;
                break;
            case 3:
                chest3.interactable = true;
                break;
            case 4:
                chest4.interactable = true;
                break;
            case 5:
                chest5.interactable = true;
                break;
            case 6:
                chest6.interactable = true;
                break;
            case 7:
                chest7.interactable = true;
                break;
        }
    }


    #endregion

    #region Reset (for testing)
    public void ResetTimer()
    {
        Debug.Log("Daily Reward Timer Reset for testing!");

        currentDay = 0;
        nextRewardTime = DateTime.Now;

        foreach (var img in rewardImages)
            img.color = unclaimedColor;

        SaveData();
        UpdateRewardUI();
        UpdateProgressSlider();
    }
    #endregion

    #region Countdown
    void UpdateCountdown()
    {
        TimeSpan remaining = nextRewardTime - DateTime.Now;

        if (remaining.TotalSeconds <= 0)
        {
            countdownText.text = "Reward Available!";
            currencyManager.canClaim = true;
            //notificationIcon.SetActive(true);
        }
        else
        {
            countdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
    }
    #endregion

    #region Save/Load
    void LoadData()
    {
        currentDay = PlayerPrefs.GetInt(CurrentDayKey, 0);

        string savedTime = PlayerPrefs.GetString(NextRewardTimeKey, "");
        if (!string.IsNullOrEmpty(savedTime))
            nextRewardTime = DateTime.Parse(savedTime);
        else
            nextRewardTime = DateTime.Now;
    }

    void SaveData()
    {
        PlayerPrefs.SetInt(CurrentDayKey, currentDay);
        PlayerPrefs.SetString(NextRewardTimeKey, nextRewardTime.ToString());
        PlayerPrefs.Save();
    }
    #endregion

    void UpdateRewardUI()
    {
        for (int i = 0; i < rewardImages.Length; i++)
        {
            Button rewardButton = rewardImages[i].GetComponent<Button>();

            if (i < currentDay)
            {
                rewardImages[i].color = claimedColor;  
                rewardButton.interactable = false;     
            }
            else if (i == currentDay && DateTime.Now >= nextRewardTime)
            {
                rewardImages[i].color = Color.yellow;  
                rewardButton.interactable = true;      
            }
            else
            {
                rewardImages[i].color = unclaimedColor; 
                rewardButton.interactable = false;
            }
        }
    }



    void UpdateProgressSlider()
    {
        if (progressSlider != null)
        {
            progressSlider.maxValue = rewardImages.Length; 
            progressSlider.value = currentDay;            
        }
    }


    #region Testing
    public void TestClaimNextReward()
    {
        //ClaimReward();

        nextRewardTime = DateTime.Now;

        SaveData();
        UpdateCountdown();
        UpdateRewardUI();
        UpdateProgressSlider();

        Debug.Log(" Test mode: Instantly moved to next day reward!");
    }

    public void TestResetAll()
    {
        Debug.Log(" Test mode: Resetting all daily rewards!");

        currentDay = 0;
        nextRewardTime = DateTime.Now; 

        foreach (var img in rewardImages)
            img.color = unclaimedColor;

        if (progressSlider != null)
            progressSlider.value = 0;

        SaveData();
        UpdateCountdown();
        UpdateRewardUI();
        UpdateProgressSlider();
    }
    #endregion


}
