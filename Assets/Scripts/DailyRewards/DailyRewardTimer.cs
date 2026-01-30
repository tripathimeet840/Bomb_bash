using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyRewardTimer : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dailyRewardPanel;       
    public GameObject notificationIcon;       
    public TMP_Text countdownText;            
    public Button openButton;                 
    public Button closeButton;                

    [Header("Settings")]
    public float rewardIntervalHours = 24f;   

    private DateTime nextRewardTime;
    private const string NextRewardKey = "NextDailyReward";

    void Start()
    {
        string savedTime = PlayerPrefs.GetString(NextRewardKey, "");
        if (!string.IsNullOrEmpty(savedTime))
            nextRewardTime = DateTime.Parse(savedTime);
        else
            nextRewardTime = DateTime.Now; 

        if (openButton != null)
            openButton.onClick.AddListener(OpenDailyReward);
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDailyReward);

        UpdateNotification();
    }

    void Update()
    {
        UpdateCountdown();
    }

    void UpdateCountdown()
    {
        TimeSpan remaining = nextRewardTime - DateTime.Now;
        if (remaining.TotalSeconds <= 0)
        {
            countdownText.text = "Reward Available!";
            notificationIcon.SetActive(true);
        }
        else
        {
            countdownText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                remaining.Hours, remaining.Minutes, remaining.Seconds);
            notificationIcon.SetActive(false);
        }
    }

    public void ClaimReward()
    {
        Debug.Log("Player claimed daily reward!");

        nextRewardTime = DateTime.Now.AddHours(rewardIntervalHours);
        PlayerPrefs.SetString(NextRewardKey, nextRewardTime.ToString());
        PlayerPrefs.Save();

        UpdateNotification();
        CloseDailyReward();
    }

    public void OpenDailyReward()
    {
        dailyRewardPanel.SetActive(true);
    }

    public void CloseDailyReward()
    {
        dailyRewardPanel.SetActive(false);
    }

    void UpdateNotification()
    {
        notificationIcon.SetActive(DateTime.Now >= nextRewardTime);
    }
}
