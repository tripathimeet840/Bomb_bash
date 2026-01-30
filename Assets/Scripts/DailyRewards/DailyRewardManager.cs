using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
     
public class DailyRewardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject notificationGameObject;  
    public GameObject dailyRewardCanvas;       
    public Button claimButton;                 
    public TMP_Text claimButtonText;               
    public TMP_Text nextClaimTimeText;             
    public TMP_Text rewardText;                    
    public Image rewardImage;
    public Sprite coinSprite, GemSprite;

    [Header("Reward Settings")]
    public int[] coinRewards = { 100, 150, 250 };  
    public int[] gemRewards = { 100, 150 };        
    public float claimIntervalHours = 24f;         

    private int currentDay;                        
    private DateTime nextClaimTime;               
    private bool canClaim;                         

    private const string DayKey = "DailyRewardDay";
    private const string NextClaimTimeKey = "NextClaimTime";

    void Start()
    {
        LoadData();   
        CheckRewardAvailability();
    }

    public void OnDailyRewardButtonClicked()
    {
        SoundManager.soundManager.PlayButtonSound();

        dailyRewardCanvas.SetActive(true);
        UpdateUI();
    }

    public void OnClaimButtonClicked()
    {
        SoundManager.soundManager.PlayButtonSound();

        if (canClaim)
        {
            ClaimReward();
        }
        else
        {
            dailyRewardCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    private void ClaimReward()
    {
        if (currentDay < 3) 
        {
            int coins = coinRewards[currentDay];
            Debug.Log("Player received " + coins + " coins!");
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + coins);
        }
        else if (currentDay >= 3 && currentDay < 5)
        {
            int gems = gemRewards[currentDay - 3];
            Debug.Log("Player received " + gems + " gems!");
            PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + gems);

        }
        else
        {
            int gems = gemRewards[1];
            Debug.Log("Player received " + gems + " gems!");
        }

        currentDay++;
        if (currentDay > 5)
        {
            currentDay = 5; 
        }

        nextClaimTime = DateTime.Now.AddHours(claimIntervalHours);
        canClaim = false;
        SaveData();
        UpdateUI();
        UIManager.instance.SetCoinsText();
    }

    private void UpdateUI()
    {
        if (currentDay < 3)
        {
            rewardText.text = "+" + coinRewards[currentDay] + " Coins";
            rewardImage.sprite = coinSprite;
        }
        else if (currentDay >= 3 && currentDay < 5)
        {
            rewardText.text = "+" + gemRewards[currentDay - 3] + " Gems";
            rewardImage.sprite = GemSprite;
        }
        else
        {
            rewardText.text = "+ 150 Gems";
            rewardImage.sprite = GemSprite;

        }

        if (canClaim)
        {
            claimButtonText.text = "Claim";
        }
        else
        {
            claimButtonText.text = "Back";
        }

        TimeSpan timeRemaining = nextClaimTime - DateTime.Now;
        if (timeRemaining.TotalSeconds > 0)
        {
            nextClaimTimeText.text =  timeRemaining.Hours + ":" + timeRemaining.Minutes + ":" + timeRemaining.Seconds;
        }
        else
        {
            nextClaimTimeText.text = "00:00:00";
            canClaim = true;
        }

        notificationGameObject.SetActive(canClaim);
    }

    private void CheckRewardAvailability()
    {
        if (DateTime.Now >= nextClaimTime)
        {
            canClaim = true;
        }
        else
        {
            canClaim = false;
        }
        UpdateUI();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(DayKey, currentDay);
        PlayerPrefs.SetString(NextClaimTimeKey, nextClaimTime.ToString());
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        currentDay = PlayerPrefs.GetInt(DayKey, 0);
        string nextClaimTimeString = PlayerPrefs.GetString(NextClaimTimeKey, DateTime.Now.ToString());
        nextClaimTime = DateTime.Parse(nextClaimTimeString);
    }

    public void CloseAll()
    {
        dailyRewardCanvas.SetActive(false);
    }
}
