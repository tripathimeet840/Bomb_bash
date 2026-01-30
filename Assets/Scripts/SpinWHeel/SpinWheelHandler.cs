using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class SpinWheelHandler : MonoBehaviour
{

    
    string RewardString = "SpinWheelClaimed";
    public GameObject SpinPanel;
    [SerializeField] TMP_Text timeLeft;
    public Button _button;
    public int RefilTimeInHours;
    [SerializeField] TMP_Text timeLeftAds;
    [SerializeField] GameObject WatchAdsPanel;
    // Start is called before the first frame update
    void Start()
    {
   
        // Check when the player last claimed the daily reward
        string lastTime = PlayerPrefs.GetString(RewardString, "");
        DateTime lastClaimTime;
        _button.onClick.AddListener(spinButtonClicked);
        if (!string.IsNullOrEmpty(lastTime))
        {
            lastClaimTime = DateTime.Parse(lastTime);
        }
        else
        {
            lastClaimTime = DateTime.MinValue; // Initialize to the beginning of time
        }

        // Check if the reward is available at the start
        /* if (DateTime.Now > lastClaimTime.AddHours(24))
         {
             // If 24 hours have passed since the last claim, activate the button
             dailyRewardButton.SetActive(true);
         }
         else
         {
             // If not, disable the button and show the time remaining
             dailyRewardButton.SetActive(false);
         }*/
    }

    void spinButtonClicked()
    {
        SoundManager.soundManager.PlayButtonSound();

        string lastTime = PlayerPrefs.GetString(RewardString, "");
        if (!string.IsNullOrEmpty(lastTime))
        {
            DateTime lastClaimTime = DateTime.Parse(lastTime);
            TimeSpan timeToNextClaim = lastClaimTime.AddHours(RefilTimeInHours) - DateTime.Now;

            if (timeToNextClaim.TotalSeconds > 0)
            {
                // If time is left, debug message
                WatchAdsPanel.SetActive(true);
                return; // Exit the method without opening the spin panel
            }
        }

        // If there is no time left, or if it's the first claim
        SpinPanel.SetActive(true);
    }

    private void Update()
    {
        // Continuously update the timer
        string lastTime = PlayerPrefs.GetString(RewardString, "");

        if (!string.IsNullOrEmpty(lastTime))
        {
            //_button.interactable = false;
            timeLeft.text = GetTimeToNextClaim(DateTime.Parse(lastTime));
            timeLeftAds.text = timeLeft.text;
        }
        else
        {
           // _button.interactable = true;
            timeLeft.text = "00:00:00";
        }
    }

    string GetTimeToNextClaim(DateTime lastTime)
    {
        TimeSpan timeToNextClaim = lastTime.AddHours(RefilTimeInHours) - DateTime.Now;

        if (timeToNextClaim.TotalSeconds <= 0)
        {
            // If the time has passed, activate the daily reward button

            return "00:00:00";
        }

        // Calculate and format the time remaining
        int hours = Mathf.FloorToInt((float)timeToNextClaim.TotalHours);
        int minutes = Mathf.FloorToInt((float)timeToNextClaim.TotalMinutes) % 60;
        int seconds = Mathf.FloorToInt((float)timeToNextClaim.TotalSeconds) % 60;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    

    [ContextMenu("Test - Set Last Claimed Time to 23 Hours Ago")]
    public void SetLastClaimedTime23HoursAgo()
    {
        DateTime testTime = DateTime.Now.AddHours(-23);
        PlayerPrefs.SetString(RewardString, testTime.ToString());
    }

    [ContextMenu("Test - Set Last Claimed Time to 25 Hours Ago")]
    public void SetLastClaimedTime25HoursAgo()
    {
        DateTime testTime = DateTime.Now.AddHours(-25);
        PlayerPrefs.SetString(RewardString, testTime.ToString());
    }


    public void ReawrdButtonClicked()
    {
     
        SpinPanel.SetActive(true);

    }

    public void WatchAdsToSpinWheel()
    {
        Mediation_Manager.mediation_Manager.ShowRewardedAd(RewardType.SpinWheel);
    }

    public void GrantRewardAfterWatchAds()
    {
        WatchAdsPanel.SetActive(false);
        SpinPanel.SetActive(true);
    }

    public void CloseWatchAdsPanel()
    {
        SoundManager.soundManager.PlayButtonSound();

        WatchAdsPanel.SetActive(false);
    }
}
