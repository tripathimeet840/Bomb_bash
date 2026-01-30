using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementItemController : MonoBehaviour
{
   
    public TMP_Text Description,rewardValue;
    public Achievement achievement;
    public bool unlocked;
    Image MainImage;
    public Image FilledImage;
    [SerializeField] GameObject Redeem, Redeemed;
    public TMP_Text ProgressText;
    public Color LockedColor, UnlockedColor, RedmeedmedColor;
    public Image RewardImage;
    public Sprite Coins, Gems;

    public void RefreshView()
    {
        MainImage = GetComponent<Image>();
        GetCurrentNumber();
        if (achievement.CurrentNumber > achievement.TotalNumber)
            achievement.CurrentNumber = achievement.TotalNumber;
        ProgressText.text = achievement.CurrentNumber + "/" + achievement.TotalNumber;
        Description.text = achievement.title;
        rewardValue.text ="+"+ achievement.WonPrice.ToString();
        float fillAmount = (float)achievement.CurrentNumber / (float)achievement.TotalNumber;
        FilledImage.fillAmount = fillAmount;
        if (achievement.reward == Achievement.RewardType.Coins)
        {
            RewardImage.sprite = Coins;
        }
        else
        {
            RewardImage.sprite = Gems;
        }

        if (unlocked)
        {
            MainImage.color = Color.white;
            if (achievement.Redeemed)
            {
                Redeemed.SetActive(true);
                MainImage.color = RedmeedmedColor;
                
            }
            else
            {
                Redeem.SetActive(true);
                //Redeem.transform.GetComponentInChildren<TMP_Text>().text = "Claim " + achievement.WonPrice;
                MainImage.color = UnlockedColor;
            }


        }
        else
        {

            MainImage.color = LockedColor;
        }
    }

   
    public void RedeemAchievement()
    {
        if (unlocked && !achievement.Redeemed)
        {
            achievement.Redeemed = true;
            SoundManager.soundManager.PlayButtonSound();
            PlayerPrefs.SetInt(achievement.id + "_Redeemed", 1);
            Redeem.SetActive(false);
            Redeemed.SetActive(true);
            if (achievement.reward == Achievement.RewardType.Coins)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + achievement.WonPrice);

            }
            else
            {
                PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") + achievement.WonPrice);

            }
            UIManager.instance.SetCoinsText();

            //change MainImage to hexadecimal 79BC41
            MainImage.color = RedmeedmedColor;
            FindObjectOfType<AchievementsManager>().CheckAvaialble();

        }
    }

    void GetCurrentNumber()
    {
        string id = achievement.id;

        if (id.Contains("First"))
        {
            // FirstRun counts total games played (wins + losses)
            int wins = PlayerPrefs.GetInt("GamesWon", 0);
            int losses = PlayerPrefs.GetInt("LoseCount", 0);
            achievement.CurrentNumber = wins + losses;

            // Automatically unlock if at least 1 game played
            unlocked = achievement.CurrentNumber >= 1;
        }
        if (id.Contains("Played"))
        {
            achievement.CurrentNumber = PlayerPrefs.GetInt("GamesPlayed");

            // Unlock automatically if current >= total
            unlocked = achievement.CurrentNumber >= achievement.TotalNumber;
        }
        if (id.Contains("Won"))
        {
            achievement.CurrentNumber = PlayerPrefs.GetInt("GamesWon");

            // Unlock automatically if current >= total
            unlocked = achievement.CurrentNumber >= achievement.TotalNumber;
        }
        if (id.Contains("Lose"))
        {
            achievement.CurrentNumber = PlayerPrefs.GetInt("GamesLost");

            // Unlock automatically if current >= total
            unlocked = achievement.CurrentNumber >= achievement.TotalNumber;
        }
        if (id.Contains("Purchase"))
        {
            achievement.CurrentNumber = PlayerPrefs.GetInt("PurchaseCount");
        }
        if (id.Contains("Character"))
        {
            achievement.CurrentNumber = PlayerPrefs.GetInt("CharacterCount");
        }
    }
}